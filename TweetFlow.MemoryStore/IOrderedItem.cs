namespace TweetFlow.MemoryStore
{
    public interface IOrderedItem<TScore, TContent>
    {
        TScore Score { get; set; }
        TContent Content { get; set; }
        void SetCustomScoreCalculator(IScoredCalculator<TScore, TContent> calculator);
        TScore CalculateScore();
    }
}