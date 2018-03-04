using Microsoft.EntityFrameworkCore;
using System.Linq;
using TweetFlow.DatabaseModel;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWUserProvider : TWBaseProvider
    {
        // TODO: context lifecycle kezelése normálisan... ez így pure rák

        public TWUserProvider(DbContextOptions<TweetFlowContext> options)
            : base(options) { }

        public TWUser Add(TWUser user)
        {
            using (var context = new TweetFlowContext(this.options))
            {
                var result = context.TWUsers.Add(user);
                return result.Entity;
            }
        }

        public TWUser GetByTwitterId(long twitterId)
        {
            using (var context = new TweetFlowContext(this.options))
            {
                var result = context.TWUsers.FirstOrDefault(p => p.TwitterId == twitterId);
                return result;
            }
        }

        public (int HashtagBannPenalty, int WordBannPenalty) IncreaseWordAndHashtagBannCountAndPenaltyCount(long twitterId, int increaseWordBannCountWith, int increaseHashtagBannCountWith)
        {
            using (var context = new TweetFlowContext(this.options))
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
                var hashtagPenalty = twUserInContext.HashtagBannCount + increaseHashtagBannCountWith;
                var wordPenalty = twUserInContext.WordBannCount + increaseWordBannCountWith;

                twUserInContext.HashtagBannCount += increaseHashtagBannCountWith;
                twUserInContext.WordBannCount += increaseWordBannCountWith;
                context.SaveChanges();
                return (hashtagPenalty, wordPenalty);
            }
        }
    }
}
