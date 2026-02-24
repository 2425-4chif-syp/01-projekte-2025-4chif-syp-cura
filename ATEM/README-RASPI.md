# ATEM Medication Alert - Raspberry Pi Setup

Automatischer ATEM Switcher für Medikamenten-Erinnerungen.

## 🎯 Was macht es?

- Läuft dauerhaft im Hintergrund auf dem Raspberry Pi
- Fragt alle 30 Sekunden das Backend ab ob Medikamente fällig sind
- **Wenn Medikament fällig aber nicht genommen:** ATEM auf HDMI 2 schalten (Alert-Screen)
- **Wenn Medikament genommen wurde:** ATEM zurück auf HDMI 1 (Hauptsignal)

## 📋 Voraussetzungen

- Raspberry Pi mit Netzwerk
- Python 3.7+
- ATEM Mini erreichbar im Netzwerk (192.168.0.226)
- Backend erreichbar (https://vm12.htl-leonding.ac.at)

## 🚀 Installation

### 1. Dateien auf Raspi kopieren

```bash
# Auf deinem PC
scp atem_medication_daemon.py pi@raspi:/home/pi/atem/
scp atem-medication-alert.service pi@raspi:/home/pi/atem/
```

### 2. Python Environment einrichten

```bash
# SSH auf Raspi
ssh pi@raspi

# Venv erstellen
cd /home/pi/atem
python3 -m venv venv
source venv/bin/activate

# Dependencies installieren
pip install PyATEMMax requests
```

### 3. Konfiguration anpassen (optional)

Bearbeite `atem_medication_daemon.py` falls nötig:

```python
BACKEND_URL = "https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1"
ATEM_IP = "192.168.0.226"
PATIENT_ID = 1
POLL_INTERVAL = 30  # Sekunden
```

### 4. Manuell testen

```bash
cd /home/pi/atem
source venv/bin/activate
python3 atem_medication_daemon.py
```

Solltest du sehen:
```
============================================================
🏥 ATEM Medication Alert Daemon gestartet
Backend: https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1
ATEM IP: 192.168.0.226
Patient ID: 1
Poll Intervall: 30s
============================================================
2025-02-26 10:30:00 - INFO - Verbinde mit ATEM auf 192.168.0.226...
2025-02-26 10:30:01 - INFO - ✓ ATEM verbunden
2025-02-26 10:30:01 - INFO - ✓ OK: Schalte zurück auf HDMI 1
2025-02-26 10:30:01 - DEBUG - ✓ Alles OK, keine Medikamente fällig
...
```

**Mit Strg+C beenden** wenn es funktioniert.

### 5. Als systemd Service installieren

```bash
# Service File kopieren
sudo cp /home/pi/atem/atem-medication-alert.service /etc/systemd/system/

# Log-Datei vorbereiten
sudo touch /var/log/atem-medication-alert.log
sudo chown pi:pi /var/log/atem-medication-alert.log

# Service aktivieren
sudo systemctl daemon-reload
sudo systemctl enable atem-medication-alert
sudo systemctl start atem-medication-alert
```

### 6. Status checken

```bash
# Service Status
sudo systemctl status atem-medication-alert

# Live Logs
sudo journalctl -u atem-medication-alert -f

# Log-Datei
tail -f /var/log/atem-medication-alert.log
```

## 🔧 Service Verwaltung

```bash
# Starten
sudo systemctl start atem-medication-alert

# Stoppen
sudo systemctl stop atem-medication-alert

# Neu starten
sudo systemctl restart atem-medication-alert

# Deaktivieren (nicht mehr automatisch starten)
sudo systemctl disable atem-medication-alert
```

## 📊 State Machine

```
┌─────────────┐                    ┌─────────────┐
│   NORMAL    │                    │    ALERT    │
│  (HDMI 1)   │                    │  (HDMI 2)   │
└──────┬──────┘                    └──────┬──────┘
       │                                  │
       │  Backend: ShouldAlert=true       │
       │  (Medikament fällig)             │
       └──────────────────────────────────┤
                                          │
       ┌──────────────────────────────────┘
       │  Backend: ShouldAlert=false
       │  (Medikament genommen)
       └───►
```

## 🐛 Troubleshooting

### ATEM nicht erreichbar

```bash
ping 192.168.0.226
```

Falls nicht: IP-Adresse in `atem_medication_daemon.py` anpassen.

### Backend nicht erreichbar

```bash
curl https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1
```

Sollte JSON zurückgeben:
```json
{
  "shouldAlert": false,
  "reason": "...",
  "timeSlot": "Morning"
}
```

### Service startet nicht

```bash
# Detaillierte Fehler
sudo journalctl -u atem-medication-alert -n 50

# Manuell testen
cd /home/pi/atem
source venv/bin/activate
python3 atem_medication_daemon.py
```

### Logs

```bash
# Systemd Journal
sudo journalctl -u atem-medication-alert -f

# Log-Datei
tail -f /var/log/atem-medication-alert.log
```

## ⚙️ Konfiguration

| Variable | Default | Beschreibung |
|----------|---------|--------------|
| `BACKEND_URL` | `https://vm12.htl-leonding.ac.at/api/...` | Backend Alert-Endpoint |
| `ATEM_IP` | `192.168.0.226` | IP des ATEM Mini |
| `PATIENT_ID` | `1` | Patient ID für Backend-Abfrage |
| `POLL_INTERVAL` | `30` | Sekunden zwischen Checks |
| `INPUT_NORMAL` | `1` | HDMI 1 - Hauptsignal |
| `INPUT_ALERT` | `2` | HDMI 2 - Alert-Screen |

## 📝 Backend Endpoint

Der Daemon nutzt: `GET /api/MedicationAlert/should-alert/{patientId}`

Response:
```json
{
  "shouldAlert": true,
  "reason": "Es gibt 2 geplante Medikamente für Morning, 0 davon wurden eingenommen",
  "timeSlot": "Morning",
  "currentTime": "2025-02-26T10:30:00",
  "scheduledMedications": 2,
  "takenMedications": 0,
  "medications": [
    {
      "medicationId": 1,
      "name": "Aspirin",
      "quantity": 1
    }
  ]
}
```

## 🎬 ATEM Inputs

- **Input 1 (HDMI 1):** Normal/Hauptsignal (z.B. Live-Kamera)
- **Input 2 (HDMI 2):** Alert-Screen (z.B. "Bitte Medikament nehmen!")

Verbinde die entsprechenden Quellen mit den HDMI-Eingängen am ATEM Mini.

## 📱 Test-Szenario

1. **Backend deployed** mit `MedicationAlertController`
2. **Daemon läuft** auf Raspi
3. **ATEM verbunden** mit beiden HDMI-Quellen
4. **Zeitpunkt erreichen** wo Medikament fällig ist (z.B. 08:00 Morning)
5. **Ohne Scan:** ATEM sollte auf HDMI 2 schalten (Alert)
6. **RFID-Chip scannen:** Backend registriert Einnahme
7. **Nach ~30 Sekunden:** ATEM sollte zurück auf HDMI 1 schalten

## 🔒 Security

- Service läuft als User `pi` (nicht root)
- `NoNewPrivileges=true` verhindert Privilege Escalation
- `PrivateTmp=true` isoliert /tmp

## 📄 Logs

Logs werden geschrieben nach:
- `/var/log/atem-medication-alert.log` (File)
- `journalctl -u atem-medication-alert` (systemd)

Format:
```
2025-02-26 10:30:00 - INFO - ✓ OK: Schalte zurück auf HDMI 1
2025-02-26 10:45:00 - INFO - 🔔 Medikament fällig!
2025-02-26 10:45:00 - INFO -   - Aspirin (1x)
2025-02-26 10:45:00 - INFO - 📢 ALERT: Schalte auf HDMI 2
```

## 🆘 Support

Bei Problemen:
1. Logs checken: `sudo journalctl -u atem-medication-alert -f`
2. Manuell testen: `python3 atem_medication_daemon.py`
3. Backend testen: `curl https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1`
4. ATEM testen: `python3 test_atem.py`
