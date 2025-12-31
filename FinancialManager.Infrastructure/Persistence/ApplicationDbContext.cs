using FinancialManager.Domain.Entities;
using FinancialManager.Domain.Enums;
using FinancialManager.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<BankAccount> BankAccounts => Set<BankAccount>();
    public DbSet<BankTransaction> BankTransactions => Set<BankTransaction>();
    public DbSet<Income> Incomes => Set<Income>();
    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<CreditCardTransaction> CreditCardTransactions => Set<CreditCardTransaction>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<BankAccount>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Bank).HasMaxLength(120).IsRequired();
            entity.Property(p => p.InitialBalance).HasColumnType("decimal(18,2)");
            entity.Property(p => p.CurrentBalance).HasColumnType("decimal(18,2)");
        });

        builder.Entity<BankTransaction>(entity =>
        {
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Description).HasMaxLength(250);
            entity.Property(p => p.Category).HasMaxLength(120);
            entity.HasOne(p => p.BankAccount)
                  .WithMany(a => a.Transactions)
                  .HasForeignKey(p => p.BankAccountId);
        });

        builder.Entity<Income>(entity =>
        {
            entity.Property(p => p.Title).HasMaxLength(160).IsRequired();
            entity.Property(p => p.Category).HasMaxLength(120);
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.HasOne(p => p.BankAccount)
                .WithMany(a => a.Incomes)
                  .HasForeignKey(p => p.BankAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<Expense>(entity =>
        {
            entity.Property(p => p.Title).HasMaxLength(160).IsRequired();
            entity.Property(p => p.Category).HasMaxLength(120);
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.HasOne(p => p.BankAccount)
                .WithMany(a => a.Expenses)
                  .HasForeignKey(p => p.BankAccountId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(p => p.CreditCard)
                .WithMany(c => c.Expenses)
                  .HasForeignKey(p => p.CreditCardId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<CreditCard>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Bank).HasMaxLength(120);
            entity.Property(p => p.Limit).HasColumnType("decimal(18,2)");
        });

        builder.Entity<CreditCardTransaction>(entity =>
        {
            entity.Property(p => p.Description).HasMaxLength(160).IsRequired();
            entity.Property(p => p.Category).HasMaxLength(120);
            entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
            entity.HasOne(p => p.CreditCard)
                  .WithMany(c => c.Transactions)
                  .HasForeignKey(p => p.CreditCardId);
        });
    }
}
