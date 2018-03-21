using MoreLinq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using TweetFlow.Model;
using TweetFlow.Services;

namespace TweetFlow.MemoryStore
{
    public class OrderedQueue
    {
        private Timer timer;
        private Dictionary<string, Stopwatch> stopwatchContainer;
        private Dictionary<string, DateTime> retweets;
        private TweetService tweetService;
        private const int defaultReadyWhenCountReached = 30;

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
                if(this.cachedItems == null || this.cachedItems.Count < 1)
                {
                    this.cachedItems = this.tweetService.GetCachedTweets().ToList();
                }
                return new ReadOnlyCollection<Tweet>(this.cachedItems);
            }
        }

        public event EventHandler<Tweet> ContentAdded;

        public OrderedQueue()
        {
            this.timer = new Timer();
            this.timer.Enabled = true;
            this.timer.Interval = (1000 * 60 * 10);
            this.timer.Elapsed += new ElapsedEventHandler(PersistCache);
            this.stopwatchContainer = new Dictionary<string, Stopwatch>();
            this.readyWhenCountReached = defaultReadyWhenCountReached;
            this.retweets = new Dictionary<string, DateTime>();
            this.items = new List<ScoredItem>();
        }

        public OrderedQueue SetCache(TweetService tweetService)
        {
            this.tweetService = tweetService;
            return this;
        }

        private void PersistCache(object source, ElapsedEventArgs elapsedEventHandler)
        {
            if(tweetService == null || this.cachedItems.Count < 1)
            {
                return;
            }
            this.tweetService.SaveChachedTweets(this.cachedItems);
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
            if(minimumScoredItem == null || newItem.Score >= minimumScoredItem.Score)
            {
                if (minimumScoredItem != null)
                {
                    this.items.Remove(minimumScoredItem);
                    this.items.Add(newItem);
                }

                if (newItem.Content.CelebrityHighlighted || this.stopwatch.ElapsedMilliseconds >= (10*1000))
                {
                    this.stopwatch.Restart();
                    var maximumScoredItem = this.RemoveMaximumScoredItem();
                    if(maximumScoredItem == null)
                    {
                        return;
                    }
                    this.CacheItem(maximumScoredItem.Content);
                    if (maximumScoredItem.Content.ConvertedToOriginal)
                    {
                        if (!this.retweets.ContainsKey(maximumScoredItem.Content.StrId))
                        {
                            this.retweets.Add(maximumScoredItem.Content.StrId, DateTime.UtcNow);
                        }
                    }
                    this.ContentAdded?.Invoke(null, maximumScoredItem.Content);
                }

            }
        }

        private void CacheItem(Tweet tweet)
        {
            if(this.cachedItems.Count(p => p.Type == this.queueType) >= this.readyWhenCountReached)
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

            var valueAccessedSuccessfully = this.retweets.TryGetValue(item.Content.StrId, out DateTime addedAt);
            if (valueAccessedSuccessfully)
            {
                var outdatedRetweets = this.retweets.Where(p => p.Value < DateTime.UtcNow.AddMinutes(-10));
                foreach(var outdatedRetweet in outdatedRetweets.ToList())
                {
                    this.retweets.Remove(outdatedRetweet.Key);
                }
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
            return this.typedItems.FirstOrDefault(item => item.Content.StrId == strId  || item.Content.FullText == fullText);
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