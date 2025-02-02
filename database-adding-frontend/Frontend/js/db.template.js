const { Pool } = require('pg');

// Create this file as 'db.js' and fill in your database credentials
const pool = new Pool({
    user: 'your_username',
    host: 'your_host',
    database: 'your_database',
    password: 'your_password', 
    port: 5432,
});

module.exports = {
    query: (text, params) => pool.query(text, params),
}; 