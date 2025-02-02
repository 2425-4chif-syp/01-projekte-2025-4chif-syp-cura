// Function to load existing patients and contact persons
async function loadExistingPersons() {
    try {
        const response = await fetch('http://localhost:3000/api/persons');
        const persons = await response.json();
        
        const patientSelect = document.getElementById('patientId');
        const contactPersonSelect = document.getElementById('contactPersonId');
        
        // Clear existing options
        patientSelect.innerHTML = '<option value="">Select a patient (optional)...</option>';
        contactPersonSelect.innerHTML = '<option value="">Select a contact person (optional)...</option>';
        
        // Add persons to appropriate dropdowns
        persons.forEach(person => {
            if (person.role === 'patient') {
                const option = document.createElement('option');
                option.value = person.id;
                option.textContent = `${person.first_name} ${person.last_name}`;
                patientSelect.appendChild(option);
            } else if (person.role === 'caretaker') {
                const option = document.createElement('option');
                option.value = person.id;
                option.textContent = `${person.first_name} ${person.last_name}`;
                contactPersonSelect.appendChild(option);
            }
        });
    } catch (error) {
        console.error('Error loading existing persons:', error);
    }
}

// Function to handle role change
function handleRoleChange() {
    const role = document.getElementById('role').value;
    const contactPersonGroup = document.getElementById('contactPersonGroup');
    const patientGroup = document.getElementById('patientGroup');
    
    // Reset display
    contactPersonGroup.style.display = 'none';
    patientGroup.style.display = 'none';
    
    // Show appropriate field based on role
    if (role === 'patient') {
        contactPersonGroup.style.display = 'block';
    } else if (role === 'caretaker') {
        patientGroup.style.display = 'block';
    }
}

// Handle form submission
async function handleAddPerson(event) {
    event.preventDefault();
    
    // Reset error messages
    document.querySelectorAll('.error-message').forEach(el => el.style.display = 'none');

    // Get form values
    const role = document.getElementById('role').value;
    const firstName = document.getElementById('firstName').value.trim();
    const lastName = document.getElementById('lastName').value.trim();
    const phoneNumber = document.getElementById('phoneNumber').value.trim();

    // Get address values
    const address = {
        country: document.getElementById('country').value.trim(),
        state: document.getElementById('state').value.trim(),
        city: document.getElementById('city').value.trim(),
        street: document.getElementById('street').value.trim(),
        houseNumber: parseInt(document.getElementById('houseNumber').value) || null,
        stair: parseInt(document.getElementById('stair').value) || null,
        floor: parseInt(document.getElementById('floor').value) || null,
        doorNumber: parseInt(document.getElementById('doorNumber').value) || null,
        addressExtra: document.getElementById('addressExtra').value.trim()
    };

    // Validate inputs
    let hasError = false;

    if (!role) {
        document.getElementById('roleError').style.display = 'block';
        hasError = true;
    }

    if (!firstName) {
        document.getElementById('firstNameError').style.display = 'block';
        hasError = true;
    }

    if (!lastName) {
        document.getElementById('lastNameError').style.display = 'block';
        hasError = true;
    }

    if (!phoneNumber) {
        document.getElementById('phoneNumberError').style.display = 'block';
        hasError = true;
    }

    if (!address.street) {
        document.getElementById('streetError').style.display = 'block';
        hasError = true;
    }

    if (hasError) return;

    try {
        const response = await fetch('http://localhost:3000/api/persons', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                role,
                firstName,
                lastName,
                phoneNumber,
                address
            })
        });

        if (response.ok) {
            alert('Person successfully added!');
            navigateTo('index.html');
        } else {
            const error = await response.json();
            alert(`Error: ${error.message}`);
        }
    } catch (error) {
        console.error('Error adding person:', error);
        alert('Failed to add person. Please try again.');
    }
}

// Load existing persons when page loads
document.addEventListener('DOMContentLoaded', () => {
    loadExistingPersons();
}); 