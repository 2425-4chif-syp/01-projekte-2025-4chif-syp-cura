#!/bin/bash
# ATEM Medication Alert Display Autostart Script
# Startet Chromium im Kiosk-Mode mit der Medication Alert Seite

# Warten bis X-Server bereit ist
sleep 10

# Display einschalten (falls ausgeschaltet)
xset -dpms
xset s off
xset s noblank

# Mauszeiger verstecken
unclutter -idle 0 &

# Chromium im Kiosk-Mode starten (Fullscreen, ohne UI)
chromium-browser \
    --kiosk \
    --noerrdialogs \
    --disable-infobars \
    --no-first-run \
    --disable-features=TranslateUI \
    --disable-session-crashed-bubble \
    --disable-component-update \
    --start-fullscreen \
    file:///home/pi/medication-alert-display.html

# Falls Chromium crasht, nach 5 Sekunden neu starten
sleep 5
exec "$0"
