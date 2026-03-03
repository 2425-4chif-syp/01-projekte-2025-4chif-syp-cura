-- ========================================
-- TESTDATEN FÜR ELISABETH GRUBER (PATIENT ID 2)
-- MÄRZ 2026 - KOMPLETTER MONAT
-- ========================================
-- 
-- VOR DEM IMPORT ALTE DATEN LÖSCHEN:
-- DELETE FROM medication_intakes WHERE patient_id = 2;
-- DELETE FROM medication_plans WHERE patient_id = 2;
--
-- Tägliche Einnahmen:
-- - Morgens: Blutverdünnung (2x) + Vitamine (1x)
-- - Mittags: Omega drei (1x)
-- - Abends: Blutverdünnung (1x)
--
-- Problematische Tage:
-- - 15. März: Vergisst Abendmedikation
-- - 28. März: Vergisst Mittag- und Abendmedikation
-- ========================================

-- ========================================
-- MEDIKAMENTENPLAN FÜR ELISABETH GRUBER
-- ========================================
-- Patient 2: Elisabeth Gruber
-- Weekday flags: 127 = alle Tage (Sun=1, Mon=2, Tue=4, Wed=8, Thu=16, Fri=32, Sat=64)
-- Day time flags: Morning=1, Noon=2, Afternoon=4, Evening=8
-- Neue Medikamente: 9=Blutverdünnung, 11=Vitamine, 12=Omega drei
INSERT INTO medication_plans (patient_id, medication_id, caregiver_id, weekday_flags, day_time_flags, quantity, valid_from, valid_to, notes) VALUES
(2, 9, 2, 127, 1, 2, '2024-01-01', '2026-12-31', 'Blutverdünnung morgens 2 Tabletten'),     -- Plan ID 351
(2, 12, 2, 127, 2, 1, '2024-01-01', '2026-12-31', 'Omega drei mittags'),                     -- Plan ID 352
(2, 11, 2, 127, 1, 1, '2024-01-01', '2026-12-31', 'Vitamine morgens'),                       -- Plan ID 353
(2, 9, 2, 127, 8, 1, '2024-01-01', '2026-12-31', 'Blutverdünnung abends 1 Tablette');        -- Plan ID 354

-- ========================================
-- RFID CHIPS FÜR ELISABETH
-- ========================================
-- RFID Chips für Elisabeth (falls noch nicht vorhanden)
INSERT INTO rfid_chips (chip_id, patient_id, weekday) VALUES
('0551BC7FCB3B92', 2, 'MONDAY'),
('05F8BF7FCB3B92', 2, 'TUESDAY'),
('05F7BF7FCB3B92', 2, 'WEDNESDAY'),
('05F6BF7FCB3B92', 2, 'THURSDAY'),
('05F5BF7FCB3B92', 2, 'FRIDAY'),
('05E0BF7FCB3B92', 2, 'SATURDAY'),
('05DEBF7FCB3B92', 2, 'SUNDAY');

-- ========================================
-- MÄRZ 2026 - MEDICATION INTAKES
-- ========================================

-- Sonntag, 1. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-01 08:15:00', 2, 'Morgen - Blutverdünnung', '05DEBF7FCB3B92'),
(2, 353, '2026-03-01 08:15:00', 1, 'Morgen - Vitamine', '05DEBF7FCB3B92'),
(2, 352, '2026-03-01 12:20:00', 1, 'Mittag - Omega drei', '05DEBF7FCB3B92'),
(2, 354, '2026-03-01 20:10:00', 1, 'Abend - Blutverdünnung', '05DEBF7FCB3B92');

-- Montag, 2. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-02 08:30:00', 2, 'Morgen - Blutverdünnung', '0551BC7FCB3B92'),
(2, 353, '2026-03-02 08:30:00', 1, 'Morgen - Vitamine', '0551BC7FCB3B92'),
(2, 352, '2026-03-02 12:15:00', 1, 'Mittag - Omega drei', '0551BC7FCB3B92'),
(2, 354, '2026-03-02 19:45:00', 1, 'Abend - Blutverdünnung', '0551BC7FCB3B92');

-- Dienstag, 3. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-03 08:20:00', 2, 'Morgen - Blutverdünnung', '05F8BF7FCB3B92'),
(2, 353, '2026-03-03 08:20:00', 1, 'Morgen - Vitamine', '05F8BF7FCB3B92'),
(2, 352, '2026-03-03 12:30:00', 1, 'Mittag - Omega drei', '05F8BF7FCB3B92'),
(2, 354, '2026-03-03 20:00:00', 1, 'Abend - Blutverdünnung', '05F8BF7FCB3B92');

-- Mittwoch, 4. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-04 08:10:00', 2, 'Morgen - Blutverdünnung', '05F7BF7FCB3B92'),
(2, 353, '2026-03-04 08:10:00', 1, 'Morgen - Vitamine', '05F7BF7FCB3B92'),
(2, 352, '2026-03-04 12:25:00', 1, 'Mittag - Omega drei', '05F7BF7FCB3B92'),
(2, 354, '2026-03-04 19:50:00', 1, 'Abend - Blutverdünnung', '05F7BF7FCB3B92');

-- Donnerstag, 5. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-05 08:25:00', 2, 'Morgen - Blutverdünnung', '05F6BF7FCB3B92'),
(2, 353, '2026-03-05 08:25:00', 1, 'Morgen - Vitamine', '05F6BF7FCB3B92'),
(2, 352, '2026-03-05 12:10:00', 1, 'Mittag - Omega drei', '05F6BF7FCB3B92'),
(2, 354, '2026-03-05 20:15:00', 1, 'Abend - Blutverdünnung', '05F6BF7FCB3B92');

-- Freitag, 6. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-06 08:05:00', 2, 'Morgen - Blutverdünnung', '05F5BF7FCB3B92'),
(2, 353, '2026-03-06 08:05:00', 1, 'Morgen - Vitamine', '05F5BF7FCB3B92'),
(2, 352, '2026-03-06 12:35:00', 1, 'Mittag - Omega drei', '05F5BF7FCB3B92'),
(2, 354, '2026-03-06 19:55:00', 1, 'Abend - Blutverdünnung', '05F5BF7FCB3B92');

-- Samstag, 7. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-07 08:40:00', 2, 'Morgen - Blutverdünnung', '05E0BF7FCB3B92'),
(2, 353, '2026-03-07 08:40:00', 1, 'Morgen - Vitamine', '05E0BF7FCB3B92'),
(2, 352, '2026-03-07 12:20:00', 1, 'Mittag - Omega drei', '05E0BF7FCB3B92'),
(2, 354, '2026-03-07 20:05:00', 1, 'Abend - Blutverdünnung', '05E0BF7FCB3B92');

-- Sonntag, 8. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-08 08:50:00', 2, 'Morgen - Blutverdünnung', '05DEBF7FCB3B92'),
(2, 353, '2026-03-08 08:50:00', 1, 'Morgen - Vitamine', '05DEBF7FCB3B92'),
(2, 352, '2026-03-08 12:15:00', 1, 'Mittag - Omega drei', '05DEBF7FCB3B92'),
(2, 354, '2026-03-08 19:40:00', 1, 'Abend - Blutverdünnung', '05DEBF7FCB3B92');

-- Montag, 9. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-09 08:15:00', 2, 'Morgen - Blutverdünnung', '0551BC7FCB3B92'),
(2, 353, '2026-03-09 08:15:00', 1, 'Morgen - Vitamine', '0551BC7FCB3B92'),
(2, 352, '2026-03-09 12:30:00', 1, 'Mittag - Omega drei', '0551BC7FCB3B92'),
(2, 354, '2026-03-09 20:00:00', 1, 'Abend - Blutverdünnung', '0551BC7FCB3B92');

-- Dienstag, 10. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-10 08:20:00', 2, 'Morgen - Blutverdünnung', '05F8BF7FCB3B92'),
(2, 353, '2026-03-10 08:20:00', 1, 'Morgen - Vitamine', '05F8BF7FCB3B92'),
(2, 352, '2026-03-10 12:25:00', 1, 'Mittag - Omega drei', '05F8BF7FCB3B92'),
(2, 354, '2026-03-10 19:45:00', 1, 'Abend - Blutverdünnung', '05F8BF7FCB3B92');

-- Mittwoch, 11. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-11 08:10:00', 2, 'Morgen - Blutverdünnung', '05F7BF7FCB3B92'),
(2, 353, '2026-03-11 08:10:00', 1, 'Morgen - Vitamine', '05F7BF7FCB3B92'),
(2, 352, '2026-03-11 12:40:00', 1, 'Mittag - Omega drei', '05F7BF7FCB3B92'),
(2, 354, '2026-03-11 20:10:00', 1, 'Abend - Blutverdünnung', '05F7BF7FCB3B92');

-- Donnerstag, 12. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-12 08:30:00', 2, 'Morgen - Blutverdünnung', '05F6BF7FCB3B92'),
(2, 353, '2026-03-12 08:30:00', 1, 'Morgen - Vitamine', '05F6BF7FCB3B92'),
(2, 352, '2026-03-12 12:15:00', 1, 'Mittag - Omega drei', '05F6BF7FCB3B92'),
(2, 354, '2026-03-12 19:50:00', 1, 'Abend - Blutverdünnung', '05F6BF7FCB3B92');

-- Freitag, 13. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-13 08:25:00', 2, 'Morgen - Blutverdünnung', '05F5BF7FCB3B92'),
(2, 353, '2026-03-13 08:25:00', 1, 'Morgen - Vitamine', '05F5BF7FCB3B92'),
(2, 352, '2026-03-13 12:20:00', 1, 'Mittag - Omega drei', '05F5BF7FCB3B92'),
(2, 354, '2026-03-13 20:05:00', 1, 'Abend - Blutverdünnung', '05F5BF7FCB3B92');

-- Samstag, 14. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-14 08:35:00', 2, 'Morgen - Blutverdünnung', '05E0BF7FCB3B92'),
(2, 353, '2026-03-14 08:35:00', 1, 'Morgen - Vitamine', '05E0BF7FCB3B92'),
(2, 352, '2026-03-14 12:30:00', 1, 'Mittag - Omega drei', '05E0BF7FCB3B92'),
(2, 354, '2026-03-14 19:55:00', 1, 'Abend - Blutverdünnung', '05E0BF7FCB3B92');

-- Sonntag, 15. März 2026 ⚠️ PROBLEMATISCH - Abendmedikation vergessen
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-15 08:45:00', 2, 'Morgen - Blutverdünnung', '05DEBF7FCB3B92'),
(2, 353, '2026-03-15 08:45:00', 1, 'Morgen - Vitamine', '05DEBF7FCB3B92'),
(2, 352, '2026-03-15 12:25:00', 1, 'Mittag - Omega drei', '05DEBF7FCB3B92');
-- Abend vergessen!

-- Montag, 16. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-16 08:15:00', 2, 'Morgen - Blutverdünnung', '0551BC7FCB3B92'),
(2, 353, '2026-03-16 08:15:00', 1, 'Morgen - Vitamine', '0551BC7FCB3B92'),
(2, 352, '2026-03-16 12:20:00', 1, 'Mittag - Omega drei', '0551BC7FCB3B92'),
(2, 354, '2026-03-16 20:00:00', 1, 'Abend - Blutverdünnung', '0551BC7FCB3B92');

-- Dienstag, 17. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-17 08:20:00', 2, 'Morgen - Blutverdünnung', '05F8BF7FCB3B92'),
(2, 353, '2026-03-17 08:20:00', 1, 'Morgen - Vitamine', '05F8BF7FCB3B92'),
(2, 352, '2026-03-17 12:35:00', 1, 'Mittag - Omega drei', '05F8BF7FCB3B92'),
(2, 354, '2026-03-17 19:45:00', 1, 'Abend - Blutverdünnung', '05F8BF7FCB3B92');

-- Mittwoch, 18. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-18 08:10:00', 2, 'Morgen - Blutverdünnung', '05F7BF7FCB3B92'),
(2, 353, '2026-03-18 08:10:00', 1, 'Morgen - Vitamine', '05F7BF7FCB3B92'),
(2, 352, '2026-03-18 12:15:00', 1, 'Mittag - Omega drei', '05F7BF7FCB3B92'),
(2, 354, '2026-03-18 20:10:00', 1, 'Abend - Blutverdünnung', '05F7BF7FCB3B92');

-- Donnerstag, 19. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-19 08:30:00', 2, 'Morgen - Blutverdünnung', '05F6BF7FCB3B92'),
(2, 353, '2026-03-19 08:30:00', 1, 'Morgen - Vitamine', '05F6BF7FCB3B92'),
(2, 352, '2026-03-19 12:25:00', 1, 'Mittag - Omega drei', '05F6BF7FCB3B92'),
(2, 354, '2026-03-19 19:50:00', 1, 'Abend - Blutverdünnung', '05F6BF7FCB3B92');

-- Freitag, 20. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-20 08:15:00', 2, 'Morgen - Blutverdünnung', '05F5BF7FCB3B92'),
(2, 353, '2026-03-20 08:15:00', 1, 'Morgen - Vitamine', '05F5BF7FCB3B92'),
(2, 352, '2026-03-20 12:30:00', 1, 'Mittag - Omega drei', '05F5BF7FCB3B92'),
(2, 354, '2026-03-20 20:00:00', 1, 'Abend - Blutverdünnung', '05F5BF7FCB3B92');

-- Samstag, 21. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-21 08:40:00', 2, 'Morgen - Blutverdünnung', '05E0BF7FCB3B92'),
(2, 353, '2026-03-21 08:40:00', 1, 'Morgen - Vitamine', '05E0BF7FCB3B92'),
(2, 352, '2026-03-21 12:20:00', 1, 'Mittag - Omega drei', '05E0BF7FCB3B92'),
(2, 354, '2026-03-21 19:55:00', 1, 'Abend - Blutverdünnung', '05E0BF7FCB3B92');

-- Sonntag, 22. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-22 08:50:00', 2, 'Morgen - Blutverdünnung', '05DEBF7FCB3B92'),
(2, 353, '2026-03-22 08:50:00', 1, 'Morgen - Vitamine', '05DEBF7FCB3B92'),
(2, 352, '2026-03-22 12:15:00', 1, 'Mittag - Omega drei', '05DEBF7FCB3B92'),
(2, 354, '2026-03-22 20:05:00', 1, 'Abend - Blutverdünnung', '05DEBF7FCB3B92');

-- Montag, 23. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-23 08:10:00', 2, 'Morgen - Blutverdünnung', '0551BC7FCB3B92'),
(2, 353, '2026-03-23 08:10:00', 1, 'Morgen - Vitamine', '0551BC7FCB3B92'),
(2, 352, '2026-03-23 12:25:00', 1, 'Mittag - Omega drei', '0551BC7FCB3B92'),
(2, 354, '2026-03-23 19:50:00', 1, 'Abend - Blutverdünnung', '0551BC7FCB3B92');

-- Dienstag, 24. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-24 08:25:00', 2, 'Morgen - Blutverdünnung', '05F8BF7FCB3B92'),
(2, 353, '2026-03-24 08:25:00', 1, 'Morgen - Vitamine', '05F8BF7FCB3B92'),
(2, 352, '2026-03-24 12:10:00', 1, 'Mittag - Omega drei', '05F8BF7FCB3B92'),
(2, 354, '2026-03-24 20:00:00', 1, 'Abend - Blutverdünnung', '05F8BF7FCB3B92');

-- Mittwoch, 25. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-25 08:35:00', 2, 'Morgen - Blutverdünnung', '05F7BF7FCB3B92'),
(2, 353, '2026-03-25 08:35:00', 1, 'Morgen - Vitamine', '05F7BF7FCB3B92'),
(2, 352, '2026-03-25 12:30:00', 1, 'Mittag - Omega drei', '05F7BF7FCB3B92'),
(2, 354, '2026-03-25 19:45:00', 1, 'Abend - Blutverdünnung', '05F7BF7FCB3B92');

-- Donnerstag, 26. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-26 08:20:00', 2, 'Morgen - Blutverdünnung', '05F6BF7FCB3B92'),
(2, 353, '2026-03-26 08:20:00', 1, 'Morgen - Vitamine', '05F6BF7FCB3B92'),
(2, 352, '2026-03-26 12:20:00', 1, 'Mittag - Omega drei', '05F6BF7FCB3B92'),
(2, 354, '2026-03-26 20:10:00', 1, 'Abend - Blutverdünnung', '05F6BF7FCB3B92');

-- Freitag, 27. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-27 08:15:00', 2, 'Morgen - Blutverdünnung', '05F5BF7FCB3B92'),
(2, 353, '2026-03-27 08:15:00', 1, 'Morgen - Vitamine', '05F5BF7FCB3B92'),
(2, 352, '2026-03-27 12:25:00', 1, 'Mittag - Omega drei', '05F5BF7FCB3B92'),
(2, 354, '2026-03-27 19:50:00', 1, 'Abend - Blutverdünnung', '05F5BF7FCB3B92');

-- Samstag, 28. März 2026 ⚠️ PROBLEMATISCH - Mittag und Abend vergessen
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-28 08:40:00', 2, 'Morgen - Blutverdünnung', '05E0BF7FCB3B92'),
(2, 353, '2026-03-28 08:40:00', 1, 'Morgen - Vitamine', '05E0BF7FCB3B92');
-- Mittag und Abend vergessen!

-- Sonntag, 29. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-29 08:45:00', 2, 'Morgen - Blutverdünnung', '05DEBF7FCB3B92'),
(2, 353, '2026-03-29 08:45:00', 1, 'Morgen - Vitamine', '05DEBF7FCB3B92'),
(2, 352, '2026-03-29 12:05:00', 1, 'Mittag - Omega drei', '05DEBF7FCB3B92'),
(2, 354, '2026-03-29 20:00:00', 1, 'Abend - Blutverdünnung', '05DEBF7FCB3B92');

-- Montag, 30. März 2026 ✅
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-30 08:30:00', 2, 'Morgen - Blutverdünnung', '0551BC7FCB3B92'),
(2, 353, '2026-03-30 08:30:00', 1, 'Morgen - Vitamine', '0551BC7FCB3B92'),
(2, 352, '2026-03-30 12:15:00', 1, 'Mittag - Omega drei', '0551BC7FCB3B92'),
(2, 354, '2026-03-30 19:55:00', 1, 'Abend - Blutverdünnung', '0551BC7FCB3B92');

-- Dienstag, 31. März 2026 ✅x
INSERT INTO medication_intakes (patient_id, medication_plan_id, intake_time, quantity, notes, rfid_tag) VALUES
(2, 351, '2026-03-31 08:20:00', 2, 'Morgen - Blutverdünnung', '05F8BF7FCB3B92'),
(2, 353, '2026-03-31 08:20:00', 1, 'Morgen - Vitamine', '05F8BF7FCB3B92'),
(2, 352, '2026-03-31 12:10:00', 1, 'Mittag - Omega drei', '05F8BF7FCB3B92'),
(2, 354, '2026-03-31 20:05:00', 1, 'Abend - Blutverdünnung', '05F8BF7FCB3B92');

-- ========================================
-- ZUSAMMENFASSUNG MÄRZ 2026
-- ========================================
-- Gesamt: 31 Tage
-- Erwartete Einnahmen: 31 Tage × 4 Einnahmen = 124 Einnahmen
-- Tatsächliche Einnahmen: 121 Einnahmen
-- Vergessene Einnahmen: 3 (15. März Abend + 28. März Mittag + Abend)
-- Erfolgsrate: 97,6% (121/124)
-- 
-- Medication Plan IDs:
-- Plan 351: Blutverdünnung morgens (2x) - Medication 9
-- Plan 352: Omega drei mittags (1x) - Medication 12
-- Plan 353: Vitamine morgens (1x) - Medication 11
-- Plan 354: Blutverdünnung abends (1x) - Medication 9
-- ========================================
