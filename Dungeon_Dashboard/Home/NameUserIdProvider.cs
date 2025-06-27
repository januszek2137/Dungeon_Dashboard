using Microsoft.AspNetCore.SignalR;

namespace Dungeon_Dashboard.Home {

    public class NameUserIdProvider : IUserIdProvider {

        public string GetUserId(HubConnectionContext connection) {
            return connection.User?.Identity?.Name;
        }
    }
}