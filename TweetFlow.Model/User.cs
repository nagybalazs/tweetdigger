using System;

namespace TweetFlow.Model
{
    public class User
    {
        /// <summary>
        /// When true, indicates that the user has a verified account. An account may be verified if it is determined to be an account of public interest. Typically this includes accounts maintained by users in music, acting, etc.
        /// </summary>
        public bool Verified { get; set; }
        /// <summary>
        /// The number of followers this account currently has.
        /// </summary>
        public int FollowersCount { get; set; }
        /// <summary>
        /// The number of users this account is following.
        /// </summary>
        public int FriendsCount { get; set; }
        /// <summary>
        /// The number of public lists that this user is a member of.
        /// </summary>
        public int ListedCount { get; set; }
        /// <summary>
        /// The number of Tweets this user has liked in the account’s lifetime.
        /// </summary>
        public int FavouritesCount { get; set; }
        /// <summary>
        /// The number of Tweets (including retweets) issued by the user.
        /// </summary>
        public int StatusesCount { get; set; }
        /// <summary>
        /// The UTC datetime that the user account was created.
        /// </summary>
        public DateTime CreatedAt { get; set; }
        public string ProfileImageUrl400x400 { get; set; }
        public string Name { get; set; }
        public string ScreenName { get; set; }
    }
}
