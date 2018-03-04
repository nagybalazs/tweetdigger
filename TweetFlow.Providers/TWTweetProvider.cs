using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWTweetProvider : TWBaseProvider
    {

        public TWTweetProvider(DbContextOptions<TweetFlowContext> options)
            : base(options) { }

        public void SaveCachedTweets(IEnumerable<TWTweet> tweets)
        {
            using(var context = new TweetFlowContext(this.options))
            {
                context.TWTWeet.RemoveRange(context.TWTWeet);
                context.TWTWeet.AddRange(tweets);
                context.SaveChanges();
            }
        }

        public IEnumerable<TWTweet> GetCachedTweets()
        {
            using(var context = new TweetFlowContext(this.options))
            {
                return context.TWTWeet.AsNoTracking().ToList();
            }
        }
    }
}
