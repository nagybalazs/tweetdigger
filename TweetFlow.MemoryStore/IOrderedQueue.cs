using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TweetFlow.MemoryStore
{
    public interface IOrderedQueue<TScore, TItemContent, TItem> where TItem : IOrderedItem<TScore, TItemContent>
    {
        event EventHandler<TItemContent> ContentAdded;
        IOrderedQueue<TScore, TItemContent, TItem> SetReadyWhenCountReached(int readyWhenCountReached);
        void Add(TItem item);
        void Remove(TItem item);
        IReadOnlyCollection<TItemContent> CachedItems { get; }
    }
}