using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.Home.Controllers {

    public class SidebarController : Controller {

        [Authorize]
        public IActionResult Planner() {
            return View();
        }

        [Authorize]
        public IActionResult Characters() {
            return View();
        }

        [Authorize]
        public IActionResult Journal() {
            return View();
        }
    }
}