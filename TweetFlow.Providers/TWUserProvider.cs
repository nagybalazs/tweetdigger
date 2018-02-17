using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWUserProvider
    {
        private TweetFlowContext context;

        public TWUserProvider(TweetFlowContext context)
        {
            this.context = context;
        }

        public TWUser Add(TWUser user)
        {
            var result = this.context.TWUsers.Add(user);
            return result.Entity;
        }

        public TWUser GetByTwitterId(long twitterId)
        {
            var result = this.context.TWUsers.FirstOrDefault(p => p.TwitterId == twitterId);
            return result;
        }

        // TODO: repo, provider, service, UW pattern....
        public void IncreaseWordAndHashtagBannCount(TWUser twUserInContext, int increaseWordBannCountWith = 1, int increaseHashtagBannCountWith = 1)
        {
            if(twUserInContext == null)
            {
                return;
            }
            twUserInContext.HashtagBannCount += increaseHashtagBannCountWith;
            twUserInContext.WordBannCount += increaseWordBannCountWith;
            this.context.SaveChanges();
        }
    }
}
