using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TweetFlow.Model;
using TweetFlow.StreamService;

namespace TweetFlow.Portal.Controllers
{
    [Route("tweet")]
    public class TweetController : Controller
    {
        private StreamFactory streamFactory;
        public TweetController(StreamFactory streamFactory)
        {
            this.streamFactory = streamFactory;
        }

        [Route("cachedtweets")]
        public IActionResult CachedTweets()
        {
            var result = this.streamFactory.GetStream().Queue.CachedItems;
            return Ok(result);
        }
    }
}
