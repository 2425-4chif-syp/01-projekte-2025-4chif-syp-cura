# 🔐 Keycloak Integration - Setup Anleitung

## ✅ Was bereits gemacht wurde:

1. ✅ **docker-compose.yaml** - Keycloak Container hinzugefügt
2. ✅ **init.sql** - Keycloak Datenbank erstellt
3. ✅ **environment.ts** - Keycloak Config für Development
4. ✅ **environment.prod.ts** - Keycloak Config für Production

---

## 📋 Was du noch machen musst:

### **1. Keycloak starten (auf dem Server)**

````bash
# SSH zum Server
ssh curaadm@vm12.htl-leonding.ac.at

# Zum Projekt-Verzeichnis
cd ~/cura-project/database

# Keycloak Container starten
docker compose up -d keycloak

# Logs checken
docker compose logs -f keycloak

# Warte bis du siehst:
# "Keycloak 23.0 started in XXXms"
````

---

### **2. Keycloak Admin Console öffnen**

```
URL: http://vm12.htl-leonding.ac.at:8180
Login: admin / admin
```

---

### **3. Realm "cura" erstellen**

1. Oben links: **Master** Dropdown → **Create Realm**
2. Name: `cura`
3. **Create** klicken

---

### **4. Client "cura-frontend" erstellen**

1. Im Realm "cura": **Clients** → **Create client**
2. **Client ID**: `cura-frontend`
3. **Client type**: `OpenID Connect`
4. **Next** klicken
5. **Client authentication**: `OFF` (Public Client)
6. **Authorization**: `OFF`
7. **Next** klicken
8. **Valid redirect URIs**:
   ```
   http://localhost:4200/*
   http://vm12.htl-leonding.ac.at/*
   ```
9. **Web origins**:
   ```
   http://localhost:4200
   http://vm12.htl-leonding.ac.at
   ```
10. **Save** klicken

---

### **5. Rollen erstellen**

1. **Realm roles** → **Create role**
2. Erstelle folgende Rollen:
   - `patient` (für Patienten)
   - `caregiver` (für Pflegepersonal)
   - `admin` (für Administratoren)

---

### **6. Test-User erstellen**

1. **Users** → **Add user**
2. **Username**: `pali`
3. **Email**: `pali@cura.at`
4. **First name**: `Pali`
5. **Last name**: `Test`
6. **Email verified**: `ON`
7. **Create** klicken

**Passwort setzen:**
1. Tab **Credentials** → **Set password**
2. **Password**: `pali123`
3. **Temporary**: `OFF`
4. **Save**

**Rolle zuweisen:**
1. Tab **Role mapping** → **Assign role**
2. Rolle `patient` auswählen
3. **Assign** klicken

---

### **7. Angular Keycloak Library installieren**

````bash
# Auf deinem PC im Frontend-Verzeichnis
cd web/cura-frontend

# Keycloak Angular installieren
npm install keycloak-angular keycloak-js
````

---

### **8. Keycloak Init File erstellen**

Erstelle: `src/app/auth/keycloak-init.factory.ts`

````typescript
import { KeycloakService } from 'keycloak-angular';
import { environment } from '../environments/environment';

export function initializeKeycloak(keycloak: KeycloakService) {
  return () =>
    keycloak.init({
      config: {
        url: environment.keycloak.url,
        realm: environment.keycloak.realm,
        clientId: environment.keycloak.clientId,
      },
      initOptions: {
        onLoad: 'login-required',
        checkLoginIframe: false,
      },
      enableBearerInterceptor: true,
      bearerPrefix: 'Bearer',
      bearerExcludedUrls: ['/assets']
    });
}
````

---

### **9. App Config anpassen**

Erstelle: `src/app/app.config.ts` (falls nicht vorhanden)

````typescript
import { ApplicationConfig, APP_INITIALIZER } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { KeycloakAngularModule, KeycloakService } from 'keycloak-angular';
import { initializeKeycloak } from './auth/keycloak-init.factory';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    KeycloakService,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      multi: true,
      deps: [KeycloakService]
    }
  ]
};
````

---

### **10. Main.ts anpassen**

Ändere `src/main.ts`:

````typescript
import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';

bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
````

---

### **11. User-Info im Component anzeigen**

Ändere `src/app/app.component.ts`:

````typescript
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { KeycloakService } from 'keycloak-angular';
// ... andere imports

export class AppComponent implements OnInit {
  userName: string = 'User';
  userRoles: string[] = [];
  
  constructor(
    private keycloak: KeycloakService,
    private calendarService: CalendarService,
    private medicationPlanService: MedicationPlanService
  ) {}

  async ngOnInit() {
    // User-Info von Keycloak laden
    try {
      const profile = await this.keycloak.loadUserProfile();
      this.userName = profile.firstName || 'User';
      this.userRoles = this.keycloak.getUserRoles();
    } catch (error) {
      console.error('Fehler beim Laden des User-Profils:', error);
    }

    // Rest deines Codes...
    this.currentMonth = this.calendarService.getCurrentMonth();
    this.loadCalendar();
    this.loadMedicationPlans();
  }

  logout() {
    this.keycloak.logout(window.location.origin);
  }
  
  // ... restliche Methoden
}
````

---

### **12. Logout Button im HTML hinzufügen**

Ändere `src/app/app.component.html`:

````html
<header>
  <h1>CURA</h1>
  <div class="user-info">
    <i class="fa-solid fa-user"></i>
    <span>{{ userName }}, {{ currentDate }}</span>
    <button (click)="logout()" class="btn-logout" title="Abmelden">
      <i class="fa-solid fa-right-from-bracket"></i>
    </button>
  </div>
</header>
````

---

### **13. CSS für Logout Button**

Füge in `src/app/app.component.css` hinzu:

````css
.btn-logout {
  background: transparent;
  border: none;
  color: var(--text-secondary);
  cursor: pointer;
  padding: var(--space-sm);
  border-radius: var(--radius-sm);
  transition: var(--transition-fast);
  margin-left: var(--space-sm);
}

.btn-logout:hover {
  background: var(--bg-hover);
  color: var(--primary-green);
}
````

---

### **14. Nginx Proxy für Keycloak (auf Server)**

````bash
ssh curaadm@vm12.htl-leonding.ac.at

sudo nano /etc/nginx/sites-available/default
````

Füge nach dem `/api/` Block hinzu:

````nginx
    # Keycloak
    location /auth/ {
        proxy_pass http://localhost:8180/;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
        proxy_buffer_size 128k;
        proxy_buffers 4 256k;
        proxy_busy_buffers_size 256k;
    }
````

````bash
# Nginx neu laden
sudo nginx -t
sudo systemctl reload nginx
````

---

### **15. Testen!**

````bash
# Frontend starten (lokal)
cd web/cura-frontend
ng serve

# Browser öffnen
http://localhost:4200
````

**Was sollte passieren:**
1. ✅ Du wirst automatisch zu Keycloak Login redirected
2. ✅ Login mit `pali` / `pali123`
3. ✅ Redirect zurück zur App
4. ✅ Header zeigt "Pali" statt "User"
5. ✅ Logout Button funktioniert

---

### **16. Production Build & Deploy**

````bash
# Production Build
ng build --configuration production

# Auf Server kopieren
scp -r dist/cura-frontend/browser/* curaadm@vm12.htl-leonding.ac.at:~/cura-project/web/cura-frontend/dist/cura-frontend/browser/
````

---

## 🎨 Optional: Keycloak Theme anpassen

Erstelle auf deinem PC:

````
keycloak-themes/
└── cura/
    └── login/
        ├── theme.properties
        └── resources/
            └── css/
                └── login.css
````

**theme.properties:**
````properties
parent=keycloak
import=common/keycloak
styles=css/login.css
````

**login.css:** (Deine CURA-Farben)
````css
:root {
  --primary-green: #4CAF93;
  --primary-teal: #26A69A;
}

body {
  background: linear-gradient(135deg, var(--primary-green) 0%, var(--primary-teal) 100%);
}

#kc-container {
  background: white;
  border-radius: 12px;
  padding: 2rem;
  box-shadow: 0 8px 32px rgba(76, 175, 147, 0.25);
}

#kc-login {
  background: var(--primary-green);
  border: none;
  border-radius: 8px;
  padding: 1rem;
  font-weight: 600;
}

#kc-login:hover {
  background: var(--primary-teal);
}
````

Kopiere auf Server:
````bash
scp -r keycloak-themes curaadm@vm12.htl-leonding.ac.at:~/cura-project/
````

Aktiviere in Keycloak:
```
Realm "cura" → Realm Settings → Themes → Login Theme: cura
```

---

## ✅ Fertig!

Jetzt hast du:
- ✅ Keycloak läuft im Docker Container
- ✅ Angular redirected zu Keycloak Login
- ✅ User-Info wird angezeigt
- ✅ Logout funktioniert
- ✅ Optional: Custom Theme mit deinen Farben

Bei Fragen: Frag mich! 🚀
