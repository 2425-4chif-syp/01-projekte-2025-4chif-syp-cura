# Medikamenten Alert Display

Fullscreen-Display für HDMI 2 Input am ATEM Switcher.

## 🎨 Features

- **Automatische Anzeige** der fälligen Medikamente
- **Farbcodierung** nach Medikamentengruppe:
  - 🔴 Blutverdünner (Pink/Rot)
  - 🔵 Schmerzmittel (Blau/Türkis)
  - 🟢 Antibiotika (Grün)
  - 🟡 Blutdruck (Gelb/Pink)
  - 🟣 Diabetes (Lila)
  - 🩵 Herzmedikamente (Hellblau/Pink)
- **Auto-Refresh** alle 10 Sekunden
- **Responsive Design** für verschiedene Bildschirmgrößen
- **Animations** für bessere Sichtbarkeit

## 🚀 Setup

### Option 1: Lokaler Computer

```bash
# Einfach im Browser öffnen
firefox medication-alert-display.html
# oder
chromium-browser medication-alert-display.html

# Fullscreen mit F11
```

### Option 2: Raspberry Pi (empfohlen)

```bash
# Chromium installieren
sudo apt install chromium-browser unclutter

# Auto-start Display im Kiosk-Mode
nano ~/.config/autostart/medication-display.desktop
```

Inhalt:
```desktop
[Desktop Entry]
Type=Application
Name=Medication Alert Display
Exec=chromium-browser --kiosk --noerrdialogs --disable-infobars --no-first-run --fast --fast-start --disable-features=TranslateUI --disk-cache-dir=/dev/null file:///home/pi/medication-alert-display.html
```

```bash
# Mauszeiger verstecken
nano ~/.config/autostart/unclutter.desktop
```

Inhalt:
```desktop
[Desktop Entry]
Type=Application
Name=Unclutter
Exec=unclutter -idle 0
```

### Option 3: Webserver

```bash
# In ATEM Verzeichnis
python3 -m http.server 8080

# Im Browser öffnen
http://localhost:8080/medication-alert-display.html
```

## 🎯 ATEM Setup

1. **Raspberry Pi / Computer** mit Display verbindet
2. **HDMI-Ausgang** des Geräts in **ATEM Input 2** stecken
3. **Browser im Fullscreen** öffnen (F11)
4. **ATEM Daemon** läuft und schaltet automatisch um

## 🎨 Anpassungen

### Farben ändern

In `medication-alert-display.html` Zeile 109-139:

```css
.category-blutverduenner {
    background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
}
```

### Medikamentengruppen anpassen

Zeile 182-199:

```javascript
const medicationCategories = {
    'aspirin': 'blutverduenner',
    'dein-medikament': 'schmerzmittel',
    // ...
};
```

### Refresh-Intervall

Zeile 174:

```javascript
const REFRESH_INTERVAL = 10000; // 10 Sekunden = 10000ms
```

### Backend-URL

Zeile 173:

```javascript
const BACKEND_URL = 'https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1';
```

## 📱 Display-Modi

### Alert aktiv (Medikament fällig)
```
💊 (blinkend)
Bitte nehmen Sie folgendes Medikament ein

╔════════════════════════════════════════╗
║  Aspirin                 [Blutverdünner]║
║  1 Tablette                            ║
╚════════════════════════════════════════╝

Morgens (6:00 - 11:00)
```

### Keine Medikamente fällig
```
✅
Alles in Ordnung
Keine Medikamente fällig

Morgens (6:00 - 11:00)
```

### Verbindungsfehler
```
⚠️
Verbindungsfehler
Backend nicht erreichbar: Connection failed
```

## 🎬 Vollständiger Workflow

1. **Patient vergisst Medikament** → Zeit ist 08:00, Medikament für Morning fällig
2. **Backend erkennt:** `ShouldAlert = true`
3. **ATEM Daemon schaltet** auf HDMI 2
4. **Display zeigt:** "Bitte nehmen Sie Aspirin ein" mit roter Blutverdünner-Markierung
5. **Patient nimmt Medikament** und scannt RFID
6. **Backend erkennt:** `ShouldAlert = false`
7. **ATEM Daemon schaltet** zurück auf HDMI 1
8. **Display zeigt:** "Alles in Ordnung"

## 🐛 Troubleshooting

### Display zeigt "Verbindungsfehler"

```bash
# Backend testen
curl https://vm12.htl-leonding.ac.at/api/MedicationAlert/should-alert/1
```

Falls CORS-Fehler: Browser-Console öffnen (F12)

### Browser startet nicht im Fullscreen

```bash
# Chromium Kiosk-Mode erzwingen
chromium-browser --start-fullscreen --kiosk medication-alert-display.html
```

### Mauszeiger sichtbar

```bash
# Unclutter installieren
sudo apt install unclutter
unclutter -idle 0 &
```

### Display friert ein

Auto-Reload hinzufügen in HTML:
```html
<meta http-equiv="refresh" content="300"> <!-- Reload alle 5 Min -->
```

## 📊 Unterstützte Medikamente

Automatische Kategorisierung für:
- **Blutverdünner:** Aspirin, Marcumar, Xarelto
- **Schmerzmittel:** Ibuprofen, Paracetamol, Novalgin
- **Antibiotika:** Amoxicillin, Azithromycin
- **Blutdruck:** Ramipril, Bisoprolol
- **Diabetes:** Metformin, Insulin
- **Herzmedikamente:** Digitalis, Digoxin

Andere Medikamente → Standardfarbe (Lila)

## ⚙️ Technische Details

- **Framework:** Vanilla HTML/CSS/JavaScript (keine Dependencies)
- **API-Polling:** Fetch API alle 10 Sekunden
- **Animations:** CSS keyframes für Pulse/Fade/Slide
- **Responsive:** Flexbox für verschiedene Auflösungen
- **Auto-Refresh:** Visibility API für Tab-Fokus

## 🔧 Erweiterte Optionen

### Mehrere Patienten

Dropdown hinzufügen:
```html
<select id="patient-select" onchange="changePatient()">
    <option value="1">Patient 1</option>
    <option value="2">Patient 2</option>
</select>
```

### Soundeffekt bei Alert

```javascript
const audio = new Audio('alert.mp3');
if (data.shouldAlert) {
    audio.play();
}
```

### Touch-Display Support

```css
.medication-card {
    cursor: pointer;
}
```

```javascript
card.addEventListener('click', () => {
    // Medikament als genommen markieren
});
```
