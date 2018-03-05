using System;
using System.Collections.Generic;
using System.Linq;
using TweetFlow.MemoryStore;

namespace TweetFlow.Stream
{
    public static class Extensions
    {
        public static string AsStr(this Tweetinvi.Models.StreamState state)
        {
            return Enum.GetName(typeof(Tweetinvi.Models.StreamState), state);
        }

        public static IEnumerable<string> ExtractHashtags(this Tweetinvi.Models.ITweet tweet)
        {
            if (tweet == null)
            {
                return Enumerable.Empty<string>();
            }
            return tweet.Hashtags.Select(hashtag => hashtag.Text);
        }

        public static IEnumerable<string> ExtractHashTagsAsQueueType(this Tweetinvi.Models.ITweet tweet)
        {
            return tweet.Hashtags.Select(hashtag => hashtag.Text.Replace("#", string.Empty).ToLower());
        }

        public static bool IsInvalidRetweet(this Tweetinvi.Models.ITweet tweet)
        {
            if (!tweet.IsRetweet)
            {
                return false;
            }
            
            if(tweet.RetweetedTweet == null || tweet.RetweetedTweet.CreatedAt < DateTime.UtcNow.AddDays(-1))
            {
                return true;
            }
            return false;
        }

        public static Model.Tweet ToStreamTweet(this Tweetinvi.Models.ITweet tweet)
        {
            var convertedToOriginal = false;
            var createdAt = tweet.CreatedAt;
            if (tweet.IsRetweet)
            {
                tweet = tweet.RetweetedTweet;
                convertedToOriginal = true;
            }

            var streamTweet = new Model.Tweet
            {
                StrId = tweet.IdStr,
                FullText = tweet.FullText,
                CreatedAt = createdAt,
                FavoriteCount = tweet.FavoriteCount,
                Favorited = tweet.Favorited,
                QuoteCount = 0,
                ReplyCount = 0,
                RetweetCount = tweet.RetweetCount,
                IsRetweet = tweet.IsRetweet,
                Hashtags = tweet.ExtractHashtags().ToList(),
                ConvertedToOriginal = convertedToOriginal,
                UserMentionsCount = tweet.UserMentions.Count,
                User = new Model.User
                {
                    StrId = tweet.CreatedBy.IdStr,
                    CreatedAt = tweet.CreatedBy.CreatedAt,
                    FavouritesCount = tweet.CreatedBy.FavouritesCount,
                    FollowersCount = tweet.CreatedBy.FollowersCount,
                    FriendsCount = tweet.CreatedBy.FriendsCount,
                    ListedCount = tweet.CreatedBy.ListedCount,
                    StatusesCount = tweet.CreatedBy.StatusesCount,
                    Verified = tweet.CreatedBy.Verified,
                    ProfileImageUrl400x400 = tweet.CreatedBy.ProfileImageUrl400x400,
                    ScreenName = tweet.CreatedBy.ScreenName,
                    Name = tweet.CreatedBy.Name,
                    Id = tweet.CreatedBy.Id
                }
            };

            return streamTweet;
        }

        public static ScoredItem ToScoredItem(this Tweetinvi.Models.ITweet tweet)
        {
            var streamTweet = tweet.ToStreamTweet();
            return new ScoredItem(streamTweet);
        }
    }
}
