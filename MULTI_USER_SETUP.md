# 🔐 Multi-User Setup für Cura

## ✅ Was wurde geändert?

### 1. Keycloak Konfiguration (`keycloak-config/cura-realm.json`)

**Mapper angepasst:**
- `patient_id` → `patientId` (camelCase für Frontend-Kompatibilität)
- User Attribute: `patientId`
- Token Claim: `patientId`

**User hinzugefügt:**
- ✅ **pali** (Patient ID: 1)
  - Username: `pali`
  - Password: `pali123`
  - Patient ID: `1` (Johann Meier)
  
- ✅ **elisabeth** (Patient ID: 2)
  - Username: `elisabeth`
  - Password: `elisabeth123`
  - Patient ID: `2` (Elisabeth Gruber)

---

## 📋 Import-Anleitung

### Schritt 1: Keycloak Realm neu importieren

```bash
cd keycloak-config

# Realm importieren (überschreibt bestehende Config)
./import-realm.sh
```

**Oder manuell in Keycloak Admin Console:**

1. **Keycloak Admin Console öffnen:** `https://vm12.htl-leonding.ac.at/auth/admin`
2. **Realm "cura" auswählen** (oder löschen und neu erstellen)
3. **Realm settings → Action → Partial import**
4. **Datei auswählen:** `keycloak-config/cura-realm.json`
5. **"Skip" bei Conflicts** falls Realm existiert
6. **Import starten**

### Schritt 2: Client Scope aktivieren

1. **Clients → cura-frontend**
2. **Client scopes** Tab
3. **Add client scope → patient_id**
4. **Assigned type: Default**

### Schritt 3: User-Attribute verifizieren

**Für User "pali":**
1. **Users → pali → Attributes** Tab
2. **Prüfen:** `patientId = 1`
3. Falls nicht vorhanden: **Add attribute**
   - Key: `patientId`
   - Value: `1`

**Für User "elisabeth":**
1. **Users → elisabeth → Attributes** Tab
2. **Prüfen:** `patientId = 2`
3. Falls nicht vorhanden: **Add attribute**
   - Key: `patientId`
   - Value: `2`

---

## 🧪 Testing

### Test 1: User "pali" (Patient 1)

```bash
# Frontend starten
cd web/cura-frontend
npm start

# Browser öffnen
http://localhost:4200

# Login
Username: pali
Password: pali123
```

**Erwartetes Ergebnis:**
- ✅ Console Log: `Angemeldeter Patient ID: 1`
- ✅ Zeigt Medikamentenpläne von **Johann Meier** (Patient 1)
- ✅ 4 Medikamente: Blutdruckmittel, Herzmittel, Aspirin, Magenschutz, Vitamin D3

### Test 2: User "elisabeth" (Patient 2)

```bash
# Gleiche Frontend-Instanz, neu einloggen
# Logout → Login

# Login
Username: elisabeth
Password: elisabeth123
```

**Erwartetes Ergebnis:**
- ✅ Console Log: `Angemeldeter Patient ID: 2`
- ✅ Zeigt Medikamentenpläne von **Elisabeth Gruber** (Patient 2)
- ✅ 4 Medikamente: Metformin (2x), Magenschutz, Paracetamol
- ❌ **NICHT** die Daten von "pali"!

---

## 🔍 Debugging

### Token-Inhalt prüfen

**In Browser Console (F12):**

```javascript
// Keycloak Instance abrufen
const kc = window.ng?.getInjector()?.get('KeycloakService')?.keycloakInstance;

// Token anzeigen
console.log(kc?.tokenParsed);
```

**Expected Output:**

```json
{
  "sub": "...",
  "preferred_username": "elisabeth",
  "given_name": "Elisabeth",
  "patientId": 2,  // ← Muss vorhanden sein!
  "realm_access": { ... }
}
```

### Falls `patientId` fehlt:

1. **Client Scope prüfen:** Ist `patient_id` Scope beim Client aktiv?
2. **Mapper prüfen:** Ist `patient-id-mapper` korrekt konfiguriert?
3. **User-Attribut prüfen:** Hat der User das Attribut `patientId` gesetzt?
4. **Token neu laden:** Logout → Login (Token wird neu generiert)

---

## 🎯 Zusammenfassung der Patient-IDs

| User | Password | Patient ID | Name | Medikamente |
|------|----------|------------|------|-------------|
| `pali` | `pali123` | `1` | Johann Meier | Blutdruck, Herz, Aspirin, Magenschutz, Vitamin D3 |
| `elisabeth` | `elisabeth123` | `2` | Elisabeth Gruber | Metformin (2x), Magenschutz, Paracetamol |

---

## 🚀 Weitere User hinzufügen

### Variante A: Über Keycloak Admin Console

1. **Users → Add user**
   ```
   Username: neueruser
   Email: user@email.at
   First name: Vorname
   Last name: Nachname
   ```

2. **Credentials → Set password**
   ```
   Password: password123
   Temporary: OFF
   ```

3. **Role mapping → Assign role**
   ```
   patient ← Rolle zuweisen
   ```

4. **Attributes → Add attribute**
   ```
   Key: patientId
   Value: <Patient-ID aus Datenbank>
   ```

### Variante B: Import über JSON

Füge in `keycloak-config/cura-realm.json` hinzu:

```json
{
  "username": "franz",
  "enabled": true,
  "emailVerified": true,
  "firstName": "Franz",
  "lastName": "Wagner",
  "email": "franz.wagner@email.at",
  "attributes": {
    "patientId": ["3"]
  },
  "credentials": [
    {
      "type": "password",
      "value": "franz123",
      "temporary": false
    }
  ],
  "realmRoles": ["patient"]
}
```

Dann Realm neu importieren!

---

## ✅ Checkliste

- [x] Keycloak Config aktualisiert (`patientId` statt `patient_id`)
- [x] User "pali" mit `patientId = 1`
- [x] User "elisabeth" mit `patientId = 2`
- [ ] Realm in Keycloak importiert
- [ ] Client Scope `patient_id` beim Client aktiviert
- [ ] Mit "pali" getestet → zeigt Patient 1 Daten
- [ ] Mit "elisabeth" getestet → zeigt Patient 2 Daten
- [ ] Console Log zeigt korrekte Patient-ID
- [ ] Multi-User funktioniert! 🎉

---

## 🐛 Probleme?

### Problem: Token zeigt kein `patientId`

**Lösung:**
1. Client Scope aktivieren (siehe Schritt 2)
2. Logout + Login (Token erneuern)
3. User-Attribut prüfen

### Problem: Beide User sehen gleiche Daten

**Lösung:**
- Prüfe Console Log: Zeigt es die richtige Patient-ID?
- Wenn beide User `patientId = 1` im Token haben → User-Attribute falsch gesetzt

### Problem: Import schlägt fehl

**Lösung:**
```bash
# Prüfe JSON-Syntax
cat keycloak-config/cura-realm.json | jq .

# Falls Fehler: JSON manuell in Editor prüfen
```

---

**Erstellt:** 24. Februar 2026  
**Autor:** GitHub Copilot  
**Projekt:** Cura Medikamenten-Management-System
