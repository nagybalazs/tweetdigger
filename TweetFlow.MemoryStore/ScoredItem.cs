using System;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class ScoredItem : IOrderedItem<int, Tweet>
    {
        public ScoredItem(Tweet content)
        {
            this.Content = content;
            this.Score = this.CalculateScore();
        }

        public int Score { get; set; }
        public Tweet Content { get; set; }

        private int CalculateScore()
        {
            var score =
                (this.Content.RetweetCount * 5) +
                (this.Content.FavoriteCount * 5) +
                (this.Content.ReplyCount * 5) +
                (this.Content.User.FollowersCount * 10) +
                (this.Content.User.FavouritesCount * 2) +
                (this.Content.User.FriendsCount * 4) +
                (this.Content.User.ListedCount * 2) +
                (this.Content.User.StatusesCount * 4);

            if (this.Content.User.Verified)
            {
                score = score * 2;
            }

            return score;
        }
    }
}
