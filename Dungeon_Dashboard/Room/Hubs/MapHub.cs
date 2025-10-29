using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Room.Models;
using Microsoft.AspNetCore.SignalR;

namespace Dungeon_Dashboard.Room.Hubs;

public class MapHub : Hub {
    private readonly IMapService _mapService;
    private readonly AppDBContext _context;

    public MapHub(IMapService mapService, AppDBContext context) {
        _mapService = mapService;
        _context = context;
    }

    public override async Task OnConnectedAsync() {
        var roomId = Context.GetHttpContext()?.Request.Query["roomId"].ToString();
        if(!string.IsNullOrWhiteSpace(roomId))
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await base.OnConnectedAsync();
    }
    
    public async Task LoadMarkers(int roomId) {
        var markers = await _mapService.GetMarkersAsync(roomId);
        
        var userId = Context.User?.Identity?.Name;
        if (!string.IsNullOrEmpty(userId)) {
            var existing = markers.FirstOrDefault(m => m.UserId == userId);
            if (existing == null) {
                int gridSize = 50;
                int startX   = 50, startY = 50;
                int cols     = 5; 

                int offset = markers.Count;

                int row = offset / cols;
                int col = offset % cols;

                int x = startX + col * gridSize;
                int y = startY + row * gridSize;

                var newMarker = new MarkerModel
                {
                    RoomId = roomId,
                    UserId = userId,
                    X      = x,
                    Y      = y,
                    Color  = "#000000"
                };

                await _mapService.MoveMarkerAsync(roomId, userId, x, y);
                markers.Add(newMarker);
            }
        }
        
        await Clients.Caller.SendAsync("MarkersLoaded", markers);
    }

    public async Task MoveMarker(int roomId, string userId, int x, int y) {
        var marker = await _mapService.MoveMarkerAsync(roomId, userId, x, y);
        await Clients.Group(roomId.ToString()).SendAsync("MarkerMoved", marker);
    }

    public async Task ChangeMarkerColor(int roomId, string userId, string color) {
        var marker = _context.MarkerModel
            .FirstOrDefault(m=>m.RoomId == roomId && m.UserId == userId);
        if (marker != null) {
            marker.Color = color;
            await _context.SaveChangesAsync();
        }
        
        await Clients.Group(roomId.ToString()).SendAsync("MarkerColorChanged", new {userId, color});
    }
}