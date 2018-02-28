using System.Collections.Generic;
using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.Model;
using TweetFlow.Providers;

namespace TweetFlow.Services
{
    public class TweetService
    {
        private TWTweetProvider tWTweetProvider;
        public TweetService(TWTweetProvider tWTweetProvider)
        {
            this.tWTweetProvider = tWTweetProvider;
        }

        public void SaveChachedTweets(IEnumerable<Tweet> tweets)
        {
            var mapped = tweets.Select(tweet => new TWTweet
            {
                CreatedAt = tweet.CreatedAt,
                CelebrityHighlighted = tweet.CelebrityHighlighted,
                ConvertedToOriginal = tweet.ConvertedToOriginal,
                FavoriteCount = tweet.FavoriteCount,
                Favorited = tweet.Favorited,
                FullText = tweet.FullText,
                IsRetweet = tweet.IsRetweet,
                QuoteCount = tweet.QuoteCount,
                ReplyCount = tweet.ReplyCount,
                RetweetCount = tweet.ReplyCount,
                RetweetHighlighted = tweet.RetweetHighlighted,
                StrId = tweet.StrId,
                Type = tweet.Type,
                UserCreatedAt = tweet.User.CreatedAt,
                UserFavouritesCount = tweet.User.FavouritesCount,
                UserFollowersCount = tweet.User.FollowersCount,
                UserFriendsCount = tweet.User.FriendsCount,
                UserId = tweet.User.Id,
                UserListedCount = tweet.User.ListedCount,
                UserName = tweet.User.Name,
                UserProfileImageUrl400x400 = tweet.User.ProfileImageUrl400x400,
                UserScreenName = tweet.User.ScreenName,
                UserStatusesCount = tweet.User.StatusesCount,
                UserStrId = tweet.User.StrId,
                UserVerified = tweet.User.Verified
            });

            this.tWTweetProvider.SaveCachedTweets(mapped);
        }

        public IEnumerable<Tweet> GetCachedTweets()
        {
            var dbResult = this.tWTweetProvider.GetCachedTweets();
            var mapped = dbResult.Select(tweet => new Tweet
            {
                CreatedAt = tweet.CreatedAt,
                CelebrityHighlighted = tweet.CelebrityHighlighted,
                ConvertedToOriginal = tweet.ConvertedToOriginal,
                FavoriteCount = tweet.FavoriteCount,
                Favorited = tweet.Favorited,
                FullText = tweet.FullText,
                IsRetweet = tweet.IsRetweet,
                QuoteCount = tweet.QuoteCount,
                ReplyCount = tweet.ReplyCount,
                RetweetCount = tweet.RetweetCount,
                RetweetHighlighted = tweet.RetweetHighlighted,
                StrId = tweet.StrId,
                Type = tweet.Type,
                User = new User
                {
                    CreatedAt = tweet.UserCreatedAt,
                    FavouritesCount = tweet.UserFavouritesCount,
                    FollowersCount = tweet.UserFollowersCount,
                    FriendsCount = tweet.UserFriendsCount,
                    Id = tweet.UserId,
                    ListedCount = tweet.UserListedCount,
                    Name = tweet.UserName,
                    ProfileImageUrl400x400 = tweet.UserProfileImageUrl400x400,
                    ScreenName = tweet.UserScreenName,
                    StatusesCount = tweet.UserStatusesCount,
                    StrId = tweet.StrId,
                    Verified = tweet.UserVerified
                }
            });
            return mapped;
        }
    }
}
