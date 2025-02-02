const mqtt = require('mqtt');
const express = require('express');
const cors = require('cors');
const path = require('path');
const db = require('./db');

const app = express();
const port = 3000;

// Enable CORS
app.use(cors());

// Parse JSON bodies
app.use(express.json());

// Serve static files from the Frontend directory
app.use(express.static(path.join(__dirname, '..')));

// Redirect root to index.html
app.get('/', (req, res) => {
    res.redirect('/html/index.html');
});

// MQTT-Broker connection
const brokerUrl = "mqtt://192.168.1.103";
const options = {
    username: "poldi",
    password: "Maxi2005"
};

const client = mqtt.connect(brokerUrl, options);

let latestData = {
    tag: null,
    tag_removed: null
};

// Connect and subscribe to both topics
client.on('connect', () => {
    console.log('Connected to MQTT broker');
    client.subscribe(['rc522/tag', 'rc522/tag_removed']);
});

// Receive messages and store
client.on('message', (topic, message) => {
    if (topic === 'rc522/tag') {
        latestData.tag = message.toString();
    } else if (topic === 'rc522/tag_removed') {
        latestData.tag_removed = message.toString();
    }
    console.log(`Message received: ${topic} - ${message}`);
});

// API endpoint for the frontend
app.get('/api/data', (req, res) => {
    res.json(latestData);
});

// API endpoint to get all persons
app.get('/api/persons', async (req, res) => {
    try {
        const result = await db.query(`
            SELECT p.*, a.*
            FROM "Person" p
            LEFT JOIN "Address" a ON p."addressId" = a."AddressId"
        `);
        res.json(result.rows);
    } catch (error) {
        console.error('Error getting persons:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to get all patients
app.get('/api/patients', async (req, res) => {
    try {
        const result = await db.query(`
            SELECT p.*, per.*, a.*
            FROM "Patient" p
            JOIN "Person" per ON p."personId" = per."PersonId"
            LEFT JOIN "Address" a ON per."addressId" = a."AddressId"
        `);
        res.json(result.rows);
    } catch (error) {
        console.error('Error getting patients:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to get all caretakers (contact persons)
app.get('/api/caretakers', async (req, res) => {
    try {
        const result = await db.query(`
            SELECT c.*, per.*, a.*
            FROM "ContactPerson" c
            JOIN "Person" per ON c."personId" = per."PersonId"
            LEFT JOIN "Address" a ON per."addressId" = a."AddressId"
        `);
        res.json(result.rows);
    } catch (error) {
        console.error('Error getting caretakers:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to get all medicaments
app.get('/api/medicaments', async (req, res) => {
    try {
        const result = await db.query('SELECT * FROM "Medicament"');
        res.json(result.rows);
    } catch (error) {
        console.error('Error getting medicaments:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to add a new person
app.post('/api/persons', async (req, res) => {
    const { role, firstName, lastName, phoneNumber, address } = req.body;
    const { country, state, city, street, houseNumber, stair, floor, doorNumber, addressExtra } = address || {};

    // Validate required fields
    if (!firstName || !lastName || !phoneNumber || !street) {
        return res.status(400).json({ message: 'All basic fields are required' });
    }

    try {
        // Start a transaction
        await db.query('BEGIN');

        // First, insert the address
        const addressResult = await db.query(
            `INSERT INTO "Address" ("country", "state", "city", "street", "houseNumber", "stair", "floor", "doorNumber", "addressExtra")
             VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9) RETURNING "AddressId"`,
            [country || null, state || null, city || null, street, houseNumber || null, stair || null, floor || null, doorNumber || null, addressExtra || null]
        );

        // Then, insert the person
        const personResult = await db.query(
            `INSERT INTO "Person" ("firstName", "lastName", "phoneNumber", "addressId")
             VALUES ($1, $2, $3, $4) RETURNING "PersonId"`,
            [firstName, lastName, phoneNumber, addressResult.rows[0].AddressId]
        );

        // Based on role, insert into Patient or ContactPerson
        if (role === 'patient') {
            await db.query(
                `INSERT INTO "Patient" ("personId") VALUES ($1) RETURNING "PatientId"`,
                [personResult.rows[0].PersonId]
            );
        } else if (role === 'caretaker') {
            await db.query(
                `INSERT INTO "ContactPerson" ("personId") VALUES ($1) RETURNING "ContactPersonId"`,
                [personResult.rows[0].PersonId]
            );
        }

        await db.query('COMMIT');
        res.status(201).json({ message: 'Person added successfully' });
    } catch (error) {
        await db.query('ROLLBACK');
        console.error('Error adding person:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to assign a patient to a caretaker
app.post('/api/assign-patient', async (req, res) => {
    const { patientId, caretakerId } = req.body;

    try {
        const result = await db.query(
            `INSERT INTO "CaresFor" ("patientId", "contactPersonId", "contactTypeId")
             VALUES ($1, $2, $3) RETURNING *`,
            [patientId, caretakerId, 1] // Using 1 as default contactTypeId
        );

        res.json(result.rows[0]);
    } catch (error) {
        console.error('Error assigning patient:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to add a new medicament
app.post('/api/medicaments', async (req, res) => {
    const { medicamentName, description } = req.body;

    if (!medicamentName) {
        return res.status(400).json({ message: 'Medicament name is required' });
    }

    try {
        const result = await db.query(
            `INSERT INTO "Medicament" ("medicamentName", "description")
             VALUES ($1, $2) RETURNING *`,
            [medicamentName, description || null]
        );
        res.status(201).json(result.rows[0]);
    } catch (error) {
        console.error('Error adding medicament:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// Test database connection
app.get('/api/test-db', async (req, res) => {
    try {
        const result = await db.query('SELECT NOW()');
        res.json({ success: true, timestamp: result.rows[0].now });
    } catch (error) {
        console.error('Database connection test failed:', error);
        res.status(500).json({ success: false, error: error.message });
    }
});

// API endpoint to get all medication plans
app.get('/api/medication-plans', async (req, res) => {
    try {
        const result = await db.query(`
            SELECT mp.*, p.*, per.*, m.*
            FROM "MedicationPlan" mp
            JOIN "Patient" p ON mp."patientId" = p."PatientId"
            JOIN "Person" per ON p."personId" = per."PersonId"
            LEFT JOIN "MedicationPlan_Medicament" mpm ON mp."MedicationPlanId" = mpm."MedicationPlanId"
            LEFT JOIN "Medicament" m ON mpm."MedicamentId" = m."MedicamentId"
        `);
        res.json(result.rows);
    } catch (error) {
        console.error('Error getting medication plans:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to add a new medication plan
app.post('/api/medication-plans', async (req, res) => {
    const { patientId, startDate, endDate, medications } = req.body;

    // Validate required fields
    if (!patientId || !medications || !medications.length) {
        return res.status(400).json({ message: 'Patient and medications are required' });
    }

    try {
        // Start a transaction
        await db.query('BEGIN');

        // First, create the medication plan
        const planResult = await db.query(
            `INSERT INTO "MedicationPlan" ("patientId", "startDate", "endDate")
             VALUES ($1, $2, $3) RETURNING "MedicationPlanId"`,
            [patientId, startDate || null, endDate || null]
        );

        const medicationPlanId = planResult.rows[0].MedicationPlanId;

        // Then, add each medication to the plan
        for (const med of medications) {
            await db.query(
                `INSERT INTO "MedicationPlan_Medicament" 
                ("MedicationPlanId", "MedicamentId", "timeSlot", "dayOfWeek", "amountOfMedicament")
                VALUES ($1, $2, $3, $4, $5)`,
                [medicationPlanId, med.medicamentId, med.timeSlot, med.dayOfWeek, med.amount]
            );
        }

        await db.query('COMMIT');
        res.status(201).json({ message: 'Medication plan added successfully' });
    } catch (error) {
        await db.query('ROLLBACK');
        console.error('Error adding medication plan:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// API endpoint to get medication plan details
app.get('/api/medication-plans/:id', async (req, res) => {
    try {
        const result = await db.query(`
            SELECT mp.*, mpm.*, m.*, p.*, per.*
            FROM "MedicationPlan" mp
            JOIN "MedicationPlan_Medicament" mpm ON mp."MedicationPlanId" = mpm."MedicationPlanId"
            JOIN "Medicament" m ON mpm."MedicamentId" = m."MedicamentId"
            JOIN "Patient" p ON mp."patientId" = p."PatientId"
            JOIN "Person" per ON p."personId" = per."PersonId"
            WHERE mp."MedicationPlanId" = $1`,
            [req.params.id]
        );

        if (result.rows.length === 0) {
            return res.status(404).json({ message: 'Medication plan not found' });
        }

        res.json(result.rows);
    } catch (error) {
        console.error('Error getting medication plan:', error);
        res.status(500).json({ message: 'Internal server error' });
    }
});

// Start server
app.listen(port, () => {
    console.log(`Server running at http://localhost:${port}`);
}); 