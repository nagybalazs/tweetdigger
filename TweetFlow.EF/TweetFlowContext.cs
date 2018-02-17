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
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TweetFlow;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
