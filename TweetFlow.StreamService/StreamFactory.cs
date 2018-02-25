using TweetFlow.MemoryStore;
using TweetFlow.Stream;
using TweetFlow.Model;
using TweetFlow.Providers;

namespace TweetFlow.StreamService
{
    public class StreamFactory
    {
        private ICredentials credentials;
        private TweetScoreCalculator tweetScoreCalculator;
        private SampleStream stream;
        private OrderedQueue orderedQueue;
        private TWStreamInfoProvider tWStreamInfoProvider;

        public bool Subsribed { get; set; }

        public StreamFactory(ICredentials credentials, TweetScoreCalculator tweetScoreCalculator, TWStreamInfoProvider tWStreamInfoProvider)
        {
            this.credentials = credentials;
            this.tweetScoreCalculator = tweetScoreCalculator;
            this.tWStreamInfoProvider = tWStreamInfoProvider;
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
                stream.Rekt += (a, b) =>
                {
                    // újraindítás, de hogy? :(
                };
                stream.StartAsync();
            }
            return stream;
        }

        private SampleStream CreateTrackedStream(params string[] track)
        {
            if (this.stream == null)
            {
                this.orderedQueue = new OrderedQueue();

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
