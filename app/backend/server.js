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


const PORT = 3000;
app.listen(PORT, () => {
    console.log(`Server läuft auf Port ${PORT}`);
});
