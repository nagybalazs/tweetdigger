using TweetFlow.MemoryStore;
using TweetFlow.Stream;
using TweetFlow.Services;
using TweetFlow.Model;

namespace TweetFlow.StreamService
{
    public class StreamFactory
    {
        public bool Running { get; set; }
        private ICredentials credentials;
        private IScoredCalculator<int, Tweet> tweetScoreCalculator;
        
        public StreamFactory(ICredentials credentials, IScoredCalculator<int, Tweet> tweetScoreCalculator)
        {
            this.credentials = credentials;
            this.tweetScoreCalculator = tweetScoreCalculator;
        }

        public SampleStream Bitcoin()
        {
            return this.CreateTrackedStream("#bitcoin", "#ripple", "#litecoin", "#ethereum");
        }

        private SampleStream CreateTrackedStream(params string[] track)
        {
            this.Running = true;

            var oq = new OrderedQueue();

            var ct = new SampleStream(credentials, oq, tweetScoreCalculator)
                .AddLanguage(Language.English)
                .AddTracks(track)
                .AddQueryParameter("result_type", "recent");

            return ct;
        }
    }
}
