using System;
using System.Threading.Tasks;

namespace TweetFlow.MemoryStore
{
    public interface IOrderedQueue<TScore, TItemContent, TItem> where TItem : IOrderedItem<TScore, TItemContent>
    {
        event EventHandler<TItemContent> ContentAdded;
        bool InReadyState { get; }
        IOrderedQueue<TScore, TItemContent, TItem> SetReadyWhenCountReached(int readyWhenCountReached);
        void Add(TItem item);
        Task AddAsync(TItem item);
        void Remove(TItem item);
        TItem Get();
    }
}