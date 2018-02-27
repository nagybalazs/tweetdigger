using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Diagnostics;
using TweetFlow.Model;
using TweetFlow.Model.Hubs;
using TweetFlow.Portal.Model;
using TweetFlow.Providers;
using TweetFlow.Stream;
using TweetFlow.StreamService;

namespace TweetFlow.Portal.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        private StreamFactory streamFactory;
        private StreamWatch streamWatch;
        private TWStreamInfoProvider tWStreamInfoProvider;
        private IHubContext<BitCoinHub> bitcoinHub;
        private IHubContext<EthereumHub> ethereumHub;
        private IHubContext<RippleHub> rippleHub;
        private IHubContext<LiteCoinHub> liteCoinHub;
        
        // HubDictionary? => külön osztály, object, tryGetValue-ban cast... Csehszlovák, de rövid, és kintről szép lehet

        public AdminController(StreamFactory streamFactory, StreamWatch streamWatch, TWStreamInfoProvider tWStreamInfoProvider, IHubContext<BitCoinHub> bitcoinHub, IHubContext<EthereumHub> ethereumHub, IHubContext<RippleHub> rippleHub, IHubContext<LiteCoinHub> liteCoinHub)
        { 
            this.streamFactory = streamFactory;
            this.bitcoinHub = bitcoinHub;
            this.ethereumHub = ethereumHub;
            this.rippleHub = rippleHub;
            this.liteCoinHub = liteCoinHub;
            this.tWStreamInfoProvider = tWStreamInfoProvider;
            this.streamWatch = streamWatch;
        }
        public IActionResult Index()
        {
            var model = new AdminModel();
            model.StreamInfo = this.tWStreamInfoProvider.GetLast();
            model.State = this.streamFactory.GetStream().CurrentState;
            return View(model);
        }

        public IActionResult StartStream()
        {

            var stream = this.streamFactory.Start();
            if (!this.streamFactory.Subsribed)
            {
                this.streamFactory.Subsribed = true;
                stream.Queue.ContentAdded += (a, b) =>
                {
                    this.SeparateTweet(b);
                };
                stream.Rekt += (a, b) =>
                {
                    if (!this.streamWatch.RestartInProgress)
                    {
                        this.streamWatch.Restart();
                        if (!this.streamWatch.Subscribed)
                        {
                            this.streamWatch.RestartNow += (c, d) =>
                            {
                                this.streamWatch.Subscribed = true;
                                if (stream.CurrentState != StreamState.Stopped)
                                {
                                    this.streamWatch.Kill();
                                }
                                else
                                {
                                    this.StartStream();
                                }
                            };
                        }
                    }
                };
            }
            return RedirectToAction("Index");
        }

        private void SeparateTweet(Tweet tweet)
        {
            switch (tweet.Type)
            {
                case "bitcoin":
                    {
                        this.InvokeSend(this.bitcoinHub, tweet);
                        break;
                    }
                case "ethereum":
                    {
                        this.InvokeSend(this.ethereumHub, tweet);
                        break;
                    }
                case "litecoin":
                    {
                        this.InvokeSend(this.liteCoinHub, tweet);
                        break;
                    };
                case "ripple":
                    {
                        this.InvokeSend(this.rippleHub, tweet);
                        break;
                    }
            }
        }

        private void InvokeSend<THub>(IHubContext<THub> hubContext, Tweet tweet) where THub : Hub
        {
            hubContext.Clients.All.InvokeAsync("send", tweet);
        }
    }
}
