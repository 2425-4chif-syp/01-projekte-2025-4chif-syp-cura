# Keycloak Configuration

Dieses Verzeichnis enthält die Keycloak Realm-Konfiguration für das CURA Medication System.

## 📁 Dateien

- **`cura-realm.json`** - Keycloak Realm Export (✅ im Git)
- **`import-realm.sh`** - Script zum Importieren des Realms
- **`export-realm.sh`** - Script zum Exportieren des Realms

## 🚀 Quick Start (Nach Git Clone/Pull)

```bash
# 1. Container starten
cd database
docker compose up -d

# 2. Realm importieren (nach 90 Sekunden)
cd ../keycloak-config
./import-realm.sh
```

## 📋 Realm Inhalt

### Client: `cura-frontend`
- **Type:** Public Client (OpenID Connect)
- **Redirect URIs:**
  - `https://vm12.htl-leonding.ac.at/*`
  - `http://localhost:4200/*`

### Rollen
- `user` - Standard Benutzer
- `patient` - Patient
- `caregiver` - Pflegepersonal

### Test User
- **Username:** `pali`
- **Password:** `pali123`
- **Rollen:** `user`, `patient`

### Client Scopes
- `patient_id` - Fügt Patient ID zum Token hinzu
- `caregiver_id` - Fügt Caregiver ID zum Token hinzu

## 🔧 Realm Aktualisieren

### 1. Änderungen in Keycloak Admin Console machen

Öffne: `http://vm12.htl-leonding.ac.at/auth/admin/`
- Login: `admin` / `admin`
- Mache deine Änderungen (User, Clients, Rollen, etc.)

### 2. Realm exportieren

```bash
cd keycloak-config
./export-realm.sh
```

### 3. Ins Git committen

```bash
git add cura-realm.json
git commit -m "Update Keycloak realm configuration"
git push
```

## ⚠️ Wichtig für das Team

### ❌ NIEMALS verwenden:
```bash
docker compose down -v        # Löscht Volumes!
docker volume rm database_postgres_data  # Löscht alle Daten!
docker system prune --volumes # Löscht alles!
```

### ✅ Stattdessen verwenden:
```bash
docker compose down           # Container stoppen (Daten bleiben)
docker compose restart        # Container neu starten (Daten bleiben)
```

## 🔄 Workflow für Team-Mitglieder

### Nach `git pull`:

```bash
# 1. Prüfe ob Realm existiert
curl http://localhost:8180/auth/realms/cura

# Wenn 404:
cd keycloak-config
./import-realm.sh
```

## 🧪 Realm Testen

```bash
# 1. Realm Endpoint prüfen
curl http://vm12.htl-leonding.ac.at/auth/realms/cura/.well-known/openid-configuration

# 2. Admin Console öffnen
open http://vm12.htl-leonding.ac.at/auth/admin/

# 3. Test Login in der App
# - Öffne http://localhost:4200
# - Login mit pali / pali123
```

## 📚 Weitere Dokumentation

- [Keycloak Docs](https://www.keycloak.org/docs/latest/)
- [OpenID Connect](https://openid.net/connect/)
- [keycloak-angular](https://github.com/mauriciovigolo/keycloak-angular)

## 🐛 Troubleshooting

### Problem: "Realm not enabled"
```bash
# Lösung: Realm in Admin Console aktivieren
# Realm Settings → General → Enabled: ON → Save
```

### Problem: Realm Import schlägt fehl
```bash
# Check Keycloak Logs
docker logs cura_keycloak

# Realm manuell löschen
# Admin Console → Realm Settings → Action → Delete
# Dann import-realm.sh nochmal ausführen
```

### Problem: Volumes wurden gelöscht
```bash
# 1. Container neu starten
cd database
docker compose up -d

# 2. Warte 90 Sekunden
sleep 90

# 3. Realm importieren
cd ../keycloak-config
./import-realm.sh
```

## 🔐 Sicherheit

⚠️ **Produktionsumgebung:**
- Ändere Admin-Passwort: `admin` → starkes Passwort
- Aktiviere HTTPS-Only
- Konfiguriere E-Mail Server für Password Reset
- Aktiviere 2FA
- Review Security Settings in Realm

## 📝 Notizen

- Realm-Daten werden in Postgres DB `keycloak` gespeichert
- Docker Volume: `database_postgres_data`
- Keycloak läuft auf Port 8180 (internal HTTP)
- Nginx proxied von HTTPS → HTTP
