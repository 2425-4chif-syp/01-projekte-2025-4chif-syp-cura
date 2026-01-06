-- CURA Medication Management System
-- Drops and recreates all tables

-- Drop tables in reverse order (due to Foreign Key Constraints)
DROP TABLE IF EXISTS medication_intakes CASCADE;
DROP TABLE IF EXISTS medication_plans CASCADE;
DROP TABLE IF EXISTS rfid_chips CASCADE;
DROP TABLE IF EXISTS patients CASCADE;
DROP TABLE IF EXISTS caregivers CASCADE;
DROP TABLE IF EXISTS medications CASCADE;
DROP TABLE IF EXISTS locations CASCADE;

-- 1. Locations Table
CREATE TABLE locations (
    id SERIAL PRIMARY KEY,
    street VARCHAR(255) NOT NULL,
    house_number VARCHAR(10) NOT NULL,
    postal_code VARCHAR(10) NOT NULL,
    city VARCHAR(100) NOT NULL,
    floor VARCHAR(10)
);

-- 2. Medications Table
CREATE TABLE medications (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    active_ingredient VARCHAR(255)
);

-- 3. Caregivers Table
CREATE TABLE caregivers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    phone_number VARCHAR(50),
    email VARCHAR(255) UNIQUE,
    location_id INTEGER REFERENCES locations(id)
);

-- 4. Patients Table
CREATE TABLE patients (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    age INTEGER CHECK (age > 0 AND age < 150),
    location_id INTEGER REFERENCES locations(id),
    phone_number VARCHAR(50),
    email VARCHAR(255)
);

-- 5. RFID Chips Table (for weekdays - moved after patients)
CREATE TABLE rfid_chips (
    id SERIAL PRIMARY KEY,
    chip_id VARCHAR(50) UNIQUE NOT NULL,
    patient_id INTEGER NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
    weekday VARCHAR(20) NOT NULL CHECK (weekday IN ('MONDAY', 'TUESDAY', 'WEDNESDAY', 'THURSDAY', 'FRIDAY', 'SATURDAY', 'SUNDAY')),
    is_active BOOLEAN DEFAULT true
);

-- 6. Medication Plans Table (combined with details, binary flags)
CREATE TABLE medication_plans (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL REFERENCES patients(id),
    medication_id INTEGER NOT NULL REFERENCES medications(id),
    caregiver_id INTEGER REFERENCES caregivers(id),
    
    -- Binary weekdays: Sun=1, Mon=2, Tue=4, Wed=8, Thu=16, Fri=32, Sat=64
    -- Example: Monday+Wednesday+Friday = 2+8+32 = 42
    weekday_flags INTEGER NOT NULL DEFAULT 0 CHECK (weekday_flags >= 0 AND weekday_flags <= 127),
    
    -- Binary day times: Morning=1, Noon=2, Afternoon=4, Evening=8
    -- Example: Morning+Evening = 1+8 = 9
    day_time_flags INTEGER NOT NULL DEFAULT 0 CHECK (day_time_flags >= 0 AND day_time_flags <= 15),
    
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    valid_from DATE NOT NULL,
    valid_to DATE,
    notes TEXT,
    is_active BOOLEAN DEFAULT true
);

-- 7. Drawer Openings Table (logs when drawer was opened - not specific medications)
-- System can only detect: day (via RFID) + time of day (morning/noon/afternoon/evening)
CREATE TABLE medication_intakes (
    id SERIAL PRIMARY KEY,
    patient_id INTEGER NOT NULL REFERENCES patients(id) ON DELETE CASCADE,
    intake_date DATE NOT NULL,
    
    -- Day time when drawer was opened: Morning=1, Noon=2, Afternoon=4, Evening=8
    -- Only ONE value per entry (not combined like in medication_plans)
    day_time_flag INTEGER NOT NULL CHECK (day_time_flag IN (1, 2, 4, 8)),
    
    opened_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    rfid_tag VARCHAR(50),
    notes TEXT,
    
    -- One entry per patient, date, and time of day
    CONSTRAINT unique_opening_per_day_time UNIQUE (patient_id, intake_date, day_time_flag)
);

-- Indexes for better performance
CREATE INDEX idx_rfid_chips_weekday ON rfid_chips(weekday);
CREATE INDEX idx_rfid_chips_chip_id ON rfid_chips(chip_id);
CREATE INDEX idx_rfid_chips_patient ON rfid_chips(patient_id);
CREATE INDEX idx_patients_name ON patients(name);
CREATE INDEX idx_medication_plans_patient ON medication_plans(patient_id);
CREATE INDEX idx_medication_plans_medication ON medication_plans(medication_id);
CREATE INDEX idx_medications_name ON medications(name);

-- Indexes for medication_intakes (critical for calendar queries)
CREATE INDEX idx_medication_intakes_patient ON medication_intakes(patient_id);
CREATE INDEX idx_medication_intakes_date ON medication_intakes(intake_date);
CREATE INDEX idx_medication_intakes_patient_date ON medication_intakes(patient_id, intake_date);
CREATE INDEX idx_medication_intakes_day_time ON medication_intakes(day_time_flag);
CREATE INDEX idx_medication_intakes_rfid ON medication_intakes(rfid_tag) WHERE rfid_tag IS NOT NULL;

-- Create Keycloak Database
CREATE DATABASE keycloak;



