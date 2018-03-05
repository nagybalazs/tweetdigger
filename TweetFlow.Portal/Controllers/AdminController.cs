using TweetFlow.Portal.Model;
using TweetFlow.Stream.Watch;
using Microsoft.AspNetCore.Mvc;
using TweetFlow.Stream.Factory;
using Microsoft.Extensions.Logging;

namespace TweetFlow.Portal.Controllers
{
    //[Authorize]
    public class AdminController : Controller
    {
        private StreamFactory streamFactory;
        private StreamWatch streamWatch;
        private ILogger<AdminController> logger;
        private ChannelFactory channelFactory;

        public AdminController(
            ChannelFactory channelFactory,
            StreamFactory streamFactory, 
            StreamWatch streamWatch,  
            ILogger<AdminController> logger)
        {
            this.channelFactory = channelFactory;
            this.logger = logger;
            this.streamFactory = streamFactory;
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

                stream.Queue.ContentAdded += (sender, tweet) =>
                {
                    this.channelFactory.InvokeSend(tweet);
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
    }
}
