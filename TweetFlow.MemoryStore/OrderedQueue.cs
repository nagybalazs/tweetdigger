using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using TweetFlow.Model;

namespace TweetFlow.MemoryStore
{
    public class OrderedQueue
    {
        private Dictionary<string, Stopwatch> stopwatchContainer;
        private const int defaultReadyWhenCountReached = 50;

        private string queueType { get; set; }

        private IList<ScoredItem> items;
        private IList<ScoredItem> typedItems
        {
            get
            {
                return this.items.Where(p => p.Content.Type == this.queueType).ToList();
            }
        }
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
            this.stopwatchContainer = new Dictionary<string, Stopwatch>();
            this.readyWhenCountReached = defaultReadyWhenCountReached;
            this.items = new List<ScoredItem>();
        }

        private ScoredItem GetMinimumScoredItem()
        {
            if(typedItems.Count() < 1)
            {
                return null;
            }
            return typedItems.MinBy(item => item.Score);
        }

        private ScoredItem GetMaximumScoredItem()
        {
            if(typedItems.Count() < 1)
            {
                return null;
            }
            return this.typedItems.MaxBy(item => item.Score);
        }

        private ScoredItem RemoveMaximumScoredItem()
        {
            var maximumScoredItem = this.GetMaximumScoredItem();
            if(maximumScoredItem == null)
            {
                return null;
            }
            this.Remove(maximumScoredItem);
            return maximumScoredItem;
        }

        private Stopwatch stopwatch
        {
            get
            {
                if (this.stopwatchContainer.ContainsKey(this.queueType))
                {
                    this.stopwatchContainer.TryGetValue(this.queueType, out Stopwatch stopWatch);
                    return stopWatch;
                }
                else
                {
                    var newWatch = new Stopwatch();
                    newWatch.Start();
                    this.stopwatchContainer.Add(this.queueType, newWatch);
                    return newWatch;
                }
            }
        }

        private void OutScoreMinimumScoredItem(ScoredItem newItem)
        {
            var minimumScoredItem = this.GetMinimumScoredItem();
            if(minimumScoredItem == null || newItem.Score > minimumScoredItem.Score)
            {
                if (minimumScoredItem != null)
                {
                    this.items.Remove(minimumScoredItem);
                    this.items.Add(newItem);
                }

                if (this.stopwatch.ElapsedMilliseconds >= (10*1000))
                {
                    this.stopwatch.Restart();
                    var maximumScoredItem = this.RemoveMaximumScoredItem();
                    if(maximumScoredItem == null)
                    {
                        return;
                    }
                    this.CacheItem(maximumScoredItem.Content);
                    this.ContentAdded?.Invoke(null, maximumScoredItem.Content);
                }

            }
        }

        private void CacheItem(Tweet tweet)
        {
            if(this.cachedItems.Count(p => p.Type == this.queueType) >= 100)
            {
                var remove = this.cachedItems.FirstOrDefault(p => p.Type == this.queueType);
                if(remove != null)
                {
                    this.cachedItems.Remove(remove);
                }
            }
            this.cachedItems.Add(tweet);
        }

        public void Add(ScoredItem item)
        {
            var alreadyAdded = this.GetItemByStrIdAndType(item.Content.StrId, item.Content.FullText);
            if (alreadyAdded != null)
            {
                return;
            }

            if (this.typedItems.Count >= this.readyWhenCountReached)
            {
                this.OutScoreMinimumScoredItem(item);
            }
            else
            {
                if (item.Score > 0)
                {
                    this.items.Add(item);
                }
            }
        }

        private ScoredItem GetItemByStrIdAndType(string strId, string fullText)
        {
            return this.typedItems.FirstOrDefault(item => item.Content.StrId == strId  && item.Content.FullText == fullText);
        }

        public void Remove(ScoredItem item)
        {
            this.items.Remove(item);
        }

        public OrderedQueue SetReadyWhenCountReached(int readyWhenCountReached)
        {
            this.readyWhenCountReached = readyWhenCountReached;
            return this;
        }

        public OrderedQueue SetQueueType(string queueType)
        {
            this.queueType = queueType;
            return this;
        }
    }
}