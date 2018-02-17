using System.Collections.Generic;
using System.Linq;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class ScoredItem : IOrderedItem<int, Tweet>
    {
        private IScoredCalculator<Tweet> calculator;
        public ScoredItem(Tweet content)
        {
            this.Content = content;
            this.Score = this.CalculateScore();
        }

        public int Score { get; set; }
        public Tweet Content { get; set; }

        public IOrderedItem<int, Tweet> SetCustomScoreCalculator(IScoredCalculator<Tweet> calculator)
        {
            this.calculator = calculator;
            return this;
        }

        private int CalculateScore()
        {
            var score =
                (this.Content.RetweetCount * 5) +
                (this.Content.FavoriteCount * 5) +
                (this.Content.ReplyCount * 5) +
                (this.Content.User.FollowersCount * 5) +
                (this.Content.User.FriendsCount * 5) +
                (this.Content.User.ListedCount * 5) +
                (
                    (this.Content.User.FavouritesCount) /
                    (this.Content.User.StatusesCount) * 10  
                );

            if(this.Content.Hashtags.Count > 7)
            {
                score = 0;
            }

            var trimmed = this.Content.FullText.Replace(" ", string.Empty);

            // TODO: finomítani, mert így szar
            var unallowedTags = new List<string> { "signup", "token", "project", "sale", "free", "grab your", "win", "join", "grab your", "easy money" };
            if(unallowedTags.Any(w => this.Content.FullText.ToLower().Contains(w)))
            {
                score = 0;
            }

            if (this.Content.User.Verified)
            {
                score = score * 2;
            }

            if (this.calculator != null)
            {
                calculator.CalculateScore(this.Content);
            }

            return score;
        }
    }
}
