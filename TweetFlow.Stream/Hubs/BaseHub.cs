using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using TweetFlow.Stream.Factory;

namespace TweetFlow.Stream.Hubs
{
    public class BaseHub : Hub
    {
        private ChannelFactory channelFactory;
        public BaseHub(ChannelFactory channelFactory)
        {
            this.channelFactory = channelFactory;
        }

        public async Task SendAsync(string data)
        {
            await Clients.All.SendAsync("send", data);
        }

        public async Task JoinGroup(string group)
        {
            await this.Groups.AddAsync(this.Context.ConnectionId, group);
            await this.Clients.Group(group).SendAsync("JoinGroup", $"Joined: {group}");
        }

        public async Task LeaveGroup(string group)
        {
            await this.Clients.Group(group).SendAsync("LeaveGroup", $"Left: {group}");
            await this.Groups.RemoveAsync(this.Context.ConnectionId, group);
        }
    }
}
