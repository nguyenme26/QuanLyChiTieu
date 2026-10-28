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

        private void SeedSampleData(ModelBuilder modelBuilder)
        {
            var demoPasswordHash = BCrypt.Net.BCrypt.HashPassword("demo123");
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            // Sample user
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    FullName = "Demo User",
                    Email = "demo@example.com",
                    PasswordHash = demoPasswordHash,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // Sample wallets
            modelBuilder.Entity<Wallet>().HasData(
                new Wallet { Id = 1, Name = "Tiền mặt", Balance = 5000000, Currency = "VND", IsDefault = true, UserId = 1, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Wallet { Id = 2, Name = "Thẻ Ngân hàng", Balance = 20000000, Currency = "VND", IsDefault = false, UserId = 1, CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Sample budgets
            modelBuilder.Entity<Budget>().HasData(
                new Budget { Id = 1, Name = "Dự phòng", Amount = 10000000, CategoryId = 1, WalletId = 1, UserId = 1, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Budget { Id = 2, Name = "Ăn uống", Amount = 5000000, CategoryId = 5, WalletId = 1, UserId = 1, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Budget { Id = 3, Name = "Mua sắm", Amount = 8000000, CategoryId = 7, WalletId = 2, UserId = 1, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Budget { Id = 4, Name = "Giáo dục", Amount = 3000000, CategoryId = 10, WalletId = 2, UserId = 1, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Sample transactions
            var today = DateTime.Today;
            var transactions = new List<Transaction>
            {
                // Income - Lương (Wallet 1: Tiền mặt)
                new Transaction { Id = 1, Amount = 10000000, Note = "Lương tháng 10", IsIncome = true, WalletId = 1, CategoryId = 1, UserId = 1, TransactionDate = today.AddDays(-5), CreatedAt = today, UpdatedAt = today },
                new Transaction { Id = 2, Amount = 3000000, Note = "Thưởng", IsIncome = true, WalletId = 1, CategoryId = 2, UserId = 1, TransactionDate = today.AddDays(-2), CreatedAt = today, UpdatedAt = today },

                // Expense - Ăn uống (Wallet 1: Tiền mặt)
                new Transaction { Id = 3, Amount = 850000, Note = "Ăn uống", IsIncome = false, WalletId = 1, CategoryId = 5, UserId = 1, TransactionDate = today.AddDays(-4), CreatedAt = today, UpdatedAt = today },
                new Transaction { Id = 4, Amount = 620000, Note = "Ăn uống", IsIncome = false, WalletId = 1, CategoryId = 5, UserId = 1, TransactionDate = today.AddDays(-3), CreatedAt = today, UpdatedAt = today },

                // Expense - Mua sắm (Wallet 2: Thẻ Ngân hàng)
                new Transaction { Id = 5, Amount = 2500000, Note = "Mua sắm", IsIncome = false, WalletId = 2, CategoryId = 7, UserId = 1, TransactionDate = today.AddDays(-6), CreatedAt = today, UpdatedAt = today },
                new Transaction { Id = 6, Amount = 1800000, Note = "Mua sắm", IsIncome = false, WalletId = 2, CategoryId = 7, UserId = 1, TransactionDate = today.AddDays(-1), CreatedAt = today, UpdatedAt = today },

                // Expense - Di chuyển
                new Transaction { Id = 7, Amount = 450000, Note = "Di chuyển", IsIncome = false, WalletId = 1, CategoryId = 6, UserId = 1, TransactionDate = today.AddDays(-2), CreatedAt = today, UpdatedAt = today },

                // Expense - Tiện ích
                new Transaction { Id = 8, Amount = 1200000, Note = "Tiện ích", IsIncome = false, WalletId = 2, CategoryId = 11, UserId = 1, TransactionDate = today.AddDays(-1), CreatedAt = today, UpdatedAt = today }
            };

            foreach (var t in transactions)
            {
                modelBuilder.Entity<Transaction>().HasData(t);
            }

            // Seed user account: ngienme892@gmail.com
            SeedUserAccountData(modelBuilder);

            // Seed random transactions for reporting (2024-2026)
            SeedRandomTransactions(modelBuilder);
        }

        private void SeedUserAccountData(ModelBuilder modelBuilder)
        {
            var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("nct071126");

            // Create user ngienme892@gmail.com
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 2,
                    FullName = "Nguyen Van A",
                    Email = "nguyenme892@gmail.com",
                    PasswordHash = passwordHash,
                    IsActive = true,
                    CreatedAt = seedDate,
                    UpdatedAt = seedDate
                }
            );

            // Create 3 wallets for nguyenme892@gmail.com
            modelBuilder.Entity<Wallet>().HasData(
                new Wallet { Id = 3, Name = "Tiền mặt", Balance = 15000000, Currency = "VND", IsDefault = true, UserId = 2, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Wallet { Id = 4, Name = "Tài khoản ngân hàng", Balance = 85000000, Currency = "VND", IsDefault = false, UserId = 2, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Wallet { Id = 5, Name = "Thẻ tín dụng", Balance = -5000000, Currency = "VND", IsDefault = false, UserId = 2, CreatedAt = seedDate, UpdatedAt = seedDate }
            );

            // Create 3 budgets for nguyenme892@gmail.com
            modelBuilder.Entity<Budget>().HasData(
                new Budget { Id = 5, Name = "Dự phòng", Amount = 50000000, CategoryId = 1, WalletId = 3, UserId = 2, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Budget { Id = 6, Name = "Ăn uống", Amount = 3000000, CategoryId = 5, WalletId = 3, UserId = 2, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate },
                new Budget { Id = 7, Name = "Mua sắm", Amount = 5000000, CategoryId = 7, WalletId = 4, UserId = 2, IsActive = true, CreatedAt = seedDate, UpdatedAt = seedDate }
            );
        }

        private void SeedRandomTransactions(ModelBuilder modelBuilder)
        {
            var rng = new Random(42); // Fixed seed for reproducibility
            var categories = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
            var wallets = new int[] { 3, 4, 5 };  // Wallets for nguyenme892@gmail.com (Ids 3, 4, 5)
            var isIncomes = new bool[] { true, false };
            var noteSuffixes = new string[]
            {
                "tháng 1", "tháng 2", "tháng 3", "tháng 4", "tháng 5", "tháng 6",
                "tháng 7", "tháng 8", "tháng 9", "tháng 10", "tháng 11", "tháng 12"
            };

            // Generate 500-800 random transactions from 2024 to 2026
            var totalTransactions = rng.Next(500, 801);
            var count = 0;
            var startDate = new DateTime(2024, 1, 1);
            var endDate = new DateTime(2026, 12, 31);

            for (var i = 0; i < totalTransactions; i++)
            {
                // Random date between 2024 and 2026
                var yearOffset = rng.Next(0, 3);
                var month = rng.Next(1, 13);
                var day = rng.Next(1, 29);
                var date = new DateTime(2024 + yearOffset, month, day > 28 ? 28 : day);

                // Random category
                var categoryId = categories[rng.Next(categories.Length)];

                // Random wallet (3, 4, 5)
                var walletId = wallets[rng.Next(wallets.Length)];

                // Random amount
                var isIncome = isIncomes[rng.Next(isIncomes.Length)];
                var amount = isIncome
                    ? rng.Next(2000000, 30000001)  // Income: 2M - 30M VND
                    : rng.Next(20000, 5000001);    // Expense: 20K - 5M VND

                // Random note
                var noteSuffix = noteSuffixes[rng.Next(noteSuffixes.Length)];
                var note = isIncome ? "Thu nhập" : "Chi tiêu";

                var transaction = new Transaction
                {
                    Id = 2000 + i,
                    Amount = amount,
                    Note = note + " " + noteSuffix,
                    IsIncome = isIncome,
                    WalletId = walletId,
                    CategoryId = categoryId,
                    UserId = 2,  // This is for nguyenme892@gmail.com (UserId = 2)
                    TransactionDate = date,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                modelBuilder.Entity<Transaction>().HasData(transaction);
                count++;
            }
        }
    }
}
