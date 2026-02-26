#!/bin/bash
# ATEM Medication Alert Display Autostart Script
# Startet Chromium im Kiosk-Mode mit der Medication Alert Seite

# Warten bis X-Server bereit ist
sleep 10

# Display einschalten (falls ausgeschaltet)
xset s off 2>/dev/null || true
xset s noblank 2>/dev/null || true

# Mauszeiger verstecken (optional, falls installiert)
command -v unclutter >/dev/null 2>&1 && unclutter -idle 0 &

# Lokalen Webserver starten
cd /home/raspi/atem
python3 -m http.server 8080 &
WEBSERVER_PID=$!
sleep 2

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

# Cleanup
kill $WEBSERVER_PID 2>/dev/null

# Falls Chromium crasht, nach 5 Sekunden neu starten
sleep 5
exec "$0"
