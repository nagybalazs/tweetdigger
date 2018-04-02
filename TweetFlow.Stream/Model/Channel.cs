using System;
using System.Collections.Generic;
using System.Linq;
using TweetFlow.Model;

namespace TweetFlow.Stream
{
    public class Channel
    {
        public string Name { get; set; }
        public string Endpoint { get; set; }
        public List<Tweet> Tweets { get; set; }

        public Channel()
        {
            this.Tweets = new List<Tweet>();
        }
    }
}
