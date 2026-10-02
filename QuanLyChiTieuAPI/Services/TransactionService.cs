using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuAPI.Data;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Models;

namespace QuanLyChiTieuAPI.Services
{
    public interface ITransactionService
    {
        Task<PagedResultDto<TransactionDto>> GetTransactionsAsync(int userId, TransactionFilterDto filter);
        Task<TransactionDto?> GetTransactionByIdAsync(int userId, int transactionId);
        Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionDto createDto);
        Task<TransactionDto?> UpdateTransactionAsync(int userId, int transactionId, UpdateTransactionDto updateDto);
        Task<bool> DeleteTransactionAsync(int userId, int transactionId);
        Task<decimal> GetTotalIncomeAsync(int userId, DateTime startDate, DateTime endDate, int? walletId = null);
        Task<decimal> GetTotalExpenseAsync(int userId, DateTime startDate, DateTime endDate, int? walletId = null);
    }

    public class TransactionService : ITransactionService
    {
        private readonly ApplicationDbContext _context;

        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PagedResultDto<TransactionDto>> GetTransactionsAsync(int userId, TransactionFilterDto filter)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId);

            if (filter.StartDate.HasValue)
                query = query.Where(t => t.TransactionDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(t => t.TransactionDate <= filter.EndDate.Value);

            if (filter.WalletId.HasValue)
                query = query.Where(t => t.WalletId == filter.WalletId.Value);

            if (filter.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == filter.CategoryId.Value);

            if (filter.IsIncome.HasValue)
                query = query.Where(t => t.IsIncome == filter.IsIncome.Value);

            var totalCount = await query.CountAsync();

            var items = await query
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .OrderByDescending(t => t.TransactionDate)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Note = t.Note,
                    TransactionDate = t.TransactionDate,
                    IsIncome = t.IsIncome,
                    WalletId = t.WalletId,
                    WalletName = t.Wallet!.Name,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category!.Name,
                    CategoryIcon = t.Category.Icon ?? string.Empty,
                    CategoryColor = t.Category.Color ?? string.Empty,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return new PagedResultDto<TransactionDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = filter.Page,
                PageSize = filter.PageSize
            };
        }

        public async Task<TransactionDto?> GetTransactionByIdAsync(int userId, int transactionId)
        {
            return await _context.Transactions
                .Where(t => t.UserId == userId && t.Id == transactionId)
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    Amount = t.Amount,
                    Note = t.Note,
                    TransactionDate = t.TransactionDate,
                    IsIncome = t.IsIncome,
                    WalletId = t.WalletId,
                    WalletName = t.Wallet!.Name,
                    CategoryId = t.CategoryId,
                    CategoryName = t.Category!.Name,
                    CategoryIcon = t.Category.Icon ?? string.Empty,
                    CategoryColor = t.Category.Color ?? string.Empty,
                    CreatedAt = t.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionDto createDto)
        {
            // Validate wallet belongs to user
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == createDto.WalletId && w.UserId == userId && w.IsActive);
            
            if (wallet == null)
                throw new InvalidOperationException("Ví không tồn tại hoặc không thuộc về bạn");

            // Validate category
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == createDto.CategoryId && (c.UserId == userId || c.IsSystem) && c.IsActive);
            
            if (category == null)
                throw new InvalidOperationException("Danh mục không tồn tại");

            var transaction = new Transaction
            {
                Amount = createDto.Amount,
                Note = createDto.Note,
                TransactionDate = createDto.TransactionDate,
                IsIncome = createDto.IsIncome,
                WalletId = createDto.WalletId,
                CategoryId = createDto.CategoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);

            // Update wallet balance
            if (createDto.IsIncome)
                wallet.Balance += createDto.Amount;
            else
                wallet.Balance -= createDto.Amount;

            wallet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Note = transaction.Note,
                TransactionDate = transaction.TransactionDate,
                IsIncome = transaction.IsIncome,
                WalletId = transaction.WalletId,
                WalletName = wallet.Name,
                CategoryId = transaction.CategoryId,
                CategoryName = category.Name,
                CategoryIcon = category.Icon ?? string.Empty,
                CategoryColor = category.Color ?? string.Empty,
                CreatedAt = transaction.CreatedAt
            };
        }

        public async Task<TransactionDto?> UpdateTransactionAsync(int userId, int transactionId, UpdateTransactionDto updateDto)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == transactionId);

            if (transaction == null)
                return null;

            // Validate new wallet
            var newWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.Id == updateDto.WalletId && w.UserId == userId && w.IsActive);
            
            if (newWallet == null)
                throw new InvalidOperationException("Ví không tồn tại hoặc không thuộc về bạn");

            // Validate new category
            var newCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == updateDto.CategoryId && (c.UserId == userId || c.IsSystem) && c.IsActive);
            
            if (newCategory == null)
                throw new InvalidOperationException("Danh mục không tồn tại");

            // Revert old wallet balance
            if (transaction.IsIncome)
                transaction.Wallet!.Balance -= transaction.Amount;
            else
                transaction.Wallet!.Balance += transaction.Amount;
            
            transaction.Wallet!.UpdatedAt = DateTime.UtcNow;

            // Apply new transaction
            transaction.Amount = updateDto.Amount;
            transaction.Note = updateDto.Note;
            transaction.TransactionDate = updateDto.TransactionDate;
            transaction.IsIncome = updateDto.IsIncome;
            transaction.WalletId = updateDto.WalletId;
            transaction.CategoryId = updateDto.CategoryId;
            transaction.UpdatedAt = DateTime.UtcNow;

            // Update new wallet balance
            if (updateDto.IsIncome)
                newWallet.Balance += updateDto.Amount;
            else
                newWallet.Balance -= updateDto.Amount;
            
            newWallet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new TransactionDto
            {
                Id = transaction.Id,
                Amount = transaction.Amount,
                Note = transaction.Note,
                TransactionDate = transaction.TransactionDate,
                IsIncome = transaction.IsIncome,
                WalletId = transaction.WalletId,
                WalletName = newWallet.Name,
                CategoryId = transaction.CategoryId,
                CategoryName = newCategory.Name,
                CategoryIcon = newCategory.Icon ?? string.Empty,
                CategoryColor = newCategory.Color ?? string.Empty,
                CreatedAt = transaction.CreatedAt
            };
        }

        public async Task<bool> DeleteTransactionAsync(int userId, int transactionId)
        {
            var transaction = await _context.Transactions
                .Include(t => t.Wallet)
                .FirstOrDefaultAsync(t => t.UserId == userId && t.Id == transactionId);

            if (transaction == null)
                return false;

            // Revert wallet balance
            if (transaction.IsIncome)
                transaction.Wallet!.Balance -= transaction.Amount;
            else
                transaction.Wallet!.Balance += transaction.Amount;
            
            transaction.Wallet!.UpdatedAt = DateTime.UtcNow;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetTotalIncomeAsync(int userId, DateTime startDate, DateTime endDate, int? walletId = null)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId && t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate <= endDate);

            if (walletId.HasValue)
                query = query.Where(t => t.WalletId == walletId.Value);

            return await query.SumAsync(t => t.Amount);
        }

        public async Task<decimal> GetTotalExpenseAsync(int userId, DateTime startDate, DateTime endDate, int? walletId = null)
        {
            var query = _context.Transactions
                .Where(t => t.UserId == userId && !t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate <= endDate);

            if (walletId.HasValue)
                query = query.Where(t => t.WalletId == walletId.Value);

            return await query.SumAsync(t => t.Amount);
        }
    }
}
