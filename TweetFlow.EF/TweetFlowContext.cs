using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TweetFlow.DatabaseModel;

namespace TweetFlow.EF
{
    public class TweetFlowContext : DbContext
    {
        public DbSet<TWUser> TWUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //Data Source=tcp:tweetflow.database.windows.net,1433;Initial Catalog=TweetFlow;User Id=nagy.balazs@tweetflow.database.windows.net;Password=Password11;
            //"Server=(localdb)\\mssqllocaldb;Database=TweetFlow;Trusted_Connection=True;MultipleActiveResultSets=true"
            optionsBuilder.UseSqlServer("Data Source=tcp:tweetflow.database.windows.net,1433;Initial Catalog=TweetFlow;User Id=nagy.balazs@tweetflow.database.windows.net;Password=Password11;");
        }
    }
}
