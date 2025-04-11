using Microsoft.AspNetCore.Mvc;

namespace Dungeon_Dashboard.Controllers {
    public class InvitationsViewController : Controller {
        public IActionResult Index() {
            return View();
        }
    }
}
