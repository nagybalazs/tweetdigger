using Microsoft.EntityFrameworkCore;
using TweetFlow.DatabaseModel;

namespace TweetFlow.EF
{
    public class TweetFlowContext : DbContext
    {
        public TweetFlowContext(DbContextOptions<TweetFlowContext> options)
         : base(options) { }

        public DbSet<TWUser> TWUsers { get; set; }
    }
}
