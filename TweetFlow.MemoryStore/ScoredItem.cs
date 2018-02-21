using System;
using System.Collections.Generic;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class ScoredItem
    {
        private List<string> celebrityIds;
        public bool IsCelebrity
        {
            get
            {
                return this.celebrityIds.Contains(this.Content.StrId);
            }
        }

        private TweetScoreCalculator calculator;
        public ScoredItem(Tweet content)
        {
            this.celebrityIds = new List<string> { "" };
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
