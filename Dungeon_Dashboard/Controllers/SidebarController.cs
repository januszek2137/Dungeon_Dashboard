using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.Controllers {
    public class SidebarController : Controller {

        public IActionResult Planner() {
            return View();
        }

        public IActionResult Characters() {
            return View();
        }

        public IActionResult Journal() {
            return View();
        }
    }
}
