using Dungeon_Dashboard.Models;
using Dungeon_Dashboard.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class InvitationsController : ControllerBase {
        private readonly AppDBContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<NotificationHub> _logger;

        public InvitationsController(AppDBContext context, IHubContext<NotificationHub> hubContext, ILogger<NotificationHub> logger) {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        public async Task<List<string>> GetParticipantsForRoomAsync(int roomId) {
            var room = await _context.RoomModel
                .FirstOrDefaultAsync(r => r.Id == roomId);

            return room?.Participants.ToList() ?? new List<string>();
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvitation([FromBody] InvitationModel invitation) {
            if(invitation == null || string.IsNullOrWhiteSpace(invitation.Invitee)) {
                return BadRequest("Invalid invitation data");
            }

            List<string> participants = await GetParticipantsForRoomAsync(invitation.RoomId);

            if(participants.Any(u => u.Equals(invitation.Invitee, StringComparison.OrdinalIgnoreCase))) {
                return Conflict("This user has already accepted their invitation");
            }

            invitation.Inviter = User.Identity.Name ?? "Annonymous";
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"CreateInvitation - Inviter={invitation.Inviter}, Invitee={invitation.Invitee}",
            User.Identity?.Name,
            invitation.Invitee);

            await _hubContext.Clients.User(invitation.Invitee).SendAsync("ReceiveNotification", invitation);
            //await _hubContext.Clients.All.SendAsync("ReceiveNotification", invitation);
            return Ok(invitation);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserInvitations() {
            var user = User.Identity?.Name;
            var invitations = await _context.InvitationModel.Where(i => i.Invitee == user).ToListAsync();
            return Ok(invitations);
        }

        [HttpPost("{id}/accept")]
        public async Task<IActionResult> AcceptInvitation(int id) {
            var invitation = await _context.InvitationModel.FindAsync(id);
            if(invitation == null || invitation.Invitee != User.Identity.Name) {
                return NotFound("Invitation not found");
            }

            invitation.IsAccepted = true;
            var room = await _context.RoomModel.FindAsync(invitation.RoomId);
            if(room == null) {
                return NotFound("Room not found");
            }

            room.Participants.Add(invitation.Invitee);
            await _context.SaveChangesAsync();

            //await _hubContext.Clients.User(invitation.Inviter).SendAsync("ReceiveNotification", $"{invitation.Invitee} has accepted your invitation");

            await _hubContext.Clients.User(invitation.Inviter).SendAsync("InvitationAcceptedDeclined", new {
                message = $"{invitation.Invitee} has accepted your invitation",
                hasActions = false,
            });

            return Ok(room);
        }

        [HttpPost("{id}/decline")]
        public async Task<IActionResult> DeclineInvitation(int id) {
            var invitation = await _context.InvitationModel.FindAsync(id);
            if(invitation == null || invitation.Invitee != User.Identity.Name) {
                return NotFound("Invitation not found");
            }

            _context.InvitationModel.Remove(invitation);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(invitation.Inviter).SendAsync("InvitationAcceptedDeclined", new {
                message = $"{invitation.Invitee} has declined your invitation",
                hasActions = false
            });

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvitation(int id) {
            var invitation = await _context.InvitationModel.FindAsync(id);
            if(invitation == null || invitation.Inviter != User.Identity.Name) {
                return NotFound("Invitation not found");
            }

            _context.InvitationModel.Remove(invitation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}