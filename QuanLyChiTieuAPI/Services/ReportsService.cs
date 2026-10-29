using System.Globalization;
using Microsoft.EntityFrameworkCore;
using QuanLyChiTieuAPI.Data;
using QuanLyChiTieuAPI.DTOs;
using QuanLyChiTieuAPI.Models;

namespace QuanLyChiTieuAPI.Services
{
    public interface IReportsService
    {
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(int userId);
        Task<MonthlyReportDto> GetMonthlyReportAsync(int userId, int year, int month);
        Task<YearlyReportDto> GetYearlyReportAsync(int userId, int year);
        Task<ExportResultDto> GetExportReportAsync(int userId, ExportRequestDto request);
    }

    public class ReportsService : IReportsService
    {
        private readonly ApplicationDbContext _context;

        public ReportsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(int userId)
        {
            // Default to current month to date
            var startOfPeriod = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endOfPeriod = DateTime.UtcNow;

            var totalIncome = await GetTotalAmountAsync(userId, startOfPeriod, endOfPeriod, isIncome: true);
            var totalExpense = await GetTotalAmountAsync(userId, startOfPeriod, endOfPeriod, isIncome: false);

            var wallets = await _context.Wallets
                .Where(w => w.UserId == userId && w.IsActive)
                .Select(w => new WalletBalanceDto
                {
                    WalletId = w.Id,
                    WalletName = w.Name,
                    Balance = w.Balance,
                    Currency = w.Currency
                })
                .ToListAsync();

            var totalTransactions = await _context.Transactions.CountAsync(t => t.UserId == userId);
            var activeBudgets = await _context.Budgets.CountAsync(b => b.UserId == userId && b.IsActive);

            // Manual budget alerts
            var budgetAlertsList = new List<BudgetAlertDto>();
            var budgets = await _context.Budgets
                .Where(b => b.UserId == userId && b.IsActive)
                .Include(b => b.Category)
                .ToListAsync();

            foreach (var budget in budgets)
            {
                var spentAmount = await GetTotalAmountAsync(userId, budget.StartDate, budget.EndDate, isIncome: false);
                var percentage = budget.Amount > 0 ? (double)(spentAmount / budget.Amount * 100) : 0;

                if (spentAmount > budget.Amount)
                {
                    budgetAlertsList.Add(new BudgetAlertDto
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
                    budgetAlertsList.Add(new BudgetAlertDto
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

            return new DashboardSummaryDto
            {
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                TotalWallets = wallets.Count,
                TotalTransactions = totalTransactions,
                ActiveBudgets = activeBudgets,
                BudgetAlerts = budgetAlertsList,
                WalletBalances = wallets
            };
        }

        public async Task<MonthlyReportDto> GetMonthlyReportAsync(int userId, int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            // Load all transactions for this user in one query
            var allTransactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionDate >= startDate && t.TransactionDate < endDate)
                .ToListAsync();

            // Load categories for the transactions
            var categoryIds = allTransactions.Select(t => t.CategoryId).Where(id => id != 0).Distinct().ToList();
            var categories = await _context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            var categoryLookup = categories.ToDictionary(c => c.Id, c => c);

            var totalIncome = allTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            var totalExpense = allTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

            var incomeByCategory = allTransactions
                .Where(t => t.IsIncome)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = categoryLookup.TryGetValue(g.Key, out var cat) ? cat.Name : "Unknown",
                    CategoryIcon = categoryLookup.TryGetValue(g.Key, out var c) ? c.Icon ?? string.Empty : string.Empty,
                    CategoryColor = categoryLookup.TryGetValue(g.Key, out var c2) ? c2.Color ?? string.Empty : string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .ToList();

            var expenseByCategory = allTransactions
                .Where(t => !t.IsIncome)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = categoryLookup.TryGetValue(g.Key, out var cat) ? cat.Name : "Unknown",
                    CategoryIcon = categoryLookup.TryGetValue(g.Key, out var c) ? c.Icon ?? string.Empty : string.Empty,
                    CategoryColor = categoryLookup.TryGetValue(g.Key, out var c2) ? c2.Color ?? string.Empty : string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .ToList();

            var days = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .ToList();

            var dailyBalances = new List<DailyBalanceDto>();
            foreach (var d in days)
            {
                var dayStart = new DateTime(year, month, d);
                var dayEnd = dayStart.AddDays(1);
                var dayTransactions = allTransactions.Where(t => t.TransactionDate >= dayStart && t.TransactionDate < dayEnd);
                var dayIncome = dayTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
                var dayExpense = dayTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);
                dailyBalances.Add(new DailyBalanceDto
                {
                    Date = dayStart,
                    Income = dayIncome,
                    Expense = dayExpense,
                    Balance = dayIncome - dayExpense
                });
            }

            // Calculate percentages for categories
            if (totalExpense > 0)
            {
                foreach (var cat in expenseByCategory)
                    cat.Percentage = (double)(cat.Amount / totalExpense) * 100;
            }
            if (totalIncome > 0)
            {
                foreach (var cat in incomeByCategory)
                    cat.Percentage = (double)(cat.Amount / totalIncome) * 100;
            }

            return new MonthlyReportDto
            {
                Year = year,
                Month = month,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                IncomeByCategory = incomeByCategory,
                ExpenseByCategory = expenseByCategory,
                DailyBalances = dailyBalances
            };
        }

        public async Task<YearlyReportDto> GetYearlyReportAsync(int userId, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59);

            var allTransactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .ToListAsync();

            // Load categories for the transactions
            var categoryIds = allTransactions.Select(t => t.CategoryId).Where(id => id != 0).Distinct().ToList();
            var categories = await _context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync();

            var categoryLookup = categories.ToDictionary(c => c.Id, c => c);

            var totalIncome = allTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            var totalExpense = allTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

            var monthlySummaries = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Month = m,
                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
                })
                .ToList();

            var monthlyData = new List<MonthlySummaryDto>();
            foreach (var ms in monthlySummaries)
            {
                var monthStart = new DateTime(year, ms.Month, 1);
                var monthEnd = monthStart.AddMonths(1);
                var monthTransactions = allTransactions.Where(t => t.TransactionDate >= monthStart && t.TransactionDate < monthEnd);
                var income = monthTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
                var expense = monthTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);
                monthlyData.Add(new MonthlySummaryDto
                {
                    Month = ms.Month,
                    MonthName = ms.Name,
                    Income = income,
                    Expense = expense,
                    Balance = income - expense
                });
            }

            // Top expense categories (top 5)
            var topExpenseCategories = allTransactions
                .Where(t => !t.IsIncome)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = categoryLookup.TryGetValue(g.Key, out var cat) ? cat.Name : "Unknown",
                    CategoryIcon = categoryLookup.TryGetValue(g.Key, out var c) ? c.Icon ?? string.Empty : string.Empty,
                    CategoryColor = categoryLookup.TryGetValue(g.Key, out var c2) ? c2.Color ?? string.Empty : string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToList();

            var topIncomeCategories = allTransactions
                .Where(t => t.IsIncome)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = categoryLookup.TryGetValue(g.Key, out var cat) ? cat.Name : "Unknown",
                    CategoryIcon = categoryLookup.TryGetValue(g.Key, out var c) ? c.Icon ?? string.Empty : string.Empty,
                    CategoryColor = categoryLookup.TryGetValue(g.Key, out var c2) ? c2.Color ?? string.Empty : string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToList();

            // Calculate percentages
            var topExpense = topExpenseCategories.Sum(c => c.Amount);
            var topIncome = topIncomeCategories.Sum(c => c.Amount);

            foreach (var cat in topExpenseCategories)
            {
                cat.Percentage = topExpense > 0 ? (double)(cat.Amount / topExpense) * 100 : 0;
            }

            foreach (var cat in topIncomeCategories)
            {
                cat.Percentage = topIncome > 0 ? (double)(cat.Amount / topIncome) * 100 : 0;
            }

            return new YearlyReportDto
            {
                Year = year,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                Balance = totalIncome - totalExpense,
                MonthlySummaries = monthlyData,
                TopExpenseCategories = topExpenseCategories,
                TopIncomeCategories = topIncomeCategories
            };
        }

        public async Task<ExportResultDto> GetExportReportAsync(int userId, ExportRequestDto request)
        {
            var startDate = request.StartDate;
            var endDate = request.EndDate;

            var transactions = await _context.Transactions
                .Where(t => t.UserId == userId && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .OrderBy(t => t.TransactionDate)
                .ToListAsync();

            return new ExportResultDto
            {
                ReportType = request.ReportType,
                StartDate = startDate,
                EndDate = endDate,
                WalletId = request.WalletId,
                TransactionCount = transactions.Count,
                TotalIncome = transactions.Where(t => t.IsIncome).Sum(t => t.Amount),
                TotalExpense = transactions.Where(t => !t.IsIncome).Sum(t => t.Amount),
                Transactions = transactions.Select(t => new ExportTransactionDto
                {
                    Id = t.Id,
                    Note = t.Note,
                    TransactionDate = t.TransactionDate,
                    IsIncome = t.IsIncome,
                    Amount = t.Amount,
                    WalletName = "Unknown",
                    CategoryName = "Unknown"
                }).ToList()
            };
        }

        private async Task<decimal> GetTotalAmountAsync(int userId, DateTime startDate, DateTime endDate, bool isIncome)
        {
            return isIncome
                ? await _context.Transactions
                    .Where(t => t.UserId == userId && t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                    .SumAsync(t => t.Amount)
                : await _context.Transactions
                    .Where(t => t.UserId == userId && !t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                    .SumAsync(t => t.Amount);
        }
    }
}
