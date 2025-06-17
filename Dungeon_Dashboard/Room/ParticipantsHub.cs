using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Dungeon_Dashboard.Group {

    public class ParticipantsHub : Hub {
        private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, string>> _rooms = new();

        public async Task JoinRoom(int roomId, string userName) {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId.ToString());
            var users = _rooms.GetOrAdd(roomId, _ => new());
            users[Context.ConnectionId] = userName;
            await Clients.Group(roomId.ToString()).SendAsync("UpdateParticipants", users.Values);
        }

        public override async Task OnDisconnectedAsync(Exception? ex) {
            foreach(var room in _rooms) {
                if(room.Value.TryRemove(Context.ConnectionId, out _)) {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, room.Key.ToString());
                    await Clients.Group(room.Key.ToString()).SendAsync("UpdateParticipants", room.Value.Values);
                }
            }
            await base.OnDisconnectedAsync(ex);
        }
    }
}