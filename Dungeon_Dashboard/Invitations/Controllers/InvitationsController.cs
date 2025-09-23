using Dungeon_Dashboard.Invitations.Models;
using Dungeon_Dashboard.Invitations.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;

namespace Dungeon_Dashboard.Invitations.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InvitationsController : ControllerBase
    {
        private readonly IInvitationService _invitationService;

        public InvitationsController(IInvitationService invitationService)
        {
            _invitationService = invitationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvitation([FromBody] InvitationModel invitation)
        {
            if (invitation == null || string.IsNullOrWhiteSpace(invitation.Invitee))
            {
                return BadRequest("Invalid invitation data");
            }

            var inviter = User.Identity?.Name ?? "Anonymous";
            var created = await _invitationService.CreateInvitationAsync(invitation, inviter);

            if (created == null)
            {
                return Conflict("This user has already accepted their invitation");
            }

            return Ok(created);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInvitations()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized();

            var invitations = await _invitationService.GetUserInvitationsAsync(username);
            return Ok(invitations);
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptInvitation(int id)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized();

            var result = await _invitationService.AcceptInvitationAsync(id, username);

            if (result == null)
                return NotFound("Invitation or room not found");

            return Ok(result);
        }

        [HttpPost("{id}/decline")]
        public async Task<IActionResult> DeclineInvitation(int id)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized();

            var success = await _invitationService.DeclineInvitationAsync(id, username);

            return success ? NoContent() : NotFound("Invitation not found");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvitation(int id)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrWhiteSpace(username))
                return Unauthorized();

            var success = await _invitationService.DeleteInvitationAsync(id, username);

            return success ? NoContent() : NotFound("Invitation not found");
        }
    }
}
