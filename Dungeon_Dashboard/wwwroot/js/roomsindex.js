window.openInviteModal = function (roomId) {
    document.getElementById("invite-roomId").value = roomId;
    document.getElementById("invite-username").value = "";
    $('#inviteModal').modal('show');
}