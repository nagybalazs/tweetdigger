using System.Collections.Generic;
using System.Linq;
using TweetFlow.MemoryStore;
using TweetFlow.Model;
using TweetFlow.Providers;

namespace TweetFlow.Services
{
    public class TweetScoreCalculator : IScoredCalculator<int, Tweet>
    {
        private TWUserProvider provider;
        public TweetScoreCalculator(TWUserProvider provider)
        {
            this.provider = provider;
        }

        public int CalculateScore(Tweet tweet)
        {
            var score =
                (tweet.RetweetCount * 5) +
                (tweet.FavoriteCount * 5) +
                (tweet.ReplyCount * 5) +
                (tweet.User.FollowersCount * 5) +
                (tweet.User.FriendsCount * 5) +
                (tweet.User.ListedCount * 5) +
                (
                    (tweet.User.FavouritesCount) /
                    (tweet.User.StatusesCount) * 10
                );

            var increaseWordBannCount = 0;
            var increaseHashBannCount = 0;

            if (tweet.Hashtags.Count > 1)
            {
                increaseWordBannCount = 1;
                score = 0;
            }

            var trimmed = tweet.FullText.Replace(" ", string.Empty);

            // TODO: finomítani, mert így szar
            var unallowedTags = new List<string> { "signup", "token", "project", "sale", "free", "grab your", "win", "join", "grab your", "easy money" };
            if (unallowedTags.Any(w => tweet.FullText.ToLower().Contains(w)))
            {
                increaseWordBannCount = 1;
                score = 0;
            }

            if(increaseHashBannCount > 0 || increaseWordBannCount > 0)
            {
                this.provider.IncreaseWordAndHashtagBannCount(tweet.User.Id, increaseWordBannCount, increaseWordBannCount);
            }

            if (tweet.User.Verified)
            {
                score = score * 2;
            }

            return score;
        }
    }
}
