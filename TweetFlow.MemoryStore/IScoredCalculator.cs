namespace TweetFlow.MemoryStore
{
    public interface IScoredCalculator<TContent>
    {
        void CalculateScore(TContent orderedItem);
    }
}
