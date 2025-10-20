# CURA Medikamenten-Erinnerungssystem - Deployment Guide

## 🚀 Server-Deployment auf vm12.htl-leonding.ac.at

### Voraussetzungen
- Docker und Docker Compose sind installiert
- Git ist verfügbar
- Ports 8081, 5434, 8080, 1883, 9001 sind frei

### 1. Repository klonen
```bash
git clone https://github.com/2425-4chif-syp/01-projekte-2025-4chif-syp-cura.git
cd 01-projekte-2025-4chif-syp-cura
```

### 2. Services starten
```bash
cd database
docker-compose up -d
```

### 3. Status überprüfen
```bash
# Alle Container anzeigen
docker-compose ps

# Logs überprüfen
docker-compose logs backend
docker-compose logs mosquitto
docker-compose logs postgres
```

### 4. Medikamenten-Erinnerungssystem testen

#### 4.1 API-Endpunkte
- **Swagger UI**: `http://vm12.htl-leonding.ac.at:8081/swagger`
- **Test-Nachricht**: `POST http://vm12.htl-leonding.ac.at:8081/api/medicationreminder/test-message`
- **Aktive Pläne**: `GET http://vm12.htl-leonding.ac.at:8081/api/medicationreminder/active-plans`

#### 4.2 Test-Nachricht senden
```bash
curl -X POST "http://vm12.htl-leonding.ac.at:8081/api/medicationreminder/test-message" \
     -H "Content-Type: application/json" \
     -d "\"TEST: Medikamente einnehmen!\""
```

#### 4.3 MQTT-Topic direkt überwachen
```bash
# MQTT-Client installieren (falls nicht vorhanden)
apt-get install mosquitto-clients

# Display-Topic überwachen
mosquitto_sub -h vm12.htl-leonding.ac.at -p 1883 -t "display/message"
```

### 5. Funktionsweise der Medikamenten-Erinnerungen

#### Zeitpläne:
- **Morning (Flag 1)**: 08:00 Uhr
- **Noon (Flag 2)**: 12:00 Uhr  
- **Afternoon (Flag 4)**: 16:00 Uhr
- **Evening (Flag 8)**: 20:00 Uhr

#### Wochentage-Flags:
- **Sonntag**: 1
- **Montag**: 2
- **Dienstag**: 4
- **Mittwoch**: 8
- **Donnerstag**: 16
- **Freitag**: 32
- **Samstag**: 64

#### Beispiel Medikamentenplan:
```sql
-- Medikament täglich morgens und abends (Mo-Fr)
INSERT INTO medication_plans (
    patient_id, medication_id, 
    weekday_flags,  -- Mo(2) + Di(4) + Mi(8) + Do(16) + Fr(32) = 62
    day_time_flags, -- Morning(1) + Evening(8) = 9
    quantity, valid_from
) VALUES (1, 1, 62, 9, 1, '2025-01-01');
```

### 6. Services verwalten

#### Stoppen:
```bash
docker-compose down
```

#### Neustarten nach Code-Änderungen:
```bash
docker-compose down
docker-compose build --no-cache backend
docker-compose up -d
```

#### Logs in Echtzeit:
```bash
docker-compose logs -f backend
```

### 7. Troubleshooting

#### Backend startet nicht:
```bash
# Container-Status prüfen
docker-compose ps

# Backend-Logs anschauen
docker-compose logs backend

# Datenbank-Verbindung testen
docker exec -it cura_postgres psql -U admin -d cura -c "\dt"
```

#### MQTT funktioniert nicht:
```bash
# Mosquitto-Status prüfen
docker-compose logs mosquitto

# MQTT-Verbindung testen
mosquitto_pub -h vm12.htl-leonding.ac.at -p 1883 -t "test/topic" -m "Hello MQTT"
mosquitto_sub -h vm12.htl-leonding.ac.at -p 1883 -t "test/topic"
```

#### Medikamenten-Service läuft nicht:
```bash
# Backend-Logs für MedicationReminderService
docker-compose logs backend | grep "MedicationReminderService"

# Aktive Pläne über API prüfen
curl -X GET "http://vm12.htl-leonding.ac.at:8081/api/medicationreminder/active-plans"
```

### 8. Zugangsdaten

- **PostgreSQL**: 
  - Host: `vm12.htl-leonding.ac.at:5434`
  - User: `admin`
  - Password: `passme`
  - Database: `cura`

- **pgAdmin**: 
  - URL: `http://vm12.htl-leonding.ac.at:8080`
  - Email: `admin@cura.at`
  - Password: `passme`

- **MQTT Broker**:
  - Host: `vm12.htl-leonding.ac.at`
  - Port: `1883` (TCP) oder `9001` (WebSocket)
  - Topics: `display/message`, `rc522/tag`, `cura/rfid/scanned`

### 9. Monitoring

Das System sendet automatisch Erinnerungen zur korrekten Zeit. Überwache das `display/message` Topic:

```bash
# Dauerhaft überwachen
mosquitto_sub -h vm12.htl-leonding.ac.at -p 1883 -t "display/message" -v
```

### ✅ Ready to Deploy!

Das Medikamenten-Erinnerungssystem ist deployment-ready! Alle Komponenten sind konfiguriert und das System läuft automatisch im Hintergrund.