using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TweetFlow.Stream.Hubs
{
    public class BaseHub : Hub
    { 
        public async Task Send(string data)
        {
            await Clients.All.SendAsync("send", data);
        }
    }
}
