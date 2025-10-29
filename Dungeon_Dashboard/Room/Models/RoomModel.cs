using Dungeon_Dashboard.Room.Notes.Models;
using System.ComponentModel.DataAnnotations;

namespace Dungeon_Dashboard.Room.Models {

    public class RoomModel {
        public int Id { get; set; }

        [Required] public string? Name { get; set; }

        [Required] public string? Description { get; set; }

        public string?       CreatedBy    { get; set; }
        public string?       MapUrl       { get; set; }
        public List<string>? Participants { get; set; } = new List<string>();

        public ICollection<NoteModel>   Notes   { get; set; } = new List<NoteModel>();
        public ICollection<MarkerModel> Markers { get; set; } = new List<MarkerModel>();
    }
}