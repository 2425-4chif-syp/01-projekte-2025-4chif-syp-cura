#!/usr/bin/env python3
"""
ATEM Medication Alert Daemon
Läuft dauerhaft auf dem Raspberry Pi und steuert den ATEM Switcher.

- Fragt Backend ab ob Medikament fällig ist
- Schaltet auf HDMI 2 (Alert) wenn Medikament fällig
- Schaltet zurück auf HDMI 1 wenn genommen
"""

from PyATEMMax.ATEMMax import ATEMMax
import requests
import time
import logging
from datetime import datetime

# ===== KONFIGURATION =====
BACKEND_URL = "https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1"
ATEM_IP = "192.168.0.226"
PATIENT_ID = 1

# test
# Polling Intervall in Sekunden
POLL_INTERVAL = 30  # Alle 30 Sekunden checken

# ATEM Inputs
INPUT_NORMAL = 2   # HDMI 2 - Normal/Hauptsignal
INPUT_ALERT = 1    # HDMI 1 - Alert/Raspi Display

# ===== LOGGING =====
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(levelname)s - %(message)s',
    handlers=[
        logging.FileHandler('/home/raspi/atem/medication-alert.log'),
        logging.StreamHandler()
    ]
)
logger = logging.getLogger(__name__)

# ===== STATE =====
current_state = "normal"  # "normal" oder "alert"
atem = None


def connect_atem():
    """Verbindung zum ATEM herstellen"""
    global atem
    try:
        logger.info(f"Verbinde mit ATEM auf {ATEM_IP}...")
        atem = ATEMMax()
        atem.connect(ATEM_IP)
        atem.waitForConnection(timeout=5)
        logger.info("✓ ATEM verbunden")
        return True
    except Exception as e:
        logger.error(f"✗ ATEM Verbindung fehlgeschlagen: {e}")
        atem = None
        return False


def switch_to_alert():
    """Auf Alert-Signal (HDMI 2) schalten"""
    global current_state
    if not atem:
        logger.warning("ATEM nicht verbunden, versuche Reconnect...")
        if not connect_atem():
            return False
    
    try:
        logger.info(f"📢 ALERT: Schalte auf HDMI {INPUT_ALERT}")
        atem.setProgramInputVideoSource(0, INPUT_ALERT)
        current_state = "alert"
        return True
    except Exception as e:
        logger.error(f"✗ Fehler beim Schalten: {e}")
        return False


def switch_to_normal():
    """Zurück auf Normal-Signal (HDMI 1)"""
    global current_state
    if not atem:
        logger.warning("ATEM nicht verbunden, versuche Reconnect...")
        if not connect_atem():
            return False
    
    try:
        logger.info(f"✓ OK: Schalte zurück auf HDMI {INPUT_NORMAL}")
        atem.setProgramInputVideoSource(0, INPUT_NORMAL)
        current_state = "normal"
        return True
    except Exception as e:
        logger.error(f"✗ Fehler beim Schalten: {e}")
        return False


def check_medication_status():
    """Backend abfragen ob Medikament fällig ist"""
    try:
        response = requests.get(BACKEND_URL, timeout=10)
        response.raise_for_status()
        data = response.json()
        
        should_alert = data.get('shouldAlert', False)
        reason = data.get('reason', 'Unknown')
        time_slot = data.get('timeSlot', 'Unknown')
        
        logger.debug(f"Backend Status: should_alert={should_alert}, reason={reason}, time_slot={time_slot}")
        
        return should_alert, data
    
    except requests.exceptions.RequestException as e:
        logger.error(f"✗ Backend Request fehlgeschlagen: {e}")
        return None, None
    except Exception as e:
        logger.error(f"✗ Fehler beim Parsen der Response: {e}")
        return None, None


def main_loop():
    """Haupt-Loop"""
    global current_state
    
    logger.info("=" * 60)
    logger.info("🏥 ATEM Medication Alert Daemon gestartet")
    logger.info(f"Backend: {BACKEND_URL}")
    logger.info(f"ATEM IP: {ATEM_IP}")
    logger.info(f"Patient ID: {PATIENT_ID}")
    logger.info(f"Poll Intervall: {POLL_INTERVAL}s")
    logger.info("=" * 60)
    
    # Initial ATEM verbinden
    if not connect_atem():
        logger.warning("Starte trotzdem - werde bei Bedarf reconnecten...")
    else:
        # Stelle sicher dass wir auf Normal-Signal starten
        switch_to_normal()
    
    consecutive_errors = 0
    max_consecutive_errors = 5
    
    while True:
        try:
            should_alert, data = check_medication_status()
            
            # Verbindungsfehler
            if should_alert is None:
                consecutive_errors += 1
                if consecutive_errors >= max_consecutive_errors:
                    logger.warning(f"⚠️  {consecutive_errors} aufeinanderfolgende Fehler - warte länger...")
                    time.sleep(POLL_INTERVAL * 2)  # Doppelt so lange warten
                else:
                    time.sleep(POLL_INTERVAL)
                continue
            
            # Verbindung OK - Reset error counter
            consecutive_errors = 0
            
            # State-Machine
            if should_alert and current_state == "normal":
                # Medikament fällig → auf Alert schalten
                logger.info("🔔 Medikament fällig!")
                if data and 'medications' in data:
                    for med in data['medications']:
                        logger.info(f"  - {med.get('name')} ({med.get('quantity')}x)")
                switch_to_alert()
            
            elif not should_alert and current_state == "alert":
                # Medikament wurde genommen → zurück auf Normal
                reason = data.get('reason', 'Unknown')
                logger.info(f"✅ Medikament genommen: {reason}")
                switch_to_normal()
            
            elif should_alert and current_state == "alert":
                # Immer noch fällig - bleiben auf Alert
                logger.debug("⏳ Warte auf Medikamenteneinnahme...")
            
            elif not should_alert and current_state == "normal":
                # Alles OK - bleiben auf Normal
                logger.debug("✓ Alles OK, keine Medikamente fällig")
            
            # Warten bis zum nächsten Check
            time.sleep(POLL_INTERVAL)
        
        except KeyboardInterrupt:
            logger.info("\n⛔ Shutdown durch Benutzer")
            break
        
        except Exception as e:
            logger.error(f"✗ Unerwarteter Fehler in main_loop: {e}")
            time.sleep(POLL_INTERVAL)
    
    # Cleanup
    if atem:
        try:
            logger.info("Schalte zurück auf Normal und trenne ATEM...")
            switch_to_normal()
            atem.disconnect()
        except:
            pass
    
    logger.info("🛑 Daemon beendet")


if __name__ == "__main__":
    try:
        main_loop()
    except Exception as e:
        logger.critical(f"💥 Kritischer Fehler: {e}")
        exit(1)
