using Microsoft.EntityFrameworkCore;
using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWUserProvider
    {
        public TWUserProvider() { }

        public TWUser Add(TWUser user)
        {
            using (var context = new TweetFlowContext())
            {
                var result = context.TWUsers.Add(user);
                return result.Entity;
            }
        }

        public TWUser GetByTwitterId(long twitterId)
        {
            using (var context = new TweetFlowContext())
            {
                var result = context.TWUsers.FirstOrDefault(p => p.TwitterId == twitterId);
                return result;
            }
        }

        // TODO: repo, provider, service, UW pattern....
        public void IncreaseWordAndHashtagBannCount(long twitterId, int increaseWordBannCountWith = 1, int increaseHashtagBannCountWith = 1)
        {
            using (var context = new TweetFlowContext())
            {
                var twUserInContext = context.TWUsers.FirstOrDefault(p => p.TwitterId == twitterId);
                if (twUserInContext == null)
                {
                    twUserInContext = new TWUser
                    {
                        TwitterId = twitterId,
                        HashtagBannCount = 0,
                        WordBannCount = 0,
                    };
                    context.Add(twUserInContext);
                }
                twUserInContext.HashtagBannCount += increaseHashtagBannCountWith;
                twUserInContext.WordBannCount += increaseWordBannCountWith;
                context.SaveChanges();
            }
        }
    }
}
