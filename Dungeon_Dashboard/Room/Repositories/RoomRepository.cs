using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Room.Models;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Room;

public interface IRoomRepository {
    Task                  AddAsync(RoomModel          room,   CancellationToken ct                         = default);
    Task<List<RoomModel>> GetForUserAsync(string      user,   CancellationToken ct                         = default);
    Task<RoomModel?>      GetByIdAsync(int            roomId, CancellationToken ct                         = default);
    Task<RoomModel?>      GetByIdWithDetailsAsync(int id,     CancellationToken ct                         = default);
    Task<string>          SaveMapAsync(RoomModel      room,   IFormFile         file, CancellationToken ct = default);
}

public class RoomRepository : IRoomRepository {
    private readonly IWebHostEnvironment _environment;
    private readonly AppDBContext        _context;
    
    public RoomRepository(IWebHostEnvironment environment, AppDBContext context) {
        _environment = environment;
        _context     = context;
    }
    
    public async Task AddAsync(RoomModel room, CancellationToken ct = default) {
        _context.RoomModel.Add(room);
        await _context.SaveChangesAsync(ct);
    }

    public Task<List<RoomModel>> GetForUserAsync(string user, CancellationToken ct = default) {
        return _context.RoomModel
            .Where(r => r.CreatedBy == user || r.Participants.Contains(user))
            .ToListAsync(ct);
    }

    public Task<RoomModel?> GetByIdAsync(int roomId, CancellationToken ct = default) {
        return _context.RoomModel.FirstOrDefaultAsync(r=> r.Id == roomId, ct);
    }

    public Task<RoomModel?> GetByIdWithDetailsAsync(int id, CancellationToken ct = default) {
        return _context.RoomModel
            .Include(r=>r.Notes)
            .Include(r=>r.Markers)
            .FirstOrDefaultAsync(r => r.Id == id, ct);
    }

    public async Task<string> SaveMapAsync(RoomModel room, IFormFile file, CancellationToken ct = default) {
        var mapsDir = Path.Combine(_environment.WebRootPath, "maps");
        Directory.CreateDirectory(mapsDir);
        
        if (!string.IsNullOrEmpty(room.MapUrl)) {
            var oldPath = Path.Combine(_environment.WebRootPath, room.MapUrl.TrimStart('/'));
            if (File.Exists(oldPath))
                File.Delete(oldPath);
        }

        var safeName = $"{room.Id}_{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(mapsDir, safeName);

        await using var fs = File.Create(fullPath);
        await file.CopyToAsync(fs, ct);
        
        room.MapUrl = $"/maps/{safeName}";
        await _context.SaveChangesAsync(ct);
        
        return room.MapUrl;
    }
}