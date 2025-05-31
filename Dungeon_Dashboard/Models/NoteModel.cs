using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dungeon_Dashboard.Models {

    public class NoteModel {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public string CreatedBy { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.Now;

        [ForeignKey("Room")]
        public int RoomId { get; set; }

        public RoomModel Room { get; set; } = null!;
    }
}