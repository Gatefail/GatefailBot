using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GatefailBot.Database
{
    public class WowTrackerContextDesignTimeFactory : IDesignTimeDbContextFactory<WowTrackerContext>
    {
        public WowTrackerContext CreateDbContext(string[] args)
        {
            var configuration = BotConfigurationBuilder.Build();

            var builder = new DbContextOptionsBuilder<WowTrackerContext>();

            var connectionString = configuration.GetBotDbConnectionString();

            builder.UseSqlite(connectionString);

            return new WowTrackerContext(builder.Options);
        }
    }
}