using Microsoft.AspNetCore.SignalR;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class OrderedQueue : IOrderedQueue<int, Tweet, ScoredItem>
    {
        private const int defaultReadyWhenCountReached = 10;

        private IList<ScoredItem> items;
        private int readyWhenCountReached;

        public event EventHandler<Tweet> ContentAdded;

        public bool InReadyState { get; private set; }

        public OrderedQueue()
        {
            this.readyWhenCountReached = defaultReadyWhenCountReached;
            this.items = new List<ScoredItem>();
        }

        private ScoredItem GetMinimumScoredItem()
        {
            return this.items.MinBy(item => item.Score);
        }

        private ScoredItem GetMaximumScoredItem()
        {
            return this.items.MaxBy(item => item.Score);
        }

        private ScoredItem RemoveMinimumScoredItem()
        {
            var minimumScoredItem = this.GetMinimumScoredItem();
            this.Remove(minimumScoredItem);
            return minimumScoredItem;
        }
        private ScoredItem RemoveMaximumScoredItem()
        {
            var maximumScoredItem = this.GetMaximumScoredItem();
            this.Remove(maximumScoredItem);
            return maximumScoredItem;
        }

        private void OutScoreMinimumScoredItem(ScoredItem newItem)
        {
            var minimumScoredItem = this.GetMinimumScoredItem();
            if(newItem.Score > minimumScoredItem.Score)
            {
                this.items.Remove(minimumScoredItem);
                this.items.Add(newItem);
                var maximumScoredItem = this.GetMaximumScoredItem();
                if (this.InReadyState)
                {
                    this.RemoveMaximumScoredItem();
                    this.ContentAdded?.Invoke(null, maximumScoredItem.Content);
                }
            }
        }
        public ScoredItem Get()
        {
            return this.RemoveMaximumScoredItem();
        }

        public async Task AddAsync(ScoredItem item)
        {
            throw new NotImplementedException();
        }

        public void Add(ScoredItem item)
        {
            var alreadyAdded = this.items.FirstOrDefault(p => p.Content.StrId == item.Content.StrId);
            if (alreadyAdded != null)
            {
                return;
            }
            if (this.items.Count == this.readyWhenCountReached)
            {
                this.InReadyState = true;
                this.OutScoreMinimumScoredItem(item);
            }
            else
            {
                this.items.Add(item);
            }
        }

        public void Remove(ScoredItem item)
        {
            this.items.Remove(item);
        }

        public IOrderedQueue<int, Tweet, ScoredItem> SetReadyWhenCountReached(int readyWhenCountReached)
        {
            this.readyWhenCountReached = readyWhenCountReached;
            return this;
        }
    }
}