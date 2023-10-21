using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using LiveGameApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace LiveGameApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatClient>
    {
        public async Task SendMessage(Directmessage message)
        {

            var claim = Context.User.Claims.Where(m => m.Type == ClaimTypes.Name).FirstOrDefault();
            var msg = Context.User.Claims.Where(m => m.Type == ClaimTypes.NameIdentifier).DefaultIfEmpty(new Claim("", "0")).First().Value;

            foreach (var c in Context.User.Claims)
            {
                msg += (" " + c.ToString());
            }
            //throw new Exception(msg);
            //throw new Exception(message.RecipientId.ToString());
            await Clients.Caller.ReceiveMessage(message);
            await Clients.User(message.RecipientId.ToString()).ReceiveMessage(message);
            //await Clients.All.ReceiveMessage(message);
        }
    }
}