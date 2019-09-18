using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace GatefailBot.Database
{
    public class GatefailContextDesignTimeFactory : IDesignTimeDbContextFactory<GatefailContext>
    {
        public GatefailContext CreateDbContext(string[] args)
        {
            var configuration = BotConfigurationBuilder.Build();

            var builder = new DbContextOptionsBuilder<GatefailContext>();

            var connectionString = configuration.GetBotDbConnectionString();

            builder.UseSqlite(connectionString);

            return new GatefailContext(builder.Options);
        }
    }
}