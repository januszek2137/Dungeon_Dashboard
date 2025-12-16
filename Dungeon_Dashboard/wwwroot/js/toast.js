function showToast(options) {
    const toastContainer = document.getElementById('toast-container');
    const toastEl = document.createElement('div');
    toastEl.className = `toast`;
    toastEl.setAttribute('role', 'alert');
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');

    if (options.hasActions) {
        toastEl.innerHTML = `
            <div class="toast-body card">
                <div class="toast-message">
                    ${options.message}
                    <div class="">
                        <button class="btn btn-primary" data-invitation-id="${options.invitationId}" data-room-id="${options.roomId}">Accept</button>
                        <button class="btn btn-secondary" data-invitation-id="${options.invitationId}">Decline</button>
                    </div>
                </div>
                <button type="button" class="btn-close-modal" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        `;
    } else {
        toastEl.innerHTML = `
            <div class="toast-body card">
            
                <div class="toast-message">
                    ${options.message}
                </div>
                <button type="button" data-bs-dismiss="toast" aria-label="Close" class="btn-close-modal"></button>
            </div>
        `;
    }

    toastContainer.appendChild(toastEl);

    const bsToast = new bootstrap.Toast(toastEl, {delay: options.delay || 5000});
    bsToast.show();

    toastEl.addEventListener('hidden.bs.toast', () => {
        toastEl.remove();
    });

    if (options.hasActions) {
        const acceptBtn = toastEl.querySelector('.btn-accept');
        const rejectBtn = toastEl.querySelector('.btn-reject');

        acceptBtn.addEventListener('click', function () {
            const inviteId = this.getAttribute('data-invitation-id');
            const roomId = this.getAttribute('data-room-id');
            acceptInvitation(inviteId, roomId);
            bsToast.hide();
        });

        rejectBtn.addEventListener('click', function () {
            const inviteId = this.getAttribute('data-invitation-id');
            rejectInvitation(inviteId);
            bsToast.hide();
        });
    }
}

function acceptInvitation(invitationId, roomId) {
    fetch(`/api/invitations/${invitationId}/accept`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'}
    })
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error("Error while accepting the invite");
            }
        })
        .then(data => {
            window.location.href = `/RoomsView/Room?id=${roomId}`;
        })
        .catch(err => {
            console.error(err);
            showToast({message: "Error while accepting the invite: " + err.message, type: "danger"});
        });
}

function rejectInvitation(invitationId) {
    fetch(`/api/invitations/${invitationId}/decline`, {
        method: 'POST',
        headers: {'Content-Type': 'application/json'}
    })
        .then(response => {
            if (response.ok) {
                return response.text();
            } else {
                throw new Error("Error while declining the invitation");
            }
        })
        .then(() => {
            showToast({message: "The invitation has been declined", type: "success"});
        })
        .catch(err => {
            console.error(err);
            showToast({message: "Error while declining the invitation: " + err.message, type: "danger"});
        });
}

document.addEventListener("DOMContentLoaded", function () {
    const inviteSubmitBtn = document.getElementById("invite-submit");
    inviteSubmitBtn.addEventListener("click", function () {
        const invitee = document.getElementById("invite-username").value.trim();
        const roomId = document.getElementById("invite-roomId").value;

        if (!invitee) {
            showToast({message: "Please provide a username to invite.", type: "danger"});
            return;
        }

        fetch('/api/invitations', {
            method: 'POST',
            headers: {'Content-Type': 'application/json'},
            body: JSON.stringify({invitee: invitee, roomId: parseInt(roomId)})
        })
            .then(response => {
                if (response.ok) {
                    return response.json();
                } else if (response.status === 409) {
                    return response.text().then(text => {
                        const inviteModalElem = document.getElementById("inviteModal");
                        const inviteModal = bootstrap.Modal.getInstance(inviteModalElem);
                        inviteModal.hide();
                        throw new Error(text || "This user has already accepted their invitation");
                    });
                } else {
                    throw new Error("Error while sending an invite");
                }
            })
            .then(invitation => {
                showToast({message: `Send an invite to: ${invitee}`, type: "success"});
                const inviteModalElem = document.getElementById("inviteModal");
                const inviteModal = bootstrap.Modal.getInstance(inviteModalElem);
                inviteModal.hide();
            })
            .catch(err => {
                console.error(err);
                showToast({message: "Error while sending an invite: " + err.message, type: "danger"});
            });
    });
});

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub", {withCredentials: true})
    .build();

connection.on("ReceiveNotification", function (invitation) {
    const message = `Nowe zaproszenie od: ${invitation.inviter}, RoomID: ${invitation.roomId}`;
    showToast({
        message: message,
        hasActions: true,
        invitationId: invitation.id,
        roomId: invitation.roomId,
        type: "primary",
        delay: 10000
    });
});

connection.on("InvitationAcceptedDeclined", function (data) {
    showToast({
        message: data.message,
        type: "primary",
        delay: 10000,
        hasActions: false
    });
});

connection.start()
    .then(() => {
        console.log("Connected to SignalR");
    })
    .catch(err => showToast({message: "Error while connecting to SignalR: " + err, type: "danger"}));