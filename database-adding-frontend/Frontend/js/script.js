// Navigation function
function navigateTo(page) {
    window.location.href = '../html/' + page;
}

// Modal functions
function openModal() {
    document.getElementById('optionsModal').style.display = 'block';
}

function closeModal() {
    document.getElementById('optionsModal').style.display = 'none';
}

function addNewPatient() {
    // TODO: Implement new patient addition
    console.log('Adding new patient');
    window.location.href = '../html/add-person.html';
}

function assignPatientToCaretaker() {
    // TODO: Implement patient assignment
    console.log('Assigning patient to caretaker');
    window.location.href = '../html/assign-patient.html';
}

// Close modal when clicking outside
window.onclick = function(event) {
    if (event.target == document.getElementById('optionsModal')) {
        closeModal();
    }
}

async function fetchData() {
    try {
        const response = await fetch('http://localhost:3000/api/data');
        if (!response.ok) throw new Error('Fehler beim Abrufen der Daten');
        const data = await response.json();

        const formattedData = `
            <p><strong>Tag erkannt:</strong> ${data.tag || 'Keine Daten'}</p>
            <p><strong>Tag entfernt:</strong> ${data.tag_removed || 'Keine Daten'}</p>
        `;
        document.getElementById('data').innerHTML = formattedData;
    } catch (error) {
        console.error('Fehler:', error);
        document.getElementById('data').innerText = 'Fehler beim Laden der Daten.';
    }
}

// Initialize when document is loaded
document.addEventListener('DOMContentLoaded', () => {
    // Only start MQTT data fetching if we're on the mqtt.html page
    if (window.location.pathname.endsWith('mqtt.html')) {
        setInterval(fetchData, 10);
        fetchData();
    }
});
