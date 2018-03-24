using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TweetFlow.Stream.Factory;

namespace TweetFlow.Portal.Controllers
{
    [Route("api/tweet")]
    public class TweetController : Controller
    {
        private StreamFactory streamFactory;
        public TweetController(StreamFactory streamFactory)
        {
            this.streamFactory = streamFactory;
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
    }
}
