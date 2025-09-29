using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Room.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Dungeon_Dashboard.Room.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class MapController : ControllerBase {
        private readonly IWebHostEnvironment _environment;
        private readonly AppDBContext        _context;
        private readonly IHubContext<MapHub> _hubContext;

        public MapController(IWebHostEnvironment environment, AppDBContext context, IHubContext<MapHub> hub) {
            _environment = environment;
            _context     = context;
            _hubContext  = hub;
        }

        [HttpPost("{roomId:int}/upload")]
        public async Task<IActionResult> Upload(int roomId, IFormFile file, CancellationToken ct = default) {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded");
            var room = await _context.RoomModel.FindAsync([roomId], ct);
            if (room == null)
                return NotFound();

            if (!string.Equals(room.CreatedBy, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
                return Forbid();
            
            var mapsDir = Path.Combine(_environment.WebRootPath, "maps");
            Directory.CreateDirectory(mapsDir);

            if (!string.IsNullOrEmpty(room.MapUrl)) {
                var oldPath = Path.Combine(_environment.WebRootPath, room.MapUrl.TrimStart('/'));
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            var safeName = $"{roomId}_{Guid.NewGuid():N}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(mapsDir, safeName);

            await using var fs = System.IO.File.Create(fullPath);
            await file.CopyToAsync(fs, ct);

            room.MapUrl = $"/maps/{safeName}";
            await _context.SaveChangesAsync();
            
            await _hubContext.Clients.Group(roomId.ToString())
                .SendAsync("MapUpdated", room.MapUrl);

            return Ok(new { roomId, room.MapUrl });
        }

        [HttpGet("{roomId:int}")]
        public async Task<IActionResult> GetRoom(int roomId, CancellationToken ct) {
            var room = await _context.RoomModel.FindAsync([roomId], ct);
            if (room == null)
                return NotFound();
            return Ok(new { room.Id, room.Name, room.MapUrl });
        }
    }
}
