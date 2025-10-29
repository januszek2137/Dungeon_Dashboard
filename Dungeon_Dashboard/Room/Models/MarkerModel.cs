namespace Dungeon_Dashboard.Room.Models;

public class MarkerModel
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public int X { get; set; }
    public int Y { get; set; }
    public string Color { get; set; } = "#000000";

    public int RoomId { get; set; }
    public RoomModel? Room { get; set; }
}