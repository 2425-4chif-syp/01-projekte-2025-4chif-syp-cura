#!/bin/bash
# RFID Tag Scan Simulator
# Simuliert einen RFID-Scan für Testzwecke

BACKEND_URL="https://vm12.htl-leonding.ac.at/api/RfidChips/scan"
CHIP_ID="ABC12345"  # Beispiel RFID Chip ID - anpassen wenn nötig
EVENT="scanned"

echo "🏷️  Simuliere RFID-Scan für Donnerstag, 27. Februar 2026"
echo "Backend: $BACKEND_URL"
echo "Chip ID: $CHIP_ID"
echo "Event: $EVENT"
echo ""

# JSON Body erstellen
JSON_BODY=$(cat <<EOF
{
  "chipId": "$CHIP_ID",
  "event": "$EVENT",
  "timestamp": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")"
}
EOF
)

echo "Sende Request..."
echo "Body: $JSON_BODY"
echo ""

# POST Request senden
RESPONSE=$(curl -k -s -w "\nHTTP_CODE:%{http_code}" \
  -X POST \
  -H "Content-Type: application/json" \
  -d "$JSON_BODY" \
  "$BACKEND_URL")

# HTTP Status Code extrahieren
HTTP_CODE=$(echo "$RESPONSE" | grep "HTTP_CODE:" | cut -d: -f2)
BODY=$(echo "$RESPONSE" | sed '/HTTP_CODE:/d')

echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"
echo "📡 Response:"
echo "HTTP Status: $HTTP_CODE"
echo "Body: $BODY"
echo "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━"

if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    echo "✅ RFID-Scan erfolgreich!"
else
    echo "❌ RFID-Scan fehlgeschlagen (HTTP $HTTP_CODE)"
fi
