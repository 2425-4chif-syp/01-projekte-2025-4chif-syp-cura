# 🏥 CURA - Medication Management System

A modern PostgreSQL-based system for managing medication plans with **binary flags** for optimal performance and RFID integration.

## 🚀 System Overview

The system manages:
- **Patients** with address data and contact information
- **Caregivers** with responsibility areas
- **RFID chips** for automatic weekday detection
- **Medications** with active ingredient information
- **Optimized medication plans** with binary time flags
- **Flexible intake times** (Morning, Noon, Afternoon, Evening)

## Quick Start

### 1. Start Database
```bash
docker-compose up -d
```

This starts:
- PostgreSQL on port `5434`
- pgAdmin on port `8080` (optional for web interface)

### 2. Database Connection
```
Host: localhost
Port: 5434
Database: cura
User: admin
Password: passme
```

### 3. pgAdmin (Web Interface)
- URL: http://localhost:8080
- Email: admin@cura.at
- Password: passme

## 📊 Database Schema (Optimized)

### Main Tables

1. **`rfid_chips`** - RFID chips with weekday assignment
2. **`locations`** - Addresses for patients and caregivers
3. **`medications`** - Medication master data (name, active ingredient)
4. **`caregivers`** - Caregiver data with contact information
5. **`patients`** - Patient data with address reference
6. **`medication_plans`** - **Central table** with binary time flags

### 🔥 Binary Flags System

**Weekdays** (`weekdays_flags`, 0-127):
- Sunday = 1, Monday = 2, Tuesday = 4, Wednesday = 8
- Thursday = 16, Friday = 32, Saturday = 64
- **Examples**: Daily = 127, Weekdays = 62, Mon+Wed+Fri = 42

**Day Times** (`day_times_flags`, 0-15):
- Morning = 1, Noon = 2, Afternoon = 4, Evening = 8
- **Examples**: Morning+Evening = 9, 3x daily = 13

### 🏷️ RFID Chips Standard Assignment

| Weekday | Chip-ID | Example-UID |
|---------|---------|-------------|
| Monday | CHIP_MONDAY_001 | A1B2C3D4E5F6 |
| Tuesday | CHIP_TUESDAY_001 | B2C3D4E5F6A1 |
| Wednesday | CHIP_WEDNESDAY_001 | C3D4E5F6A1B2 |
| Thursday | CHIP_THURSDAY_001 | D4E5F6A1B2C3 |
| Friday | CHIP_FRIDAY_001 | E5F6A1B2C3D4 |
| Saturday | CHIP_SATURDAY_001 | F6A1B2C3D4E5 |
| Sunday | CHIP_SUNDAY_001 | A1B2C3D4E5F7 |

## 🔍 Important Views & Helper Functions

### Readable Medication Plans
```sql
SELECT * FROM v_medication_plan_readable WHERE patient_name = 'John Doe';
```

### Convert Binary Flags
```sql
-- Weekday to Flag: 'MONDAY' → 2
SELECT weekday_to_flag('MONDAY');

-- Flag to Weekdays: 42 → ['MONDAY', 'WEDNESDAY', 'FRIDAY']
SELECT flag_to_weekdays(42);

-- Day time to Flag: 'MORNING' → 1  
SELECT day_time_to_flag('MORNING');

-- Flag to Day times: 9 → ['MORNING', 'EVENING']
SELECT flag_to_day_times(9);
```

## 🎯 Example Queries

### Medication plan for today (Monday)
```sql
SELECT 
    p.name as patient,
    m.name as medication,
    mp.quantity,
    flag_to_day_times(mp.day_times_flags) as day_times,
    mp.notes
FROM medication_plans mp
JOIN patients p ON mp.patient_id = p.id
JOIN medications m ON mp.medication_id = m.id
WHERE (mp.weekdays_flags & weekday_to_flag('MONDAY')) > 0
  AND mp.active = true
  AND CURRENT_DATE BETWEEN mp.valid_from AND COALESCE(mp.valid_until, '2099-12-31')
ORDER BY p.name;
```

### RFID chip scan for current weekday
```sql
SELECT 
    p.name as patient,
    m.name as medication,
    mp.quantity,
    flag_to_day_times(mp.day_times_flags) as intake_times,
    mp.notes
FROM rfid_chips r
JOIN medication_plans mp ON (mp.weekdays_flags & weekday_to_flag(r.weekday)) > 0
JOIN patients p ON mp.patient_id = p.id
JOIN medications m ON mp.medication_id = m.id
WHERE r.chip_id = 'CHIP_MONDAY_001' -- From RFID scanner
  AND mp.active = true
  AND CURRENT_DATE BETWEEN mp.valid_from AND COALESCE(mp.valid_until, '2099-12-31')
ORDER BY p.name;
```

### Create new medication plan
```sql
INSERT INTO medication_plans (
    patient_id, medication_id, caregiver_id,
    weekdays_flags, day_times_flags, quantity,
    valid_from, notes
) VALUES (
    1, 4, 1,
    127, -- Daily (Sun=1 + Mon=2 + ... + Sat=64 = 127)
    1,   -- Morning (Morning=1)
    1,   -- 1 piece
    '2024-01-01',
    'Take on empty stomach'
);
```

## 🔧 Advanced Features

### Complex intake times with binary flags
```sql
-- Only weekdays, morning and evening (Mon-Fri = 2+4+8+16+32 = 62, Morning+Evening = 1+8 = 9)
INSERT INTO medication_plans (patient_id, medication_id, weekdays_flags, day_times_flags, quantity, valid_from)
VALUES (1, 2, 62, 9, 1, '2024-01-01');

-- Monday, Wednesday, Friday at noon (Mon+Wed+Fri = 2+8+32 = 42, Noon = 2)
INSERT INTO medication_plans (patient_id, medication_id, weekdays_flags, day_times_flags, quantity, valid_from)
VALUES (1, 3, 42, 2, 2, '2024-01-01');
```

### End/pause medication plan
```sql
UPDATE medication_plans 
SET active = FALSE, valid_until = CURRENT_DATE 
WHERE id = 1;
```

### Performance-optimized queries
```sql
-- Index-optimized search for patient medication plans
SELECT * FROM medication_plans 
WHERE patient_id = 1 AND active = true
  AND CURRENT_DATE BETWEEN valid_from AND COALESCE(valid_until, '2099-12-31');
```

## Maintenance

### Create backup
```bash
docker exec cura_postgres pg_dump -U admin cura > backup_$(date +%Y%m%d).sql
```

### Restore backup
```bash
docker exec -i cura_postgres psql -U admin cura < backup_20240922.sql
```

### Show logs
```bash
docker-compose logs postgres
```

## Development

### Add new column
```sql
ALTER TABLE patients ADD COLUMN insurance_number VARCHAR(50);
```

### Add index for performance
```sql
CREATE INDEX idx_custom_name ON table_name(column_name);
```

## Troubleshooting

### Restart container
```bash
docker-compose restart
```

### Reset database
```bash
docker-compose down -v
docker-compose up -d
```

### Test connection
```bash
docker exec -it cura_postgres psql -U admin -d cura -c "SELECT COUNT(*) FROM patients;"
```

## 📈 ERD & Documentation

- **PlantUML ERD**: `CURA_ERD.puml` - Detailed Entity-Relationship-Diagram
- **Markdown Docs**: `ERD_CURA_System.md` - Complete system documentation

## 🚀 Performance Features

- **Binary flags** instead of separate tables → 90% fewer JOINs
- **Optimized indexes** for frequent queries
- **Minimal schema** with maximum flexibility
- **PostgreSQL 16** with latest features

test