using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class OrderedQueue : IOrderedQueue<int, Tweet, ScoredItem>
    {
        private const int defaultReadyWhenCountReached = 50;

        private Stopwatch stopwatch;
        private IList<ScoredItem> items;
        private int readyWhenCountReached;

        private IList<Tweet> cachedItems;
        public IReadOnlyCollection<Tweet> CachedItems
        {
            get
            {
                if(this.cachedItems == null)
                {
                    this.cachedItems = new List<Tweet>();
                }
                return new ReadOnlyCollection<Tweet>(this.cachedItems);
            }
        }

        public event EventHandler<Tweet> ContentAdded;

        public OrderedQueue()
        {
            this.stopwatch = new Stopwatch();
            this.stopwatch.Start();
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
                if (this.stopwatch.ElapsedMilliseconds >= (10*1000))
                {
                    this.stopwatch.Restart();
                    var maximumScoredItem = this.RemoveMaximumScoredItem(newItem.Content.Type);
                    if(maximumScoredItem == null)
                    {
                        return;
                    }
                    if (this.cachedItems.Count >= 100)
                    {
                        var remove = this.cachedItems.FirstOrDefault();
                        if(remove != null)
                        {
                            this.cachedItems.Remove(remove);
                        }
                    }
                    this.cachedItems.Add(maximumScoredItem.Content);
                    this.ContentAdded?.Invoke(null, maximumScoredItem.Content);
                }
            }
        }

        public void Add(ScoredItem item)
        {
            var alreadyAdded = this.GetItemByStrIdAndType(item.Content.StrId, item.Content.FullText, item.Content.Type);
            if (alreadyAdded != null)
            {
                return;
            }

            if(items.Count > 0)
            {
                var kek = items.Average(p => p.Score);
                Debug.WriteLine($"AVG: {kek.ToString()}");
            }
            if (this.items.Count == this.readyWhenCountReached)
            {
                this.OutScoreMinimumScoredItem(item);
            }
            else
            {
                this.items.Add(item);
            }
        }

        private ScoredItem GetItemByStrIdAndType(string strId, string fullText, TweetType type)
        {
            return this.items.FirstOrDefault(item => item.Content.StrId == strId && item.Content.Type == type && item.Content.FullText == fullText);
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