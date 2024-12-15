document.addEventListener("DOMContentLoaded", () => {
    let participants = [];
    let currentIndex = 0;

    function saveParticipantsToLocalStorage() {
        localStorage.setItem("participants", JSON.stringify(participants));
    }

    function loadParticipantsFromLocalStorage() {
        const data = localStorage.getItem("participants");
        participants = data ? JSON.parse(data) : [];
    }

    function renderTable() {
        const tbody = document.querySelector("#initiativeTable tbody");
        tbody.innerHTML = participants
            .map(
                (p, index) => `
            <tr class="${index === currentIndex ? 'highlight' : ''}">
                <td>${p.name}</td>
                <td>${p.initiative}</td>
                <td>
                    <span class="editable-hp" onclick="openHealthModal(${index})">${p.health}</span>
                </td>
                <td>
                    <button onclick="removeParticipant(${index})" class="btn btn-danger">Remove</button>
                </td>
            </tr>`
            )
            .join('');
    }

    function showCurrentTurn() {
        if (participants.length > 0) {
            document.getElementById("currentTurn").innerText = `Current Turn: ${participants[currentIndex].name}`;
        } else {
            document.getElementById("currentTurn").innerText = "No participants";
        }
    }

    function addParticipant(event) {
        event.preventDefault();
        const name = document.getElementById("participantName").value;
        const initiative = parseInt(document.getElementById("participantInitiative").value);
        const health = parseInt(document.getElementById("participantHealth").value);

        participants.push({ name, initiative, health });
        participants.sort((a, b) => b.initiative - a.initiative);
        saveParticipantsToLocalStorage();
        document.getElementById("addParticipantForm").reset();

        renderTable();
        showCurrentTurn();
    }

    let editIndex = null;

    window.openHealthModal = function (index) {
        const healthInput = document.getElementById("healthInput");
        const modalTitle = document.getElementById("editHealthLabel");

        editIndex = index;
        modalTitle.innerText = `Edit Health for ${participants[index].name}`;
        healthInput.value = participants[index].health;

        const modal = new bootstrap.Modal(document.getElementById("editHealthModal"));
        modal.show();
    };

    window.saveHealth = function () {
        const healthInput = document.getElementById("healthInput");
        const newHealth = parseInt(healthInput.value);

        if (!isNaN(newHealth) && newHealth >= 0) {
            participants[editIndex].health = newHealth;
            saveParticipantsToLocalStorage();
            renderTable();

            const modal = bootstrap.Modal.getInstance(document.getElementById("editHealthModal"));
            modal.hide();
        } else {
            alert("Please enter a valid number for health.");
        }
    };

    window.removeParticipant = function (index) {
        participants.splice(index, 1);
        currentIndex = currentIndex % participants.length;
        saveParticipantsToLocalStorage();
        renderTable();
        showCurrentTurn();
    };

    window.removeAllParticipants = function () {
        participants = [];
        saveParticipantsToLocalStorage();
        renderTable();
        showCurrentTurn();
    };

    document.getElementById("addParticipantForm").addEventListener("submit", addParticipant);
    document.getElementById("nextTurn").addEventListener("click", () => {
        if (participants.length > 0) {
            currentIndex = (currentIndex + 1) % participants.length;
            renderTable();
            showCurrentTurn();
        }
    });

    loadParticipantsFromLocalStorage();
    renderTable();
    showCurrentTurn();
});