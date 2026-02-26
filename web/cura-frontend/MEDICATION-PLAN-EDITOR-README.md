# Medikamentenplan Editor

## Features ✨

- **Drag & Drop**: Medikamente per Drag & Drop in Zeitslots ziehen
- **Zeitslots**: Morgens, Mittags, Abends, Nachts
- **Wochentage**: Montag bis Sonntag
- **Neue Medikamente**: Direkt neue Medikamente hinzufügen
- **Dosierung**: Anzahl der Tabletten pro Einnahme
- **Tag kopieren**: Einen Tag auf andere Tage übertragen
- **Laden & Speichern**: Bestehenden Plan laden und Änderungen speichern

## Verwendung 🚀

### Starten:
```bash
cd web/cura-frontend
npm install
npm start
```

Dann öffne: `http://localhost:4200/medication-plan-editor`

### Workflow:

1. **Medikament hinzufügen**:
   - Klick auf "➕ Neues Medikament"
   - Name eingeben und speichern

2. **Medikament zuweisen**:
   - Medikament aus der linken Liste
   - Per Drag & Drop in einen Zeitslot ziehen
   - Dosierung mit +/- anpassen

3. **Tag kopieren**:
   - Klick auf 📋 bei einem Tag
   - Tagnummern eingeben (z.B. "2,3,5" für Di, Mi, Fr)

4. **Plan speichern**:
   - Klick auf "💾 Plan speichern"
   - Plan wird an Backend gesendet

## Technische Details 🔧

### Komponenten:
- `medication-plan-editor.component.ts` - TypeScript Logic
- `medication-plan-editor.component.html` - HTML Template
- `medication-plan-editor.component.css` - Styling

### Dependencies:
- Angular CDK für Drag & Drop
- MedicationPlanService für API-Calls
- FormsModule für Eingabefelder

### API Endpoints:
- `GET /api/Medications` - Alle Medikamente laden
- `GET /api/MedicationPlans/patient/{id}` - Plan laden
- `POST /api/MedicationPlans` - Plan speichern
- `POST /api/Medications` - Neues Medikament erstellen

## Datenbankflags 🏁

### Wochentage (WeekdayFlags):
- Sonntag = 1
- Montag = 2
- Dienstag = 4
- Mittwoch = 8
- Donnerstag = 16
- Freitag = 32
- Samstag = 64

### Tageszeiten (DayTimeFlags):
- Morgens = 1
- Mittags = 2
- Abends = 8
- Nachts = 16

## TODOs 📝

- [ ] PatientId aus Auth-Service holen
- [ ] API-Calls für Speichern implementieren
- [ ] Alte Pläne vor dem Speichern löschen
- [ ] Error Handling verbessern
- [ ] Loading Spinner hinzufügen
- [ ] Validierung der Eingaben
- [ ] Undo/Redo Funktion

## Troubleshooting 🐛

**Problem**: Medikamente werden nicht geladen
- Prüfe Backend-Verbindung: `https://vm12.htl-leonding.ac.at/api/Medications`
- Prüfe CORS-Konfiguration im Backend

**Problem**: Drag & Drop funktioniert nicht
- Stelle sicher dass `@angular/cdk` installiert ist: `npm install @angular/cdk`

**Problem**: Plan wird nicht gespeichert
- API-Endpoint noch nicht implementiert (siehe TODO)
- Prüfe Console für Fehler
