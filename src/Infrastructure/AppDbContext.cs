using Domain.Models;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace BankSystem
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Pan> Pans { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>().HasMany(a => a.refreshTokens).WithOne(b => b.AppUser).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Client>().HasMany(b => b.AppUsers).WithOne(a => a.Client).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Client>().HasMany(a => a.Accounts).WithOne(b => b.Client).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Card>().HasOne(a => a.Account).WithMany(c => c.Cards).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Card>().HasOne(p => p.Pan).WithOne(a => a.Card).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey<Card>(k => k.PanId);
            
        }
    }
}
