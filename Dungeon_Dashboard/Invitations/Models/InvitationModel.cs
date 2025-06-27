namespace Dungeon_Dashboard.Invitations.Models {

    public class InvitationModel {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? Inviter { get; set; }
        public string? Invitee { get; set; }
        public bool? IsAccepted { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    }
}