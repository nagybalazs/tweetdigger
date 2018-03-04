using Microsoft.EntityFrameworkCore;
using TweetFlow.EF;

namespace TweetFlow.Providers
{
    public class TWBaseProvider
    {
        protected DbContextOptions<TweetFlowContext> options;
        public TWBaseProvider(DbContextOptions<TweetFlowContext> options)
        {
            this.options = options;
        }
    }
}
