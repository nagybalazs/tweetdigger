using System;
using System.Collections.Generic;

namespace TweetFlow.Model
{
    public class Tweet
    {
        public string Type { get; set; }
        public string StrId { get; set; }
        public string FullText { get; set; }
        /// <summary>
        /// UTC time when this Tweet was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Indicates approximately how many times this Tweet has been quoted by Twitter users.
        /// </summary>
        public int QuoteCount { get; set; }
        /// <summary>
        /// Number of times this Tweet has been replied to.
        /// </summary>
        public int ReplyCount { get; set; }
        /// <summary>
        /// Number of times this Tweet has been retweeted.
        /// </summary>
        public int RetweetCount { get; set; }
        /// <summary>
        /// Number of times this Tweet has been retweeted.
        /// </summary>
        public int FavoriteCount { get; set; }
        /// <summary>
        /// Indicates whether this Tweet has been liked by the authenticating user.
        /// </summary>
        public bool Favorited { get; set; }
        /// <summary>
        /// Indicates whether this Tweet has been Retweeted by the authenticating user.
        /// </summary>
        public bool IsRetweet { get; set; }

        public User User { get; set; }
        public ICollection<string> Hashtags { get; set; }
        public bool Celebrity { get; set; }
        public bool ConvertedToOriginal { get; set; }
    }
}
