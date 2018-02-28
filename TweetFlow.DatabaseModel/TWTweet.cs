using System;

namespace TweetFlow.DatabaseModel
{
    public class TWTweet
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string StrId { get; set; }
        public string FullText { get; set; }
        public DateTime CreatedAt { get; set; }
        public int QuoteCount { get; set; }
        public int ReplyCount { get; set; }
        public int RetweetCount { get; set; }
        public int FavoriteCount { get; set; }
        public bool Favorited { get; set; }
        public bool IsRetweet { get; set; }
        public bool CelebrityHighlighted { get; set; }
        public bool RetweetHighlighted { get; set; }
        public bool ConvertedToOriginal { get; set; }
        public bool UserVerified { get; set; }
        public int UserFollowersCount { get; set; }
        public int UserFriendsCount { get; set; }
        public int UserListedCount { get; set; }
        public int UserFavouritesCount { get; set; }
        public int UserStatusesCount { get; set; }
        public DateTime UserCreatedAt { get; set; }
        public string UserProfileImageUrl400x400 { get; set; }
        public string UserName { get; set; }
        public string UserScreenName { get; set; }
        public long UserId { get; set; }
        public string UserStrId { get; set; }
    }
}
