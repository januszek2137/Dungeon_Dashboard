using Dungeon_Dashboard.Room.Models;

namespace Dungeon_Dashboard.Room;

public interface IMapService {
    Task<MarkerModel> MoveMarkerAsync(int roomId, string userId, int x, int y, CancellationToken ct = default);
    Task<List<MarkerModel>> GetMarkersAsync(int roomId, CancellationToken ct = default);
}

public class MapService : IMapService {
    private readonly IMarkerRepository _repo;
    public MapService(IMarkerRepository repo) => _repo = repo;
    
    public async Task<MarkerModel> MoveMarkerAsync(int roomId, string userId, int x, int y, CancellationToken ct = default) {
        var marker = new MarkerModel {
            RoomId = roomId,
            UserId = userId,
            X      = x,
            Y      = y
        };
        
        await _repo.UpsertAsync(marker, ct);
        return marker;
    }
    
    public Task<List<MarkerModel>> GetMarkersAsync(int roomId, CancellationToken ct = default)
        => _repo.GetByRoomAsync(roomId, ct);
}