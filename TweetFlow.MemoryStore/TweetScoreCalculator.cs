using System.Collections.Generic;
using TweetFlow.Model;
using TweetFlow.Providers;

namespace TweetFlow.MemoryStore
{
    public class TweetScoreCalculator //: IScoredCalculator<int, Tweet>
    {
        private TWUserProvider provider;
        private int score;
        private Tweet tweet;

        public TweetScoreCalculator(TWUserProvider provider)
        {
            this.score = 0;
            this.provider = provider;
        }

        private TweetScoreCalculator CalculateInitialScore()
        {
            this.score =
                (this.tweet.RetweetCount * 5) +
                (this.tweet.FavoriteCount * 5) +
                (this.tweet.ReplyCount * 5) +
                (this.tweet.User.FollowersCount * 5) +
                (this.tweet.User.FriendsCount * 5) +
                (this.tweet.User.ListedCount * 5) +
                (
                    (this.tweet.User.FavouritesCount) /
                    (this.tweet.User.StatusesCount) * 10
                );

            if (this.tweet.User.Verified)
            {
                this.score = this.score * 2;
            }

            return this;
        }

        private TweetScoreCalculator CalculateWordAndHashtagBann()
        {
            var increaseWordBannCountWith = 0;
            var increaseHashtagBannCountWith = 0;

            var hashtagCount = this.tweet.Hashtags.Count;
            if (hashtagCount > 1 && hashtagCount <= 3)
            {
                increaseHashtagBannCountWith = 1;
            }
            else if (hashtagCount > 3 && hashtagCount <= 5)
            {
                increaseHashtagBannCountWith = 2;
                this.score = 0;
            }
            else if (hashtagCount > 5 && hashtagCount <= 9)
            {
                increaseHashtagBannCountWith = 3;
            }
            else if (hashtagCount > 9)
            {
                increaseHashtagBannCountWith = 5;
            }

            var unallowedTags =
                new List<string> { "signup", "token", "project", "sale", "free", "grabyour", "win", "join", "grab your", "easymoney", "freebitcoin", "passiveincome", ":arrow_left:", ":tada:", ":red_circle", "join", "joining", "register", "registering", "don'tmiss", "dontmiss", "click", ":fire:", ":rocket:", "comingsoon", "promotion", "promotions", ":heavy_check_mark:", "youcan", "contribute", "deposit", ":zap:", "pre-sale", "sale", "bonus", "initialcoinoffering", "presale", "pre-sale", "earnbitcoin", "giveaway" };

            var trimmed = this.tweet.FullText.Replace(" ", string.Empty).ToLower();

            var bannedWordCounter = 0;
            foreach (var word in unallowedTags)
            {
                if (trimmed.Contains(word))
                {
                    bannedWordCounter += 1;
                    this.score = 0;
                }
            }

            if (bannedWordCounter > 0)
            {
                if (bannedWordCounter > 0 && bannedWordCounter <= 3)
                {
                    increaseWordBannCountWith = 1;
                }
                else if (bannedWordCounter > 3 && bannedWordCounter <= 5)
                {
                    increaseWordBannCountWith = 2;
                }
                else if (bannedWordCounter > 5 && bannedWordCounter <= 9)
                {
                    increaseWordBannCountWith = 3;
                }
                else if (bannedWordCounter > 9)
                {
                    increaseWordBannCountWith = 5;
                }
            }

            var actualPenalty = 1;
            if (increaseHashtagBannCountWith > 0 || increaseWordBannCountWith > 0)
            {
                var penalty = this.provider.IncreaseWordAndHashtagBannCountAndPenaltyCount(tweet.User.Id, increaseWordBannCountWith, increaseHashtagBannCountWith);
                actualPenalty = penalty.HashtagBannPenalty + penalty.WordBannPenalty;
            }

            if (actualPenalty > 0)
            {
                this.score = score / actualPenalty;
            }

            return this;

        }

        public int CalculateScore(Tweet tweet)
        {
            this.tweet = tweet;

            this
                .CalculateInitialScore()
                .CalculateWordAndHashtagBann();

            return this.score;
        }
    }
}
