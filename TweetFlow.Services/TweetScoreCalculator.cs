﻿using System.Collections.Generic;
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

            var hashtagCount = tweet.Hashtags.Count;
            if (hashtagCount > 1 && hashtagCount <= 3)
            {
                increaseHashBannCount = 1;
            }
            else if(hashtagCount > 3 && hashtagCount <= 5)
            {
                increaseHashBannCount = 2;
            }
            else if(hashtagCount > 5 && hashtagCount <= 9)
            {
                increaseHashBannCount = 3;
            }
            else if(hashtagCount > 9)
            {
                increaseHashBannCount = 5;
            }

            var trimmed = tweet.FullText.Replace(" ", string.Empty).ToLower();

            // TODO: finomítani, mert így szar
            var unallowedTags = 
                new List<string> { "signup", "token", "project", "sale", "free", "grabyour", "win", "join", "grab your", "easymoney", "freebitcoin", "passiveincome", ":arrow_left:", ":tada:", ":red_circle", "join", "joining", "register", "registering", "don'tmiss", "dontmiss", "click", ":fire:", ":rocket:", "comingsoon", "promotion", "promotions", ":heavy_check_mark:", "youcan", "contribute", "deposit", ":zap:", "pre-sale", "sale", "bonus", "initialcoinoffering", "presale", "pre-sale", "earnbitcoin", "giveaway" };

            var bannedWordCounter = 0;
            foreach(var word in unallowedTags)
            {
                if (trimmed.Contains(word))
                {
                    bannedWordCounter += 1;
                }
            }

            if(bannedWordCounter > 0)
            {
                if(bannedWordCounter > 0 && bannedWordCounter <= 3)
                {
                    increaseWordBannCount = 1;
                }
                else if(bannedWordCounter > 3 && bannedWordCounter <= 5)
                {
                    increaseWordBannCount = 2;
                }
                else if(bannedWordCounter > 5 && bannedWordCounter <= 9)
                {
                    increaseWordBannCount = 3;
                }
                else if(bannedWordCounter > 9)
                {
                    increaseWordBannCount = 5;
                }
            }

            var actualPenalty = 1;
            if(increaseHashBannCount > 0 || increaseWordBannCount > 0)
            {
                var penalty = this.provider.IncreaseWordAndHashtagBannCountAndPenaltyCount(tweet.User.Id, increaseWordBannCount, increaseHashBannCount);
                actualPenalty = penalty.HashtagBannPenalty + penalty.WordBannPenalty;
            }

            if(actualPenalty > 0)
            {
                score = score / actualPenalty;
            }

            if (tweet.User.Verified)
            {
                score = score * 2;
            }

            return score;
        }
    }
}
