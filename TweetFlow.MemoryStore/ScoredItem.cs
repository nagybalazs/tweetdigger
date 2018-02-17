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
                (this.Content.RetweetCount ) +
                (this.Content.FavoriteCount ) +
                (this.Content.ReplyCount ) +
                (this.Content.User.FollowersCount ) +
                (this.Content.User.FriendsCount ) +
                (this.Content.User.ListedCount ) +
                (
                    (this.Content.User.FavouritesCount) /
                    (this.Content.User.StatusesCount) 
                );

            if (this.Content.User.Verified)
            {
                score = score * 2;
            }

            return score;
        }
    }
}
