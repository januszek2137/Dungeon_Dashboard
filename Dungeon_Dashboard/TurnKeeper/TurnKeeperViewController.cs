using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.TurnKeeper {
    public class TurnKeeperViewController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
