using Dungeon_Dashboard.Room.Hubs;
using Dungeon_Dashboard.Room.Models;
using Microsoft.AspNetCore.SignalR;

namespace Dungeon_Dashboard.Room;

public interface IMapService {
    Task<RoomModel?>           UploadMapAsync(int  roomId, IFormFile file, string? userName, CancellationToken ct = default);
    Task<MarkerModel>       MoveMarkerAsync(int roomId, string userId, int x, int y, CancellationToken ct = default);
    Task<List<MarkerModel>> GetMarkersAsync(int roomId, CancellationToken ct = default);
}

public class MapService : IMapService {
    private readonly IMarkerRepository   _markerRepo;
    private readonly IRoomRepository     _roomRepo;
    private readonly IHubContext<MapHub> _hubContext;
    public MapService(IMarkerRepository markerRepo, IRoomRepository roomRepo, IHubContext<MapHub> hubContext) { 
        _markerRepo = markerRepo;
        _roomRepo   = roomRepo;
        _hubContext = hubContext;
    }

    public async Task<RoomModel?> UploadMapAsync(int roomId, IFormFile file, string userName, CancellationToken ct = default) {
        var room = await _roomRepo.GetByIdAsync(roomId, ct);
        if (room == null)
            return null;
        
        if(!string.Equals(room.CreatedBy, userName, StringComparison.OrdinalIgnoreCase))
            return null;
        
        var mapUrl = await _roomRepo.SaveMapAsync(room, file, ct);
        
        await _hubContext.Clients.Group(roomId.ToString())
            .SendAsync("MapUpdated", mapUrl, ct);
        
        return room;
    }

    public async Task<MarkerModel> MoveMarkerAsync(int roomId, string userId, int x, int y, CancellationToken ct = default) {
        var marker = new MarkerModel {
            RoomId = roomId,
            UserId = userId,
            X      = x,
            Y      = y
        };
        
        await _markerRepo.UpsertAsync(marker, ct);
        return marker;
    }
    
    public Task<List<MarkerModel>> GetMarkersAsync(int roomId, CancellationToken ct = default)
        => _markerRepo.GetByRoomAsync(roomId, ct);
}