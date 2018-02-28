using TweetFlow.MemoryStore;
using TweetFlow.Stream;
using TweetFlow.Model;
using TweetFlow.Providers;
using TweetFlow.Services;

namespace TweetFlow.StreamService
{
    public class StreamFactory
    {
        private ICredentials credentials;
        private TweetScoreCalculator tweetScoreCalculator;
        private SampleStream stream;
        private OrderedQueue orderedQueue;
        private TWStreamInfoProvider tWStreamInfoProvider;
        private TweetService tweetService;

        public bool Subsribed { get; set; }

        public StreamFactory(ICredentials credentials, TweetService tweetService, TweetScoreCalculator tweetScoreCalculator, TWStreamInfoProvider tWStreamInfoProvider)
        {
            this.credentials = credentials;
            this.tweetScoreCalculator = tweetScoreCalculator;
            this.tWStreamInfoProvider = tWStreamInfoProvider;
            this.tweetService = tweetService;
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
                    new SampleStream(this.credentials, this.orderedQueue, this.tweetScoreCalculator, this.tWStreamInfoProvider)
                        .AddLanguage(Language.English)
                        .AddTracks(track)
                        .AddQueryParameter("result_type", "recent");

            }
            return this.stream;
        }
    }
}
