# 🏥 CURA - Medikamentenverwaltungs-System

Ein hochmodernes PostgreSQL-basiertes System zur Verwaltung von Medikamentenplänen mit **binären Flags** für optimale Performance und RFID-Integration.

## 🚀 System-Übersicht

Das System verwaltet:
- **Patienten** mit Adressdaten und Kontaktinformationen
- **Pfleger** mit Zuständigkeitsbereichen
- **RFID-Chips** für automatische Wochentag-Erkennung
- **Medikamente** mit Wirkstoff-Informationen
- **Optimierte Medikamentenpläne** mit binären Zeit-Flags
- **Flexible Einnahmezeiten** (Morgens, Mittags, Nachmittags, Abends)

## Schnellstart

### 1. Datenbank starten
```bash
docker-compose up -d
```

Das startet:
- PostgreSQL auf Port `5434`
- pgAdmin auf Port `8080` (optional für Web-Interface)

### 2. Verbindung zur Datenbank
```
Host: localhost
Port: 5434
Database: cura
User: admin
Password: passme
```

### 3. pgAdmin (Web-Interface)
- URL: http://localhost:8080
- Email: admin@cura.at
- Password: passme

## 📊 Datenbank-Schema (Optimiert)

### Haupttabellen

1. **`rfid_chips`** - RFID-Chips mit Wochentag-Zuordnung
2. **`orte`** - Adressen für Patienten und Pfleger
3. **`medikamente`** - Medikamentenstammdaten (Name, Wirkstoff)
4. **`pfleger`** - Pflegerdaten mit Kontaktinformationen
5. **`patienten`** - Patientendaten mit Adress-Referenz
6. **`medikamentenplaene`** - **Zentrale Tabelle** mit binären Zeit-Flags

### 🔥 Binäres Flags System

**Wochentage** (`wochentage_flags`, 0-127):
- Sonntag = 1, Montag = 2, Dienstag = 4, Mittwoch = 8
- Donnerstag = 16, Freitag = 32, Samstag = 64
- **Beispiele**: Täglich = 127, Werktags = 62, Mo+Mi+Fr = 42

**Tageszeiten** (`tageszeiten_flags`, 0-15):
- Morgens = 1, Mittags = 2, Nachmittags = 4, Abends = 8
- **Beispiele**: Morgens+Abends = 9, 3x täglich = 13

### 🏷️ RFID-Chips Standard-Zuordnung

| Wochentag | Chip-ID | Beispiel-UID |
|-----------|---------|--------------|
| Montag | CHIP_MONTAG_001 | A1B2C3D4E5F6 |
| Dienstag | CHIP_DIENSTAG_001 | B2C3D4E5F6A1 |
| Mittwoch | CHIP_MITTWOCH_001 | C3D4E5F6A1B2 |
| Donnerstag | CHIP_DONNERSTAG_001 | D4E5F6A1B2C3 |
| Freitag | CHIP_FREITAG_001 | E5F6A1B2C3D4 |
| Samstag | CHIP_SAMSTAG_001 | F6A1B2C3D4E5 |
| Sonntag | CHIP_SONNTAG_001 | A1B2C3D4E5F7 |

## 🔍 Wichtige Views & Hilfsfunktionen

### Lesbare Medikamentenpläne
```sql
SELECT * FROM v_medikamentenplan_readable WHERE patient_name = 'Johann Meier';
```

### Binäre Flags umwandeln
```sql
-- Wochentag zu Flag: 'MONTAG' → 2
SELECT wochentag_zu_flag('MONTAG');

-- Flag zu Wochentagen: 42 → ['MONTAG', 'MITTWOCH', 'FREITAG']
SELECT flag_zu_wochentage(42);

-- Tageszeit zu Flag: 'MORGENS' → 1  
SELECT tageszeit_zu_flag('MORGENS');

-- Flag zu Tageszeiten: 9 → ['MORGENS', 'ABENDS']
SELECT flag_zu_tageszeiten(9);
```

## 🎯 Beispiel-Abfragen

### Medikamentenplan für heute (Montag)
```sql
SELECT 
    p.name as patient,
    m.name as medikament,
    mp.stueckzahl,
    flag_zu_tageszeiten(mp.tageszeiten_flags) as tageszeiten,
    mp.notizen
FROM medikamentenplaene mp
JOIN patienten p ON mp.patient_id = p.id
JOIN medikamente m ON mp.medikament_id = m.id
WHERE (mp.wochentage_flags & wochentag_zu_flag('MONTAG')) > 0
  AND mp.aktiv = true
  AND CURRENT_DATE BETWEEN mp.gueltig_von AND COALESCE(mp.gueltig_bis, '2099-12-31')
ORDER BY p.name;
```

### RFID-Chip scannen für aktuellen Wochentag
```sql
SELECT 
    p.name as patient,
    m.name as medikament,
    mp.stueckzahl,
    flag_zu_tageszeiten(mp.tageszeiten_flags) as einnahmezeiten,
    mp.notizen
FROM rfid_chips r
JOIN medikamentenplaene mp ON (mp.wochentage_flags & wochentag_zu_flag(r.wochentag)) > 0
JOIN patienten p ON mp.patient_id = p.id
JOIN medikamente m ON mp.medikament_id = m.id
WHERE r.chip_id = 'CHIP_MONTAG_001' -- Von RFID-Scanner
  AND mp.aktiv = true
  AND CURRENT_DATE BETWEEN mp.gueltig_von AND COALESCE(mp.gueltig_bis, '2099-12-31')
ORDER BY p.name;
```

### Neuen Medikamentenplan erstellen
```sql
INSERT INTO medikamentenplaene (
    patient_id, medikament_id, pfleger_id,
    wochentage_flags, tageszeiten_flags, stueckzahl,
    gueltig_von, notizen
) VALUES (
    1, 4, 1,
    127, -- Täglich (So=1 + Mo=2 + ... + Sa=64 = 127)
    1,   -- Morgens (Morgens=1)
    1,   -- 1 Stück
    '2024-01-01',
    'Nüchtern einnehmen'
);
```

## 🔧 Erweiterte Features

### Komplexe Einnahmezeiten mit binären Flags
```sql
-- Nur Werktage, morgens und abends (Mo-Fr = 2+4+8+16+32 = 62, Morgens+Abends = 1+8 = 9)
INSERT INTO medikamentenplaene (patient_id, medikament_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von)
VALUES (1, 2, 62, 9, 1, '2024-01-01');

-- Montag, Mittwoch, Freitag zur Mittagszeit (Mo+Mi+Fr = 2+8+32 = 42, Mittags = 2)
INSERT INTO medikamentenplaene (patient_id, medikament_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von)
VALUES (1, 3, 42, 2, 2, '2024-01-01');
```

### Medikamentenplan beenden/pausieren
```sql
UPDATE medikamentenplaene 
SET aktiv = FALSE, gueltig_bis = CURRENT_DATE 
WHERE id = 1;
```

### Performance-optimierte Abfragen
```sql
-- Index-optimierte Suche nach Patienten-Medikamentenplänen
SELECT * FROM medikamentenplaene 
WHERE patient_id = 1 AND aktiv = true
  AND CURRENT_DATE BETWEEN gueltig_von AND COALESCE(gueltig_bis, '2099-12-31');
```

## Wartung

### Backup erstellen
```bash
docker exec cura_postgres pg_dump -U admin cura > backup_$(date +%Y%m%d).sql
```

### Backup wiederherstellen
```bash
docker exec -i cura_postgres psql -U admin cura < backup_20240922.sql
```

### Logs anzeigen
```bash
docker-compose logs postgres
```

## Entwicklung

### Neue Spalte hinzufügen
```sql
ALTER TABLE patients ADD COLUMN insurance_number VARCHAR(50);
```

### Index hinzufügen für Performance
```sql
CREATE INDEX idx_custom_name ON table_name(column_name);
```

## Troubleshooting

### Container neustarten
```bash
docker-compose restart
```

### Datenbank zurücksetzen
```bash
docker-compose down -v
docker-compose up -d
```

### Verbindung testen
```bash
docker exec -it cura_postgres psql -U admin -d cura -c "SELECT COUNT(*) FROM patienten;"
```

## 📈 ERD & Dokumentation

- **PlantUML ERD**: `CURA_ERD.puml` - Detailliertes Entity-Relationship-Diagram
- **Markdown Docs**: `ERD_CURA_System.md` - Vollständige Systemdokumentation

## 🚀 Performance Features

- **Binäre Flags** statt separater Tabellen → 90% weniger JOINs
- **Optimierte Indizes** für häufige Abfragen
- **Minimales Schema** mit maximaler Flexibilität
- **PostgreSQL 16** mit modernsten Features