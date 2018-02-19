using MoreLinq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class OrderedQueue : IOrderedQueue<int, Tweet, ScoredItem>
    {
        private const int defaultReadyWhenCountReached = 50;

        private IList<ScoredItem> items;
        private int readyWhenCountReached;

        public event EventHandler<Tweet> ContentAdded;

        public OrderedQueue()
        {
            this.readyWhenCountReached = defaultReadyWhenCountReached;
            this.items = new List<ScoredItem>();
        }

        private ScoredItem GetMinimumScoredItem(TweetType tweetType)
        {
            var itemsWithSameType = items.Where(item => item.Content.Type == tweetType);
            if(itemsWithSameType.Count() < 1)
            {
                return null;
            }
            return itemsWithSameType.MinBy(item => item.Score);
        }

        private ScoredItem GetMaximumScoredItem(TweetType type)
        {
            var itemsWithSameType = items.Where(item => item.Content.Type == type);
            if(itemsWithSameType.Count() < 1)
            {
                return null;
            }
            return this.items.MaxBy(item => item.Score);
        }

        private ScoredItem RemoveMaximumScoredItem(TweetType type)
        {
            var maximumScoredItem = this.GetMaximumScoredItem(type);
            if(maximumScoredItem == null)
            {
                return null;
            }
            this.Remove(maximumScoredItem);
            return maximumScoredItem;
        }

        private void OutScoreMinimumScoredItem(ScoredItem newItem)
        {
            var minimumScoredItem = this.GetMinimumScoredItem(newItem.Content.Type);
            if(minimumScoredItem == null || newItem.Score > minimumScoredItem.Score)
            {
                if (minimumScoredItem != null)
                {
                    this.items.Remove(minimumScoredItem);
                }
                this.items.Add(newItem);
                var maximumScoredItem = this.RemoveMaximumScoredItem(newItem.Content.Type);
                if(maximumScoredItem == null)
                {
                    return;
                }
                this.ContentAdded?.Invoke(null, maximumScoredItem.Content);
            }
        }

        public void Add(ScoredItem item)
        {
            var alreadyAdded = this.GetItemByStrIdAndType(item.Content.StrId, item.Content.Type);
            if (alreadyAdded != null)
            {
                return;
            }
            if (this.items.Count == this.readyWhenCountReached)
            {
                this.OutScoreMinimumScoredItem(item);
            }
            else
            {
                this.items.Add(item);
                this.ContentAdded?.Invoke(null, item.Content);
            }
        }

        private ScoredItem GetItemByStrIdAndType(string strId, TweetType type)
        {
            return this.items.FirstOrDefault(item => item.Content.StrId == strId && item.Content.Type == type);
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