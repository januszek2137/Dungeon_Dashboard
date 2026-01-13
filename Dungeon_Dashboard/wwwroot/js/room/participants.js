document.addEventListener("DOMContentLoaded", () => {
    const listEl = document.getElementById("participantsList");

    function renderList(onlineUsers) {
        listEl.innerHTML = "";
        const onlineSet = new Set(onlineUsers.map(u => u.toLowerCase()));
        allUsers.forEach(u => {
            const li = document.createElement("li");
            li.className = "list-group-item";
            const dot = document.createElement("span");
            dot.className = "status-dot " + (onlineSet.has(u.toLowerCase()) ? "online" : "offline");
            const details = document.createElement("div");
            details.className = "user-details";
            details.innerHTML = `<div class="user-name">${u}</div>
                                     <div class="user-status ${onlineSet.has(u.toLowerCase()) ? "online" : "offline"}">
                                       ${onlineSet.has(u.toLowerCase()) ? "Online" : "Offline"}
                                     </div>`;
            li.append(dot, details);
            listEl.append(li);
        });
    }

    const participantsConn = new signalR.HubConnectionBuilder()
        .withUrl("/participantsHub")
        .withAutomaticReconnect()
        .build();

    participantsConn.on("UpdateParticipants", renderList);
    participantsConn.start()
        .then(() => participantsConn.invoke("JoinRoom", roomId, userName))
        .catch(err => console.error("ParticipantsHub error:", err));
});