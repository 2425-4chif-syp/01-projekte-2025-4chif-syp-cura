# Keycloak Konfigurationsanleitung für CURA

Diese Anleitung beschreibt die vollständige Einrichtung von Keycloak für das CURA Medikamentenverwaltungssystem mit dem **Hybrid-Ansatz**:
- **Keycloak**: Authentifizierung, Autorisierung, Benutzerverwaltung
- **PostgreSQL**: Geschäftsdaten (Patienten, Medikamente, Einnahmen)

## 🎯 Architektur-Überblick

```
┌─────────────┐     JWT Token      ┌──────────────┐
│   Angular   │ ◄────────────────► │   Keycloak   │
│   Frontend  │                     │  (Auth/IAM)  │
└─────────────┘                     └──────────────┘
       │                                     │
       │ HTTP + JWT                          │
       ▼                                     ▼
┌─────────────┐                     ┌──────────────┐
│   .NET API  │ ◄──────────────────►│  PostgreSQL  │
│   Backend   │  patient_id aus JWT │  (Geschäfts- │
└─────────────┘                     │    daten)    │
                                    └──────────────┘
```

### Datenmapping

| Keycloak Attribute | PostgreSQL Tabelle | Zweck |
|-------------------|-------------------|-------|
| `patient_id`      | `patients.id`     | Verknüpfung Benutzer → Patient |
| `caregiver_id`    | `caregivers.id`   | Verknüpfung Benutzer → Pfleger |
| `location_id`     | `locations.id`    | Standort-basierte Zugriffskontrolle |

---

## 📋 Schritt 1: Realm erstellen

1. **Login zu Keycloak Admin Console**:
   - URL: `http://vm12.htl-leonding.ac.at:8080`
   - Username: `admin`
   - Password: [Dein Admin-Passwort]

2. **Neuen Realm erstellen**:
   - Klicke auf Dropdown oben links (neben "Master")
   - Wähle **"Create Realm"**
   - **Realm name**: `cura`
   - **Enabled**: ✅
   - Klicke **"Create"**

---

## 🔐 Schritt 2: Client erstellen

1. **Zu Clients navigieren**:
   - Im Realm `cura` → Linke Navigation → **"Clients"**
   - Klicke **"Create client"**

2. **Client Settings - General Settings**:
   - **Client type**: `OpenID Connect`
   - **Client ID**: `cura-frontend`
   - Klicke **"Next"**

3. **Client Settings - Capability config**:
   - **Client authentication**: `OFF` (Public Client für Angular)
   - **Authorization**: `OFF`
   - **Authentication flow**:
     - ✅ **Standard flow** (Authorization Code Flow)
     - ✅ **Direct access grants** (für Testing mit Postman)
     - ❌ **Implicit flow**
     - ❌ **Service accounts roles**
   - Klicke **"Next"**

4. **Client Settings - Login settings**:
   - **Root URL**: `http://vm12.htl-leonding.ac.at`
   - **Home URL**: `http://vm12.htl-leonding.ac.at`
   - **Valid redirect URIs**:
     ```
     http://localhost:4200/*
     http://vm12.htl-leonding.ac.at/*
     ```
   - **Valid post logout redirect URIs**:
     ```
     http://localhost:4200/*
     http://vm12.htl-leonding.ac.at/*
     ```
   - **Web origins**:
     ```
     http://localhost:4200
     http://vm12.htl-leonding.ac.at
     ```
   - Klicke **"Save"**

5. **Advanced Settings konfigurieren** (Optional aber empfohlen):
   - Im Tab **"Advanced"** des Clients:
   - **Access Token Lifespan**: `15 minutes` (Standard: 5 min)
   - **Client Session Idle**: `30 minutes`
   - **Client Session Max**: `12 hours`

---

## 👥 Schritt 3: Realm Roles erstellen

1. **Zu Realm Roles navigieren**:
   - Linke Navigation → **"Realm roles"**
   - Klicke **"Create role"**

2. **Role: patient**:
   - **Role name**: `patient`
   - **Description**: `Patient role - can only view own medical data`
   - Klicke **"Save"**

3. **Role: caregiver**:
   - **Role name**: `caregiver`
   - **Description**: `Caregiver role - can view and manage patients in their location`
   - Klicke **"Save"**

4. **Role: admin**:
   - **Role name**: `admin`
   - **Description**: `Admin role - full access to all features`
   - Klicke **"Save"**

---

## 🏷️ Schritt 4: User Attributes definieren

Wir erstellen Custom Attributes, die später in den JWT Token gemappt werden.

### Für Patient-Benutzer benötigte Attribute:
- `patient_id` (Integer) → ID aus `patients` Tabelle
- `location_id` (Integer) → ID aus `locations` Tabelle

### Für Caregiver-Benutzer benötigte Attribute:
- `caregiver_id` (Integer) → ID aus `caregivers` Tabelle
- `location_id` (Integer) → ID aus `locations` Tabelle

**Hinweis**: Diese werden beim Erstellen der Benutzer gesetzt (siehe Schritt 6).

---

## 🗺️ Schritt 5: Client Scopes und Mapper konfigurieren

Dies ist der **wichtigste Schritt**, um die Custom Attributes in den JWT Token zu bekommen!

### 5.1 Patient ID Mapper erstellen

1. **Zu Client navigieren**:
   - **Clients** → `cura-frontend` → Tab **"Client scopes"**
   - Klicke auf den **"Dedicated"** Scope (`cura-frontend-dedicated`)

2. **Neuen Mapper erstellen**:
   - Klicke **"Add mapper"** → **"By configuration"**
   - Wähle **"User Attribute"**

3. **Mapper Settings für patient_id**:
   - **Name**: `patient_id mapper`
   - **User Attribute**: `patient_id`
   - **Token Claim Name**: `patient_id`
   - **Claim JSON Type**: `String` (Backend kann zu int parsen)
   - **Add to ID token**: ✅
   - **Add to access token**: ✅
   - **Add to userinfo**: ✅
   - **Multivalued**: ❌
   - Klicke **"Save"**

### 5.2 Caregiver ID Mapper erstellen

Wiederhole die Schritte von 5.1 mit folgenden Werten:
- **Name**: `caregiver_id mapper`
- **User Attribute**: `caregiver_id`
- **Token Claim Name**: `caregiver_id`
- **Claim JSON Type**: `String`
- Rest wie oben ✅

### 5.3 Location ID Mapper erstellen

Wiederhole die Schritte von 5.1 mit folgenden Werten:
- **Name**: `location_id mapper`
- **User Attribute**: `location_id`
- **Token Claim Name**: `location_id`
- **Claim JSON Type**: `String`
- Rest wie oben ✅

### 5.4 Realm Roles Mapper konfigurieren

1. Gehe zu **Client Scopes** → `roles` (vordefiniert)
2. Im Tab **"Mappers"** findest du:
   - `realm roles` Mapper
3. **Bearbeite den Mapper**:
   - **Token Claim Name**: `realm_roles` (das erwartet unser Backend)
   - **Add to ID token**: ✅
   - **Add to access token**: ✅
   - Klicke **"Save"**

---

## 👤 Schritt 6: Test-Benutzer erstellen

### 6.1 Patient-Benutzer erstellen: Max Mustermann

1. **Zu Users navigieren**:
   - Linke Navigation → **"Users"**
   - Klicke **"Create new user"**

2. **User Details**:
   - **Username**: `max.mustermann`
   - **Email**: `max.mustermann@cura.at`
   - **Email verified**: ✅
   - **First name**: `Max`
   - **Last name**: `Mustermann`
   - **Enabled**: ✅
   - Klicke **"Create"**

3. **Credentials setzen**:
   - Im User → Tab **"Credentials"**
   - Klicke **"Set password"**
   - **Password**: `Test1234!`
   - **Password confirmation**: `Test1234!`
   - **Temporary**: ❌ (sonst muss User bei erstem Login ändern)
   - Klicke **"Save"**

4. **Attributes hinzufügen**:
   - Im User → Tab **"Attributes"**
   - Klicke **"Add attribute"**
   
   **Attribute 1**:
   - **Key**: `patient_id`
   - **Value**: `1` (entspricht Max Mustermann in deiner DB)
   
   **Attribute 2**:
   - **Key**: `location_id`
   - **Value**: `1` (Seniorenheim Leonding)
   
   - Klicke **"Save"**

5. **Role zuweisen**:
   - Im User → Tab **"Role mapping"**
   - Klicke **"Assign role"**
   - Filter: **"Filter by realm roles"**
   - Wähle `patient` ✅
   - Klicke **"Assign"**

### 6.2 Caregiver-Benutzer erstellen: Anna Schmidt

1. **User erstellen**:
   - **Username**: `anna.schmidt`
   - **Email**: `anna.schmidt@cura.at`
   - **Email verified**: ✅
   - **First name**: `Anna`
   - **Last name**: `Schmidt`
   - **Enabled**: ✅

2. **Credentials**:
   - **Password**: `Test1234!`
   - **Temporary**: ❌

3. **Attributes**:
   - **Key**: `caregiver_id` → **Value**: `1`
   - **Key**: `location_id` → **Value**: `1`

4. **Roles**:
   - Zuweisen: `caregiver` ✅

### 6.3 Admin-Benutzer erstellen: Admin User

1. **User erstellen**:
   - **Username**: `admin`
   - **Email**: `admin@cura.at`
   - **Email verified**: ✅
   - **First name**: `Admin`
   - **Last name**: `User`
   - **Enabled**: ✅

2. **Credentials**:
   - **Password**: `Admin1234!`
   - **Temporary**: ❌

3. **Attributes**:
   - **Key**: `location_id` → **Value**: `1` (falls benötigt)

4. **Roles**:
   - Zuweisen: `admin` ✅

---

## 🧪 Schritt 7: Token testen

### 7.1 Token über Keycloak UI inspizieren

1. **Zu Client navigieren**:
   - **Clients** → `cura-frontend`
   - Tab **"Client scopes"**
   - Klicke auf **"Evaluate"**

2. **User auswählen**:
   - **User**: `max.mustermann`
   - Tab **"Generated access token"**

3. **Token Content prüfen**:
   ```json
   {
     "exp": 1709823456,
     "iat": 1709822556,
     "iss": "http://vm12.htl-leonding.ac.at:8080/realms/cura",
     "sub": "a1b2c3d4-e5f6-1234-5678-abcdef123456",
     "preferred_username": "max.mustermann",
     "email": "max.mustermann@cura.at",
     "patient_id": "1",
     "location_id": "1",
     "realm_roles": ["patient", "default-roles-cura"],
     ...
   }
   ```

   ✅ **Wichtig**: `patient_id`, `location_id` und `realm_roles` müssen vorhanden sein!

### 7.2 Token per Postman/cURL holen

```bash
curl -X POST "http://vm12.htl-leonding.ac.at:8080/realms/cura/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=cura-frontend" \
  -d "username=max.mustermann" \
  -d "password=Test1234!"
```

**Response**:
```json
{
  "access_token": "eyJhbGci...",
  "expires_in": 900,
  "refresh_token": "eyJhbGci...",
  "token_type": "Bearer"
}
```

**Token dekodieren**:
- Gehe zu [jwt.io](https://jwt.io)
- Füge `access_token` ein
- Prüfe Payload auf `patient_id`, `location_id`, `realm_roles`

---

## 🔧 Schritt 8: Backend-Integration prüfen

### 8.1 appsettings.json prüfen

```json
{
  "Keycloak": {
    "Authority": "http://vm12.htl-leonding.ac.at:8080",
    "Realm": "cura",
    "ClientId": "cura-frontend"
  }
}
```

### 8.2 API mit Token testen

```bash
# Token holen
TOKEN=$(curl -s -X POST "http://vm12.htl-leonding.ac.at:8080/realms/cura/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password" \
  -d "client_id=cura-frontend" \
  -d "username=max.mustermann" \
  -d "password=Test1234!" | jq -r '.access_token')

# API aufrufen
curl -X GET "http://localhost:5000/api/MedicationCalendar/month/1/2025/3" \
  -H "Authorization: Bearer $TOKEN"
```

**Erwartetes Ergebnis**:
- ✅ Status 200 OK mit Kalenderdaten (wenn `patient_id=1`)
- ❌ Status 403 Forbidden (wenn `patient_id` nicht 1 ist)

---

## 📱 Schritt 9: Frontend-Integration

Dein Angular Frontend ist bereits konfiguriert! Prüfe nur:

### environment.ts
```typescript
export const environment = {
  production: false,
  keycloak: {
    issuer: 'http://vm12.htl-leonding.ac.at:8080',
    realm: 'cura',
    clientId: 'cura-frontend',
  },
  apiUrl: 'http://localhost:5000/api'
};
```

### Patient ID aus Token extrahieren

In deinen Angular Services:

```typescript
import { KeycloakService } from 'keycloak-angular';

export class MedicationService {
  constructor(private keycloak: KeycloakService) {}

  async getPatientId(): Promise<number | null> {
    const token = await this.keycloak.getKeycloakInstance().loadUserProfile();
    const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
    
    // @ts-ignore - patient_id ist custom claim
    const patientId = tokenParsed?.patient_id;
    
    return patientId ? parseInt(patientId) : null;
  }

  async getMonthCalendar(year: number, month: number) {
    const patientId = await this.getPatientId();
    if (!patientId) {
      throw new Error('Patient ID not found in token');
    }
    
    return this.http.get(
      `${environment.apiUrl}/MedicationCalendar/month/${patientId}/${year}/${month}`
    );
  }
}
```

---

## 🎨 Schritt 10: Keycloak Theme anpassen (Optional)

1. **Zu Realm Settings navigieren**:
   - Linke Navigation → **"Realm settings"**
   - Tab **"Themes"**

2. **Login Theme**:
   - **Login theme**: `keycloak` (Standard)
   - Für Custom Theme: Erstelle eigenes Theme unter `themes/` im Keycloak-Verzeichnis

3. **Internationalisierung**:
   - Tab **"Localization"**
   - **Internationalization**: ✅
   - **Supported locales**: `de`, `en`
   - **Default locale**: `de`

---

## ✅ Checkliste: Ist alles konfiguriert?

- [ ] Realm `cura` erstellt
- [ ] Client `cura-frontend` mit korrekten Redirect URIs
- [ ] Realm Roles: `patient`, `caregiver`, `admin`
- [ ] Client Scope Mapper für `patient_id`, `caregiver_id`, `location_id`
- [ ] Realm Roles Mapper auf `realm_roles` gesetzt
- [ ] Test-User `max.mustermann` mit `patient_id=1` und Role `patient`
- [ ] Test-User `anna.schmidt` mit `caregiver_id=1` und Role `caregiver`
- [ ] Admin-User mit Role `admin`
- [ ] Token generiert und auf jwt.io geprüft (enthält alle Claims)
- [ ] Backend läuft und akzeptiert Token
- [ ] Frontend holt Token und kann patient_id extrahieren

---

## 🚨 Troubleshooting

### Problem: Claims fehlen im Token

**Lösung**:
1. Prüfe Client Scopes → `cura-frontend-dedicated` → Tab "Mappers"
2. Stelle sicher, dass alle Mapper aktiv sind (grüner Status)
3. Prüfe beim User unter "Attributes", ob `patient_id` etc. gesetzt sind
4. **Wichtig**: Logout + Login erforderlich nach Änderungen!

### Problem: Backend gibt 401 Unauthorized

**Lösung**:
1. Prüfe `appsettings.json`: Authority URL korrekt?
2. Prüfe Token Issuer: Muss mit Authority URL übereinstimmen
3. Prüfe Token Audience: Muss `cura-frontend` enthalten
4. Logs prüfen: `OnAuthenticationFailed` Event zeigt Fehler

### Problem: Backend gibt 403 Forbidden

**Lösung**:
1. Rolle im Token vorhanden? → jwt.io prüfen
2. Endpoint hat `[Authorize]` Attribut?
3. `patient_id` im Token stimmt mit angeforderter patient_id überein?
4. Caregiver/Admin: Ist Role richtig zugewiesen?

### Problem: CORS Fehler im Browser

**Lösung**:
1. Backend CORS Policy prüft `http://vm12.htl-leonding.ac.at`
2. Keycloak Client: "Web origins" enthält Frontend URL
3. Browser Cache leeren

---

## 📚 Weitere Ressourcen

- [Keycloak Documentation](https://www.keycloak.org/documentation)
- [JWT.io Debugger](https://jwt.io)
- [keycloak-angular GitHub](https://github.com/mauriciovigolo/keycloak-angular)

---

## 🎓 Nächste Schritte

1. **Mehr Benutzer anlegen**: Für jeden Patienten aus der Datenbank
2. **Passwort-Reset Flow**: Keycloak bietet "Forgot Password" Feature
3. **Multi-Faktor-Authentifizierung**: Über Keycloak einrichtbar
4. **Session Management**: User können Sessions einsehen und beenden
5. **Audit Logging**: Wer hat wann was gemacht (via Keycloak Events)

---

**Stand**: 2025-03-07  
**Version**: 1.0  
**Autor**: CURA Development Team
