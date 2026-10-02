using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuAPI.Data;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Models;

namespace QuanLyChiTieuAPI.Services
{
    public interface IBudgetService
    {
        Task<List<BudgetDto>> GetAllBudgetsAsync(int userId);
        Task<BudgetDto?> GetBudgetByIdAsync(int userId, int budgetId);
        Task<BudgetDto> CreateBudgetAsync(int userId, CreateBudgetDto createDto);
        Task<BudgetDto?> UpdateBudgetAsync(int userId, int budgetId, UpdateBudgetDto updateDto);
        Task<bool> DeleteBudgetAsync(int userId, int budgetId);
        Task<List<BudgetAlertDto>> GetBudgetAlertsAsync(int userId);
        Task<BudgetDto?> RecalculateBudgetAsync(int userId, int budgetId);
    }

    public class BudgetService : IBudgetService
    {
        private readonly ApplicationDbContext _context;

        public BudgetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BudgetDto>> GetAllBudgetsAsync(int userId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive)
                .Include(b => b.Category)
                .Include(b => b.Wallet)
                .OrderByDescending(b => b.CreatedAt)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Amount = b.Amount,
                    SpentAmount = b.SpentAmount,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category!.Name,
                    CategoryIcon = b.Category.Icon ?? string.Empty,
                    CategoryColor = b.Category.Color ?? string.Empty,
                    WalletId = b.WalletId,
                    WalletName = b.Wallet != null ? b.Wallet.Name : null,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<BudgetDto?> GetBudgetByIdAsync(int userId, int budgetId)
        {
            return await _context.Budgets
                .Where(b => b.UserId == userId && b.Id == budgetId && b.IsActive)
                .Include(b => b.Category)
                .Include(b => b.Wallet)
                .Select(b => new BudgetDto
                {
                    Id = b.Id,
                    Name = b.Name,
                    Amount = b.Amount,
                    SpentAmount = b.SpentAmount,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    CategoryId = b.CategoryId,
                    CategoryName = b.Category!.Name,
                    CategoryIcon = b.Category.Icon ?? string.Empty,
                    CategoryColor = b.Category.Color ?? string.Empty,
                    WalletId = b.WalletId,
                    WalletName = b.Wallet != null ? b.Wallet.Name : null,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BudgetDto> CreateBudgetAsync(int userId, CreateBudgetDto createDto)
        {
            // Validate category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == createDto.CategoryId && (c.UserId == userId || c.IsSystem) && c.IsActive && !c.IsIncome);
            
            if (category == null)
                throw new InvalidOperationException("Danh mục chi tiêu không tồn tại");

            // Validate wallet if provided
            if (createDto.WalletId.HasValue)
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.Id == createDto.WalletId.Value && w.UserId == userId && w.IsActive);
                
                if (wallet == null)
                    throw new InvalidOperationException("Ví không tồn tại hoặc không thuộc về bạn");
            }

            // Calculate current spent amount for this period
            var spentAmount = await CalculateSpentAmountAsync(userId, createDto.CategoryId, createDto.WalletId, createDto.StartDate, createDto.EndDate);

            var budget = new Budget
            {
                Name = createDto.Name,
                Amount = createDto.Amount,
                SpentAmount = spentAmount,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                CategoryId = createDto.CategoryId,
                WalletId = createDto.WalletId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Budgets.Add(budget);
            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Amount = budget.Amount,
                SpentAmount = budget.SpentAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CategoryId = budget.CategoryId,
                CategoryName = category.Name,
                CategoryIcon = category.Icon ?? string.Empty,
                CategoryColor = category.Color ?? string.Empty,
                WalletId = budget.WalletId,
                WalletName = null,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };
        }

        public async Task<BudgetDto?> UpdateBudgetAsync(int userId, int budgetId, UpdateBudgetDto updateDto)
        {
            var budget = await _context.Budgets
                .Include(b => b.Category)
                .Include(b => b.Wallet)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId && b.IsActive);

            if (budget == null)
                return null;

            // Validate category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == updateDto.CategoryId && (c.UserId == userId || c.IsSystem) && c.IsActive && !c.IsIncome);
            
            if (category == null)
                throw new InvalidOperationException("Danh mục chi tiêu không tồn tại");

            // Validate wallet if provided
            if (updateDto.WalletId.HasValue)
            {
                var wallet = await _context.Wallets
                    .FirstOrDefaultAsync(w => w.Id == updateDto.WalletId.Value && w.UserId == userId && w.IsActive);
                
                if (wallet == null)
                    throw new InvalidOperationException("Ví không tồn tại hoặc không thuộc về bạn");
            }

            budget.Name = updateDto.Name;
            budget.Amount = updateDto.Amount;
            budget.StartDate = updateDto.StartDate;
            budget.EndDate = updateDto.EndDate;
            budget.CategoryId = updateDto.CategoryId;
            budget.WalletId = updateDto.WalletId;
            budget.IsActive = updateDto.IsActive;
            budget.UpdatedAt = DateTime.UtcNow;

            // Recalculate spent amount
            budget.SpentAmount = await CalculateSpentAmountAsync(userId, updateDto.CategoryId, updateDto.WalletId, updateDto.StartDate, updateDto.EndDate);

            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Amount = budget.Amount,
                SpentAmount = budget.SpentAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CategoryId = budget.CategoryId,
                CategoryName = category.Name,
                CategoryIcon = category.Icon ?? string.Empty,
                CategoryColor = category.Color ?? string.Empty,
                WalletId = budget.WalletId,
                WalletName = budget.Wallet?.Name,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };
        }

        public async Task<bool> DeleteBudgetAsync(int userId, int budgetId)
        {
            var budget = await _context.Budgets
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId && b.IsActive);

            if (budget == null)
                return false;

            budget.IsActive = false;
            budget.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BudgetAlertDto>> GetBudgetAlertsAsync(int userId)
        {
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive)
                .Include(b => b.Category)
                .ToListAsync();

            var alerts = new List<BudgetAlertDto>();

            foreach (var budget in budgets)
            {
                // Recalculate spent amount
                var spentAmount = await CalculateSpentAmountAsync(userId, budget.CategoryId, budget.WalletId, budget.StartDate, budget.EndDate);
                
                if (spentAmount != budget.SpentAmount)
                {
                    budget.SpentAmount = spentAmount;
                    budget.UpdatedAt = DateTime.UtcNow;
                }

                var percentage = budget.Amount > 0 ? (double)(spentAmount / budget.Amount * 100) : 0;

                if (spentAmount > budget.Amount)
                {
                    alerts.Add(new BudgetAlertDto
                    {
                        BudgetId = budget.Id,
                        BudgetName = budget.Name,
                        CategoryName = budget.Category!.Name,
                        Amount = budget.Amount,
                        SpentAmount = spentAmount,
                        PercentageUsed = percentage,
                        AlertType = "OVER_BUDGET",
                        Message = $"Đã vượt ngân sách: {spentAmount:N0} / {budget.Amount:N0} VND ({percentage:F1}%)"
                    });
                }
                else if (spentAmount >= budget.Amount * 0.8m)
                {
                    alerts.Add(new BudgetAlertDto
                    {
                        BudgetId = budget.Id,
                        BudgetName = budget.Name,
                        CategoryName = budget.Category!.Name,
                        Amount = budget.Amount,
                        SpentAmount = spentAmount,
                        PercentageUsed = percentage,
                        AlertType = "WARNING",
                        Message = $"Sắp vượt ngân sách: {spentAmount:N0} / {budget.Amount:N0} VND ({percentage:F1}%)"
                    });
                }
            }

            if (alerts.Any(b => b.SpentAmount != b.Amount)) // Check if any budget was updated
            {
                await _context.SaveChangesAsync();
            }

            return alerts.OrderByDescending(a => a.PercentageUsed).ToList();
        }

        public async Task<BudgetDto?> RecalculateBudgetAsync(int userId, int budgetId)
        {
            var budget = await _context.Budgets
                .Include(b => b.Category)
                .Include(b => b.Wallet)
                .FirstOrDefaultAsync(b => b.UserId == userId && b.Id == budgetId && b.IsActive);

            if (budget == null)
                return null;

            var spentAmount = await CalculateSpentAmountAsync(userId, budget.CategoryId, budget.WalletId, budget.StartDate, budget.EndDate);
            budget.SpentAmount = spentAmount;
            budget.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new BudgetDto
            {
                Id = budget.Id,
                Name = budget.Name,
                Amount = budget.Amount,
                SpentAmount = budget.SpentAmount,
                StartDate = budget.StartDate,
                EndDate = budget.EndDate,
                CategoryId = budget.CategoryId,
                CategoryName = budget.Category!.Name,
                CategoryIcon = budget.Category.Icon ?? string.Empty,
                CategoryColor = budget.Category.Color ?? string.Empty,
                WalletId = budget.WalletId,
                WalletName = budget.Wallet?.Name,
                CreatedAt = budget.CreatedAt,
                UpdatedAt = budget.UpdatedAt
            };
        }

        private async Task<decimal> CalculateSpentAmountAsync(int userId, int categoryId, int? walletId, DateTime startDate, DateTime endDate)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId 
                    && t.CategoryId == categoryId 
                    && !t.IsIncome
                    && t.TransactionDate >= startDate 
                    && t.TransactionDate <= endDate);

            if (walletId.HasValue)
                query = query.Where(t => t.WalletId == walletId.Value);

            return await query.SumAsync(t => t.Amount);
        }
    }
}
