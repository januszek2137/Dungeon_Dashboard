using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Room.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Room.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService) {
            _roomService = roomService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom([FromBody] RoomModel room) { // out with this
            //if(!ModelState.IsValid) 
                //return BadRequest(ModelState);
            
           // var createdBy = User.Identity?.Name ?? "Annonymous";
            //var newRoom   = await _roomService.CreateRoomAsync(createdBy);
            //return Ok(newRoom);
            
            /*room.CreatedBy = User.Identity?.Name ?? "Annonymous";
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
            }#1#*/
            throw new Exception("Deprecated");
        } //out with this

        [HttpGet]
        public async Task<IActionResult> GetRooms() {
            var user  = User.Identity?.Name ?? "Annonymous";
            var rooms = await _roomService.GetRoomsForUserAsync(user);
            return Ok(rooms); 
            
            /*var user = User.Identity.Name;
            var rooms = await _context.RoomModel.Where(r => r.CreatedBy == user || r.Participants.Contains(user)).ToListAsync();
            return Ok(rooms);#1#*/
        } //change this ffs roomsindex.js this doesn't need to be here this whole controller needs to go
    }
}