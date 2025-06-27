using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.TurnKeeper.Controllers {
    public class TurnKeeperViewController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
