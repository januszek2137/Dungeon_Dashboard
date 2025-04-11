document.getElementById("toggle-dashboard").addEventListener("click", function () {
    var sidebar = document.getElementById("sidebar");
    var contentWrapper = document.getElementById("content-wrapper");
    sidebar.classList.toggle("active");
    contentWrapper.classList.toggle("active");

    if (this.classList.contains("rotate-180")) {
        this.classList.remove("rotate-180");
        this.classList.add("rotate-360");
    } else if (this.classList.contains("rotate-360")) {
        this.classList.remove("rotate-360");
        this.classList.add("rotate-180");
    } else {
        this.classList.add("rotate-180");
    }
});

function updateSliderValue(sliderId, valueId) {
    const slider = document.getElementById(sliderId);
    const value = document.getElementById(valueId);

    value.textContent = slider.value;

    slider.addEventListener('input', () => {
        value.textContent = slider.value;
    });
}
updateSliderValue('sliderStrength', 'sliderValueStrength');
updateSliderValue('sliderDexterity', 'sliderValueDexterity');
updateSliderValue('sliderConstitution', 'sliderValueConstitution');
updateSliderValue('sliderIntelligence', 'sliderValueIntelligence');
updateSliderValue('sliderWisdom', 'sliderValueWisdom');
updateSliderValue('sliderCharisma', 'sliderValueCharisma');

function rollDice() {
    const diceType = parseInt(document.getElementById("diceType").value);
    const diceCount = parseInt(document.getElementById("diceCount").value);
    const diceContainer = document.getElementById("diceContainer");
    const resultElement = document.getElementById("diceResult");

    diceContainer.innerHTML = '';
    resultElement.innerText = '';

    if (diceCount > 50) {
        errorMessage.innerText = "You cannot roll more than 50 dice at once";
        return;
    }

    const results = [];
    let diceFinished = 0;

    errorMessage.innerText = "";

    for (let i = 0; i < diceCount; i++) {
        const die = document.createElement('div');
        die.className = 'die';

        switch (diceType) {
            case 4:
                die.classList.add('d4');
                break;
            case 6:
                die.classList.add('d6');
                break;
            case 8:
                die.classList.add('d8');
                break;
            case 10:
                die.classList.add('d10');
                break;
            case 12:
                die.classList.add('d12');
                break;
            case 20:
                die.classList.add('d20');
                break;
            case 100:
                die.classList.add('d100');
                break;
            default:
                break;
        }

        die.innerHTML = '<span class="die-value"></span>';

        diceContainer.appendChild(die);

        let animationDuration = 1000 + Math.random() * 1000;
        let startTime = Date.now();

        const interval = setInterval(function () {
            let elapsed = Date.now() - startTime;

            let randomFace = Math.floor(Math.random() * diceType) + 1;
            die.querySelector('.die-value').innerText = randomFace;

            if (elapsed >= animationDuration) {
                const finalValue = Math.floor(Math.random() * diceType) + 1;
                die.querySelector('.die-value').innerText = finalValue;
                results.push(finalValue);
                diceFinished++;
                clearInterval(interval);

                if (diceFinished === diceCount) {
                    const total = results.reduce((a, b) => a + b, 0);
                    resultElement.innerText = `Total: ${total}`;
                }
            }
        }, 50);
    }
}

//function showToast(options) {
//    const toastContainer = document.getElementById('toast-container');
//    const toastEl = document.createElement('div');
//    toastEl.className = `toast align-items-center custom-toast mb-2`;
//    toastEl.setAttribute('role', 'alert');
//    toastEl.setAttribute('aria-live', 'assertive');
//    toastEl.setAttribute('aria-atomic', 'true');

//    if (options.hasActions) {
//        toastEl.innerHTML = `
//                                    <div class="d-flex">
//                                        <div class="toast-body">
//                                            ${options.message}
//                                            <div class="mt-2">
//                                                <button class="btn btn-sm btn-accept me-2" data-invitation-id="${options.invitationId}" data-room-id="${options.roomId}">Akceptuj</button>
//                                                <button class="btn btn-sm btn-reject" data-invitation-id="${options.invitationId}">Odrzuć</button>
//                                            </div>
//                                        </div>
//                                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
//                                    </div>
//                                `;
//    } else {
//        toastEl.innerHTML = `
//                                    <div class="d-flex">
//                                        <div class="toast-body">
//                                            ${options.message}
//                                        </div>
//                                        <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
//                                    </div>
//                                `;
//    }

//    toastContainer.appendChild(toastEl);

//    const bsToast = new bootstrap.Toast(toastEl, { delay: options.delay || 5000 });
//    bsToast.show();

//    toastEl.addEventListener('hidden.bs.toast', () => {
//        toastEl.remove();
//    });

//    if (options.hasActions) {
//        const acceptBtn = toastEl.querySelector('.btn-accept');
//        const rejectBtn = toastEl.querySelector('.btn-reject');

//        acceptBtn.addEventListener('click', function () {
//            const inviteId = this.getAttribute('data-invitation-id');
//            const roomId = this.getAttribute('data-room-id');
//            acceptInvitation(inviteId, roomId);
//            bsToast.hide();
//        });

//        rejectBtn.addEventListener('click', function () {
//            const inviteId = this.getAttribute('data-invitation-id');
//            rejectInvitation(inviteId);
//            bsToast.hide();
//        });
//    }
//}

//function acceptInvitation(invitationId, roomId) {
//    fetch('/api/invitations/accept', {
//        method: 'POST',
//        headers: { 'Content-Type': 'application/json' },
//        body: JSON.stringify({ invitationId: invitationId, roomId: roomId })
//    })
//        .then(response => {
//            if (response.ok) {
//                return response.json();
//            } else {
//                throw new Error("Błąd przy akceptacji zaproszenia");
//            }
//        })
//        .then(data => {
//            window.location.href = `/Room/Details/${roomId}`;
//        })
//        .catch(err => {
//            console.error(err);
//            showToast({ message: "Błąd podczas akceptacji zaproszenia: " + err.message, type: "danger" });
//        });
//}

//function rejectInvitation(invitationId) {
//    fetch('/api/invitations/reject', {
//        method: 'POST',
//        headers: { 'Content-Type': 'application/json' },
//        body: JSON.stringify({ invitationId: invitationId })
//    })
//        .then(response => {
//            if (response.ok) {
//                return response.json();
//            } else {
//                throw new Error("Błąd przy odrzuceniu zaproszenia");
//            }
//        })
//        .then(data => {
//            showToast({ message: "Zaproszenie zostało odrzucone.", type: "success" });
//        })
//        .catch(err => {
//            console.error(err);
//            showToast({ message: "Błąd podczas odrzucenia zaproszenia: " + err.message, type: "danger" });
//        });
//}

//document.addEventListener("DOMContentLoaded", function () {
//    const inviteSubmitBtn = document.getElementById("invite-submit");
//    inviteSubmitBtn.addEventListener("click", function () {
//        const invitee = document.getElementById("invite-username").value.trim();
//        const roomId = document.getElementById("invite-roomId").value;

//        if (!invitee) {
//            showToast({ message: "Podaj nazwę użytkownika do zaproszenia.", type: "danger" });
//            return;
//        }

//        fetch('/api/invitations', {
//            method: 'POST',
//            headers: { 'Content-Type': 'application/json' },
//            body: JSON.stringify({ invitee: invitee, roomId: parseInt(roomId) })
//        })
//            .then(response => {
//                if (response.ok) {
//                    return response.json();
//                } else {
//                    throw new Error("Błąd przy wysyłaniu zaproszenia");
//                }
//            })
//            .then(invitation => {
//                showToast({ message: `Wysłano zaproszenie do: ${invitee}`, type: "success" });
//                const inviteModalElem = document.getElementById("inviteModal");
//                const inviteModal = bootstrap.Modal.getInstance(inviteModalElem);
//                inviteModal.hide();
//            })
//            .catch(err => {
//                console.error(err);
//                showToast({ message: "Błąd podczas wysyłania zaproszenia: " + err.message, type: "danger" });
//            });
//    });
//});

//const connection = new signalR.HubConnectionBuilder()
//    .withUrl("/notificationHub", { withCredentials: true })
//    .build();

//connection.on("ReceiveNotification", function (invitation) {
//    const message = `Nowe zaproszenie od: ${invitation.inviter}, RoomID: ${invitation.roomId}`;
//    showToast({
//        message: message,
//        hasActions: true,
//        invitationId: invitation.id,
//        roomId: invitation.roomId,
//        type: "primary",
//        delay: 10000
//    });
//});

//connection.start()
//    .then(() => {
//        console.log("Połączono z SignalR");
//    })
//    .catch(err => showToast({ message: "Błąd połączenia z SignalR: " + err, type: "danger" }));