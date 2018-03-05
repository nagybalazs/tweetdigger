using TweetFlow.MemoryStore;
using TweetFlow.Stream;
using TweetFlow.Services;
using Microsoft.Extensions.Logging;

namespace TweetFlow.StreamService
{
    public class StreamFactory
    {
        private ICredentials credentials;
        private TweetScoreCalculator tweetScoreCalculator;
        private SampleStream stream;
        private OrderedQueue orderedQueue;
        private TweetService tweetService;
        private ILogger<SampleStream> logger;

        public bool Subsribed { get; set; }

        public StreamFactory(ICredentials credentials, TweetService tweetService, TweetScoreCalculator tweetScoreCalculator, ILogger<SampleStream> logger)
        {
            this.credentials = credentials;
            this.tweetScoreCalculator = tweetScoreCalculator;
            this.tweetService = tweetService;
            this.logger = logger;
        }

        public SampleStream GetStream()
        {
            return this.CreateTrackedStream("#bitcoin", "#ripple", "#litecoin", "#ethereum");
        }

        public SampleStream Start()
        {
            var stream = this.GetStream();
            if (!stream.IsStarted)
            {
                stream.StartAsync();
            }
            return stream;
        }

        private SampleStream CreateTrackedStream(params string[] track)
        {
            if (this.stream == null)
            {
                this.orderedQueue = new OrderedQueue().SetCache(this.tweetService);

                this.stream =
                    new SampleStream(this.credentials, this.orderedQueue, this.tweetScoreCalculator, logger)
                        .AddLanguage(Language.English)
                        .AddTracks(track)
                        .AddQueryParameter("result_type", "recent");

            }
            return this.stream;
        }
    }
}
