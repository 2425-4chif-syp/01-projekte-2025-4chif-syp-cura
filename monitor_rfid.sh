#!/bin/bash

# Install mosquitto-clients if not already installed
# sudo apt-get install mosquitto-clients

# Function to check MQTT message
check_mqtt() {
    message=$(mosquitto_sub -h cura.local -t "rc522/tag" -C 1)
    if [ ! -z "$message" ]; then
        handle_mqtt_message "$message"
    fi
}

# Function to handle MQTT messages
handle_mqtt_message() {
    local message="$1"
    echo "RFID Tag erkannt: $message"
    
    # Prüfe ob die gleiche Nachricht schon mal kam
    if [ -f "/tmp/last_rfid" ] && [ "$(cat /tmp/last_rfid)" = "$message" ]; then
        echo "Gleiche Nachricht erkannt - zeige Danke an..."
        if ssh raspi@cura.local "echo \" \n  Danke fürs Einnehmen 😀 \n \" > ~/Desktop/warning-text.txt"; then
            echo "Danke-Nachricht angezeigt"
            # Warte 5 Sekunden
            sleep 5
            # Lösche die Nachricht
            ssh raspi@cura.local "echo \"\" > ~/Desktop/warning-text.txt"
            # Lösche die temporäre Datei
            rm /tmp/last_rfid
        else
            echo "Fehler beim Ausführen des SSH Befehls"
        fi
    else
        echo "Erste Nachricht - zeige Aufforderung an..."
        if ssh raspi@cura.local "echo \" \n  Bitte Ihre Medikamente nehmen  \n \" > ~/Desktop/warning-text.txt"; then
            echo "SSH Befehl erfolgreich ausgeführt"
            # Speichere die Nachricht für späteren Vergleich
            echo "$message" > /tmp/last_rfid
        else
            echo "Fehler beim Ausführen des SSH Befehls"
        fi
    fi
}   



# Main loop
while true; do
    # Check for MQTT messages
    check_mqtt
done
