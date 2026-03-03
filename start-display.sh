#!/bin/bash
# ATEM Medication Alert Display Autostart Script
# Startet Chromium im Kiosk-Mode mit der Medication Alert Seite

# Log-Datei
LOG_FILE="/tmp/display-start.log"
echo "$(date): Display-Start Script gestartet" >> "$LOG_FILE"

# Warten bis X-Server bereit ist
echo "$(date): Warte auf X-Server..." >> "$LOG_FILE"
sleep 10

# Display einschalten (falls ausgeschaltet)
xset s off 2>/dev/null || true
xset s noblank 2>/dev/null || true
echo "$(date): Display-Einstellungen gesetzt" >> "$LOG_FILE"

# Mauszeiger verstecken (optional, falls installiert)
command -v unclutter >/dev/null 2>&1 && unclutter -idle 0 &

# Lokalen Webserver starten
cd /home/raspi/atem
python3 -m http.server 8080 &
WEBSERVER_PID=$!
echo "$(date): Webserver gestartet (PID: $WEBSERVER_PID)" >> "$LOG_FILE"
sleep 2

# Warte bis Netzwerk bereit ist (max 60 Sekunden)
echo "$(date): Warte auf Netzwerk..." >> "$LOG_FILE"
for i in {1..60}; do
    if ping -c 1 -W 2 vm12.htl-leonding.ac.at > /dev/null 2>&1; then
        echo "$(date): Netzwerk erreichbar!" >> "$LOG_FILE"
        break
    fi
    if [ $i -eq 60 ]; then
        echo "$(date): WARNUNG: Netzwerk-Timeout nach 60 Sekunden" >> "$LOG_FILE"
    fi
    sleep 1
done

# Warte bis Backend API erreichbar ist (max 30 Sekunden)
echo "$(date): Prüfe Backend-Erreichbarkeit..." >> "$LOG_FILE"
for i in {1..30}; do
    if curl -k -s -f --max-time 3 https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1 > /dev/null 2>&1; then
        echo "$(date): Backend API erreichbar!" >> "$LOG_FILE"
        break
    fi
    if [ $i -eq 30 ]; then
        echo "$(date): WARNUNG: Backend-Timeout nach 30 Sekunden - starte trotzdem" >> "$LOG_FILE"
    fi
    sleep 1
done

# Extra Wartezeit für Stabilität
sleep 3
echo "$(date): Starte Chromium..." >> "$LOG_FILE"

# Chromium im Kiosk-Mode starten (Fullscreen, ohne UI)
chromium-browser \
    --kiosk \
    --noerrdialogs \
    --disable-infobars \
    --no-first-run \
    --disable-features=TranslateUI,Translate \
    --disable-session-crashed-bubble \
    --disable-component-update \
    --start-fullscreen \
    --ignore-certificate-errors \
    --allow-insecure-localhost \
    --disable-web-security \
    --disable-translate \
    --disable-notifications \
    --disable-popup-blocking \
    --user-data-dir=/tmp/chromium-kiosk \
    --lang=de \
    http://localhost:8080/medication-alert-display.html

echo "$(date): Chromium beendet" >> "$LOG_FILE"

# Cleanup
kill $WEBSERVER_PID 2>/dev/null
echo "$(date): Webserver gestoppt" >> "$LOG_FILE"

# Falls Chromium crasht, nach 5 Sekunden neu starten
sleep 5
echo "$(date): Neustart in 5 Sekunden..." >> "$LOG_FILE"
exec "$0"
