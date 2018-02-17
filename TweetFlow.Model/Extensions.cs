using System;

namespace TweetFlow.Model
{
    public static class Extensions
    {
        public static string AsLoweredString(TweetType tweetType)
        {
            return Enum.GetName(typeof(TweetType), tweetType).ToLower();
        }

        public static string RemoveHashtag(this string str)
        {
            return str.Replace("#", string.Empty);
        }

        public static bool IsMatchToType(this string str, TweetType tweetType)
        {
            return str.RemoveHashtag() == AsLoweredString(tweetType);
        }
    }
}
