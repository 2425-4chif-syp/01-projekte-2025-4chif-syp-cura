# ATEM Display Autostart Setup für Raspberry Pi

Automatischer Start des Medication Alert Displays beim Booten.

## 📦 Dateien

- `start-display.sh` - Shell-Skript das Chromium im Kiosk-Mode startet
- `medication-display.desktop` - Autostart-Eintrag
- `medication-alert-display.html` - Die eigentliche Display-Seite

## 🚀 Installation auf Raspberry Pi

### 1. Abhängigkeiten installieren

```bash
sudo apt update
sudo apt install -y chromium-browser unclutter xdotool
```

### 2. Dateien kopieren

```bash
# Auf deinem PC
scp medication-alert-display.html pi@raspi-ip:/home/pi/
scp start-display.sh pi@raspi-ip:/home/pi/
scp medication-display.desktop pi@raspi-ip:/home/pi/
```

### 3. Skript ausführbar machen

```bash
# Auf dem Raspberry Pi
ssh pi@raspi-ip
chmod +x /home/pi/start-display.sh
```

### 4. Autostart einrichten

**Option A: Mit Desktop-Datei (empfohlen für Raspberry Pi OS Desktop)**

```bash
# Autostart-Ordner erstellen (falls nicht vorhanden)
mkdir -p ~/.config/autostart

# Desktop-Datei kopieren
cp /home/pi/medication-display.desktop ~/.config/autostart/

# Testen
/home/pi/start-display.sh
```

**Option B: Mit rc.local (für Lite-Version ohne Desktop)**

```bash
sudo nano /etc/rc.local
```

Vor `exit 0` hinzufügen:
```bash
su - pi -c "DISPLAY=:0 /home/pi/start-display.sh" &
```

**Option C: Mit systemd Service (für Production)**

```bash
sudo nano /etc/systemd/system/medication-display.service
```

Inhalt:
```ini
[Unit]
Description=ATEM Medication Alert Display
After=graphical.target

[Service]
Type=simple
User=pi
Environment=DISPLAY=:0
Environment=XAUTHORITY=/home/pi/.Xauthority
ExecStart=/home/pi/start-display.sh
Restart=always
RestartSec=10

[Install]
WantedBy=graphical.target
```

Aktivieren:
```bash
sudo systemctl daemon-reload
sudo systemctl enable medication-display.service
sudo systemctl start medication-display.service
```

### 5. Display-Einstellungen optimieren

```bash
# In /boot/config.txt für bessere Display-Performance
sudo nano /boot/config.txt
```

Hinzufügen:
```ini
# Display-Einstellungen
hdmi_force_hotplug=1
hdmi_drive=2
disable_overscan=1

# GPU Memory für bessere Browser-Performance
gpu_mem=128
```

### 6. Bildschirmschoner deaktivieren

```bash
# In ~/.config/lxsession/LXDE-pi/autostart
nano ~/.config/lxsession/LXDE-pi/autostart
```

Hinzufügen:
```bash
@xset s off
@xset -dpms
@xset s noblank
```

## 🧪 Testen

### Manuell starten
```bash
/home/pi/start-display.sh
```

### Service Status prüfen
```bash
sudo systemctl status medication-display.service
```

### Browser beenden
```bash
pkill chromium
```

### Neustart testen
```bash
sudo reboot
```

Nach dem Reboot sollte automatisch Chromium im Fullscreen starten!

## 🎬 Was das Skript macht

1. **Wartet 10 Sekunden** bis X-Server bereit ist
2. **Deaktiviert Bildschirmschoner** (xset)
3. **Versteckt Mauszeiger** (unclutter)
4. **Startet Chromium** im Kiosk-Mode:
   - `--kiosk`: Fullscreen ohne Browser-UI
   - `--noerrdialogs`: Keine Error-Popups
   - `--disable-infobars`: Keine Info-Leisten
   - `--no-first-run`: Kein Setup-Wizard
   - `--start-fullscreen`: Erzwingt Fullscreen
5. **Auto-Restart** bei Crash nach 5 Sekunden

## ⚙️ Konfiguration

### HTML-Datei Pfad ändern

In `start-display.sh` Zeile 18:
```bash
file:///home/pi/medication-alert-display.html
```

### Backend-URL ändern

In `medication-alert-display.html` Zeile 184:
```javascript
const BACKEND_URL = 'https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1';
```

### Wartezeit beim Start

In `start-display.sh` Zeile 5:
```bash
sleep 10  # Sekunden warten
```

## 🐛 Troubleshooting

### Display startet nicht automatisch

```bash
# Logs prüfen
journalctl -u medication-display.service -f

# X-Server testen
echo $DISPLAY  # sollte :0 sein

# Manuell testen mit DISPLAY
DISPLAY=:0 /home/pi/start-display.sh
```

### Chromium zeigt Fehlermeldung

```bash
# Chromium Cache löschen
rm -rf ~/.config/chromium/Default/

# Chromium komplett neu installieren
sudo apt remove --purge chromium-browser
sudo apt install chromium-browser
```

### Mauszeiger sichtbar

```bash
# Unclutter prüfen
ps aux | grep unclutter

# Manuell starten
unclutter -idle 0 &
```

### Display schaltet sich aus

```bash
# In /boot/config.txt
hdmi_blanking=1

# Und in autostart
@xset s off
@xset -dpms
@xset s noblank
```

### Browser friert ein

```bash
# Watchdog-Timer für automatischen Neustart
sudo apt install watchdog
sudo systemctl enable watchdog
```

## 🔄 Updates deployen

### Nur HTML-Datei updaten

```bash
# Auf PC
scp medication-alert-display.html pi@raspi-ip:/home/pi/

# Auf Raspi - Browser neu laden
pkill chromium
# Startet automatisch neu durch das Skript
```

### Skript updaten

```bash
scp start-display.sh pi@raspi-ip:/home/pi/
ssh pi@raspi-ip "chmod +x /home/pi/start-display.sh"
ssh pi@raspi-ip "sudo systemctl restart medication-display.service"
```

## 📊 Hardware-Setup

### Empfohlene Hardware
- Raspberry Pi 4 (4GB RAM) - für flüssige Performance
- MicroSD Karte (32GB+, Class 10)
- HDMI-Kabel zu ATEM Mini Input 2
- Netzteil (offizielles 5V/3A)
- Gehäuse mit Lüfter (für 24/7 Betrieb)

### Display-Auflösung

Chromium passt sich automatisch an, aber für ATEM optimal:
- 1920x1080 (Full HD)
- 1280x720 (HD)

```bash
# In /boot/config.txt für feste Auflösung
hdmi_group=2
hdmi_mode=82  # 1080p 60Hz
```

## 🔒 Security

### Browser-Isolation

Das Skript läuft als User `pi` (nicht root) mit eingeschränkten Rechten.

### Auto-Update deaktivieren

Chromium Updates können Display stören:
```bash
sudo apt-mark hold chromium-browser
```

### Netzwerk-Zugriff

Firewall nur Backend-API erlauben:
```bash
sudo ufw allow out 443/tcp to vm12.htl-leonding.ac.at
```

## 📝 Wartung

### Logs prüfen

```bash
# Systemd Service
sudo journalctl -u medication-display.service -f

# Chromium Logs
ls -la ~/.config/chromium/chrome_debug.log
```

### Remote-Zugriff

```bash
# VNC für Remote-Desktop
sudo apt install realvnc-vnc-server
sudo raspi-config  # Interface Options → VNC → Enable
```

### Backup

```bash
# SD-Karte klonen (auf PC)
sudo dd if=/dev/sdX of=raspi-backup.img bs=4M status=progress
```

## 🎯 Produktiv-Checkliste

- [ ] Chromium installiert
- [ ] Unclutter installiert
- [ ] HTML-Datei kopiert
- [ ] Skript ausführbar gemacht
- [ ] Autostart konfiguriert
- [ ] Bildschirmschoner deaktiviert
- [ ] Backend-URL konfiguriert
- [ ] HDMI zu ATEM Input 2 verbunden
- [ ] Neustart getestet
- [ ] Display zeigt korrekte Zeit an
- [ ] Alert-Funktionalität getestet
- [ ] 24/7 Stabilität geprüft
