using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using TweetFlow.Model;
using TweetFlow.Model.Hubs;

namespace TweetFlow.StreamService
{
    public class ChannelFactory
    {
        private IHubContext<BitCoinHub> bitcoinHub;
        private IHubContext<EthereumHub> ethereumHub;
        private IHubContext<LiteCoinHub> litecoinHub;
        private IHubContext<RippleHub> rippleHub;

        private List<Channel> channels;

        public List<Channel> Channels
        {
            get
            {
                return this.contextChannels
                    .Select(channel => new Channel
                    {
                        Name = channel.Name,
                        HubType = channel.HubType
                    })
                    .ToList();
            }
        }

        private List<ContextChannel> contextChannels;

        public List<string> ChannelNames
        {
            get
            {
                return this.Channels.Select(channel => channel.Name).ToList();
            }
        }

        public ChannelFactory(
            IHubContext<BitCoinHub> bitcoinHub,
            IHubContext<EthereumHub> ethereumHub,
            IHubContext<LiteCoinHub> litecoinHub,
            IHubContext<RippleHub> rippleHub
            )
        {
            this.bitcoinHub = bitcoinHub;
            this.ethereumHub = ethereumHub;
            this.litecoinHub = litecoinHub;
            this.rippleHub = rippleHub;
            this.PopulateContextChannel();
        }

        private void PopulateContextChannel()
        {
            this.contextChannels = new List<ContextChannel>
            {
                new ContextChannel { Name = "bitcoin", HubType = typeof(BitCoinHub), HubContext = this.bitcoinHub },
                new ContextChannel { Name = "ethereum", HubType = typeof(EthereumHub), HubContext = this.ethereumHub },
                new ContextChannel { Name = "litecoin", HubType = typeof(LiteCoinHub), HubContext = this.litecoinHub },
                new ContextChannel { Name = "ripple", HubType = typeof(RippleHub), HubContext = this.rippleHub }
            };
        }

        public void RegisterHubs(HubRouteBuilder hubRouteBuilder)
        {
            foreach (var channel in this.Channels)
            {
                typeof(HubRouteBuilder)
                    .GetMethod("MapHub", new[] { typeof(string) })
                    .MakeGenericMethod(channel.HubType)
                    .Invoke(hubRouteBuilder, new[] { $"/{channel.Name}" });
            }
        }

        public void InvokeSend(Tweet tweet)
        {
            var channel = this.contextChannels.SingleOrDefault(contextChannel => contextChannel.Name == tweet.Type);
            if(channel == null)
            {
                return;
            }
            channel.HubContext.Clients.All.SendAsync("send", tweet);
        }
    }
}
