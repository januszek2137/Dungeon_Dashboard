fetch('/api/rooms')
    .then(response => response.json())
    .then(data => {
        const roomsContainer = document.getElementById('rooms');
        data.forEach(room => {
            const li = document.createElement('li');
            li.textContent = `${room.name} - Created by: ${room.createdBy}`;

            const inviteBtn = document.createElement('button');
            inviteBtn.textContent = "Invite";
            inviteBtn.style.marginLeft = "10px";
            inviteBtn.addEventListener('click', () => {
                openInviteModal(room.id);
            });

            const enterBtn = document.createElement('button');
            enterBtn.textContent = "Enter Room";
            enterBtn.style.marginLeft = "10px";
            enterBtn.addEventListener('click', () => {
                goToRoom(room.id);
            });

            li.appendChild(inviteBtn);
            li.appendChild(enterBtn);
            roomsContainer.appendChild(li);
        });
    })
    .catch(error => console.error('Error while loading rooms:', error));

window.openInviteModal = function (roomId) {
    document.getElementById("invite-roomId").value = roomId;
    document.getElementById("invite-username").value = "";
    $('#inviteModal').modal('show');
}