using TweetFlow.MemoryStore;
using TweetFlow.Model;
using TweetFlow.Stream;

namespace TweetFlow.StreamService
{
    public class StreamFactory
    {
        public bool Running { get; set; }
        private ICredentials credentials;
        
        public StreamFactory(ICredentials credentials)
        {
            this.credentials = credentials;
        }

        public SampleStream Bitcoin()
        {
            return this.CreateTrackedStream("#bitcoin");
        }

        public SampleStream Ethereum()
        {
            return this.CreateTrackedStream("#ethereum");
        }

        public SampleStream Ripple()
        {
            return this.CreateTrackedStream("#ripple");
        }

        public SampleStream Lite()
        {
            return this.CreateTrackedStream("#litecoin");
        }

        private SampleStream CreateTrackedStream(string track)
        {
            this.Running = true;

            var oq = new OrderedQueue();

            var ct = new SampleStream(credentials, oq)
                .AddLanguage(Language.English)
                .AddTrack(track)
                .AddQueryParameter("result_type", "recent");

            return ct;
        }
    }
}
