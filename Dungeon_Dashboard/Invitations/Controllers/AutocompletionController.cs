using Dungeon_Dashboard.Invitations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client.AppConfig;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : Controller {
    private readonly IUserLookupService           _lookup;
    private readonly UserManager<IdentityUser> _userManager;

    public UsersController(IUserLookupService lookup, UserManager<IdentityUser> userManager) {
        _lookup      = lookup;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("Lookup")]
    public async Task<IActionResult> Lookup(string term, CancellationToken ct) {
        var me    = _userManager.GetUserId(User);
        var items = await _lookup.SearchByEmailPrefixAsync(term, me, 10, ct);
        return Ok(items);
    }
}