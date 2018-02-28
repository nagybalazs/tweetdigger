using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWTweetProvider
    {
        public void SaveCachedTweets(IEnumerable<TWTweet> tweets)
        {
            using(var context = new TweetFlowContext())
            {
                context.TWTWeet.RemoveRange(context.TWTWeet);
                context.TWTWeet.AddRange(tweets);
                try
                {
                    context.SaveChanges();
                }
                catch(Exception ex)
                {

                }
            }
        }

        public IEnumerable<TWTweet> GetCachedTweets()
        {
            using(var context = new TweetFlowContext())
            {
                return context.TWTWeet.AsNoTracking().ToList();
            }
        }
    }
}
