// Function to load patients and caretakers from the backend
async function loadData() {
    try {
        // Load patients
        const patientsResponse = await fetch('http://localhost:3000/api/patients');
        const patients = await patientsResponse.json();
        const patientSelect = document.getElementById('patientSelect');
        patients.forEach(patient => {
            const option = document.createElement('option');
            option.value = patient.id;
            option.textContent = `${patient.first_name} ${patient.last_name}`;
            patientSelect.appendChild(option);
        });

        // Load caretakers
        const caretakersResponse = await fetch('http://localhost:3000/api/caretakers');
        const caretakers = await caretakersResponse.json();
        const caretakerSelect = document.getElementById('caretakerSelect');
        caretakers.forEach(caretaker => {
            const option = document.createElement('option');
            option.value = caretaker.id;
            option.textContent = `${caretaker.first_name} ${caretaker.last_name}`;
            caretakerSelect.appendChild(option);
        });
    } catch (error) {
        console.error('Error loading data:', error);
    }
}

// Handle form submission
async function handleAssignment(event) {
    event.preventDefault();
    
    const patientId = document.getElementById('patientSelect').value;
    const caretakerId = document.getElementById('caretakerSelect').value;

    // Reset error messages
    document.getElementById('patientError').style.display = 'none';
    document.getElementById('caretakerError').style.display = 'none';

    // Validate selections
    if (!patientId) {
        document.getElementById('patientError').style.display = 'block';
        return;
    }
    if (!caretakerId) {
        document.getElementById('caretakerError').style.display = 'block';
        return;
    }

    try {
        const response = await fetch('http://localhost:3000/api/assign-patient', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                patientId,
                caretakerId
            })
        });

        if (response.ok) {
            alert('Patient successfully assigned to caretaker!');
            navigateTo('index.html');
        } else {
            const error = await response.json();
            alert(`Error: ${error.message}`);
        }
    } catch (error) {
        console.error('Error assigning patient:', error);
        alert('Failed to assign patient. Please try again.');
    }
}

// Load data when page loads
document.addEventListener('DOMContentLoaded', loadData); 