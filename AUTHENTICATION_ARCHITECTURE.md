# CURA Authentication Architecture - Hybrid Approach

## 🎯 Übersicht

Das CURA-System verwendet einen **Hybrid-Ansatz** für Authentifizierung und Datenverwaltung:

```
┌─────────────────────────────────────────────────────────────┐
│                      HYBRID ARCHITECTURE                     │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  Keycloak (Identity & Access Management)                     │
│  ├─ Benutzer-Authentifizierung (Login/Logout)               │
│  ├─ Rollen-Verwaltung (patient, caregiver, admin)           │
│  ├─ JWT Token Ausstellung                                    │
│  └─ Custom Claims (patient_id, caregiver_id, location_id)   │
│                                                               │
│  PostgreSQL (Business Data)                                  │
│  ├─ Patienten-Stammdaten                                     │
│  ├─ Medikamentenpläne                                        │
│  ├─ Einnahme-Historie                                        │
│  ├─ Pflegepersonal-Daten                                     │
│  └─ Standort-Informationen                                   │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## 🔗 Datenmapping

| Keycloak                | PostgreSQL              | Zweck                          |
|------------------------|-------------------------|--------------------------------|
| User + `patient_id`    | `patients.id`           | Verknüpfung User → Patient     |
| User + `caregiver_id`  | `caregivers.id`         | Verknüpfung User → Pfleger     |
| User + `location_id`   | `locations.id`          | Standort-Zugriffskontrolle     |
| Realm Role `patient`   | -                       | Rolle für Patientenansicht     |
| Realm Role `caregiver` | -                       | Rolle für Pflegeransicht       |
| Realm Role `admin`     | -                       | Rolle für Administratoren      |

## 🔐 Authentication Flow

```
1. User öffnet Frontend (Angular)
   ↓
2. Keycloak prüft Session, ggf. Redirect zu Login
   ↓
3. User gibt Credentials ein (Username + Password)
   ↓
4. Keycloak validiert Credentials
   ↓
5. Keycloak erstellt JWT Token mit Claims:
   - preferred_username
   - email
   - patient_id (Custom Claim)
   - location_id (Custom Claim)
   - realm_roles: ["patient"]
   ↓
6. Frontend erhält Token und speichert es
   ↓
7. Frontend extrahiert patient_id aus Token
   ↓
8. Frontend ruft Backend API auf:
   GET /api/MedicationCalendar/month/{patient_id}/2025/3
   Header: Authorization: Bearer <token>
   ↓
9. Backend validiert Token bei Keycloak
   ↓
10. Backend extrahiert patient_id aus Token
   ↓
11. Backend prüft: Token patient_id == URL patient_id?
    - JA → Daten aus PostgreSQL laden
    - NEIN → 403 Forbidden
   ↓
12. Backend filtert Daten nach patient_id
   ↓
13. Frontend zeigt nur Daten des eingeloggten Users
```

## 🛡️ Sicherheitskonzept

### Patient-Rolle
- **Kann sehen**: Nur eigene Medikamente und Einnahmen
- **Backend-Prüfung**: `TokenHelper.GetPatientId(User) == patientId`
- **Beispiel**:
  - User: `max.mustermann` mit `patient_id=1`
  - Darf zugreifen: `/api/MedicationCalendar/month/1/...` ✅
  - Darf NICHT zugreifen: `/api/MedicationCalendar/month/2/...` ❌

### Caregiver-Rolle
- **Kann sehen**: Alle Patienten am gleichen Standort
- **Backend-Prüfung**: 
  ```csharp
  var isCaregiver = TokenHelper.HasRole(User, "caregiver");
  var locationId = TokenHelper.GetLocationId(User);
  // Filter: patient.location_id == locationId
  ```

### Admin-Rolle
- **Kann sehen**: Alle Daten aller Standorte
- **Backend-Prüfung**: `TokenHelper.HasRole(User, "admin")`

## 📦 Backend Implementation

### Program.cs - JWT Authentication
```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{keycloakUrl}/realms/{realm}";
        options.Audience = "cura-frontend";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            RoleClaimType = "realm_roles"
        };
    });
```

### Controller - Authorization
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize] // Require authentication
public class MedicationCalendarController : ControllerBase
{
    [HttpGet("month/{patientId}/{year}/{month}")]
    public async Task<IActionResult> GetMonthCalendar(int patientId, ...)
    {
        // Security Check
        var tokenPatientId = TokenHelper.GetPatientId(User);
        var isCaregiver = TokenHelper.HasRole(User, "caregiver");
        var isAdmin = TokenHelper.HasRole(User, "admin");
        
        if (!isCaregiver && !isAdmin && tokenPatientId != patientId)
        {
            return Forbid();
        }
        
        // Load data...
    }
}
```

## 📱 Frontend Implementation

### Token Service
```typescript
export class TokenService {
  constructor(private keycloak: KeycloakService) {}

  async getPatientId(): Promise<number | null> {
    const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
    // @ts-ignore
    const patientId = tokenParsed?.patient_id;
    return patientId ? parseInt(patientId) : null;
  }

  async hasRole(role: 'patient' | 'caregiver' | 'admin'): Promise<boolean> {
    const tokenParsed = this.keycloak.getKeycloakInstance().tokenParsed;
    // @ts-ignore
    const roles = tokenParsed?.realm_roles || [];
    return roles.includes(role);
  }
}
```

### API Service
```typescript
export class MedicationService {
  constructor(
    private http: HttpClient,
    private tokenService: TokenService
  ) {}

  async getMonthCalendar(year: number, month: number) {
    const patientId = await this.tokenService.getPatientId();
    
    if (!patientId) {
      throw new Error('Not logged in as patient');
    }
    
    return this.http.get(
      `${environment.apiUrl}/MedicationCalendar/month/${patientId}/${year}/${month}`
    );
  }
}
```

## 🗄️ Datenbank-Schema (Auszug)

```sql
-- Standorte
CREATE TABLE locations (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL
);

-- Patienten
CREATE TABLE patients (
    id SERIAL PRIMARY KEY,
    location_id INTEGER REFERENCES locations(id),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    date_of_birth DATE,
    -- Keycloak User wird mit patient_id verknüpft
);

-- Pflegepersonal
CREATE TABLE caregivers (
    id SERIAL PRIMARY KEY,
    location_id INTEGER REFERENCES locations(id),
    first_name VARCHAR(100),
    last_name VARCHAR(100),
    -- Keycloak User wird mit caregiver_id verknüpft
);

-- Medikamentenpläne
CREATE TABLE medication_plans (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER REFERENCES patients(id),
    medication_id INTEGER REFERENCES medications(id),
    caregiver_id INTEGER REFERENCES caregivers(id),
    -- Immer mit patient_id gefiltert!
);

-- Einnahmen
CREATE TABLE medication_intakes (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER REFERENCES patients(id),
    medication_plan_id INTEGER REFERENCES medication_plans(id),
    intake_time TIMESTAMP,
    -- Immer mit patient_id gefiltert!
);
```

## 🚀 Setup-Schritte

### 1. Keycloak konfigurieren
Siehe `KEYCLOAK_CONFIGURATION_GUIDE.md` für detaillierte Anleitung:
- Realm `cura` erstellen
- Client `cura-frontend` konfigurieren
- Roles definieren (patient, caregiver, admin)
- User Attributes Mapper (patient_id, caregiver_id, location_id)
- Test-Benutzer anlegen

### 2. Backend konfigurieren
```bash
cd backend/WebApi
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

`appsettings.json`:
```json
{
  "Keycloak": {
    "Authority": "http://vm12.htl-leonding.ac.at:8080",
    "Realm": "cura",
    "ClientId": "cura-frontend"
  }
}
```

### 3. Frontend konfigurieren
```bash
cd web/cura-frontend
npm install keycloak-angular keycloak-js
```

`environment.ts`:
```typescript
export const environment = {
  keycloak: {
    issuer: 'http://vm12.htl-leonding.ac.at:8080',
    realm: 'cura',
    clientId: 'cura-frontend',
  }
};
```

## ✅ Vorteile des Hybrid-Ansatzes

| Aspekt | Vorteil |
|--------|---------|
| **Sicherheit** | Keycloak ist auf Security spezialisiert (2FA, Session Management, etc.) |
| **Skalierbarkeit** | Geschäftsdaten in PostgreSQL, Auth in Keycloak - getrennte Optimierung |
| **Flexibilität** | Geschäftslogik-Änderungen unabhängig von Auth-System |
| **Standard-Konformität** | OAuth 2.0 / OpenID Connect (OIDC) Standard |
| **Wartbarkeit** | Klare Trennung: Auth ↔ Business Logic |
| **User Experience** | Single Sign-On (SSO) möglich, zentrale Session-Verwaltung |
| **Entwicklung** | Bewährte Libraries (keycloak-angular, JWT Bearer) |

## 📝 Best Practices

1. **Token-Validierung**: Backend MUSS Token bei jedem Request validieren
2. **patient_id Prüfung**: Backend MUSS prüfen, ob Token-patient_id mit URL-patient_id übereinstimmt
3. **Fehlerbehandlung**: Klare 401 (Unauthorized) vs 403 (Forbidden) Unterscheidung
4. **Token-Refresh**: Frontend nutzt Refresh Token für automatische Erneuerung
5. **HTTPS in Produktion**: Keycloak `RequireHttpsMetadata = true` setzen
6. **Logging**: Failed Authentication Attempts loggen (Security Audit)
7. **Token-Lebensdauer**: Kurz halten (15 Min Access Token, 24h Refresh Token)

## 📚 Dokumentation

- [Keycloak Configuration Guide](./KEYCLOAK_CONFIGURATION_GUIDE.md)
- [Backend API Documentation](./backend/README.md)
- [Frontend Setup](./web/cura-frontend/README.md)

---

**Stand**: 2025-03-07  
**Version**: 1.0
