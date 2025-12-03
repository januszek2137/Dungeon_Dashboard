namespace Dungeon_Dashboard.Event.Models {

    public class EventModel {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Start { get; set; }
        public string Location { get; set; }
    }
}