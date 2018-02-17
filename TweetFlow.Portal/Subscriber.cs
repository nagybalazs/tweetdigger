using Microsoft.AspNetCore.SignalR;
using System.Threading;
using TweetFlow.Model;
using TweetFlow.Model.Hubs;
using TweetFlow.Stream;
using TweetFlow.StreamService;

namespace TweetFlow.Portal
{
    public class Subscriber
    {
        private StreamFactory streamFactory;
        private IHubContext<BitCoinHub> bitCoinHubContext;
        private IHubContext<RippleHub> rippleHub;
        private IHubContext<EthereumHub> ethereumHub;
        private IHubContext<LiteCoinHub> liteCoinHub;

        public Subscriber(
            StreamFactory streamFactory, 
            IHubContext<BitCoinHub> bitCoinHubContext,
            IHubContext<RippleHub> rippleHub,
            IHubContext<EthereumHub> ethereumHub,
            IHubContext<LiteCoinHub> liteCoinHub)
        {
            this.streamFactory = streamFactory;
            this.bitCoinHubContext = bitCoinHubContext;
            this.rippleHub = rippleHub;
            this.ethereumHub = ethereumHub;
            this.liteCoinHub = liteCoinHub;
        }

        public void Bootstrap()
        {
            if (!this.streamFactory.Running)
            {
                var stream = this.streamFactory.Bitcoin();
                stream.Queue.ContentAdded += (sender, tweet) =>
                {
                    switch (tweet.Type)
                    {
                        case TweetType.Bitcoin:
                            {
                                this.InvokeSend(this.bitCoinHubContext, tweet);
                                break;
                            }
                        case TweetType.Ethereum:
                            {
                                this.InvokeSend(this.ethereumHub, tweet);
                                break;
                            }
                        case TweetType.LiteCoin:
                            {
                                this.InvokeSend(this.liteCoinHub, tweet);
                                break;
                            }
                        case TweetType.Ripple:
                            {
                                this.InvokeSend(this.rippleHub, tweet);
                                break;
                            }
                    }
                };
                stream.StartAsync();
            }
        }

        private void InvokeSend<THub>(IHubContext<THub> hubContext, Tweet tweet) where THub : Hub
        {
            hubContext.Clients.All.InvokeAsync("send", tweet);
        }

    }
}
