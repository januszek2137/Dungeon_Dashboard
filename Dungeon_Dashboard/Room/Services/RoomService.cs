using Dungeon_Dashboard.Room.Models;

namespace Dungeon_Dashboard.Room;

public interface IRoomService {
    Task<RoomModel>       CreateRoomAsync(RoomModel   model, string createdBy, CancellationToken ct = default);
    Task<RoomModel>       GetRoomForUserAsync(int id, string? user, CancellationToken ct = default);
    Task<List<RoomModel>> GetRoomsForUserAsync(string user,  CancellationToken ct = default);
}

public class RoomService : IRoomService {
    private readonly IRoomRepository _roomRepo;

    public RoomService(IRoomRepository roomRepo) {
        _roomRepo = roomRepo;
    }

    public async Task<RoomModel> CreateRoomAsync(RoomModel model, string createdBy, CancellationToken ct = default) {
        var room = new RoomModel {
            Name         = model.Name,
            Description  = model.Description,
            CreatedBy    = createdBy,
            Participants = new List<string> { createdBy },
            MapUrl       = "/maps/default.jpg"
        };

        await _roomRepo.AddAsync(room, ct);
        return room;
    }

    public async Task<RoomModel> GetRoomForUserAsync(int id, string? user, CancellationToken ct = default) {
        var room = await _roomRepo.GetByIdWithDetailsAsync(id, ct);
        
        if (room == null) throw new KeyNotFoundException();
        
        if (user == null || room.Participants == null || !room.Participants.Contains(user))
            throw new UnauthorizedAccessException();
        
        return room;
    }
    
    public async Task<List<RoomModel>> GetRoomsForUserAsync(string user, CancellationToken ct = default) {
        //throw new Exception("Change this please - temporary fix, roomsindex.js needs change");
        return await _roomRepo.GetForUserAsync(user, ct);
    }
}