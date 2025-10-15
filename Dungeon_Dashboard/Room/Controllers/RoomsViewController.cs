using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Room.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Room.Controllers {

    [Authorize]
    public class RoomsViewController : Controller {
        private readonly IRoomService _roomService;

        public RoomsViewController( IRoomService roomService) {
            _roomService = roomService;
        }

        public async Task<IActionResult> Index() {
            List<RoomModel> model = await _roomService.GetRoomsForUserAsync(User.Identity?.Name ?? "Annonymous");
            return View(model);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomModel model) {
            if(!ModelState.IsValid) {
                return View(model);
            }

            var createdBy = User.Identity?.Name ?? "Annonymous";
            var room = await _roomService.CreateRoomAsync(model, createdBy);

            return RedirectToAction("Room", "RoomsView", new { id = room.Id });
        }

        public async Task<IActionResult> Room(int id) {
            try {
                var room = await _roomService.GetRoomForUserAsync(id, User.Identity?.Name);
                return View(room);
            } catch(KeyNotFoundException) {
                return NotFound($"Can't find a room with id = {id}");
            } catch(UnauthorizedAccessException) {
                return Forbid();
            }
        }
    }
}