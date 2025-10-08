using Dungeon_Dashboard.Home.Data;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Room;
using Dungeon_Dashboard.Room.Models;

public interface IMarkerRepository {
    Task                    UpsertAsync(MarkerModel marker, CancellationToken ct = default);
    Task<List<MarkerModel>> GetByRoomAsync(int      roomId, CancellationToken ct = default);
}

public class MarkerRepository : IMarkerRepository {
    private readonly AppDBContext _context;
    
    public MarkerRepository(AppDBContext context, IWebHostEnvironment environment) { 
        _context = context;
    }
    
    public async Task UpsertAsync(MarkerModel marker, CancellationToken ct = default) {
        var existing = await _context.MarkerModel
            .FirstOrDefaultAsync(m=> m.RoomId == marker.RoomId && m.UserId == marker.UserId, ct);

        if (existing == null) _context.MarkerModel.Add(marker);
        else {
            existing.X      = marker.X;
            existing.Y      = marker.Y;
            _context.MarkerModel.Update(existing);
        }
        
        await _context.SaveChangesAsync(ct);
    }

    public Task<List<MarkerModel>> GetByRoomAsync(int roomId, CancellationToken ct = default) => 
        _context.MarkerModel.Where(m => m.RoomId == roomId).ToListAsync(ct);
}