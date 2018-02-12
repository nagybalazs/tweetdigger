namespace TweetFlow.MemoryStore
{
    public interface IOrderedItem<TScore, TContent>
    {
        TScore Score { get; set; }
        TContent Content { get; set; }
    }
}