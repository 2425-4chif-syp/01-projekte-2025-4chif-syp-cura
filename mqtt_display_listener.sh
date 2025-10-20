#!/bin/bash

# MQTT Display Listener Script for Raspberry Pi
# This script subscribes to the display/message topic and writes content to warning-text.txt
# Author: Cura Medication System
# Date: $(date)

# Configuration
MQTT_BROKER="vm12.htl-leonding.ac.at"
MQTT_PORT="1883"
MQTT_TOPIC="display/message"
OUTPUT_FILE="/home/pi/warning-text.txt"
LOG_FILE="/home/pi/mqtt_display.log"

# Function to log messages with timestamp
log_message() {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - $1" >> "$LOG_FILE"
}

# Function to handle script termination
cleanup() {
    log_message "Script terminated by user"
    exit 0
}

# Set up signal handlers
trap cleanup SIGINT SIGTERM

# Start logging
log_message "MQTT Display Listener started"
log_message "Broker: $MQTT_BROKER:$MQTT_PORT"
log_message "Topic: $MQTT_TOPIC"
log_message "Output file: $OUTPUT_FILE"

# Main loop - Subscribe to MQTT topic and write to file
while true; do
    log_message "Connecting to MQTT broker..."
    
    # Subscribe to MQTT topic and process messages
    mosquitto_sub -h "$MQTT_BROKER" -p "$MQTT_PORT" -t "$MQTT_TOPIC" | while read -r message; do
        # Log received message
        log_message "Received message: '$message'"
        
        # Write message to output file
        echo "$message" > "$OUTPUT_FILE"
        
        # Log file write
        log_message "Updated warning-text.txt with: '$message'"
    done
    
    # If we reach here, the connection was lost
    log_message "Connection lost, attempting to reconnect in 5 seconds..."
    sleep 5
done