using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TweetFlow.DatabaseModel;

namespace TweetFlow.EF
{
    public class TweetFlowContext : DbContext
    {
        public TweetFlowContext(DbContextOptions<TweetFlowContext> options)
            : base(options) { }

        public TweetFlowContext() { }

        public DbSet<TWUser> TWUsers { get; set; }
        public DbSet<TWAccount> TWAccount { get; set; }
        public DbSet<TWTweet> TWTWeet { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    //optionsBuilder.UseSqlServer("Data Source=tcp:tweetflow.database.windows.net,1433;Initial Catalog=TweetFlow;User Id=nagy.balazs@tweetflow.database.windows.net;Password=Password11;");
        //    optionsBuilder.UseMySql("Server=localhost;database=tweetflow;uid=root;pwd=Password11;");
        //}
    }
}
