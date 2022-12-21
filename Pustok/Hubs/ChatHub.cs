using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using MimeKit.Cryptography;
using Pustok.DAL;
using Pustok.Models;
using System;
using System.Threading.Tasks;

namespace Pustok.Hubs
{
    public class ChatHub : Hub
    {
        private readonly PustokDbContext _context;
        private readonly IHttpContextAccessor _httpAccessor;
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(PustokDbContext context, IHttpContextAccessor httpAccessor, UserManager<AppUser> userManager)
        {
            this._context = context;
            this._httpAccessor = httpAccessor;
            this._userManager = userManager;
        }
        public override async Task OnConnectedAsync()
        {
            if (_httpAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(_httpAccessor.HttpContext.User.Identity.Name);

                if (user != null)
                {
                    user.ConnectionId = Context.ConnectionId;
                    user.LastConnectedAt = DateTime.UtcNow.AddHours(4);
                    var result = await _userManager.UpdateAsync(user);
                   await  Clients.All.SendAsync("setAsOnline",user.Id);
                }


            }
            base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (_httpAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(_httpAccessor.HttpContext.User.Identity.Name);

                if (user != null)
                {
                    user.ConnectionId = null;
                    user.LastConnectedAt = DateTime.UtcNow.AddHours(4);
                    var result = await _userManager.UpdateAsync(user);
                    await Clients.All.SendAsync("setAsOffline", user.Id);

                }


            }
            base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("getMessage", message);
        }

    }
}
