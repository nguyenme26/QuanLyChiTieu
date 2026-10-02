using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuAPI.Data;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Models;

namespace QuanLyChiTieuAPI.Services
{
    public interface IWalletService
    {
        Task<List<WalletDto>> GetAllWalletsAsync(int userId);
        Task<WalletDto?> GetWalletByIdAsync(int userId, int walletId);
        Task<WalletDto> CreateWalletAsync(int userId, CreateWalletDto createDto);
        Task<WalletDto?> UpdateWalletAsync(int userId, int walletId, UpdateWalletDto updateDto);
        Task<bool> DeleteWalletAsync(int userId, int walletId);
        Task<bool> SetDefaultWalletAsync(int userId, int walletId);
        Task<WalletBalanceDto?> GetWalletBalanceAsync(int userId, int walletId);
        Task<bool> TransferAsync(int userId, TransferDto transferDto);
    }

    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _context;

        public WalletService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<WalletDto>> GetAllWalletsAsync(int userId)
        {
            return await _context.Wallets
                .Where(w => w.UserId == userId && w.IsActive)
                .OrderByDescending(w => w.IsDefault)
                .ThenBy(w => w.Name)
                .Select(w => new WalletDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    Balance = w.Balance,
                    Currency = w.Currency,
                    IsDefault = w.IsDefault,
                    IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<WalletDto?> GetWalletByIdAsync(int userId, int walletId)
        {
            return await _context.Wallets
                .Where(w => w.UserId == userId && w.Id == walletId && w.IsActive)
                .Select(w => new WalletDto
                {
                    Id = w.Id,
                    Name = w.Name,
                    Description = w.Description,
                    Balance = w.Balance,
                    Currency = w.Currency,
                    IsDefault = w.IsDefault,
                    IsActive = w.IsActive,
                    CreatedAt = w.CreatedAt,
                    UpdatedAt = w.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<WalletDto> CreateWalletAsync(int userId, CreateWalletDto createDto)
        {
            // If setting as default, unset other default wallets
            if (createDto.IsDefault)
            {
                var existingDefaults = await _context.Wallets
                    .Where(w => w.UserId == userId && w.IsDefault && w.IsActive)
                    .ToListAsync();
                
                foreach (var existingWallet in existingDefaults)
                {
                    existingWallet.IsDefault = false;
                    existingWallet.UpdatedAt = DateTime.UtcNow;
                }
            }

            var wallet = new Wallet
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Currency = createDto.Currency,
                IsDefault = createDto.IsDefault,
                Balance = 0,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();

            return new WalletDto
            {
                Id = wallet.Id,
                Name = wallet.Name,
                Description = wallet.Description,
                Balance = wallet.Balance,
                Currency = wallet.Currency,
                IsDefault = wallet.IsDefault,
                IsActive = wallet.IsActive,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            };
        }

        public async Task<WalletDto?> UpdateWalletAsync(int userId, int walletId, UpdateWalletDto updateDto)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == walletId && w.IsActive);

            if (wallet == null)
                return null;

            // If setting as default, unset other default wallets
            if (updateDto.IsDefault && !wallet.IsDefault)
            {
                var existingDefaults = await _context.Wallets
                    .Where(w => w.UserId == userId && w.IsDefault && w.IsActive && w.Id != walletId)
                    .ToListAsync();
                
                foreach (var existingWallet in existingDefaults)
                {
                    existingWallet.IsDefault = false;
                    existingWallet.UpdatedAt = DateTime.UtcNow;
                }
            }

            wallet.Name = updateDto.Name;
            wallet.Description = updateDto.Description;
            wallet.Currency = updateDto.Currency;
            wallet.IsDefault = updateDto.IsDefault;
            wallet.IsActive = updateDto.IsActive;
            wallet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new WalletDto
            {
                Id = wallet.Id,
                Name = wallet.Name,
                Description = wallet.Description,
                Balance = wallet.Balance,
                Currency = wallet.Currency,
                IsDefault = wallet.IsDefault,
                IsActive = wallet.IsActive,
                CreatedAt = wallet.CreatedAt,
                UpdatedAt = wallet.UpdatedAt
            };
        }

        public async Task<bool> DeleteWalletAsync(int userId, int walletId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == walletId && w.IsActive);

            if (wallet == null)
                return false;

            // Check if wallet has transactions
            var hasTransactions = await _context.Transactions
                .AnyAsync(t => t.WalletId == walletId);

            if (hasTransactions)
            {
                // Soft delete - mark as inactive
                wallet.IsActive = false;
                wallet.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Hard delete if no transactions
                _context.Wallets.Remove(wallet);
            }

            // If this was the default wallet, set another as default
            if (wallet.IsDefault)
            {
                var anotherWallet = await _context.Wallets
                    .Where(w => w.UserId == userId && w.IsActive && w.Id != walletId)
                    .FirstOrDefaultAsync();
                
                if (anotherWallet != null)
                {
                    anotherWallet.IsDefault = true;
                    anotherWallet.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetDefaultWalletAsync(int userId, int walletId)
        {
            var wallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == walletId && w.IsActive);

            if (wallet == null)
                return false;

            // Unset all other default wallets
            var existingDefaults = await _context.Wallets
                .Where(w => w.UserId == userId && w.IsDefault && w.IsActive && w.Id != walletId)
                .ToListAsync();
            
            foreach (var existingWallet in existingDefaults)
            {
                existingWallet.IsDefault = false;
                existingWallet.UpdatedAt = DateTime.UtcNow;
            }

            wallet.IsDefault = true;
            wallet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<WalletBalanceDto?> GetWalletBalanceAsync(int userId, int walletId)
        {
            return await _context.Wallets
                .Where(w => w.UserId == userId && w.Id == walletId && w.IsActive)
                .Select(w => new WalletBalanceDto
                {
                    WalletId = w.Id,
                    WalletName = w.Name,
                    Balance = w.Balance,
                    Currency = w.Currency
                })
                .FirstOrDefaultAsync();
        }

        public async Task<bool> TransferAsync(int userId, TransferDto transferDto)
        {
            // Validate wallets belong to user
            var fromWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == transferDto.FromWalletId && w.IsActive);
            
            var toWallet = await _context.Wallets
                .FirstOrDefaultAsync(w => w.UserId == userId && w.Id == transferDto.ToWalletId && w.IsActive);

            if (fromWallet == null || toWallet == null)
                return false;

            if (fromWallet.Id == toWallet.Id)
                return false;

            if (transferDto.Amount <= 0)
                return false;

            if (fromWallet.Balance < transferDto.Amount)
                return false; // Insufficient funds

            // Perform transfer
            fromWallet.Balance -= transferDto.Amount;
            fromWallet.UpdatedAt = DateTime.UtcNow;

            toWallet.Balance += transferDto.Amount;
            toWallet.UpdatedAt = DateTime.UtcNow;

            // Create transaction records for both wallets
            var transferCategory = await _context.Categories
                .FirstOrDefaultAsync(c => c.IsSystem && c.Name == "Khác (Chi)" && c.IsActive);

            var categoryId = transferCategory?.Id ?? 12; // Default to "Khác (Chi)"

            var outgoingTransaction = new Transaction
            {
                Amount = transferDto.Amount,
                Note = transferDto.Note ?? $"Chuyển tới {toWallet.Name}",
                TransactionDate = DateTime.UtcNow,
                IsIncome = false,
                WalletId = fromWallet.Id,
                CategoryId = categoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var incomingTransaction = new Transaction
            {
                Amount = transferDto.Amount,
                Note = transferDto.Note ?? $"Nhận từ {fromWallet.Name}",
                TransactionDate = DateTime.UtcNow,
                IsIncome = true,
                WalletId = toWallet.Id,
                CategoryId = categoryId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Transactions.AddRange(outgoingTransaction, incomingTransaction);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
