namespace TweetFlow.DatabaseModel
{
    public enum StreamInfoEventType
    {
        StreamStarted,
        StreamStopped,
        StreamDisconnected,
        LimitReached,
        FallingBehind
    }
}
