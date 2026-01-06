-- JANUARY 2026 - 31 days (Problematic: Jan 7, 15, 23, 28)
-- Wed Jan 1 (New Year) ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-01', 1, '2026-01-01 09:30:00', '04E5AE6EBA2A81'),
(1, '2026-01-01', 8, '2026-01-01 20:15:00', '04E5AE6EBA2A81');

-- Thu Jan 2 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-02', 1, '2026-01-02 08:00:00', '04E4AE6EBA2A81'),
(1, '2026-01-02', 2, '2026-01-02 12:30:00', '04E4AE6EBA2A81'),
(1, '2026-01-02', 8, '2026-01-02 20:00:00', '04E4AE6EBA2A81');

-- Fri Jan 3 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-03', 1, '2026-01-03 08:10:00', '04DFAE6EBA2A81'),
(1, '2026-01-03', 2, '2026-01-03 12:40:00', '04DFAE6EBA2A81'),
(1, '2026-01-03', 8, '2026-01-03 19:55:00', '04DFAE6EBA2A81');

-- Sat Jan 4 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-04', 1, '2026-01-04 09:15:00', '04DDAE6EBA2A81'),
(1, '2026-01-04', 8, '2026-01-04 20:30:00', '04DDAE6EBA2A81');

-- Sun Jan 5 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-05', 1, '2026-01-05 09:30:00', '0441AB6EBA2A81'),
(1, '2026-01-05', 8, '2026-01-05 20:15:00', '0441AB6EBA2A81');

-- Mon Jan 6 (Epiphany) ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-06', 1, '2026-01-06 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-01-06', 2, '2026-01-06 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-01-06', 8, '2026-01-06 20:00:00', '04E7AE6EBA2A81');

-- Tue Jan 7 ❌ PARTIALLY (forgot evening)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-07', 1, '2026-01-07 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-01-07', 2, '2026-01-07 12:35:00', '04E6AE6EBA2A81');

-- Wed Jan 8 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-08', 1, '2026-01-08 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-01-08', 2, '2026-01-08 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-01-08', 8, '2026-01-08 20:05:00', '04E5AE6EBA2A81');

-- Thu Jan 9 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-09', 1, '2026-01-09 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-01-09', 2, '2026-01-09 12:40:00', '04E4AE6EBA2A81'),
(1, '2026-01-09', 8, '2026-01-09 19:50:00', '04E4AE6EBA2A81');

-- Fri Jan 10 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-10', 1, '2026-01-10 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-01-10', 2, '2026-01-10 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-01-10', 8, '2026-01-10 20:00:00', '04DFAE6EBA2A81');

-- Sat Jan 11 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-11', 1, '2026-01-11 09:20:00', '04DDAE6EBA2A81'),
(1, '2026-01-11', 8, '2026-01-11 20:30:00', '04DDAE6EBA2A81');

-- Sun Jan 12 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-12', 1, '2026-01-12 09:30:00', '0441AB6EBA2A81'),
(1, '2026-01-12', 8, '2026-01-12 20:15:00', '0441AB6EBA2A81');

-- Mon Jan 13 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-13', 1, '2026-01-13 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-01-13', 2, '2026-01-13 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-01-13', 8, '2026-01-13 20:00:00', '04E7AE6EBA2A81');

-- Tue Jan 14 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-14', 1, '2026-01-14 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-01-14', 2, '2026-01-14 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-01-14', 8, '2026-01-14 19:55:00', '04E6AE6EBA2A81');

-- Wed Jan 15 ❌ NOTHING (sick day)

-- Thu Jan 16 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-16', 1, '2026-01-16 10:00:00', '04E4AE6EBA2A81'),
(1, '2026-01-16', 2, '2026-01-16 13:00:00', '04E4AE6EBA2A81'),
(1, '2026-01-16', 8, '2026-01-16 21:00:00', '04E4AE6EBA2A81');

-- Fri Jan 17 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-17', 1, '2026-01-17 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-01-17', 2, '2026-01-17 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-01-17', 8, '2026-01-17 20:00:00', '04DFAE6EBA2A81');

-- Sat Jan 18 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-18', 1, '2026-01-18 09:15:00', '04DDAE6EBA2A81'),
(1, '2026-01-18', 8, '2026-01-18 20:30:00', '04DDAE6EBA2A81');

-- Sun Jan 19 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-19', 1, '2026-01-19 09:30:00', '0441AB6EBA2A81'),
(1, '2026-01-19', 8, '2026-01-19 20:15:00', '0441AB6EBA2A81');

-- Mon Jan 20 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-20', 1, '2026-01-20 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-01-20', 2, '2026-01-20 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-01-20', 8, '2026-01-20 20:00:00', '04E7AE6EBA2A81');

-- Tue Jan 21 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-21', 1, '2026-01-21 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-01-21', 2, '2026-01-21 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-01-21', 8, '2026-01-21 19:55:00', '04E6AE6EBA2A81');

-- Wed Jan 22 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-22', 1, '2026-01-22 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-01-22', 2, '2026-01-22 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-01-22', 8, '2026-01-22 20:00:00', '04E5AE6EBA2A81');

-- Thu Jan 23 ❌ PARTIALLY (forgot noon)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-23', 1, '2026-01-23 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-01-23', 8, '2026-01-23 20:00:00', '04E4AE6EBA2A81');

-- Fri Jan 24 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-24', 1, '2026-01-24 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-01-24', 2, '2026-01-24 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-01-24', 8, '2026-01-24 20:00:00', '04DFAE6EBA2A81');

-- Sat Jan 25 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-25', 1, '2026-01-25 09:20:00', '04DDAE6EBA2A81'),
(1, '2026-01-25', 8, '2026-01-25 20:30:00', '04DDAE6EBA2A81');

-- Sun Jan 26 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-26', 1, '2026-01-26 09:30:00', '0441AB6EBA2A81'),
(1, '2026-01-26', 8, '2026-01-26 20:15:00', '0441AB6EBA2A81');

-- Mon Jan 27 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-27', 1, '2026-01-27 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-01-27', 2, '2026-01-27 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-01-27', 8, '2026-01-27 20:00:00', '04E7AE6EBA2A81');

-- Tue Jan 28 ❌ PARTIALLY (late morning, forgot noon and evening)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-28', 1, '2026-01-28 10:30:00', '04E6AE6EBA2A81');

-- Wed Jan 29 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-29', 1, '2026-01-29 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-01-29', 2, '2026-01-29 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-01-29', 8, '2026-01-29 20:00:00', '04E5AE6EBA2A81');

-- Thu Jan 30 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-30', 1, '2026-01-30 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-01-30', 2, '2026-01-30 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-01-30', 8, '2026-01-30 19:55:00', '04E4AE6EBA2A81');

-- Fri Jan 31 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-01-31', 1, '2026-01-31 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-01-31', 2, '2026-01-31 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-01-31', 8, '2026-01-31 20:00:00', '04DFAE6EBA2A81');


-- ========================================
-- FEBRUARY 2026 - 28 days (Problematic: Feb 5, 14, 18, 25)
-- ========================================

-- Sat Feb 1 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-01', 1, '2026-02-01 09:15:00', '04DDAE6EBA2A81'),
(1, '2026-02-01', 8, '2026-02-01 20:30:00', '04DDAE6EBA2A81');

-- Sun Feb 2 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-02', 1, '2026-02-02 09:30:00', '0441AB6EBA2A81'),
(1, '2026-02-02', 8, '2026-02-02 20:15:00', '0441AB6EBA2A81');

-- Mon Feb 3 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-03', 1, '2026-02-03 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-02-03', 2, '2026-02-03 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-02-03', 8, '2026-02-03 20:00:00', '04E7AE6EBA2A81');

-- Tue Feb 4 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-04', 1, '2026-02-04 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-02-04', 2, '2026-02-04 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-02-04', 8, '2026-02-04 19:55:00', '04E6AE6EBA2A81');

-- Wed Feb 5 ❌ PARTIALLY (forgot evening)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-05', 1, '2026-02-05 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-02-05', 2, '2026-02-05 12:30:00', '04E5AE6EBA2A81');

-- Thu Feb 6 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-06', 1, '2026-02-06 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-02-06', 2, '2026-02-06 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-02-06', 8, '2026-02-06 19:50:00', '04E4AE6EBA2A81');

-- Fri Feb 7 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-07', 1, '2026-02-07 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-02-07', 2, '2026-02-07 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-02-07', 8, '2026-02-07 20:00:00', '04DFAE6EBA2A81');

-- Sat Feb 8 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-08', 1, '2026-02-08 09:20:00', '04DDAE6EBA2A81'),
(1, '2026-02-08', 8, '2026-02-08 20:30:00', '04DDAE6EBA2A81');

-- Sun Feb 9 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-09', 1, '2026-02-09 09:30:00', '0441AB6EBA2A81'),
(1, '2026-02-09', 8, '2026-02-09 20:15:00', '0441AB6EBA2A81');

-- Mon Feb 10 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-10', 1, '2026-02-10 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-02-10', 2, '2026-02-10 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-02-10', 8, '2026-02-10 20:00:00', '04E7AE6EBA2A81');

-- Tue Feb 11 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-11', 1, '2026-02-11 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-02-11', 2, '2026-02-11 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-02-11', 8, '2026-02-11 19:55:00', '04E6AE6EBA2A81');

-- Wed Feb 12 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-12', 1, '2026-02-12 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-02-12', 2, '2026-02-12 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-02-12', 8, '2026-02-12 20:00:00', '04E5AE6EBA2A81');

-- Thu Feb 13 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-13', 1, '2026-02-13 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-02-13', 2, '2026-02-13 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-02-13', 8, '2026-02-13 19:50:00', '04E4AE6EBA2A81');

-- Fri Feb 14 ❌ NOTHING (Valentine's Day - out all day)

-- Sat Feb 15 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-15', 1, '2026-02-15 09:15:00', '04DDAE6EBA2A81'),
(1, '2026-02-15', 8, '2026-02-15 20:30:00', '04DDAE6EBA2A81');

-- Sun Feb 16 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-16', 1, '2026-02-16 09:30:00', '0441AB6EBA2A81'),
(1, '2026-02-16', 8, '2026-02-16 20:15:00', '0441AB6EBA2A81');

-- Mon Feb 17 (Rosenmontag) ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-17', 1, '2026-02-17 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-02-17', 2, '2026-02-17 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-02-17', 8, '2026-02-17 20:00:00', '04E7AE6EBA2A81');

-- Tue Feb 18 ❌ PARTIALLY (Fasching - forgot noon)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-18', 1, '2026-02-18 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-02-18', 8, '2026-02-18 19:55:00', '04E6AE6EBA2A81');

-- Wed Feb 19 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-19', 1, '2026-02-19 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-02-19', 2, '2026-02-19 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-02-19', 8, '2026-02-19 20:00:00', '04E5AE6EBA2A81');

-- Thu Feb 20 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-20', 1, '2026-02-20 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-02-20', 2, '2026-02-20 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-02-20', 8, '2026-02-20 19:50:00', '04E4AE6EBA2A81');

-- Fri Feb 21 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-21', 1, '2026-02-21 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-02-21', 2, '2026-02-21 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-02-21', 8, '2026-02-21 20:00:00', '04DFAE6EBA2A81');

-- Sat Feb 22 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-22', 1, '2026-02-22 09:20:00', '04DDAE6EBA2A81'),
(1, '2026-02-22', 8, '2026-02-22 20:30:00', '04DDAE6EBA2A81');

-- Sun Feb 23 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-23', 1, '2026-02-23 09:30:00', '0441AB6EBA2A81'),
(1, '2026-02-23', 8, '2026-02-23 20:15:00', '0441AB6EBA2A81');

-- Mon Feb 24 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-24', 1, '2026-02-24 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-02-24', 2, '2026-02-24 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-02-24', 8, '2026-02-24 20:00:00', '04E7AE6EBA2A81');

-- Tue Feb 25 ❌ PARTIALLY (forgot morning)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-25', 2, '2026-02-25 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-02-25', 8, '2026-02-25 19:55:00', '04E6AE6EBA2A81');

-- Wed Feb 26 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-26', 1, '2026-02-26 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-02-26', 2, '2026-02-26 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-02-26', 8, '2026-02-26 20:00:00', '04E5AE6EBA2A81');

-- Thu Feb 27 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-27', 1, '2026-02-27 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-02-27', 2, '2026-02-27 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-02-27', 8, '2026-02-27 19:50:00', '04E4AE6EBA2A81');

-- Fri Feb 28 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-02-28', 1, '2026-02-28 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-02-28', 2, '2026-02-28 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-02-28', 8, '2026-02-28 20:00:00', '04DFAE6EBA2A81');


-- ========================================
-- MARCH 2026 - 31 days (Problematic: Mar 8, 15, 22, 29)
-- ========================================

-- Sat Mar 1 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-01', 1, '2026-03-01 09:15:00', '04DDAE6EBA2A81'),
(1, '2026-03-01', 8, '2026-03-01 20:30:00', '04DDAE6EBA2A81');

-- Sun Mar 2 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-02', 1, '2026-03-02 09:30:00', '0441AB6EBA2A81'),
(1, '2026-03-02', 8, '2026-03-02 20:15:00', '0441AB6EBA2A81');

-- Mon Mar 3 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-03', 1, '2026-03-03 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-03-03', 2, '2026-03-03 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-03-03', 8, '2026-03-03 20:00:00', '04E7AE6EBA2A81');

-- Tue Mar 4 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-04', 1, '2026-03-04 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-03-04', 2, '2026-03-04 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-03-04', 8, '2026-03-04 19:55:00', '04E6AE6EBA2A81');

-- Wed Mar 5 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-05', 1, '2026-03-05 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-03-05', 2, '2026-03-05 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-03-05', 8, '2026-03-05 20:00:00', '04E5AE6EBA2A81');

-- Thu Mar 6 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-06', 1, '2026-03-06 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-03-06', 2, '2026-03-06 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-03-06', 8, '2026-03-06 19:50:00', '04E4AE6EBA2A81');

-- Fri Mar 7 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-07', 1, '2026-03-07 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-03-07', 2, '2026-03-07 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-03-07', 8, '2026-03-07 20:00:00', '04DFAE6EBA2A81');

-- Sat Mar 8 ❌ PARTIALLY (forgot evening)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-08', 1, '2026-03-08 09:20:00', '04DDAE6EBA2A81');

-- Sun Mar 9 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-09', 1, '2026-03-09 09:30:00', '0441AB6EBA2A81'),
(1, '2026-03-09', 8, '2026-03-09 20:15:00', '0441AB6EBA2A81');

-- Mon Mar 10 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-10', 1, '2026-03-10 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-03-10', 2, '2026-03-10 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-03-10', 8, '2026-03-10 20:00:00', '04E7AE6EBA2A81');

-- Tue Mar 11 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-11', 1, '2026-03-11 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-03-11', 2, '2026-03-11 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-03-11', 8, '2026-03-11 19:55:00', '04E6AE6EBA2A81');

-- Wed Mar 12 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-12', 1, '2026-03-12 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-03-12', 2, '2026-03-12 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-03-12', 8, '2026-03-12 20:00:00', '04E5AE6EBA2A81');

-- Thu Mar 13 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-13', 1, '2026-03-13 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-03-13', 2, '2026-03-13 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-03-13', 8, '2026-03-13 19:50:00', '04E4AE6EBA2A81');

-- Fri Mar 14 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-14', 1, '2026-03-14 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-03-14', 2, '2026-03-14 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-03-14', 8, '2026-03-14 20:00:00', '04DFAE6EBA2A81');

-- Sat Mar 15 ❌ NOTHING (weekend trip)

-- Sun Mar 16 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-16', 1, '2026-03-16 10:00:00', '0441AB6EBA2A81'),
(1, '2026-03-16', 8, '2026-03-16 21:00:00', '0441AB6EBA2A81');

-- Mon Mar 17 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-17', 1, '2026-03-17 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-03-17', 2, '2026-03-17 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-03-17', 8, '2026-03-17 20:00:00', '04E7AE6EBA2A81');

-- Tue Mar 18 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-18', 1, '2026-03-18 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-03-18', 2, '2026-03-18 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-03-18', 8, '2026-03-18 19:55:00', '04E6AE6EBA2A81');

-- Wed Mar 19 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-19', 1, '2026-03-19 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-03-19', 2, '2026-03-19 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-03-19', 8, '2026-03-19 20:00:00', '04E5AE6EBA2A81');

-- Thu Mar 20 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-20', 1, '2026-03-20 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-03-20', 2, '2026-03-20 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-03-20', 8, '2026-03-20 19:50:00', '04E4AE6EBA2A81');

-- Fri Mar 21 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-21', 1, '2026-03-21 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-03-21', 2, '2026-03-21 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-03-21', 8, '2026-03-21 20:00:00', '04DFAE6EBA2A81');

-- Sat Mar 22 ❌ PARTIALLY (forgot noon - weekend)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-22', 1, '2026-03-22 09:20:00', '04DDAE6EBA2A81'),
(1, '2026-03-22', 8, '2026-03-22 20:30:00', '04DDAE6EBA2A81');

-- Sun Mar 23 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-23', 1, '2026-03-23 09:30:00', '0441AB6EBA2A81'),
(1, '2026-03-23', 8, '2026-03-23 20:15:00', '0441AB6EBA2A81');

-- Mon Mar 24 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-24', 1, '2026-03-24 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-03-24', 2, '2026-03-24 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-03-24', 8, '2026-03-24 20:00:00', '04E7AE6EBA2A81');

-- Tue Mar 25 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-25', 1, '2026-03-25 08:10:00', '04E6AE6EBA2A81'),
(1, '2026-03-25', 2, '2026-03-25 12:35:00', '04E6AE6EBA2A81'),
(1, '2026-03-25', 8, '2026-03-25 19:55:00', '04E6AE6EBA2A81');

-- Wed Mar 26 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-26', 1, '2026-03-26 08:00:00', '04E5AE6EBA2A81'),
(1, '2026-03-26', 2, '2026-03-26 12:30:00', '04E5AE6EBA2A81'),
(1, '2026-03-26', 8, '2026-03-26 20:00:00', '04E5AE6EBA2A81');

-- Thu Mar 27 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-27', 1, '2026-03-27 08:05:00', '04E4AE6EBA2A81'),
(1, '2026-03-27', 2, '2026-03-27 12:35:00', '04E4AE6EBA2A81'),
(1, '2026-03-27', 8, '2026-03-27 19:50:00', '04E4AE6EBA2A81');

-- Fri Mar 28 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-28', 1, '2026-03-28 08:00:00', '04DFAE6EBA2A81'),
(1, '2026-03-28', 2, '2026-03-28 12:30:00', '04DFAE6EBA2A81'),
(1, '2026-03-28', 8, '2026-03-28 20:00:00', '04DFAE6EBA2A81');

-- Sat Mar 29 ❌ PARTIALLY (forgot morning)
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-29', 8, '2026-03-29 20:30:00', '04DDAE6EBA2A81');

-- Sun Mar 30 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-30', 1, '2026-03-30 09:30:00', '0441AB6EBA2A81'),
(1, '2026-03-30', 8, '2026-03-30 20:15:00', '0441AB6EBA2A81');

-- Mon Mar 31 ✅
INSERT INTO medication_intakes (patient_id, intake_date, day_time_flag, opened_at, rfid_tag) VALUES
(1, '2026-03-31', 1, '2026-03-31 08:00:00', '04E7AE6EBA2A81'),
(1, '2026-03-31', 2, '2026-03-31 12:30:00', '04E7AE6EBA2A81'),
(1, '2026-03-31', 8, '2026-03-31 20:00:00', '04E7AE6EBA2A81');
