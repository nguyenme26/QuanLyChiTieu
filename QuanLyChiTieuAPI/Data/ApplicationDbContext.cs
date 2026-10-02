using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuAPI.Models;

namespace QuanLyChiTieuAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Wallet> Wallets { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Transaction> Transactions { get; set; } = null!;
        public DbSet<Budget> Budgets { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
            });

            // Wallet configuration
            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.IsDefault }).HasFilter("[IsDefault] = 1").IsUnique();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.Balance).HasDefaultValue(0);

                entity.HasOne(w => w.User)
                    .WithMany(u => u.Wallets)
                    .HasForeignKey(w => w.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Category configuration
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

                entity.HasOne(c => c.User)
                    .WithMany(u => u.Categories)
                    .HasForeignKey(c => c.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Transaction configuration
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.TransactionDate).HasDefaultValueSql("GETUTCDATE()");
                
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.WalletId);
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => e.TransactionDate);

                entity.HasOne(t => t.User)
                    .WithMany(u => u.Transactions)
                    .HasForeignKey(t => t.UserId)
                    .OnDelete(DeleteBehavior.NoAction); // Avoid multiple cascade paths

                entity.HasOne(t => t.Wallet)
                    .WithMany(w => w.Transactions)
                    .HasForeignKey(t => t.WalletId)
                    .OnDelete(DeleteBehavior.NoAction); // Avoid multiple cascade paths

                entity.HasOne(t => t.Category)
                    .WithMany(c => c.Transactions)
                    .HasForeignKey(t => t.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            // Budget configuration
            modelBuilder.Entity<Budget>(entity =>
            {
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                entity.Property(e => e.SpentAmount).HasDefaultValue(0);
                
                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.CategoryId);
                entity.HasIndex(e => new { e.StartDate, e.EndDate });

                entity.HasOne(b => b.User)
                    .WithMany(u => u.Budgets)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.NoAction); // Avoid multiple cascade paths

                entity.HasOne(b => b.Category)
                    .WithMany(c => c.Budgets)
                    .HasForeignKey(b => b.CategoryId)
                    .OnDelete(DeleteBehavior.NoAction); // Avoid multiple cascade paths

                entity.HasOne(b => b.Wallet)
                    .WithMany()
                    .HasForeignKey(b => b.WalletId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            // Seed data for system categories
            SeedData(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Suppress the warning about dynamic values in HasData
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
            base.OnConfiguring(optionsBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            // System categories for income
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Lương", Description = "Thu nhập từ lương", Icon = "💰", Color = "#4CAF50", IsIncome = true, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 2, Name = "Thưởng", Description = "Thu nhập từ thưởng", Icon = "🎁", Color = "#8BC34A", IsIncome = true, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 3, Name = "Đầu tư", Description = "Thu nhập từ đầu tư", Icon = "📈", Color = "#CDDC39", IsIncome = true, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 4, Name = "Khác (Thu)", Description = "Thu nhập khác", Icon = "💵", Color = "#FFEB3B", IsIncome = true, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },

                // System categories for expense
                new Category { Id = 5, Name = "Ăn uống", Description = "Chi tiêu ăn uống", Icon = "🍔", Color = "#FF5722", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 6, Name = "Di chuyển", Description = "Chi tiêu di chuyển", Icon = "🚌", Color = "#FF9800", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 7, Name = "Mua sắm", Description = "Chi tiêu mua sắm", Icon = "🛍️", Color = "#E91E63", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 8, Name = "Giải trí", Description = "Chi tiêu giải trí", Icon = "🎮", Color = "#9C27B0", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 9, Name = "Y tế", Description = "Chi tiêu y tế", Icon = "🏥", Color = "#F44336", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 10, Name = "Giáo dục", Description = "Chi tiêu giáo dục", Icon = "📚", Color = "#3F51B5", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 11, Name = "Tiện ích", Description = "Chi tiêu tiện ích", Icon = "💡", Color = "#2196F3", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Category { Id = 12, Name = "Khác (Chi)", Description = "Chi tiêu khác", Icon = "💸", Color = "#607D8B", IsIncome = false, IsSystem = true, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );
        }
    }
}
