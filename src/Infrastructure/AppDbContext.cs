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
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AppUser>()
            .HasMany(a => a.refreshTokens)
            .WithOne(b => b.AppUser)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Client>()
            .HasMany(b => b.AppUsers)
            .WithOne(a => a.Client)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Client>()
            .HasMany(c => c.Accounts)
            .WithOne(a => a.Client)
            .HasForeignKey(a => a.ClientId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Account>()
            .HasMany(a => a.Cards)
            .WithOne(c => c.Account)
            .HasForeignKey(c => c.AccountId)
            .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Card>()
            .HasOne(c => c.Pan)
            .WithOne(p => p.Card)
            .HasForeignKey<Pan>(p => p.CardId)  
            .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pan>()
            .HasOne(p => p.Card)
            .WithOne(c => c.Pan)
            .HasForeignKey<Pan>(p => p.CardId);

            modelBuilder.Entity<Client>()
            .HasMany(p => p.Transactions)
            .WithOne(c => c.Client)
            .OnDelete(DeleteBehavior.Restrict); 


        }
    }
}
