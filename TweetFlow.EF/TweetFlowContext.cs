using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using TweetFlow.DatabaseModel;

namespace TweetFlow.EF
{
    public class TweetFlowContext : DbContext
    {
        public DbSet<TWUser> TWUsers { get; set; }
        public DbSet<TWStreamInfo> TWStreamInfo { get; set; }
        public DbSet<TWAccount> TWAccount { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.MapTWStreamInfo(modelBuilder.Entity<TWStreamInfo>());
        }

        private TweetFlowContext MapTWStreamInfo(EntityTypeBuilder<TWStreamInfo> entityTypeBuilder)
        {
            entityTypeBuilder.Ignore(streamInfo => streamInfo.EventTypeEnum);
            entityTypeBuilder.Property(streamInfo => streamInfo.OccuredAt).HasColumnType("datetime2");
            return this;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=tcp:tweetflow.database.windows.net,1433;Initial Catalog=TweetFlow;User Id=nagy.balazs@tweetflow.database.windows.net;Password=Password11;");
            //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=TweetFlow;Trusted_Connection=True;MultipleActiveResultSets=true");
        }
    }
}
