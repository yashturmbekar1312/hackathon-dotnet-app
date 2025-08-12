using Microsoft.EntityFrameworkCore;
using PersonalFinanceAPI.Models.Entities;
using System.Reflection;

namespace PersonalFinanceAPI.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<UserPreferences> UserPreferences { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Merchant> Merchants { get; set; }
    public DbSet<BankAggregator> BankAggregators { get; set; }
    public DbSet<LinkedAccount> LinkedAccounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionFlag> TransactionFlags { get; set; }
    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetUtilization> BudgetUtilizations { get; set; }
    public DbSet<MonthlySummary> MonthlySummaries { get; set; }
    public DbSet<FinancialGoal> FinancialGoals { get; set; }
    public DbSet<InvestmentSuggestion> InvestmentSuggestions { get; set; }
    public DbSet<SuggestionHistory> SuggestionHistory { get; set; }
    public DbSet<UserAlert> UserAlerts { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }
    public DbSet<IncomePlan> IncomePlans { get; set; }
    public DbSet<IncomeSource> IncomeSources { get; set; }
    public DbSet<IncomeEntry> IncomeEntries { get; set; }
    public DbSet<IncomePlanMilestone> IncomePlanMilestones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Configure relationships

        // User -> UserPreferences (1:1)
        modelBuilder.Entity<UserPreferences>()
            .HasOne(up => up.User)
            .WithOne(u => u.Preferences)
            .HasForeignKey<UserPreferences>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> UserSession (1:N)
        modelBuilder.Entity<UserSession>()
            .HasOne(us => us.User)
            .WithMany(u => u.Sessions)
            .HasForeignKey(us => us.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User -> LinkedAccount (1:N)
        modelBuilder.Entity<LinkedAccount>()
            .HasOne(la => la.User)
            .WithMany(u => u.LinkedAccounts)
            .HasForeignKey(la => la.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // BankAggregator -> LinkedAccount (1:N)
        modelBuilder.Entity<LinkedAccount>()
            .HasOne(la => la.BankAggregator)
            .WithMany(ba => ba.LinkedAccounts)
            .HasForeignKey(la => la.BankAggregatorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Category -> Category (Self-referencing)
        modelBuilder.Entity<Category>()
            .HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Category -> Merchant (1:N)
        modelBuilder.Entity<Merchant>()
            .HasOne(m => m.Category)
            .WithMany(c => c.Merchants)
            .HasForeignKey(m => m.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Transaction relationships
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.User)
            .WithMany(u => u.Transactions)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.LinkedAccount)
            .WithMany(la => la.Transactions)
            .HasForeignKey(t => t.LinkedAccountId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.TransferToAccount)
            .WithMany(la => la.TransferTransactions)
            .HasForeignKey(t => t.TransferToAccountId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Merchant)
            .WithMany(m => m.Transactions)
            .HasForeignKey(t => t.MerchantId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // TransactionFlag -> Transaction
        modelBuilder.Entity<TransactionFlag>()
            .HasOne(tf => tf.Transaction)
            .WithMany(t => t.Flags)
            .HasForeignKey(tf => tf.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TransactionFlag>()
            .HasOne(tf => tf.CreatedByUser)
            .WithMany()
            .HasForeignKey(tf => tf.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        // Budget relationships
        modelBuilder.Entity<Budget>()
            .HasOne(b => b.User)
            .WithMany(u => u.Budgets)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Budget>()
            .HasOne(b => b.Category)
            .WithMany(c => c.Budgets)
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // BudgetUtilization relationships
        modelBuilder.Entity<BudgetUtilization>()
            .HasOne(bu => bu.Budget)
            .WithMany(b => b.Utilizations)
            .HasForeignKey(bu => bu.BudgetId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<BudgetUtilization>()
            .HasOne(bu => bu.Transaction)
            .WithMany(t => t.BudgetUtilizations)
            .HasForeignKey(bu => bu.TransactionId)
            .OnDelete(DeleteBehavior.Cascade);

        // MonthlySummary relationships
        modelBuilder.Entity<MonthlySummary>()
            .HasOne(ms => ms.User)
            .WithMany(u => u.MonthlySummaries)
            .HasForeignKey(ms => ms.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MonthlySummary>()
            .HasOne(ms => ms.TopExpenseCategory)
            .WithMany()
            .HasForeignKey(ms => ms.TopExpenseCategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // FinancialGoal relationships
        modelBuilder.Entity<FinancialGoal>()
            .HasOne(fg => fg.User)
            .WithMany(u => u.FinancialGoals)
            .HasForeignKey(fg => fg.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // InvestmentSuggestion relationships
        modelBuilder.Entity<InvestmentSuggestion>()
            .HasOne(is_ => is_.User)
            .WithMany(u => u.InvestmentSuggestions)
            .HasForeignKey(is_ => is_.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // SuggestionHistory relationships
        modelBuilder.Entity<SuggestionHistory>()
            .HasOne(sh => sh.Suggestion)
            .WithMany(is_ => is_.History)
            .HasForeignKey(sh => sh.SuggestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<SuggestionHistory>()
            .HasOne(sh => sh.User)
            .WithMany()
            .HasForeignKey(sh => sh.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserAlert relationships
        modelBuilder.Entity<UserAlert>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.Alerts)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // AuditLog relationships
        modelBuilder.Entity<AuditLog>()
            .HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Income Plan relationships
        modelBuilder.Entity<IncomePlan>()
            .HasOne(ip => ip.User)
            .WithMany(u => u.IncomePlans)
            .HasForeignKey(ip => ip.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IncomeSource>()
            .HasOne(s => s.IncomePlan)
            .WithMany(ip => ip.IncomeSources)
            .HasForeignKey(s => s.IncomePlanId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IncomeEntry>()
            .HasOne(e => e.IncomeSource)
            .WithMany(s => s.IncomeEntries)
            .HasForeignKey(e => e.IncomeSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<IncomePlanMilestone>()
            .HasOne(m => m.IncomePlan)
            .WithMany(ip => ip.Milestones)
            .HasForeignKey(m => m.IncomePlanId)
            .OnDelete(DeleteBehavior.Cascade);

        // Unique constraints
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<UserSession>()
            .HasIndex(us => us.RefreshToken)
            .IsUnique();

        modelBuilder.Entity<MonthlySummary>()
            .HasIndex(ms => new { ms.UserId, ms.MonthYear })
            .IsUnique();

        // Configure PostgreSQL-specific features
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.HasPostgresExtension("pgcrypto");

        // Seed data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        var foodCategoryId = Guid.NewGuid();
        var transportationCategoryId = Guid.NewGuid();
        var shoppingCategoryId = Guid.NewGuid();
        var entertainmentCategoryId = Guid.NewGuid();
        var billsCategoryId = Guid.NewGuid();
        var healthcareCategoryId = Guid.NewGuid();
        var educationCategoryId = Guid.NewGuid();
        var travelCategoryId = Guid.NewGuid();
        var salaryCategoryId = Guid.NewGuid();
        var freelanceCategoryId = Guid.NewGuid();
        var investmentCategoryId = Guid.NewGuid();
        var transferCategoryId = Guid.NewGuid();

        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = foodCategoryId,
                Name = "Food & Dining",
                Type = "EXPENSE",
                Icon = "restaurant",
                Color = "#FF6B6B",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = transportationCategoryId,
                Name = "Transportation",
                Type = "EXPENSE",
                Icon = "car",
                Color = "#4ECDC4",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = shoppingCategoryId,
                Name = "Shopping",
                Type = "EXPENSE",
                Icon = "shopping-cart",
                Color = "#45B7D1",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = entertainmentCategoryId,
                Name = "Entertainment",
                Type = "EXPENSE",
                Icon = "movie",
                Color = "#96CEB4",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = billsCategoryId,
                Name = "Bills & Utilities",
                Type = "EXPENSE",
                Icon = "receipt",
                Color = "#FFEAA7",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = healthcareCategoryId,
                Name = "Healthcare",
                Type = "EXPENSE",
                Icon = "medical",
                Color = "#DDA0DD",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = educationCategoryId,
                Name = "Education",
                Type = "EXPENSE",
                Icon = "graduation-cap",
                Color = "#98D8C8",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = travelCategoryId,
                Name = "Travel",
                Type = "EXPENSE",
                Icon = "plane",
                Color = "#F7DC6F",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = salaryCategoryId,
                Name = "Salary",
                Type = "INCOME",
                Icon = "dollar-sign",
                Color = "#2ECC71",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = freelanceCategoryId,
                Name = "Freelance",
                Type = "INCOME",
                Icon = "briefcase",
                Color = "#27AE60",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = investmentCategoryId,
                Name = "Investment Returns",
                Type = "INCOME",
                Icon = "trending-up",
                Color = "#16A085",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Category
            {
                Id = transferCategoryId,
                Name = "Transfer",
                Type = "TRANSFER",
                Icon = "exchange",
                Color = "#95A5A6",
                IsSystemDefined = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Bank Aggregators
        var demoAggregatorId = Guid.NewGuid();
        var openBankingId = Guid.NewGuid();

        modelBuilder.Entity<BankAggregator>().HasData(
            new BankAggregator
            {
                Id = demoAggregatorId,
                Name = "Demo Bank Connector",
                ApiEndpoint = "https://api.demo-bank.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new BankAggregator
            {
                Id = openBankingId,
                Name = "Simulated Open Banking",
                ApiEndpoint = "https://api.openbanking-sim.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );

        // Seed Merchants
        modelBuilder.Entity<Merchant>().HasData(
            new Merchant
            {
                Id = Guid.NewGuid(),
                Name = "Swiggy",
                CategoryId = foodCategoryId,
                MccCode = "5812",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Merchant
            {
                Id = Guid.NewGuid(),
                Name = "Zomato",
                CategoryId = foodCategoryId,
                MccCode = "5812",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Merchant
            {
                Id = Guid.NewGuid(),
                Name = "Uber",
                CategoryId = transportationCategoryId,
                MccCode = "4121",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Merchant
            {
                Id = Guid.NewGuid(),
                Name = "Amazon",
                CategoryId = shoppingCategoryId,
                MccCode = "5399",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Merchant
            {
                Id = Guid.NewGuid(),
                Name = "Netflix",
                CategoryId = entertainmentCategoryId,
                MccCode = "4899",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new Merchant
            {
                Id = Guid.NewGuid(),
                Name = "BSES Delhi",
                CategoryId = billsCategoryId,
                MccCode = "4900",
                IsVerified = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        );
    }
}
