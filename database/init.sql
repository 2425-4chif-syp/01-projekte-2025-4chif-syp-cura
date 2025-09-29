-- CURA Medikamentenverwaltungs-System
-- Drops und erstellt alle Tabellen neu

-- Tabellen in umgekehrter Reihenfolge droppen (wegen Foreign Key Constraints)
DROP TABLE IF EXISTS medikamentenplaene CASCADE;
DROP TABLE IF EXISTS patienten CASCADE;
DROP TABLE IF EXISTS pfleger CASCADE;
DROP TABLE IF EXISTS medikamente CASCADE;
DROP TABLE IF EXISTS orte CASCADE;
DROP TABLE IF EXISTS rfid_chips CASCADE;

-- 1. RFID Chips Tabelle (für Wochentage)
CREATE TABLE rfid_chips (
    id SERIAL PRIMARY KEY,
    chip_id VARCHAR(50) UNIQUE NOT NULL,
    wochentag VARCHAR(20) NOT NULL CHECK (wochentag IN ('MONTAG', 'DIENSTAG', 'MITTWOCH', 'DONNERSTAG', 'FREITAG', 'SAMSTAG', 'SONNTAG')),
    aktiv BOOLEAN DEFAULT true
);

-- 2. Orte Tabelle
CREATE TABLE orte (
    id SERIAL PRIMARY KEY,
    strasse VARCHAR(255) NOT NULL,
    hausnummer VARCHAR(10) NOT NULL,
    postleitzahl VARCHAR(10) NOT NULL,
    stadt VARCHAR(100) NOT NULL,
    stockwerk VARCHAR(10)
);

-- 3. Medikamente Tabelle
CREATE TABLE medikamente (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    wirkstoff VARCHAR(255)
);

-- 4. Pfleger Tabelle
CREATE TABLE pfleger (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    telefonnummer VARCHAR(50),
    email VARCHAR(255) UNIQUE,
    ort_id INTEGER REFERENCES orte(id)
);

-- 6. Patienten Tabelle
CREATE TABLE patienten (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    alter INTEGER CHECK (alter > 0 AND alter < 150),
    ort_id INTEGER REFERENCES orte(id),
    telefonnummer VARCHAR(50),
    email VARCHAR(255)
);

-- 7. Medikamentenpläne Tabelle (vereinigt mit Details, binäre Flags)
CREATE TABLE medikamentenplaene (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL REFERENCES patienten(id),
    medikament_id INTEGER NOT NULL REFERENCES medikamente(id),
    pfleger_id INTEGER REFERENCES pfleger(id),
    
    -- Binäre Wochentage: So=1, Mo=2, Di=4, Mi=8, Do=16, Fr=32, Sa=64
    -- Beispiel: Montag+Mittwoch+Freitag = 2+8+32 = 42
    wochentage_flags INTEGER NOT NULL DEFAULT 0 CHECK (wochentage_flags >= 0 AND wochentage_flags <= 127),
    
    -- Binäre Tageszeiten: Morgens=1, Mittags=2, Nachmittags=4, Abends=8
    -- Beispiel: Morgens+Abends = 1+8 = 9
    tageszeiten_flags INTEGER NOT NULL DEFAULT 0 CHECK (tageszeiten_flags >= 0 AND tageszeiten_flags <= 15),
    
    stueckzahl INTEGER NOT NULL CHECK (stueckzahl > 0),
    gueltig_von DATE NOT NULL,
    gueltig_bis DATE,
    notizen TEXT,
    aktiv BOOLEAN DEFAULT true
);

-- Indizes für bessere Performance
CREATE INDEX idx_rfid_chips_wochentag ON rfid_chips(wochentag);
CREATE INDEX idx_rfid_chips_chip_id ON rfid_chips(chip_id);
CREATE INDEX idx_patienten_name ON patienten(name);
CREATE INDEX idx_medikamentenplaene_patient ON medikamentenplaene(patient_id);
CREATE INDEX idx_medikamentenplaene_medikament ON medikamentenplaene(medikament_id);
CREATE INDEX idx_medikamente_name ON medikamente(name);




