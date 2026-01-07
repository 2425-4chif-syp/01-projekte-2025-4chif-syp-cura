-- JANUARY 2026 - First 10 days (Problematic: Jan 7 - forgot evening)
-- Wed Jan 1 (New Year) ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-01 09:30:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-01-01 09:30:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 4, '2026-01-01 20:15:00', 5, 'Evening intake - Vitamin D3', '04E5AE6EBA2A81');

-- Thu Jan 2 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-02 08:00:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-01-02 08:00:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-01-02 12:30:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-01-02 20:00:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Jan 3 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-03 08:10:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-01-03 08:10:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-01-03 12:40:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-01-03 19:55:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');

-- Sat Jan 4 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-04 09:15:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-01-04 09:15:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-01-04 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Jan 5 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-05 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-01-05 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-01-05 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Jan 6 (Epiphany) ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-06 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-01-06 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-01-06 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-01-06 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Jan 7 ❌ PARTIALLY (forgot evening)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-07 08:10:00', 1, 'Morning intake - Blood pressure', '04E6AE6EBA2A81'),
(1, 2, '2026-01-07 08:10:00', 1, 'Morning intake - Heart medication', '04E6AE6EBA2A81'),
(1, 3, '2026-01-07 12:35:00', 1, 'Noon intake - Aspirin', '04E6AE6EBA2A81');

-- Wed Jan 8 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-08 08:00:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-01-08 08:00:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 3, '2026-01-08 12:30:00', 1, 'Noon intake - Aspirin', '04E5AE6EBA2A81'),
(1, 4, '2026-01-08 20:05:00', 5, 'Evening intake - Vitamin D3', '04E5AE6EBA2A81');

-- Thu Jan 9 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-09 08:05:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-01-09 08:05:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-01-09 12:40:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-01-09 19:50:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Jan 10 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-10 08:00:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-01-10 08:00:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-01-10 12:30:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-01-10 20:00:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');

-- Sat Jan 11 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-11 09:20:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-01-11 09:20:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-01-11 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Jan 12 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-12 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-01-12 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-01-12 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Jan 13 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-13 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-01-13 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-01-13 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-01-13 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Jan 14 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-14 08:10:00', 1, 'Morning intake - Blood pressure', '04E6AE6EBA2A81'),
(1, 2, '2026-01-14 08:10:00', 1, 'Morning intake - Heart medication', '04E6AE6EBA2A81'),
(1, 3, '2026-01-14 12:35:00', 1, 'Noon intake - Aspirin', '04E6AE6EBA2A81'),
(1, 4, '2026-01-14 19:55:00', 5, 'Evening intake - Vitamin D3', '04E6AE6EBA2A81');

-- Wed Jan 15 ❌ NOTHING (sick day)

-- Thu Jan 16 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-16 10:00:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-01-16 10:00:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-01-16 13:00:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-01-16 21:00:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Jan 17 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-17 08:00:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-01-17 08:00:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-01-17 12:30:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-01-17 20:00:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');

-- Sat Jan 18 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-18 09:15:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-01-18 09:15:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-01-18 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Jan 19 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-19 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-01-19 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-01-19 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Jan 20 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-20 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-01-20 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-01-20 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-01-20 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Jan 21 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-21 08:10:00', 1, 'Morning intake - Blood pressure', '04E6AE6EBA2A81'),
(1, 2, '2026-01-21 08:10:00', 1, 'Morning intake - Heart medication', '04E6AE6EBA2A81'),
(1, 3, '2026-01-21 12:35:00', 1, 'Noon intake - Aspirin', '04E6AE6EBA2A81'),
(1, 4, '2026-01-21 19:55:00', 5, 'Evening intake - Vitamin D3', '04E6AE6EBA2A81');

-- Wed Jan 22 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-22 08:00:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-01-22 08:00:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 3, '2026-01-22 12:30:00', 1, 'Noon intake - Aspirin', '04E5AE6EBA2A81'),
(1, 4, '2026-01-22 20:00:00', 5, 'Evening intake - Vitamin D3', '04E5AE6EBA2A81');

-- Thu Jan 23 ❌ PARTIALLY (forgot noon)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-23 08:05:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-01-23 08:05:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 4, '2026-01-23 20:00:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Jan 24 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-24 08:00:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-01-24 08:00:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-01-24 12:30:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-01-24 20:00:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');

-- Sat Jan 25 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-25 09:20:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-01-25 09:20:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-01-25 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Jan 26 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-26 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-01-26 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-01-26 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Jan 27 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-27 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-01-27 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-01-27 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-01-27 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Jan 28 ❌ PARTIALLY (late morning, forgot noon and evening)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-28 10:30:00', 1, 'Morning intake - Blood pressure', '04E6AE6EBA2A81'),
(1, 2, '2026-01-28 10:30:00', 1, 'Morning intake - Heart medication', '04E6AE6EBA2A81');

-- Wed Jan 29 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-29 08:00:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-01-29 08:00:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 3, '2026-01-29 12:30:00', 1, 'Noon intake - Aspirin', '04E5AE6EBA2A81'),
(1, 4, '2026-01-29 20:00:00', 5, 'Evening intake - Vitamin D3', '04E5AE6EBA2A81');

-- Thu Jan 30 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-30 08:05:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-01-30 08:05:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-01-30 12:35:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-01-30 19:55:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Jan 31 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-01-31 08:00:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-01-31 08:00:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-01-31 12:30:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-01-31 20:00:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');


-- ========================================
-- FEBRUARY 2026 - 28 days
-- ========================================

-- Sat Feb 1 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-01 09:15:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-02-01 09:15:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-02-01 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Feb 2 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-02 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-02-02 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-02-02 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Feb 3 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-03 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-02-03 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-02-03 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-02-03 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Feb 4 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-04 08:10:00', 1, 'Morning intake - Blood pressure', '04E6AE6EBA2A81'),
(1, 2, '2026-02-04 08:10:00', 1, 'Morning intake - Heart medication', '04E6AE6EBA2A81'),
(1, 3, '2026-02-04 12:35:00', 1, 'Noon intake - Aspirin', '04E6AE6EBA2A81'),
(1, 4, '2026-02-04 19:55:00', 5, 'Evening intake - Vitamin D3', '04E6AE6EBA2A81');

-- Wed Feb 5 ❌ PARTIALLY (forgot evening)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-05 08:00:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-02-05 08:00:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 3, '2026-02-05 12:30:00', 1, 'Noon intake - Aspirin', '04E5AE6EBA2A81');

-- Thu Feb 6 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-06 08:05:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-02-06 08:05:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-02-06 12:35:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-02-06 19:50:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Feb 7 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-07 08:00:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-02-07 08:00:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-02-07 12:30:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-02-07 20:00:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');

-- Sat Feb 8 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-08 09:20:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-02-08 09:20:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-02-08 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Feb 9 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-09 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-02-09 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-02-09 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Feb 10 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-10 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-02-10 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-02-10 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-02-10 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Feb 11 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-11 08:10:00', 1, 'Morning intake - Blood pressure', '04E6AE6EBA2A81'),
(1, 2, '2026-02-11 08:10:00', 1, 'Morning intake - Heart medication', '04E6AE6EBA2A81'),
(1, 3, '2026-02-11 12:35:00', 1, 'Noon intake - Aspirin', '04E6AE6EBA2A81'),
(1, 4, '2026-02-11 19:55:00', 5, 'Evening intake - Vitamin D3', '04E6AE6EBA2A81');

-- Wed Feb 12 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-12 08:00:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-02-12 08:00:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 3, '2026-02-12 12:30:00', 1, 'Noon intake - Aspirin', '04E5AE6EBA2A81'),
(1, 4, '2026-02-12 20:00:00', 5, 'Evening intake - Vitamin D3', '04E5AE6EBA2A81');

-- Thu Feb 13 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-13 08:05:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-02-13 08:05:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-02-13 12:35:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-02-13 19:50:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Feb 14 ❌ NOTHING (Valentine's Day - out all day)

-- Sat Feb 15 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-15 09:15:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-02-15 09:15:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-02-15 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Feb 16 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-16 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-02-16 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-02-16 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Feb 17 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-17 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-02-17 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-02-17 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-02-17 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Feb 18 ❌ PARTIALLY (forgot noon)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-18 08:10:00', 1, 'Morning intake - Blood pressure', '04E6AE6EBA2A81'),
(1, 2, '2026-02-18 08:10:00', 1, 'Morning intake - Heart medication', '04E6AE6EBA2A81'),
(1, 4, '2026-02-18 19:55:00', 5, 'Evening intake - Vitamin D3', '04E6AE6EBA2A81');

-- Wed Feb 19 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-19 08:00:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-02-19 08:00:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 3, '2026-02-19 12:30:00', 1, 'Noon intake - Aspirin', '04E5AE6EBA2A81'),
(1, 4, '2026-02-19 20:00:00', 5, 'Evening intake - Vitamin D3', '04E5AE6EBA2A81');

-- Thu Feb 20 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-20 08:05:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-02-20 08:05:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-02-20 12:35:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-02-20 19:50:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Feb 21 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-21 08:00:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-02-21 08:00:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-02-21 12:30:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-02-21 20:00:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');

-- Sat Feb 22 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-22 09:20:00', 1, 'Morning intake - Blood pressure', '04DDAE6EBA2A81'),
(1, 2, '2026-02-22 09:20:00', 1, 'Morning intake - Heart medication', '04DDAE6EBA2A81'),
(1, 4, '2026-02-22 20:30:00', 5, 'Evening intake - Vitamin D3', '04DDAE6EBA2A81');

-- Sun Feb 23 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-23 09:30:00', 1, 'Morning intake - Blood pressure', '0441AB6EBA2A81'),
(1, 2, '2026-02-23 09:30:00', 1, 'Morning intake - Heart medication', '0441AB6EBA2A81'),
(1, 4, '2026-02-23 20:15:00', 5, 'Evening intake - Vitamin D3', '0441AB6EBA2A81');

-- Mon Feb 24 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-24 08:00:00', 1, 'Morning intake - Blood pressure', '04E7AE6EBA2A81'),
(1, 2, '2026-02-24 08:00:00', 1, 'Morning intake - Heart medication', '04E7AE6EBA2A81'),
(1, 3, '2026-02-24 12:30:00', 1, 'Noon intake - Aspirin', '04E7AE6EBA2A81'),
(1, 4, '2026-02-24 20:00:00', 5, 'Evening intake - Vitamin D3', '04E7AE6EBA2A81');

-- Tue Feb 25 ❌ PARTIALLY (forgot morning)
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 3, '2026-02-25 12:35:00', 1, 'Noon intake - Aspirin', '04E6AE6EBA2A81'),
(1, 4, '2026-02-25 19:55:00', 5, 'Evening intake - Vitamin D3', '04E6AE6EBA2A81');

-- Wed Feb 26 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-26 08:00:00', 1, 'Morning intake - Blood pressure', '04E5AE6EBA2A81'),
(1, 2, '2026-02-26 08:00:00', 1, 'Morning intake - Heart medication', '04E5AE6EBA2A81'),
(1, 3, '2026-02-26 12:30:00', 1, 'Noon intake - Aspirin', '04E5AE6EBA2A81'),
(1, 4, '2026-02-26 20:00:00', 5, 'Evening intake - Vitamin D3', '04E5AE6EBA2A81');

-- Thu Feb 27 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-27 08:05:00', 1, 'Morning intake - Blood pressure', '04E4AE6EBA2A81'),
(1, 2, '2026-02-27 08:05:00', 1, 'Morning intake - Heart medication', '04E4AE6EBA2A81'),
(1, 3, '2026-02-27 12:35:00', 1, 'Noon intake - Aspirin', '04E4AE6EBA2A81'),
(1, 4, '2026-02-27 19:50:00', 5, 'Evening intake - Vitamin D3', '04E4AE6EBA2A81');

-- Fri Feb 28 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(1, 1, '2026-02-28 08:00:00', 1, 'Morning intake - Blood pressure', '04DFAE6EBA2A81'),
(1, 2, '2026-02-28 08:00:00', 1, 'Morning intake - Heart medication', '04DFAE6EBA2A81'),
(1, 3, '2026-02-28 12:30:00', 1, 'Noon intake - Aspirin', '04DFAE6EBA2A81'),
(1, 4, '2026-02-28 20:00:00', 5, 'Evening intake - Vitamin D3', '04DFAE6EBA2A81');