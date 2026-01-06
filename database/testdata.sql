-- CURA Test data for the Medication Management System with binary flags

-- Sample locations
INSERT INTO locations (street, house_number, postal_code, city, floor) VALUES
('Main Street', '15', '1010', 'Vienna', '2'),
('Mariahilfer Street', '88', '1060', 'Vienna', '1'),
('Landstraße Main Street', '45', '1030', 'Vienna', '3'),
('Währinger Street', '22', '1090', 'Vienna', '1'),
('Favoriten Street', '67', '1100', 'Vienna', '2');

-- Sample medications (name and active ingredient only)
INSERT INTO medications (name, active_ingredient) VALUES
('Aspirin 500mg', 'Acetylsalicylic acid'),
('Ibuprofen 400mg', 'Ibuprofen'),
('Paracetamol 500mg', 'Paracetamol'),
('Blood Pressure Medication', 'Ramipril'),
('Diabetes Medication', 'Metformin'),
('Vitamin D3', 'Cholecalciferol'),
('Heart Medication', 'Bisoprolol'),
('Stomach Protection', 'Pantoprazol');

-- Sample caregivers
INSERT INTO caregivers (name, phone_number, email, location_id) VALUES
('Maria Schneider', '+43 664 1234567', 'maria.schneider@cura.at', 1),
('Thomas Müller', '+43 664 2345678', 'thomas.mueller@cura.at', 2),  
('Anna Weber', '+43 664 3456789', 'anna.weber@cura.at', 3),
('Stefan Huber', '+43 664 4567890', 'stefan.huber@cura.at', 4),
('Lisa Bauer', '+43 664 5678901', 'lisa.bauer@cura.at', 5);

-- Sample patients
INSERT INTO patients (name, age, location_id, phone_number, email) VALUES
('Johann Meier', 78, 1, '+43 1 2345678', 'timon.schmalzer@gmail.com'),
('Elisabeth Gruber', 82, 2, '+43 1 3456789', 'elisabeth.gruber@email.at'),
('Franz Wagner', 75, 3, '+43 1 4567890', 'franz.wagner@email.at'),
('Margarete Koch', 89, 4, '+43 1 5678901', 'margarete.koch@email.at'),
('Karl Berger', 71, 5, '+43 1 6789012', 'karl.berger@email.at'),
('Rosa Steiner', 85, 1, '+43 1 7890123', 'rosa.steiner@email.at');

-- Sample medication plans with binary flags
-- Weekdays: Sun=1, Mon=2, Tue=4, Wed=8, Thu=16, Fri=32, Sat=64
-- Day times: Morning=1, Noon=2, Afternoon=4, Evening=8

-- Patient 1: Johann Meier - Multiple medications with different schedules
-- Morning (1): Blood pressure + Heart medication (daily)
-- Noon (2): Aspirin (weekdays only Mon-Fri)
-- Evening (8): Vitamin D3 (daily)
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(1, 4, 1, 127, 1, 1, '2024-01-01', '2026-12-31', 'Blood pressure - morning empty stomach'),
(1, 7, 1, 127, 1, 1, '2024-01-01', '2026-12-31', 'Heart medication - after breakfast'),
(1, 1, 1, 62, 2, 1, '2024-01-01', '2026-12-31', 'Aspirin - weekdays noon'),
(1, 6, 1, 127, 8, 5, '2024-01-01', '2026-12-31', 'Vitamin D3 - daily evening');

-- Patient 2: Elisabeth Gruber - Morning and noon medications
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(2, 5, 2, 127, 1, 2, '2024-01-01', '2026-12-31', 'Metformin morning with meals'),
(2, 5, 2, 127, 2, 1, '2024-01-01', '2026-12-31', 'Metformin noon with meals'),
(2, 8, 2, 127, 1, 1, '2024-01-01', '2026-12-31', 'Stomach protection before eating'),
(2, 3, 2, 127, 8, 1, '2024-01-01', '2026-12-31', 'Paracetamol evening for pain');

-- Patient 3: Franz Wagner - Heart patient
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(3, 7, 3, 127, 1, 1, '2024-01-01', '2026-12-31', 'Heart medication - check pulse'),
(3, 4, 3, 127, 1, 1, '2024-01-01', '2026-12-31', 'Blood pressure - empty stomach'),
(3, 1, 3, 62, 4, 1, '2024-01-01', '2026-12-31', 'Aspirin afternoon weekdays'),
(3, 6, 3, 127, 8, 5, '2024-01-01', '2026-12-31', 'Vitamin D3 evening');

-- Patient 4: Margarete Koch - 3x daily medication
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(4, 3, 4, 127, 13, 1, '2024-01-01', '2026-12-31', 'Paracetamol morning+afternoon+evening'),
(4, 6, 4, 127, 1, 3, '2024-01-01', '2026-12-31', 'Vitamin D3 morning in yogurt');

-- Patient 5: Karl Berger - Morning and evening
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(5, 2, 5, 127, 9, 1, '2024-01-01', '2026-12-31', 'Ibuprofen morning+evening after eating'),
(5, 8, 5, 127, 1, 1, '2024-01-01', '2026-12-31', 'Stomach protection morning'),
(5, 6, 5, 127, 8, 5, '2024-01-01', '2026-12-31', 'Vitamin D3 evening drops');

-- Patient 6: Rosa Steiner - Simple medication schedule
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(6, 3, 1, 127, 2, 1, '2024-01-01', '2026-12-31', 'Paracetamol noon for headaches'),
(6, 6, 1, 127, 1, 3, '2024-01-01', '2026-12-31', 'Vitamin D3 morning daily'),
(6, 1, 1, 127, 8, 1, '2024-01-01', '2026-12-31', 'Aspirin evening as needed');

-- Additional RFID chips for multiple sets
-- Patient 1: Johann Meier
INSERT INTO rfid_chips (chip_id, patient_id, weekday) VALUES
('0441AB6EBA2A81', 1, 'MONDAY'),
('04E7AE6EBA2A81', 1, 'TUESDAY'),
('04E6AE6EBA2A81', 1, 'WEDNESDAY'),
('04E5AE6EBA2A81', 1, 'THURSDAY'),
('04E4AE6EBA2A81', 1, 'FRIDAY'),
('04DFAE6EBA2A81', 1, 'SATURDAY'),
('04DDAE6EBA2A81', 1, 'SUNDAY');



-- ========================================
-- DRAWER OPENING LOGS (medication_intakes)
-- System tracks when drawer was opened by day and time of day
-- Day time flags: Morning=1, Noon=2, Afternoon=4, Evening=8
-- ========================================

-- Patient 1 (Johann Meier) - Complete data January to July 2026
-- Schedule: Morning (1), Noon (2 - weekdays only), Evening (8)
-- Realistic: ~4 problematic days per month (partially or nothing taken)

-- ========================================
-- JANUARY 2026
-- ========================================