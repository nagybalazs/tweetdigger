using System.Collections.Generic;
using TweetFlow.Model;
using TweetFlow.Providers;

namespace TweetFlow.MemoryStore
{
    public class TweetScoreCalculator
    {
        private List<string> unallowedTags;
        private List<string> celebrityIds;
        private TWUserProvider provider;

        public TweetScoreCalculator(TWUserProvider provider)
        {
            this.provider = provider;
            this.unallowedTags = new List<string> { "signup", "token", "project", "sale", "free", "grabyour", "win", "join", "grab your", "easymoney", "freebitcoin", "passiveincome", ":arrow_left:", ":tada:", ":red_circle", "join", "joining", "register", "registering", "don'tmiss", "dontmiss", "click", ":fire:", ":rocket:", "comingsoon", "promotion", "promotions", ":heavy_check_mark:", "youcan", "contribute", "deposit", ":zap:", "pre-sale", "sale", "bonus", "initialcoinoffering", "presale", "pre-sale", "earnbitcoin", "giveaway" };
            this.celebrityIds = new List<string> { "295218901", "378362205", "49736166", "44196397", "341746855", "893605057", "1032188509", "836336967893856257", "213410547", "508893796", "858364736240586752", "855794327213273088", "852256178021294080", "801733669023088640", "860160083132645376", "767557314823925760", "126717954", "854000068357234688", "823344672802304000", "844193096246149120", "31395520", "861054703626571776", "717191666629795840", "877728873340956672", "247857712", "241964810", "771502019471220736", "295218901", "961445378", "176758255", "40742821", "66106683", "52160722", "1621271", "26377478" };
        }

        public int CalculateScore(Tweet tweet)
        {
            if (this.unallowedTags.Contains(tweet.StrId))
            {
                tweet.Celebrity = true;
                return int.MaxValue;
            }

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

            if (tweet.User.Verified)
            {
                score = score * 2;
            }

            var increaseWordBannCountWith = 0;
            var increaseHashtagBannCountWith = 0;

            var hashtagCount = tweet.Hashtags.Count;
            if (hashtagCount > 1 && hashtagCount <= 3)
            {
                increaseHashtagBannCountWith = 1;
            }
            else if (hashtagCount > 3 && hashtagCount <= 5)
            {
                increaseHashtagBannCountWith = 2;
                score = 0;
            }
            else if (hashtagCount > 5 && hashtagCount <= 9)
            {
                increaseHashtagBannCountWith = 3;
            }
            else if (hashtagCount > 9)
            {
                increaseHashtagBannCountWith = 5;
            }

            var trimmed = tweet.FullText.Replace(" ", string.Empty).ToLower();

            var bannedWordCounter = 0;
            foreach (var word in unallowedTags)
            {
                if (trimmed.Contains(word))
                {
                    bannedWordCounter += 1;
                    score = 0;
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
                var penalty = provider.IncreaseWordAndHashtagBannCountAndPenaltyCount(tweet.User.Id, increaseWordBannCountWith, increaseHashtagBannCountWith);
                actualPenalty = penalty.HashtagBannPenalty + penalty.WordBannPenalty;
            }

            if (actualPenalty > 0)
            {
                score = score / actualPenalty;
            }

            return score;
        }
    }
}
