#!/bin/bash
# Keycloak Realm Import Script
# This script imports the cura realm configuration into Keycloak

set -e

echo "🔧 CURA Keycloak Realm Import"
echo "================================"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Error: Docker is not running"
    exit 1
fi

# Check if keycloak container exists
if ! docker ps | grep -q cura_keycloak; then
    echo "❌ Error: Keycloak container is not running"
    echo "   Start with: cd database && docker compose up -d"
    exit 1
fi

echo "⏳ Waiting for Keycloak to be ready (90 seconds)..."
sleep 90

echo "🔍 Checking if realm already exists..."

# Get admin token
TOKEN=$(docker exec cura_keycloak /opt/keycloak/bin/kcadm.sh config credentials \
  --server http://localhost:8080 \
  --realm master \
  --user admin \
  --password admin 2>&1 | grep -o 'token=[^,]*' | cut -d'=' -f2 || echo "")

# Check if realm exists
REALM_EXISTS=$(docker exec cura_keycloak curl -s \
  -H "Authorization: Bearer $TOKEN" \
  http://localhost:8080/admin/realms/cura 2>/dev/null | grep -o '"realm":"cura"' || echo "")

if [ -n "$REALM_EXISTS" ]; then
    echo "✅ Realm 'cura' already exists"
    echo "   To reimport, delete the realm first in Admin Console"
    exit 0
fi

echo "📦 Importing realm configuration..."

# Import realm
docker exec cura_keycloak /opt/keycloak/bin/kc.sh import \
  --file /opt/keycloak/data/import/cura-realm.json \
  --override false

if [ $? -eq 0 ]; then
    echo "✅ Realm 'cura' imported successfully!"
    echo ""
    echo "🎉 Setup complete!"
    echo "   Access Keycloak Admin: http://vm12.htl-leonding.ac.at/auth/admin/"
    echo "   Username: admin"
    echo "   Password: admin"
    echo ""
    echo "   Test User:"
    echo "   Username: pali"
    echo "   Password: pali123"
else
    echo "❌ Failed to import realm"
    echo "   Check logs: docker logs cura_keycloak"
    exit 1
fi
