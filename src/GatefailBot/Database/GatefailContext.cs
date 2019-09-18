using GatefailBot.Database.BatchQueries;
using GatefailBot.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace GatefailBot.Database
{
    public class GatefailContext : DbContext
    {
        public GatefailContext(DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<Guild> Guilds { get; set; }
        public virtual DbSet<GatefailUser> Users { get; set; }
        public virtual DbSet<CommandChannelRestriction> CommandChannelRestrictions { get; set; }
        public virtual DbSet<BatchQueryItem> BatchQueries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Guild>()
                .HasMany(g => g.Users)
                .WithOne(u => u.Guild)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<GatefailUser>()
                .HasIndex(w => w.DiscordId);
            modelBuilder.Entity<BatchQueryItem>()
                .HasChangeTrackingStrategy(ChangeTrackingStrategy.Snapshot);

        }
    }
}