using Dungeon_Dashboard.Home.Data;
using Dungeon_Dashboard.Invitations.Hubs;
using Dungeon_Dashboard.Invitations.Models;
using Dungeon_Dashboard.Room.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Dungeon_Dashboard.Invitations.Services
{
    public interface IInvitationService
    {
        Task<InvitationModel?> CreateInvitationAsync(InvitationModel invitation, string inviter);
        Task<List<InvitationModel>> GetUserInvitationsAsync(string username);
        Task<RoomModel?> AcceptInvitationAsync(int id, string username);
        Task<bool> DeclineInvitationAsync(int id, string username);
        Task<bool> DeleteInvitationAsync(int id, string username);
    }

    public class InvitationService : IInvitationService
    {
        private readonly AppDBContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogger<InvitationService> _logger;

        public InvitationService(AppDBContext context, IHubContext<NotificationHub> hubContext, ILogger<InvitationService> logger)
        {
            _context = context;
            _hubContext = hubContext;
            _logger = logger;
        }

        private async Task<List<string>> GetParticipantsForRoomAsync(int roomId)
        {
            var room = await _context.RoomModel.FirstOrDefaultAsync(r => r.Id == roomId);
            return room?.Participants.ToList() ?? new List<string>();
        }

        public async Task<InvitationModel?> CreateInvitationAsync(InvitationModel invitation, string inviter)
        {
            var participants = await GetParticipantsForRoomAsync(invitation.RoomId);

            if (participants.Any(u => u.Equals(invitation.Invitee, StringComparison.OrdinalIgnoreCase)))
            {
                return null;
            }

            invitation.Inviter = inviter;
            _context.InvitationModel.Add(invitation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("CreateInvitation - Inviter={Inviter}, Invitee={Invitee}", inviter, invitation.Invitee);

            await _hubContext.Clients.User(invitation.Invitee).SendAsync("ReceiveNotification", invitation);

            return invitation;
        }

        public async Task<List<InvitationModel>> GetUserInvitationsAsync(string username)
        {
            return await _context.InvitationModel.Where(i => i.Invitee == username).ToListAsync();
        }

        public async Task<RoomModel?> AcceptInvitationAsync(int id, string username)
        {
            var invitation = await _context.InvitationModel.FindAsync(id);
            if (invitation == null || invitation.Invitee != username)
                return null;

            var room = await _context.RoomModel.FindAsync(invitation.RoomId);
            if (room == null)
                return null;

            invitation.IsAccepted = true;
            room.Participants.Add(username);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(invitation.Inviter).SendAsync("InvitationAcceptedDeclined",
            new {
                message = $"{username} has accepted your invitation",
                hasActions = false,
            });

            return room;
        }

        public async Task<bool> DeclineInvitationAsync(int id, string username)
        {
            var invitation = await _context.InvitationModel.FindAsync(id);
            if (invitation == null || invitation.Invitee != username)
                return false;

            _context.InvitationModel.Remove(invitation);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(invitation.Inviter).SendAsync("InvitationAcceptedDeclined",
            new{
                message = $"{username} has declined your invitation",
                hasActions = false
            });

            return true;
        }

        public async Task<bool> DeleteInvitationAsync(int id, string username)
        {
            var invitation = await _context.InvitationModel.FindAsync(id);
            if (invitation == null || invitation.Inviter != username)
                return false;

            _context.InvitationModel.Remove(invitation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
