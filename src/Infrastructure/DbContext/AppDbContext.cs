using Domain.Models;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Domain.Models.Accounts;

namespace Infrastructure.DbContext
{
    public class AppDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        public DbSet<Client> Clients { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Pan> Pans { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<CurrentAccount> CurrentAccounts { get; set; }
        public DbSet<SavingAccount> SavingsAccounts { get; set; }
        public DbSet<LockedSavingAccount> LockedSavingAccounts { get; set; }
        public DbSet<Outbox> OutBoxes { get; set; }
        public DbSet<InterestAccrualHistory> InterestAccrualHistory { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Account>()
                .Property(a => a.DepositedLastDayMoney)
                .HasConversion(
                    money => money == null ? 0m : money.Value.Amount,
                    value => new Money(value, "KZT")
                );
            modelBuilder.Entity<Account>()
                .Property(a => a.TransferredLastDayMoney)
                .HasConversion(
                    money => money == null ? 0m : money.Value.Amount,
                    value => new Money(value, "KZT")
                );
            modelBuilder.Entity<Transaction>()
                .Property(t => t.AmountMoney)
                .HasConversion(
                    money => money.Amount,
                    value => new Money(value, "KZT")
                );


            modelBuilder.Entity<SavingAccount>(a =>
            {
                a.OwnsOne(i => i._interestRate, ir =>
                {
                    ir.Property(p => p.Rate);
                    ir.Property(p => p.EffectiveFrom);
                    ir.Property(p => p.EffectiveTo);
                    ir.Property(p => p.Type);
                });
            });


            modelBuilder.Entity<Account>()
                .HasMany(a => a.Cards)
                .WithOne(c => c.Account)
                .HasForeignKey(c => c.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Client)
                .WithMany(c => c.Accounts)
                .HasForeignKey(a => a.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(a => a.AccrualHistory)
                .WithOne(ah => ah.Account)
                .HasForeignKey(ah => ah.AccountId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<AppUser>()
            .HasMany(a => a.refreshTokens)
            .WithOne(b => b.AppUser)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Client>()
            .HasMany(b => b.AppUsers)
            .WithOne(a => a.Client)
            .OnDelete(DeleteBehavior.Restrict);
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
