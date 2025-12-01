#!/bin/bash
# Keycloak Realm Export Script
# This script exports the current cura realm configuration

set -e

echo "🔧 CURA Keycloak Realm Export"
echo "================================"

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    echo "❌ Error: Docker is not running"
    exit 1
fi

# Check if keycloak container exists
if ! docker ps | grep -q cura_keycloak; then
    echo "❌ Error: Keycloak container is not running"
    exit 1
fi

echo "📦 Exporting realm 'cura'..."

# Export realm
docker exec cura_keycloak /opt/keycloak/bin/kc.sh export \
  --realm cura \
  --file /opt/keycloak/data/import/cura-realm-export.json \
  --users realm_file

if [ $? -eq 0 ]; then
    # Copy exported file to host
    docker cp cura_keycloak:/opt/keycloak/data/import/cura-realm-export.json \
      ./cura-realm.json
    
    echo "✅ Realm exported successfully!"
    echo "   File: keycloak-config/cura-realm.json"
    echo ""
    echo "📝 Next steps:"
    echo "   1. Review the exported file"
    echo "   2. git add keycloak-config/cura-realm.json"
    echo "   3. git commit -m 'Update Keycloak realm configuration'"
    echo "   4. git push"
else
    echo "❌ Failed to export realm"
    exit 1
fi
