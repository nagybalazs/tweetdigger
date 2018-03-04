using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
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
        private IHubContext<BitCoinHub> bitcoinHub;
        private IHubContext<EthereumHub> ethereumHub;
        private IHubContext<RippleHub> rippleHub;
        private IHubContext<LiteCoinHub> liteCoinHub;
        private ILogger<AdminController> logger;

        // HubDictionary? => külön osztály, object, tryGetValue-ban cast... Csehszlovák, de rövid, és kintről szép lehet

        public AdminController(
            StreamFactory streamFactory, 
            StreamWatch streamWatch,  
            IHubContext<BitCoinHub> bitcoinHub, 
            IHubContext<EthereumHub> ethereumHub, 
            IHubContext<RippleHub> rippleHub, 
            IHubContext<LiteCoinHub> liteCoinHub,
            ILogger<AdminController> logger)
        {
            this.logger = logger;
            this.streamFactory = streamFactory;
            this.bitcoinHub = bitcoinHub;
            this.ethereumHub = ethereumHub;
            this.rippleHub = rippleHub;
            this.liteCoinHub = liteCoinHub;
            this.streamWatch = streamWatch;
        }
        public IActionResult Index()
        {
            var model = new AdminModel();
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

                stream.Stopped += (stopSender, stoppedCorrelation) =>
                {
                    this.logger.LogError($"Restarting stream invoked.");

                    this.streamWatch.Start();

                    this.logger.LogError($"Restart is now in progress.");

                    if (!this.streamWatch.Subscribed)
                    {
                        this.streamWatch.Subscribed = true;
                        this.streamWatch.RestartNow += (a, b) =>
                        {
                            this.streamWatch.Stop();
                            this.logger.LogError($"Stream is now restarted");
                            stream.StartAsync();
                        };
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
