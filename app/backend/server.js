const express = require('express');
const mysql = require('mysql2');
const cors = require('cors');
const dbConfig = require('./db-config');
const app = express();
require('./mqtt-service');  // MQTT Service einbinden

// Middleware
app.use(cors());
app.use(express.json());

// Datenbankverbindung
const db = mysql.createConnection(dbConfig);

// Verbindung mit Wiederholungsversuch
function connectWithRetry() {
    db.connect((err) => {
        if (err) {
            console.error('Fehler bei der Datenbankverbindung:', err);
            console.log('Versuche erneut in 5 Sekunden...');
            setTimeout(connectWithRetry, 5000);
            return;
        }
        console.log('Erfolgreich mit MariaDB verbunden');
    });

    db.on('error', (err) => {
        console.error('Datenbankfehler:', err);
        if (err.code === 'PROTOCOL_CONNECTION_LOST') {
            connectWithRetry();
        } else {
            throw err;
        }
    });
}

connectWithRetry();


// API Endpoints für Medikamente
app.get('/api/drugs', (req, res) => {
  db.query('SELECT * FROM drugs ORDER BY name ASC', (err, results) => {
    if (err) {
      console.error('Fehler beim Abrufen der Medikamente:', err);
      res.status(500).json({ error: err.message });
      return;
    }
    res.json(results);
  });
});

pp.post('/api/drugs', (req, res) => {
  const { name, side_effects, description } = req.body;
  db.query(
    'INSERT INTO drugs (name, side_effects, description) VALUES (?, ?, ?)',
    [name, side_effects, description],
    (err, result) => {
      if (err) {
        console.error('Fehler beim Erstellen des Medikaments:', err);
        res.status(500).json({ error: err.message });
        return;
      }
      res.json({ id: result.insertId, name, side_effects, description });
    }
  );
});

// Vereinfachter DELETE-Endpoint für Medikamente
app.delete('/api/drugs/:id', (req, res) => {
  const drugId = req.params.id;

  // Zuerst löschen wir alle zugehörigen Planeinträge
  db.query('DELETE FROM plan WHERE drug_id = ?', [drugId], (err) => {
    if (err) {
      console.error('Fehler beim Löschen der Planeinträge:', err);
      res.status(500).json({ error: err.message });
      return;
    }

    // Dann löschen wir das Medikament
    db.query('DELETE FROM drugs WHERE id = ?', [drugId], (err, result) => {
      if (err) {
        console.error('Fehler beim Löschen des Medikaments:', err);
        res.status(500).json({ error: err.message });
        return;
      }

      if (result.affectedRows === 0) {
        res.status(404).json({ error: 'Medikament nicht gefunden' });
        return;
      }

      res.json({ message: 'Medikament und zugehörige Planeinträge erfolgreich gelöscht' });
    });
  });
});

// API Endpoints für Medikamentenplan
app.get('/api/plan', (req, res) => {
  const query = `
        SELECT
            p.*,
            d.name as drug_name,
            d.description as drug_description,
            d.side_effects as drug_side_effects
        FROM plan p
        LEFT JOIN drugs d ON p.drug_id = d.id
        ORDER BY
            CASE p.day
                WHEN 'Monday' THEN 1
                WHEN 'Tuesday' THEN 2
                WHEN 'Wednesday' THEN 3
                WHEN 'Thursday' THEN 4
                WHEN 'Friday' THEN 5
                WHEN 'Saturday' THEN 6
                WHEN 'Sunday' THEN 7
            END,
            p.time
    `;

  db.query(query, (err, results) => {
    if (err) {
      console.error('Fehler beim Abrufen des Plans:', err);
      res.status(500).json({ error: err.message });
      return;
    }

    // Formatiere die Ergebnisse
    const formattedResults = results.map(row => ({
      id: row.id,
      drug_id: row.drug_id,
      day: row.day,
      time: row.time,
      drug: {
        id: row.drug_id,
        name: row.drug_name,
        description: row.drug_description,
        side_effects: row.drug_side_effects
      }
    }));

    res.json(formattedResults);
  });
});



const PORT = 3000;
app.listen(PORT, () => {
    console.log(`Server läuft auf Port ${PORT}`);
});
