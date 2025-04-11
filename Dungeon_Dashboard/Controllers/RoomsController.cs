using Dungeon_Dashboard.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase {
        private readonly AppDBContext _context;

        public RoomsController(AppDBContext context) {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomModel room) {

            room.CreatedBy = User.Identity?.Name ?? "Annonymous";
            room.Participants = new List<string> { room.CreatedBy };

            if(!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                _context.RoomModel.Add(room);
                await _context.SaveChangesAsync();
                return Ok(room);
            } catch(Exception e) {
                return StatusCode(500, $"Internal server error: {e}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms() {
            var user = User.Identity.Name;
            var rooms = await _context.RoomModel.Where(r => r.CreatedBy == user || r.Participants.Contains(user)).ToListAsync();
            return Ok(rooms);
        }
    }
}