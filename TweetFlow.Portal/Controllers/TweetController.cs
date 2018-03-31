using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TweetFlow.Stream.Factory;

namespace TweetFlow.Portal.Controllers
{
    [Route("api/tweet")]
    public class TweetController : Controller
    {
        private StreamFactory streamFactory;
        private ChannelFactory channelFactory;
        public TweetController(StreamFactory streamFactory, ChannelFactory channelFactory)
        {
            this.streamFactory = streamFactory;
            this.channelFactory = channelFactory;
        }

        [Route("cachedtweets")]
        public IActionResult CachedTweets([FromQuery] string channel)
        {
            var result =
                this.streamFactory
                    .GetStream()
                    .Queue
                    .CachedItems
                    .Where(cachedItem => cachedItem.Type == channel)
                    .OrderByDescending(d => d.CreatedAt);

            return Ok(result);
        }

        [Route("channels")]
        public IActionResult Channels()
        {
            var result = 
                this.channelFactory.ChannelNames;

            return Ok(result);
        }
    }
}
