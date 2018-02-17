using System;

namespace TweetFlow.Model
{
    public static class Extensions
    {
        public static string AsLoweredString(TweetType tweetType)
        {
            return Enum.GetName(typeof(TweetType), tweetType).ToLower();
        }

        public static string LowerAndRemoveHashtag(this string str)
        {
            return str.Replace("#", string.Empty).ToLower();
        }

        public static bool IsMatchToType(this string str, TweetType tweetType)
        {
            return str.LowerAndRemoveHashtag() == AsLoweredString(tweetType);
        }
    }
}
