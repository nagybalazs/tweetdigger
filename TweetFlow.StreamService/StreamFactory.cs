using TweetFlow.MemoryStore;
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
            return this.CreateTrackedStream("#bitcoin", "#ripple", "#litecoin", "#ethereum");
        }

        private SampleStream CreateTrackedStream(params string[] track)
        {
            this.Running = true;

            var oq = new OrderedQueue();

            var ct = new SampleStream(credentials, oq)
                .AddLanguage(Language.English)
                .AddTracks(track)
                .AddQueryParameter("result_type", "recent");

            return ct;
        }
    }
}
