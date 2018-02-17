namespace TweetFlow.MemoryStore
{
    public interface IOrderedItem<TScore, TContent>
    {
        TScore Score { get; set; }
        TContent Content { get; set; }
        IOrderedItem<TScore, TContent> SetCustomScoreCalculator(IScoredCalculator<TContent> calculator);
    }
}