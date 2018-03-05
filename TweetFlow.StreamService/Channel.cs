using System;

namespace TweetFlow.StreamService
{
    public class Channel
    {
        public string Name { get; set; }
        public Type HubType { get; set; }
    }

    internal class ContextChannel : Channel
    {
        public dynamic HubContext { get; set; }
    }
}
