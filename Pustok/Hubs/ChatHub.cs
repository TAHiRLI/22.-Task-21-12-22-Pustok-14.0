using Microsoft.AspNetCore.SignalR;
using MimeKit.Cryptography;
using System.Threading.Tasks;

namespace Pustok.Hubs
{
    public class ChatHub:Hub
    {
        public override async Task OnConnectedAsync()
        {
           await Clients.All.SendAsync("test", Context.ConnectionId);
             base.OnConnectedAsync();
        }
        public async Task SendMessage(string message)
        {
           await Clients.All.SendAsync("getMessage" ,message);
        }
    }
}
