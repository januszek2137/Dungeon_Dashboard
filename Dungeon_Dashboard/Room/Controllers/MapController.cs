using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Room.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Dungeon_Dashboard.Room.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MapController : ControllerBase {
        private readonly AppDBContext _context;
        private readonly IMapService  _mapService;
        private readonly IRoomService _roomService;

        public MapController( AppDBContext context, IMapService mapService, IRoomService roomService ) {
            _context    = context;
            _mapService = mapService;
            _roomService = roomService;
        }

        [HttpPost("{roomId:int}/upload")]
        public async Task<IActionResult> Upload(int roomId, IFormFile file, CancellationToken ct = default) {
            if (file == null || file.Length == 0)
                return BadRequest("No File Uploaded");
            
            var room = await _mapService.UploadMapAsync(roomId, file, User.Identity?.Name ?? string.Empty, ct);
            if (room == null)
                return NotFound();
            
            //if (string.IsNullOrEmpty(mapUrl))
              //  return Forbid();
            
            return Ok(room);
        }

        [HttpGet("{roomId:int}")]
        public async Task<IActionResult> GetRoom(int roomId, CancellationToken ct) {
            try {
                var room = await _roomService.GetRoomForUserAsync(roomId, User.Identity?.Name);
                return Ok(new { room.Id, room.Name, room.MapUrl });
            } catch(KeyNotFoundException) {
                return NotFound($"Can't find a room with id = {roomId}");
            } catch(UnauthorizedAccessException) {
                return Forbid();
            }
            
        }
    }
}
