using Microsoft.AspNetCore.SignalR;
using TweetFlow.Model.Hubs;

namespace TweetFlow.Model
{
    public class HubContextFactory
    {
        private IHubContext<BitCoinHub> bitCoinHub;
        private IHubContext<EthereumHub> ethereumHub;
        private IHubContext<RippleHub> rippleHub;
        private IHubContext<LiteCoinHub> liteCoinHub;

        public HubContextFactory(
            IHubContext<BitCoinHub> bitCoinHub,
            IHubContext<EthereumHub> ethereumHub,
            IHubContext<RippleHub> rippleHub,
            IHubContext<LiteCoinHub> liteCoinHub
            )
        {
            this.bitCoinHub = bitCoinHub;
            this.ethereumHub = ethereumHub;
            this.rippleHub = rippleHub;
            this.liteCoinHub = liteCoinHub;
        }

        public IHubContext<BitCoinHub> CreateBitCoinHub()
        {
            return this.bitCoinHub;
        }

        public IHubContext<EthereumHub> CreateEthereumHub()
        {
            return this.ethereumHub;
        }

        public IHubContext<RippleHub> CreateRippleHub()
        {
            return this.rippleHub;
        }

        public IHubContext<LiteCoinHub> CreateLiteCoinHub()
        {
            return this.liteCoinHub;
        }
    }
}
