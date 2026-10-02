using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuAPI.Data;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Models;

namespace QuanLyChiTieuAPI.Services
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllCategoriesAsync(int userId, bool? isIncome = null);
        Task<CategoryDto?> GetCategoryByIdAsync(int userId, int categoryId);
        Task<CategoryDto> CreateCategoryAsync(int userId, CreateCategoryDto createDto);
        Task<CategoryDto?> UpdateCategoryAsync(int userId, int categoryId, UpdateCategoryDto updateDto);
        Task<bool> DeleteCategoryAsync(int userId, int categoryId);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(int userId, bool? isIncome = null)
        {
            var query = _context.Categories
                .Where(c => (c.UserId == userId || c.IsSystem) && c.IsActive);

            if (isIncome.HasValue)
            {
                query = query.Where(c => c.IsIncome == isIncome.Value);
            }

            return await query
                .OrderBy(c => c.IsSystem ? 0 : 1)
                .ThenBy(c => c.Name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Icon = c.Icon ?? string.Empty,
                    Color = c.Color ?? string.Empty,
                    IsIncome = c.IsIncome,
                    IsSystem = c.IsSystem,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int userId, int categoryId)
        {
            return await _context.Categories
                .Where(c => (c.UserId == userId || c.IsSystem) && c.Id == categoryId && c.IsActive)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    Icon = c.Icon ?? string.Empty,
                    Color = c.Color ?? string.Empty,
                    IsIncome = c.IsIncome,
                    IsSystem = c.IsSystem,
                    IsActive = c.IsActive,
                    CreatedAt = c.CreatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryDto> CreateCategoryAsync(int userId, CreateCategoryDto createDto)
        {
            var category = new Category
            {
                Name = createDto.Name,
                Description = createDto.Description,
                Icon = createDto.Icon ?? string.Empty,
                Color = createDto.Color ?? "#000000",
                IsIncome = createDto.IsIncome,
                IsSystem = false,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Icon = category.Icon,
                Color = category.Color,
                IsIncome = category.IsIncome,
                IsSystem = category.IsSystem,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryDto?> UpdateCategoryAsync(int userId, int categoryId, UpdateCategoryDto updateDto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == categoryId && !c.IsSystem && c.IsActive);

            if (category == null)
                return null;

            category.Name = updateDto.Name;
            category.Description = updateDto.Description;
            category.Icon = updateDto.Icon ?? string.Empty;
            category.Color = updateDto.Color ?? "#000000";
            category.IsIncome = updateDto.IsIncome;
            category.IsActive = updateDto.IsActive;
            category.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Icon = category.Icon,
                Color = category.Color,
                IsIncome = category.IsIncome,
                IsSystem = category.IsSystem,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<bool> DeleteCategoryAsync(int userId, int categoryId)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == categoryId && !c.IsSystem && c.IsActive);

            if (category == null)
                return false;

            // Check if category has transactions
            var hasTransactions = await _context.Transactions
                .AnyAsync(t => t.CategoryId == categoryId);

            if (hasTransactions)
            {
                // Soft delete
                category.IsActive = false;
                category.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                // Hard delete
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
