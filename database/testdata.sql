-- CURA Testdaten für das Medikamentenverwaltungs-System mit binären Flags

-- Beispiel-Orte
INSERT INTO orte (strasse, hausnummer, postleitzahl, stadt, stockwerk) VALUES
('Hauptstraße', '15', '1010', 'Wien', '2'),
('Mariahilfer Straße', '88', '1060', 'Wien', '1'),
('Landstraßer Hauptstraße', '45', '1030', 'Wien', '3'),
('Währinger Straße', '22', '1090', 'Wien', '1'),
('Favoritenstraße', '67', '1100', 'Wien', '2');

-- Beispiel-Medikamente (nur name und wirkstoff)
INSERT INTO medikamente (name, wirkstoff) VALUES
('Aspirin 500mg', 'Acetylsalicylsäure'),
('Ibuprofen 400mg', 'Ibuprofen'),
('Paracetamol 500mg', 'Paracetamol'),
('Blutdrucksenker', 'Ramipril'),
('Diabetes Medikament', 'Metformin'),
('Vitamin D3', 'Cholecalciferol'),
('Herzmedikament', 'Bisoprolol'),
('Magenschutz', 'Pantoprazol');

-- Beispiel-Pfleger (ohne aktiv und erstellt_am)
INSERT INTO pfleger (name, telefonnummer, email, ort_id) VALUES
('Maria Schneider', '+43 664 1234567', 'maria.schneider@cura.at', 1),
('Thomas Müller', '+43 664 2345678', 'thomas.mueller@cura.at', 2),  
('Anna Weber', '+43 664 3456789', 'anna.weber@cura.at', 3),
('Stefan Huber', '+43 664 4567890', 'stefan.huber@cura.at', 4),
('Lisa Bauer', '+43 664 5678901', 'lisa.bauer@cura.at', 5);

-- Beispiel-Patienten (ohne notfallkontakt spalten)
INSERT INTO patienten (name, alter, ort_id, telefonnummer, email) VALUES
('Johann Meier', 78, 1, '+43 1 2345678', 'johann.meier@email.at'),
('Elisabeth Gruber', 82, 2, '+43 1 3456789', 'elisabeth.gruber@email.at'),
('Franz Wagner', 75, 3, '+43 1 4567890', 'franz.wagner@email.at'),
('Margarete Koch', 89, 4, '+43 1 5678901', 'margarete.koch@email.at'),
('Karl Berger', 71, 5, '+43 1 6789012', 'karl.berger@email.at'),
('Rosa Steiner', 85, 1, '+43 1 7890123', 'rosa.steiner@email.at');

-- Beispiel-Medikamentenpläne mit binären Flags
-- Wochentage: So=1, Mo=2, Di=4, Mi=8, Do=16, Fr=32, Sa=64
-- Tageszeiten: Morgens=1, Mittags=2, Nachmittags=4, Abends=8

-- Patient 1: Johann Meier - Blutdrucksenker täglich morgens (Mo-So = 2+4+8+16+32+64+1 = 127)
INSERT INTO medikamentenplaene (patient_id, medikament_id, pfleger_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von, gueltig_bis, notizen) VALUES
(1, 4, 1, 127, 1, 1, '2024-01-01', '2024-12-31', 'Nüchtern einnehmen'),
-- Herzmedikament täglich morgens  
(1, 7, 1, 127, 1, 1, '2024-01-01', '2024-12-31', 'Nach dem Frühstück'),
-- Aspirin nur werktags mittags (Mo-Fr = 2+4+8+16+32 = 62)
(1, 1, 1, 62, 2, 1, '2024-01-01', '2024-12-31', 'Bei Bedarf'),
-- Vitamin D3 täglich abends
(1, 6, 1, 127, 8, 5, '2024-01-01', '2024-12-31', 'In Wasser auflösen');

-- Patient 2: Elisabeth Gruber - Metformin morgens und mittags täglich
INSERT INTO medikamentenplaene (patient_id, medikament_id, pfleger_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von, gueltig_bis, notizen) VALUES
(2, 5, 2, 127, 1, 2, '2024-01-01', '2024-12-31', 'Morgens 2 Stück zu den Mahlzeiten'),
(2, 5, 2, 127, 2, 1, '2024-01-01', '2024-12-31', 'Mittags 1 Stück zu den Mahlzeiten'),
-- Magenschutz täglich morgens
(2, 8, 2, 127, 1, 1, '2024-01-01', '2024-12-31', 'Vor dem Essen'),
-- Paracetamol nur bei Bedarf abends
(2, 3, 2, 127, 8, 1, '2024-01-01', '2024-12-31', 'Bei Schmerzen');

-- Patient 3: Franz Wagner - Herzpatient, verschiedene Zeiten
INSERT INTO medikamentenplaene (patient_id, medikament_id, pfleger_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von, gueltig_bis, notizen) VALUES
-- Herzmedikament täglich morgens
(3, 7, 3, 127, 1, 1, '2024-01-01', '2024-12-31', 'Puls messen vor Einnahme'),
-- Blutdrucksenker täglich morgens
(3, 4, 3, 127, 1, 1, '2024-01-01', '2024-12-31', 'Nüchtern'),
-- Aspirin nur werktags nachmittags
(3, 1, 3, 62, 4, 1, '2024-01-01', '2024-12-31', 'Bei Bedarf'),
-- Vitamin D3 täglich abends
(3, 6, 3, 127, 8, 5, '2024-01-01', '2024-12-31', 'Tropfen in Saft');

-- Patient 4: Margarete Koch - Paracetamol 3x täglich (Morgens, Nachmittags, Abends = 1+4+8 = 13)
INSERT INTO medikamentenplaene (patient_id, medikament_id, pfleger_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von, gueltig_bis, notizen) VALUES
(4, 3, 4, 127, 13, 1, '2024-01-01', '2024-12-31', 'Überwachte Einnahme - 3x täglich'),
-- Vitamin D3 nur morgens
(4, 6, 4, 127, 1, 3, '2024-01-01', '2024-12-31', 'In Joghurt mischen');

-- Patient 5: Karl Berger - Ibuprofen morgens und abends (1+8 = 9)
INSERT INTO medikamentenplaene (patient_id, medikament_id, pfleger_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von, gueltig_bis, notizen) VALUES
(5, 2, 5, 127, 9, 1, '2024-01-01', '2024-12-31', 'Nach dem Essen - morgens und abends'),
-- Magenschutz täglich morgens
(5, 8, 5, 127, 1, 1, '2024-01-01', '2024-12-31', 'Magenschutz'),
-- Vitamin D3 täglich abends
(5, 6, 5, 127, 8, 5, '2024-01-01', '2024-12-31', 'Tropfen');

-- Patient 6: Rosa Steiner - nur wenige Medikamente
INSERT INTO medikamentenplaene (patient_id, medikament_id, pfleger_id, wochentage_flags, tageszeiten_flags, stueckzahl, gueltig_von, gueltig_bis, notizen) VALUES
-- Paracetamol nur bei Bedarf mittags
(6, 3, 1, 127, 2, 1, '2024-01-01', '2024-12-31', 'Bei Kopfschmerzen'),
-- Vitamin D3 täglich morgens
(6, 6, 1, 127, 1, 3, '2024-01-01', '2024-12-31', 'Täglich'),
-- Aspirin nur bei Bedarf abends
(6, 1, 1, 127, 8, 1, '2024-01-01', '2024-12-31', 'Bei Bedarf');

-- Zusätzliche RFID-Chips für mehrere Sets
INSERT INTO rfid_chips (chip_id, wochentag) VALUES
('CHIP_MONTAG_002', 'MONTAG'),
('CHIP_DIENSTAG_002', 'DIENSTAG'),
('CHIP_MITTWOCH_002', 'MITTWOCH'),
('CHIP_DONNERSTAG_002', 'DONNERSTAG'),
('CHIP_FREITAG_002', 'FREITAG'),
('CHIP_SAMSTAG_002', 'SAMSTAG'),
('CHIP_SONNTAG_002', 'SONNTAG');