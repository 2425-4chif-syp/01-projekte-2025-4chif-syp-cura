# Keycloak Setup Guide - AKTUALISIERT

## 🎯 Übersicht

Dieses Projekt verwendet **Keycloak 25.0.6** für Authentication und Authorization.
Die Realm-Konfiguration ist **im Git gespeichert** und wird automatisch importiert.

## 🚀 Quick Start

### Erstmaliges Setup

```bash
# 1. Container starten
cd database
docker compose up -d

# 2. Warte 90 Sekunden für Keycloak Startup
sleep 90

# 3. Realm importieren
cd ../keycloak-config
chmod +x *.sh
./import-realm.sh
```

### Nach Git Pull

```bash
# Prüfe ob Realm existiert
curl http://localhost:8180/auth/realms/cura

# Falls 404 (Realm fehlt):
cd keycloak-config
./import-realm.sh
```

## 📋 Realm Konfiguration

### Realm: `cura`

**Location:** `keycloak-config/cura-realm.json` (✅ im Git!)

**Enthält:**
- ✅ Client `cura-frontend` (Public OpenID Connect)
- ✅ Test User `pali` / `pali123`
- ✅ Rollen: `user`, `patient`, `caregiver`
- ✅ Client Scopes: `patient_id`, `caregiver_id`

### Zugriff

- **Admin Console:** http://vm12.htl-leonding.ac.at/auth/admin/
  - User: `admin`
  - Pass: `admin`

- **Realm URL:** http://vm12.htl-leonding.ac.at/auth/realms/cura

## 🔧 Realm Management

### Realm Änderungen ins Git übernehmen

```bash
# 1. Änderungen in Admin Console machen
# 2. Realm exportieren
cd keycloak-config
./export-realm.sh

# 3. Committen
git add cura-realm.json
git commit -m "Update Keycloak realm config"
git push
```

### Realm neu importieren (z.B. nach Volume-Verlust)

```bash
cd keycloak-config
./import-realm.sh
```

## ⚠️ Wichtige Regeln für das Team

### ❌ NIEMALS verwenden:

```bash
docker compose down -v              # Löscht Volumes!
docker volume rm database_postgres_data  # Löscht DB mit Realm!
docker system prune --volumes       # Löscht ALLES!
```

**Warum?** Diese Befehle löschen die Postgres-Datenbank wo Keycloak seine Realms speichert!

### ✅ Stattdessen verwenden:

```bash
docker compose down                 # Container stoppen (Daten bleiben)
docker compose restart              # Container neu starten
docker compose up -d                # Container starten
```

## 🐛 Troubleshooting

### Problem: "Realm not enabled"

**Lösung:**
```bash
# In Admin Console:
# Realm Settings → General → Enabled: ON → Save
```

### Problem: Realm wurde gelöscht

**Ursache:** Jemand hat `docker compose down -v` oder `docker volume rm` verwendet

**Lösung:**
```bash
cd keycloak-config
./import-realm.sh
```

### Problem: Import schlägt fehl

**Prüfen:**
```bash
# Logs checken
docker logs cura_keycloak

# Realm manuell löschen (falls existiert)
# Admin Console → Realm Settings → Action → Delete

# Dann nochmal importieren
./import-realm.sh
```

### Problem: Keycloak startet nicht

**Prüfen:**
```bash
# 1. Keycloak DB existiert?
docker exec -it cura_postgres psql -U admin -l | grep keycloak

# Falls nicht:
docker exec -it cura_postgres psql -U admin -d postgres -c "CREATE DATABASE keycloak;"

# 2. Keycloak neu starten
docker compose restart keycloak
sleep 90
docker logs cura_keycloak
```

## 📚 Datei-Struktur

```
cura-project/
├── keycloak-config/           ← ✅ IM GIT!
│   ├── cura-realm.json        ← Realm Export
│   ├── import-realm.sh        ← Import Script
│   ├── export-realm.sh        ← Export Script
│   └── README.md              ← Dokumentation
├── database/
│   ├── docker-compose.yaml    ← Volume Mount für Import
│   └── init.sql               ← Erstellt keycloak DB
└── web/cura-frontend/
    └── src/environments/
        ├── environment.ts
        └── environment.prod.ts
```

## 🔐 Produktions-Sicherheit

⚠️ **Vor Production Deployment:**

1. **Admin Passwort ändern:**
   ```bash
   # In Admin Console:
   # Users → admin → Credentials → Reset Password
   ```

2. **SSL erzwingen:**
   ```yaml
   # In cura-realm.json:
   "sslRequired": "all"  # statt "external"
   ```

3. **Environment Variables:**
   ```bash
   # database/.env erstellen (NICHT in Git!)
   KEYCLOAK_ADMIN=admin
   KEYCLOAK_ADMIN_PASSWORD=<starkes-passwort>
   ```

4. **Weitere Settings:**
   - E-Mail Server konfigurieren (für Password Reset)
   - 2FA aktivieren für Admin-Accounts
   - Brute Force Protection Settings anpassen
   - Session Timeouts konfigurieren

## 🧪 Testing

### Realm Endpoint testen

```bash
curl http://vm12.htl-leonding.ac.at/auth/realms/cura/.well-known/openid-configuration
```

Sollte JSON mit `issuer`, `authorization_endpoint`, etc. zurückgeben.

### Login Flow testen

```bash
# 1. Frontend öffnen
open http://localhost:4200

# 2. Sollte redirecten zu Keycloak Login
# 3. Login mit: pali / pali123
# 4. Sollte zurück zur App redirecten
```

## 📖 Weitere Dokumentation

- [Keycloak Docs](https://www.keycloak.org/docs/latest/)
- [Keycloak Import/Export](https://www.keycloak.org/docs/latest/server_admin/#_export_import)
- [keycloak-angular GitHub](https://github.com/mauriciovigolo/keycloak-angular)

## 🎯 Zusammenfassung

**Das Wichtigste:**

1. ✅ Realm Config ist **im Git** (`keycloak-config/cura-realm.json`)
2. ✅ Bei Datenverlust: `./import-realm.sh` ausführen
3. ❌ **NIEMALS** `docker compose down -v` verwenden!
4. ✅ Nach Realm-Änderungen: `./export-realm.sh` und committen
5. ✅ Nach `git pull`: Prüfen ob Realm noch existiert

**Bei Fragen oder Problemen:**
- Siehe `keycloak-config/README.md`
- Check Docker Logs: `docker logs cura_keycloak`
- Frage im Team Chat
