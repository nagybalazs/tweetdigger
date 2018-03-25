using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Linq;
using TweetFlow.Model;
using TweetFlow.Stream.Hubs;

namespace TweetFlow.Stream.Factory
{
    public class ChannelFactory
    {
        private IHubContext<BaseHub> hub;

        private List<ContextChannel> contextChannels;

        public List<string> ChannelNames
        {
            get
            {
                return new List<string>
                {
                    "bitcoin", "ethereum", "litecoin", "ripple"
                };
            }
        }

        public ChannelFactory(IHubContext<BaseHub> hub)
        {
            this.hub = hub;
        }

        public void InvokeSend(Tweet tweet)
        {
            this.hub.Clients.Group(tweet.Type).SendAsync("send", tweet);
        }
    }
}
