using System;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class ScoredItem //: IOrderedItem<int, Tweet>
    {
        private TweetScoreCalculator calculator;
        public ScoredItem(Tweet content)
        {
            this.Content = content;
        }

        public int Score { get; set; }
        public Tweet Content { get; set; }

        public int CalculateScore()
        {
            if(this.calculator == null)
            {
                throw new ArgumentNullException("CustomScoreCalculator cannot be null. Set it with SetCustomScoreCalculator method.");
            }
            this.Score = this.calculator.CalculateScore(this.Content);
            return this.Score;
        }

        public void SetCustomScoreCalculator(TweetScoreCalculator calculator)
        {
            this.calculator = calculator;
        }
    }
}
