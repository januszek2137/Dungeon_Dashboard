using Dungeon_Dashboard.Group.Notes;
using System.ComponentModel.DataAnnotations;

namespace Dungeon_Dashboard.Group {

    public class RoomModel {
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        public string? CreatedBy { get; set; }
        public List<string>? Participants { get; set; } = new List<string>();

        public ICollection<NoteModel> Notes { get; set; } = new List<NoteModel>();
    }
}