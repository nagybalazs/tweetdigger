using TweetFlow.MemoryStore;
using TweetFlow.Model;
using TweetFlow.Providers;

namespace TweetFlow.Services
{
    public class TweetScoreCalculator : IScoredCalculator<Tweet>
    {
        private TWUserProvider provider;
        public TweetScoreCalculator(TWUserProvider provider)
        {
            this.provider = provider;
        }

        public void CalculateScore(Tweet tweet)
        {
            throw new System.NotImplementedException();
        }
    }
}
