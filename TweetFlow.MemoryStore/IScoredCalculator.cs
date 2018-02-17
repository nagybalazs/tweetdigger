namespace TweetFlow.MemoryStore
{
    public interface IScoredCalculator<TScore, TContent>
    {
        TScore CalculateScore(TContent orderedItem);
    }
}
