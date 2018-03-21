using Microsoft.Extensions.Logging;
using System.Linq;
using TweetFlow.MemoryStore;
using TweetFlow.Services;

namespace TweetFlow.Stream.Factory
{
    public class StreamFactory
    {
        private StreamCredentials credentials;
        private TweetScoreCalculator tweetScoreCalculator;
        private SampleStream stream;
        private OrderedQueue orderedQueue;
        private TweetService tweetService;
        private ILogger<SampleStream> logger;
        private ChannelFactory channelFactory;

        public bool Subsribed { get; set; }

        public StreamFactory(StreamCredentials credentials, TweetService tweetService, TweetScoreCalculator tweetScoreCalculator, ILogger<SampleStream> logger, ChannelFactory channelFactory)
        {
            this.credentials = credentials;
            this.tweetScoreCalculator = tweetScoreCalculator;
            this.tweetService = tweetService;
            this.logger = logger;
            this.channelFactory = channelFactory;
        }

        public SampleStream GetStream()
        {
            var tracks = this.channelFactory.ChannelNames.Select(channel => $"{channel}").ToArray();
            return this.CreateTrackedStream(tracks);
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
                    new SampleStream(this.channelFactory, this.credentials, this.orderedQueue, this.tweetScoreCalculator, this.logger)
                        .AddLanguage(Language.English)
                        .AddTracks(track)
                        .AddQueryParameter("result_type", "recent");

            }
            return this.stream;
        }
    }
}
