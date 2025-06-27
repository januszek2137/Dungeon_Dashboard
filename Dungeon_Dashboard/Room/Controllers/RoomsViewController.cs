using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Room.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.Room.Controllers {

    [Authorize]
    public class RoomsViewController : Controller {
        private readonly AppDBContext _context;

        public RoomsViewController(AppDBContext context) {
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoomModel model) {
            if(!ModelState.IsValid) {
                return View(model);
            }

            var room = new RoomModel {
                Name = model.Name,
                Description = model.Description,
                CreatedBy = User.Identity.Name,
                Participants = new List<string> { User.Identity.Name }
            };

            _context.RoomModel.Add(room);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "RoomsView");
        }

        public async Task<IActionResult> Room(int id) {
            var room = await _context.RoomModel.FindAsync(id);

            if(room == null) {
                return NotFound($"Can't find a room with id = {id}");
            }

            if(room.Participants == null || !room.Participants.Contains(User.Identity.Name)) {
                return Forbid();
            }

            return View(room);
        }
    }
}