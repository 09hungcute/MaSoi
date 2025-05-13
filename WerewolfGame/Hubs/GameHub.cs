// GameHub.cs
using Microsoft.AspNetCore.SignalR;

namespace WerewolfGame.Hubs
{
    public class GameHub : Hub
    {
        public async Task SendGameUpdate(string gameId, string updateMessage)
        {
            // Gửi thông báo cho tất cả người chơi trong game
            await Clients.Group(gameId).SendAsync("ReceiveGameUpdate", updateMessage);
        }

        public async Task JoinGameGroup(string gameId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        }

        public async Task LeaveGameGroup(string gameId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
    }
}
