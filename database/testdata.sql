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
('Johann Meier', 78, 1, '+43 1 2345678', 'johann.meier@email.at'),
('Elisabeth Gruber', 82, 2, '+43 1 3456789', 'elisabeth.gruber@email.at'),
('Franz Wagner', 75, 3, '+43 1 4567890', 'franz.wagner@email.at'),
('Margarete Koch', 89, 4, '+43 1 5678901', 'margarete.koch@email.at'),
('Karl Berger', 71, 5, '+43 1 6789012', 'karl.berger@email.at'),
('Rosa Steiner', 85, 1, '+43 1 7890123', 'rosa.steiner@email.at');

-- Sample medication plans with binary flags
-- Weekdays: Sun=1, Mon=2, Tue=4, Wed=8, Thu=16, Fri=32, Sat=64
-- Day times: Morning=1, Noon=2, Afternoon=4, Evening=8

-- Patient 1: Johann Meier - Blood pressure medication daily in morning (Mon-Sun = 2+4+8+16+32+64+1 = 127)
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(1, 4, 1, 127, 15, 1, '2024-01-01', '2025-12-31', 'Take on empty stomach'),
-- Heart medication daily in morning  
(1, 7, 1, 127, 1, 1, '2024-01-01', '2025-12-31', 'After breakfast'),
-- Aspirin only weekdays at noon (Mon-Fri = 2+4+8+16+32 = 62)
(1, 1, 1, 62, 2, 1, '2024-01-01', '2025-12-31', 'As needed'),
-- Vitamin D3 daily in evening
(1, 6, 1, 127, 8, 5, '2024-01-01', '2025-12-31', 'Dissolve in water');

-- Patient 2: Elisabeth Gruber - Metformin morning and noon daily
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(2, 5, 2, 127, 1, 2, '2024-01-01', '2025-12-31', 'Morning 2 pieces with meals'),
(2, 5, 2, 127, 2, 1, '2024-01-01', '2025-12-31', 'Noon 1 piece with meals'),
-- Stomach protection daily in morning
(2, 8, 2, 127, 1, 1, '2024-01-01', '2025-12-31', 'Before eating'),
-- Paracetamol only as needed in evening
(2, 3, 2, 127, 8, 1, '2024-01-01', '2025-12-31', 'For pain');

-- Patient 3: Franz Wagner - Heart patient, various times
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
-- Heart medication daily in morning
(3, 7, 3, 127, 1, 1, '2024-01-01', '2025-12-31', 'Check pulse before taking'),
-- Blood pressure medication daily in morning
(3, 4, 3, 127, 1, 1, '2024-01-01', '2025-12-31', 'On empty stomach'),
-- Aspirin only weekdays in afternoon
(3, 1, 3, 62, 4, 1, '2024-01-01', '2025-12-31', 'As needed'),
-- Vitamin D3 daily in evening
(3, 6, 3, 127, 8, 5, '2024-01-01', '2025-12-31', 'Drops in juice');

-- Patient 4: Margarete Koch - Paracetamol 3x daily (Morning, Afternoon, Evening = 1+4+8 = 13)
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(4, 3, 4, 127, 13, 1, '2024-01-01', '2024-12-31', 'Supervised intake - 3x daily'),
-- Vitamin D3 only in morning
(4, 6, 4, 127, 1, 3, '2024-01-01', '2024-12-31', 'Mix in yogurt');

-- Patient 5: Karl Berger - Ibuprofen morning and evening (1+8 = 9)
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(5, 2, 5, 127, 9, 1, '2024-01-01', '2024-12-31', 'After eating - morning and evening'),
-- Stomach protection daily in morning
(5, 8, 5, 127, 1, 1, '2024-01-01', '2024-12-31', 'Stomach protection'),
-- Vitamin D3 daily in evening
(5, 6, 5, 127, 8, 5, '2024-01-01', '2024-12-31', 'Drops');

-- Patient 6: Rosa Steiner - only few medications
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
-- Paracetamol only as needed at noon
(6, 3, 1, 127, 2, 1, '2024-01-01', '2024-12-31', 'For headaches'),
-- Vitamin D3 daily in morning
(6, 6, 1, 127, 1, 3, '2024-01-01', '2024-12-31', 'Daily'),
-- Aspirin only as needed in evening
(6, 1, 1, 127, 8, 1, '2024-01-01', '2024-12-31', 'As needed');

-- Additional RFID chips for multiple sets
INSERT INTO rfid_chips (chip_id, weekday) VALUES
('0441AB6EBA2A81', 'MONDAY'),
('04E7AE6EBA2A81', 'TUESDAY'),
('04E6AE6EBA2A81', 'WEDNESDAY'),
('04E5AE6EBA2A81', 'THURSDAY'),
('04E4AE6EBA2A81', 'FRIDAY'),
('04DFAE6EBA2A81', 'SATURDAY'),
('04DDAE6EBA2A81', 'SUNDAY');

-- Sample medication intake logs for testing calendar functionality
-- Patient 1 (Johann Meier) has plans: 1, 2, 3, 4

-- Oktober 2025 - Complete week (all medications taken)
-- Monday 2025-10-20 - ALL TAKEN ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2025-10-20 08:15:00', 1, 'Taken with breakfast', '0441AB6EBA2A81'),
(1, 2, '2025-10-20 08:30:00', 1, 'After breakfast', '0441AB6EBA2A81'),
(1, 3, '2025-10-20 12:45:00', 1, 'Taken at lunch', '0441AB6EBA2A81'),
(1, 4, '2025-10-20 20:00:00', 5, 'Evening dose', '0441AB6EBA2A81');

-- Tuesday 2025-10-21 - PARTIALLY TAKEN ❌ (missing plan 3)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2025-10-21 07:45:00', 1, 'Morning dose', '04E7AE6EBA2A81'),
(1, 2, '2025-10-21 08:00:00', 1, 'With meal', '04E7AE6EBA2A81'),
(1, 4, '2025-10-21 19:30:00', 5, 'Evening dose', '04E7AE6EBA2A81');

-- Wednesday 2025-10-22 - ALL TAKEN ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2025-10-22 08:00:00', 1, 'Morning dose', '04E6AE6EBA2A81'),
(1, 2, '2025-10-22 08:15:00', 1, 'After breakfast', '04E6AE6EBA2A81'),
(1, 3, '2025-10-22 13:00:00', 1, 'Noon dose', '04E6AE6EBA2A81'),
(1, 4, '2025-10-22 20:15:00', 5, 'Evening dose', '04E6AE6EBA2A81');

-- Thursday 2025-10-23 - NOTHING TAKEN ❌
-- (No entries = all medications missed)

-- Friday 2025-10-24 - ALL TAKEN ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2025-10-24 08:10:00', 1, 'Morning routine', '04E4AE6EBA2A81'),
(1, 2, '2025-10-24 08:25:00', 1, 'Breakfast time', '04E4AE6EBA2A81'),
(1, 3, '2025-10-24 12:30:00', 1, 'Lunch dose', '04E4AE6EBA2A81'),
(1, 4, '2025-10-24 19:45:00', 5, 'Evening routine', '04E4AE6EBA2A81');

-- Saturday 2025-10-25 - PARTIALLY TAKEN ❌ (weekend - no plan 3, but still missing plan 1)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 2, '2025-10-25 09:00:00', 1, 'Late morning', '04DFAE6EBA2A81'),
(1, 4, '2025-10-25 20:30:00', 5, 'Evening dose', '04DFAE6EBA2A81');

-- Sunday 2025-10-26 - ALL TAKEN ✅ (weekend - no plan 3 required)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2025-10-26 09:15:00', 1, 'Sunday morning', '04DDAE6EBA2A81'),
(1, 2, '2025-10-26 09:30:00', 1, 'Breakfast', '04DDAE6EBA2A81'),
(1, 4, '2025-10-26 20:00:00', 5, 'Evening dose', '04DDAE6EBA2A81');

-- Patient 2 (Elisabeth Gruber) - Some entries for October
-- Plans: 5 (morning), 6 (noon), 7, 8

-- Monday 2025-10-20 - ALL TAKEN ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 5, '2025-10-20 08:00:00', 2, 'Morning dose with meal', NULL),
(2, 6, '2025-10-20 12:00:00', 1, 'Noon dose', NULL),
(2, 7, '2025-10-20 08:10:00', 1, 'Stomach protection', NULL),
(2, 8, '2025-10-20 20:00:00', 1, 'Evening pain relief', NULL);

-- Tuesday 2025-10-21 - PARTIALLY TAKEN ❌
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 5, '2025-10-21 08:15:00', 2, 'Morning dose', NULL),
(2, 7, '2025-10-21 08:20:00', 1, 'Before breakfast', NULL);
-- Missing noon and evening doses

-- Wednesday 2025-10-22 - ALL TAKEN ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 5, '2025-10-22 07:45:00', 2, 'Early morning', NULL),
(2, 6, '2025-10-22 12:15:00', 1, 'With lunch', NULL),
(2, 7, '2025-10-22 07:50:00', 1, 'Before meal', NULL),
(2, 8, '2025-10-22 19:45:00', 1, 'Evening dose', NULL);

-- Patient 3 (Franz Wagner) - Sparse entries
-- Plans: 9, 10, 11, 12

-- Monday 2025-10-20 - PARTIALLY TAKEN ❌
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(3, 9, '2025-10-20 08:00:00', 1, 'Heart medication', 'E200 1041 8064 0105 2802 3C26'),
(3, 10, '2025-10-20 08:05:00', 1, 'Blood pressure', 'E200 1041 8064 0105 2802 3C26');
-- Missing afternoon and evening doses

-- Historical data - September 2025 for Patient 1
-- Week of Sept 15-21
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
-- Monday 2025-09-15 ✅
(1, 1, '2025-09-15 08:00:00', 1, 'Regular dose'),
(1, 2, '2025-09-15 08:15:00', 1, 'Morning'),
(1, 3, '2025-09-15 12:30:00', 1, 'Noon'),
(1, 4, '2025-09-15 20:00:00', 5, 'Evening'),
-- Tuesday 2025-09-16 ✅
(1, 1, '2025-09-16 08:10:00', 1, 'Morning'),
(1, 2, '2025-09-16 08:20:00', 1, 'With breakfast'),
(1, 3, '2025-09-16 12:45:00', 1, 'Lunch time'),
(1, 4, '2025-09-16 19:45:00', 5, 'Evening'),
-- Wednesday 2025-09-17 ❌ Nothing taken
-- Thursday 2025-09-18 ✅
(1, 1, '2025-09-18 07:50:00', 1, 'Early morning'),
(1, 2, '2025-09-18 08:00:00', 1, 'Breakfast'),
(1, 3, '2025-09-18 12:30:00', 1, 'Noon'),
(1, 4, '2025-09-18 20:10:00', 5, 'Evening'),
-- Friday 2025-09-19 ❌ Partially (missing evening)
(1, 1, '2025-09-19 08:00:00', 1, 'Morning'),
(1, 2, '2025-09-19 08:15:00', 1, 'With meal'),
(1, 3, '2025-09-19 13:00:00', 1, 'Lunch');

-- ========================================
-- EXTENDED TEST DATA FOR CALENDAR FRONTEND TESTING
-- Patient 1 (Johann Meier) - Complete Data for 3 Months
-- September 2025: Complete month (30 days)
-- October 2025: Complete month (31 days)  
-- November 2025: Partial month (until Nov 22, 2025)
-- ========================================

-- September 2025 - Remaining days (Sept 20-30)
-- Saturday 2025-09-20 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-20 09:30:00', 1, 'Weekend morning'),
(1, 2, '2025-09-20 09:45:00', 1, 'Leisurely breakfast'),
(1, 4, '2025-09-20 21:00:00', 5, 'Late evening');

-- Sunday 2025-09-21 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-21 09:15:00', 1, 'Sunday'),
(1, 2, '2025-09-21 09:30:00', 1, 'Breakfast'),
(1, 4, '2025-09-21 20:15:00', 5, 'Evening');

-- Monday 2025-09-22 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-22 08:00:00', 1, 'Monday start'),
(1, 2, '2025-09-22 08:15:00', 1, 'Morning'),
(1, 3, '2025-09-22 12:30:00', 1, 'Lunch'),
(1, 4, '2025-09-22 20:00:00', 5, 'Evening');

-- Tuesday 2025-09-23 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-23 08:10:00', 1, 'Regular time'),
(1, 2, '2025-09-23 08:25:00', 1, 'Breakfast'),
(1, 3, '2025-09-23 12:40:00', 1, 'Midday'),
(1, 4, '2025-09-23 19:55:00', 5, 'Evening');

-- Wednesday 2025-09-24 ❌ Partially (visiting family - forgot evening)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-24 08:00:00', 1, 'Morning before trip'),
(1, 2, '2025-09-24 08:15:00', 1, 'Quick breakfast'),
(1, 3, '2025-09-24 12:30:00', 1, 'On the road');

-- Thursday 2025-09-25 ❌ Partially (still away - missed noon)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-25 09:00:00', 1, 'At relatives'),
(1, 2, '2025-09-25 09:20:00', 1, 'Late breakfast'),
(1, 4, '2025-09-25 21:30:00', 5, 'Evening');

-- Friday 2025-09-26 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-26 08:00:00', 1, 'Back to routine'),
(1, 2, '2025-09-26 08:15:00', 1, 'Home again'),
(1, 3, '2025-09-26 12:30:00', 1, 'Normal schedule'),
(1, 4, '2025-09-26 20:00:00', 5, 'Regular evening');

-- Saturday 2025-09-27 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-27 09:00:00', 1, 'Weekend'),
(1, 2, '2025-09-27 09:20:00', 1, 'Late morning'),
(1, 4, '2025-09-27 20:30:00', 5, 'Evening');

-- Sunday 2025-09-28 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-28 09:30:00', 1, 'Relaxed morning'),
(1, 2, '2025-09-28 09:45:00', 1, 'Breakfast'),
(1, 4, '2025-09-28 20:45:00', 5, 'Evening');

-- Monday 2025-09-29 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-29 08:00:00', 1, 'Last Monday of month'),
(1, 2, '2025-09-29 08:15:00', 1, 'Morning'),
(1, 3, '2025-09-29 12:30:00', 1, 'Lunch'),
(1, 4, '2025-09-29 20:00:00', 5, 'Evening');

-- Tuesday 2025-09-30 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-09-30 08:05:00', 1, 'Month end'),
(1, 2, '2025-09-30 08:20:00', 1, 'Regular'),
(1, 3, '2025-09-30 12:35:00', 1, 'Noon'),
(1, 4, '2025-09-30 19:50:00', 5, 'Last day');

-- ========================================
-- OCTOBER 2025 - Remaining Days (Oct 27-31)
-- ========================================

-- Monday 2025-10-27 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-10-27 08:00:00', 1, 'Last week of October'),
(1, 2, '2025-10-27 08:15:00', 1, 'Morning'),
(1, 3, '2025-10-27 12:30:00', 1, 'Lunch'),
(1, 4, '2025-10-27 20:00:00', 5, 'Evening');

-- Tuesday 2025-10-28 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-10-28 08:10:00', 1, 'Regular'),
(1, 2, '2025-10-28 08:25:00', 1, 'Breakfast'),
(1, 3, '2025-10-28 12:40:00', 1, 'Noon'),
(1, 4, '2025-10-28 19:55:00', 5, 'Evening');

-- Wednesday 2025-10-29 ❌ Partially (forgot evening)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-10-29 08:00:00', 1, 'Morning'),
(1, 2, '2025-10-29 08:15:00', 1, 'Breakfast'),
(1, 3, '2025-10-29 12:30:00', 1, 'Lunch');

-- Thursday 2025-10-30 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-10-30 08:05:00', 1, 'Almost Halloween'),
(1, 2, '2025-10-30 08:20:00', 1, 'Morning'),
(1, 3, '2025-10-30 12:35:00', 1, 'Lunch'),
(1, 4, '2025-10-30 20:00:00', 5, 'Evening');

-- Friday 2025-10-31 ✅ Halloween!
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-10-31 08:00:00', 1, 'Happy Halloween'),
(1, 2, '2025-10-31 08:20:00', 1, 'Spooky morning'),
(1, 3, '2025-10-31 12:30:00', 1, 'Lunch'),
(1, 4, '2025-10-31 19:30:00', 5, 'Before trick-or-treaters');

-- ========================================
-- NOVEMBER 2025 - Partial Month (Nov 1-22)
-- ========================================

-- Saturday 2025-11-01 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-01 09:00:00', 1, 'New month weekend'),
(1, 2, '2025-11-01 09:20:00', 1, 'November starts'),
(1, 4, '2025-11-01 20:30:00', 5, 'Evening');

-- Sunday 2025-11-02 ❌ Partially
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-02 09:30:00', 1, 'Sunday morning'),
(1, 2, '2025-11-02 09:45:00', 1, 'Late breakfast');

-- Monday 2025-11-03 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-03 08:00:00', 1, 'Week start'),
(1, 2, '2025-11-03 08:15:00', 1, 'Monday morning'),
(1, 3, '2025-11-03 12:30:00', 1, 'Lunch'),
(1, 4, '2025-11-03 20:00:00', 5, 'Evening');

-- Tuesday 2025-11-04 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-04 08:10:00', 1, 'Regular'),
(1, 2, '2025-11-04 08:25:00', 1, 'Morning'),
(1, 3, '2025-11-04 12:40:00', 1, 'Midday'),
(1, 4, '2025-11-04 19:55:00', 5, 'Evening');

-- Wednesday 2025-11-05 ❌ Nothing taken (bad day)

-- Thursday 2025-11-06 ❌ Partially
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-06 10:00:00', 1, 'Late morning'),
(1, 2, '2025-11-06 10:15:00', 1, 'Essential meds only'),
(1, 4, '2025-11-06 21:00:00', 5, 'Evening');

-- Friday 2025-11-07 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-07 08:00:00', 1, 'Feeling better'),
(1, 2, '2025-11-07 08:20:00', 1, 'Back to normal'),
(1, 3, '2025-11-07 12:30:00', 1, 'Lunch'),
(1, 4, '2025-11-07 20:00:00', 5, 'Evening');

-- Saturday 2025-11-08 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-08 09:15:00', 1, 'Weekend'),
(1, 2, '2025-11-08 09:30:00', 1, 'Morning'),
(1, 4, '2025-11-08 20:30:00', 5, 'Evening');

-- Sunday 2025-11-09 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-09 09:00:00', 1, 'Sunday routine'),
(1, 2, '2025-11-09 09:20:00', 1, 'Breakfast'),
(1, 4, '2025-11-09 20:15:00', 5, 'Evening');

-- Monday 2025-11-10 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-10 08:00:00', 1, 'Monday'),
(1, 2, '2025-11-10 08:15:00', 1, 'Morning'),
(1, 3, '2025-11-10 12:30:00', 1, 'Lunch'),
(1, 4, '2025-11-10 20:00:00', 5, 'Evening');

-- Tuesday 2025-11-11 ❌ Partially
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-11 08:10:00', 1, 'Morning'),
(1, 2, '2025-11-11 08:25:00', 1, 'Breakfast'),
(1, 4, '2025-11-11 19:50:00', 5, 'Evening');

-- Wednesday 2025-11-12 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-12 08:00:00', 1, 'Mid-week'),
(1, 2, '2025-11-12 08:20:00', 1, 'Morning'),
(1, 3, '2025-11-12 12:45:00', 1, 'Lunch'),
(1, 4, '2025-11-12 20:05:00', 5, 'Evening');

-- Thursday 2025-11-13 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-13 08:05:00', 1, 'Regular'),
(1, 2, '2025-11-13 08:20:00', 1, 'Morning'),
(1, 3, '2025-11-13 12:35:00', 1, 'Noon'),
(1, 4, '2025-11-13 19:55:00', 5, 'Evening');

-- Friday 2025-11-14 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-14 08:00:00', 1, 'TGIF'),
(1, 2, '2025-11-14 08:20:00', 1, 'Friday'),
(1, 3, '2025-11-14 12:30:00', 1, 'Lunch'),
(1, 4, '2025-11-14 19:45:00', 5, 'Weekend soon');

-- Saturday 2025-11-15 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-15 09:30:00', 1, 'Weekend'),
(1, 2, '2025-11-15 09:45:00', 1, 'Late morning'),
(1, 4, '2025-11-15 21:00:00', 5, 'Evening');

-- Sunday 2025-11-16 ❌ Partially
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 2, '2025-11-16 10:00:00', 1, 'Lazy Sunday'),
(1, 4, '2025-11-16 20:30:00', 5, 'Evening');

-- Monday 2025-11-17 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-17 08:00:00', 1, 'Week start'),
(1, 2, '2025-11-17 08:15:00', 1, 'Monday'),
(1, 3, '2025-11-17 12:30:00', 1, 'Lunch'),
(1, 4, '2025-11-17 20:00:00', 5, 'Evening');

-- Tuesday 2025-11-18 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-18 08:10:00', 1, 'Regular'),
(1, 2, '2025-11-18 08:25:00', 1, 'Morning'),
(1, 3, '2025-11-18 12:40:00', 1, 'Midday'),
(1, 4, '2025-11-18 19:55:00', 5, 'Evening');

-- Wednesday 2025-11-19 ❌ Partially
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-19 07:45:00', 1, 'Early'),
(1, 2, '2025-11-19 08:00:00', 1, 'Quick breakfast'),
(1, 4, '2025-11-19 21:30:00', 5, 'Late evening');

-- Thursday 2025-11-20 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-20 08:00:00', 1, 'Back on track'),
(1, 2, '2025-11-20 08:20:00', 1, 'Morning'),
(1, 3, '2025-11-20 12:30:00', 1, 'Lunch'),
(1, 4, '2025-11-20 20:00:00', 5, 'Evening');

-- Friday 2025-11-21 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-21 08:05:00', 1, 'Friday feeling'),
(1, 2, '2025-11-21 08:20:00', 1, 'Morning'),
(1, 3, '2025-11-21 12:35:00', 1, 'Lunch'),
(1, 4, '2025-11-21 19:50:00', 5, 'Weekend ahead');

-- Saturday 2025-11-22 ✅ (Partial - only morning so far)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes) VALUES
(1, 1, '2025-11-22 09:00:00', 1, 'Saturday morning'),
(1, 2, '2025-11-22 09:20:00', 1, 'Weekend breakfast');

-- November 23-30 intentionally left empty for ongoing testing