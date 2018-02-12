using Microsoft.AspNetCore.SignalR;
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
        private IHubContext<EthereumHub> ethereumHubContext;
        private IHubContext<RippleHub> rippleHubContext;
        private IHubContext<LiteCoinHub> liteCoinHubContext;

        public Subscriber(
            StreamFactory streamFactory, 
            IHubContext<BitCoinHub> bitCoinHubContext, 
            IHubContext<EthereumHub> ethereumHubContext,
            IHubContext<RippleHub> rippleHubContext,
            IHubContext<LiteCoinHub> liteCoinHubContext)
        {
            this.streamFactory = streamFactory;
            this.bitCoinHubContext = bitCoinHubContext;
            this.ethereumHubContext = ethereumHubContext;
            this.liteCoinHubContext = liteCoinHubContext;
            this.rippleHubContext = rippleHubContext;
        }

        public void Bootstrap()
        {
            if (!this.streamFactory.Running)
            {
                this.StartStream(this.streamFactory.Bitcoin(), this.bitCoinHubContext);
                this.StartStream(this.streamFactory.Ethereum(), this.ethereumHubContext);
                //this.StartStream(this.streamFactory.Ripple(), this.rippleHubContext);
                //this.StartStream(this.streamFactory.Lite(), this.liteCoinHubContext);
            }
                //var coinStream = this.streamFactory.Bitcoin();
                //coinStream.Queue.ContentAdded += (a, b) =>
                //{
                //    bitCoinHubContext.Clients.All.InvokeAsync("send", b);
                //};
                //coinStream.StartAsync();

                //var ethereumStream = this.streamFactory.Ethereum();
                //ethereumStream.Queue.ContentAdded += (a, b) =>
                //{
                //    ethereumHubContext.Clients.All.InvokeAsync("send", b);
                //};
                //ethereumStream.StartAsync();

                //var liteCoinStream = this.streamFactory.Lite();
                //liteCoinStream.Queue.ContentAdded += (a, b) =>
                //{
                //    liteCoinHubContext.Clients.All.InvokeAsync("send", b);
                //};
        }

        private void StartStream<THub>(SampleStream stream, IHubContext<THub> hubContext) where THub : Hub
        {
            stream.Queue.ContentAdded += (a, b) =>
            {
                hubContext.Clients.All.InvokeAsync("send", b);
            };
            stream.StartAsync();
        }

    }
}
