using GatefailBot.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace GatefailBot.Infrastructure
{
    public class GatefailContext : DbContext
    {
        public GatefailContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<GuildEntity> Guilds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}