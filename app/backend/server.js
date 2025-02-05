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



const PORT = 3000;
app.listen(PORT, () => {
    console.log(`Server läuft auf Port ${PORT}`);
});
