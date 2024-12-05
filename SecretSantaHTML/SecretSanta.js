document.addEventListener('DOMContentLoaded', function () {
    const toggleButton = document.getElementById('toggle-participants-button');
    const participantsTable = document.getElementById('participants-table');

    toggleButton.addEventListener('click', function () {
        if (participantsTable.style.display === 'none' || participantsTable.querySelector('tbody').innerHTML === '') {
            // If hidden or empty - show participants
            fetchParticipants();
            toggleButton.textContent = 'Hide Participants';
        } else {
            // If visible - hide it
            participantsTable.style.display = 'none';
            toggleButton.textContent = 'Show Participants';
        }
    });
});

function fetchParticipants() {
    fetch('https://localhost:7195/api/users/all')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            console.log('Participants:', data);
            displayParticipants(data);
        })
        .catch(error => {
            console.error('Error fetching participants:', error);
        });
}

function displayParticipants(participants) {
    const tbody = document.querySelector('#participants-table tbody');
    tbody.innerHTML = ''; // Clear any previous participants

    participants.forEach(participant => {
        const row = document.createElement('tr');

        // Create and append table cells for each attribute
        const idCell = document.createElement('td');
        idCell.textContent = participant.id;
        row.appendChild(idCell);

        const firstNameCell = document.createElement('td');
        firstNameCell.textContent = participant.firstName;
        row.appendChild(firstNameCell);

        const lastNameCell = document.createElement('td');
        lastNameCell.textContent = participant.lastName;
        row.appendChild(lastNameCell);

        const phoneNumberCell = document.createElement('td');
        phoneNumberCell.textContent = participant.phoneNumber;
        row.appendChild(phoneNumberCell);

        const isActiveCell = document.createElement('td');
        isActiveCell.textContent = participant.isActive ? 'Yes' : 'No';
        row.appendChild(isActiveCell);

        const registerTimeCell = document.createElement('td');
        registerTimeCell.textContent = new Date(participant.registerTime).toLocaleString();
        row.appendChild(registerTimeCell);

        tbody.appendChild(row);
    });

    // Ensure the table is visible after fetching participants
    const participantsTable = document.getElementById('participants-table');
    participantsTable.style.display = 'table';
}
function displayParticipants(participants) {
    const tbody = document.querySelector('#participants-table tbody');
    tbody.innerHTML = ''; // Clear any previous participants

    participants.forEach(participant => {
        const row = document.createElement('tr');

        // Create and append table cells for each attribute
        const idCell = document.createElement('td');
        idCell.textContent = participant.id;
        row.appendChild(idCell);

        const firstNameCell = document.createElement('td');
        firstNameCell.textContent = participant.firstName;
        row.appendChild(firstNameCell);

        const lastNameCell = document.createElement('td');
        lastNameCell.textContent = participant.lastName;
        row.appendChild(lastNameCell);

        const emailCell = document.createElement('td');
        emailCell.textContent = participant.email; // Add the email field
        row.appendChild(emailCell);

        const phoneNumberCell = document.createElement('td');
        phoneNumberCell.textContent = participant.phoneNumber;
        row.appendChild(phoneNumberCell);

        const isActiveCell = document.createElement('td');
        isActiveCell.textContent = participant.isActive ? 'Yes' : 'No';
        row.appendChild(isActiveCell);

        const registerTimeCell = document.createElement('td');
        registerTimeCell.textContent = new Date(participant.registerTime).toLocaleString();
        row.appendChild(registerTimeCell);

        tbody.appendChild(row);
    });

    // Ensure the table is visible after fetching participants
    const participantsTable = document.getElementById('participants-table');
    participantsTable.style.display = 'table';
}
