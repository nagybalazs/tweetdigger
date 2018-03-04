using System;

namespace TweetFlow.Stream
{
    public static class Extensions
    {
        public static string AsStr(this Tweetinvi.Models.StreamState state)
        {
            return Enum.GetName(typeof(Tweetinvi.Models.StreamState), state);
        }
    }
}
