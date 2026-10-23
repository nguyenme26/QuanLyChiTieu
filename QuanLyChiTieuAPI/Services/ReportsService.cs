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

            var totalIncome = await GetTotalAmountAsync(userId, startDate, endDate, isIncome: true);
            var totalExpense = await GetTotalAmountAsync(userId, startDate, endDate, isIncome: false);

            var incomeByCategory = await _context.Transactions
                .Where(t => t.UserId == userId && t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate < endDate)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = g.First().Category.Name,
                    CategoryIcon = g.First().Category.Icon ?? string.Empty,
                    CategoryColor = g.First().Category.Color ?? string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .ToListAsync();

            var expenseByCategory = await _context.Transactions
                .Where(t => t.UserId == userId && !t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate < endDate)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = g.First().Category.Name,
                    CategoryIcon = g.First().Category.Icon ?? string.Empty,
                    CategoryColor = g.First().Category.Color ?? string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .ToListAsync();

            var days = Enumerable.Range(1, DateTime.DaysInMonth(year, month))
                .ToList();

            var dailyBalances = await Task.WhenAll(days.Select(async d =>
            {
                var dayStart = new DateTime(year, month, d);
                var dayEnd = dayStart.AddDays(1);
                var income = await GetTotalAmountAsync(userId, dayStart, dayEnd, isIncome: true);
                var expense = await GetTotalAmountAsync(userId, dayStart, dayEnd, isIncome: false);
                return new DailyBalanceDto
                {
                    Date = dayStart,
                    Income = income,
                    Expense = expense,
                    Balance = income - expense
                };
            }));

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
                DailyBalances = dailyBalances.ToList()
            };
        }

        public async Task<YearlyReportDto> GetYearlyReportAsync(int userId, int year)
        {
            var startDate = new DateTime(year, 1, 1);
            var endDate = new DateTime(year, 12, 31, 23, 59, 59);

            var totalIncome = await GetTotalAmountAsync(userId, startDate, endDate, isIncome: true);
            var totalExpense = await GetTotalAmountAsync(userId, startDate, endDate, isIncome: false);

            var monthlySummaries = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Month = m,
                    Name = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
                })
                .ToList();

            var monthlyDataTasks = monthlySummaries.Select(async ms =>
            {
                var monthStart = new DateTime(year, ms.Month, 1);
                var monthEnd = monthStart.AddMonths(1);
                var income = await GetTotalAmountAsync(userId, monthStart, monthEnd, isIncome: true);
                var expense = await GetTotalAmountAsync(userId, monthStart, monthEnd, isIncome: false);
                return new MonthlySummaryDto
                {
                    Month = ms.Month,
                    MonthName = ms.Name,
                    Income = income,
                    Expense = expense,
                    Balance = income - expense
                };
            });

            var monthlyData = (await Task.WhenAll(monthlyDataTasks)).ToList();

            // Top expense categories (top 5)
            var topExpenseCategories = await _context.Transactions
                .Where(t => t.UserId == userId && !t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = g.First().Category.Name,
                    CategoryIcon = g.First().Category.Icon ?? string.Empty,
                    CategoryColor = g.First().Category.Color ?? string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToListAsync();

            var topIncomeCategories = await _context.Transactions
                .Where(t => t.UserId == userId && t.IsIncome && t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .GroupBy(t => t.CategoryId)
                .Select(g => new CategorySpendingDto
                {
                    CategoryId = g.Key,
                    CategoryName = g.First().Category.Name,
                    CategoryIcon = g.First().Category.Icon ?? string.Empty,
                    CategoryColor = g.First().Category.Color ?? string.Empty,
                    Amount = g.Sum(t => t.Amount),
                    TransactionCount = g.Count()
                })
                .OrderByDescending(c => c.Amount)
                .Take(5)
                .ToListAsync();

            // Calculate percentages
            var topExpense = topExpenseCategories.Sum(c => c.Amount);
            var topIncome = topIncomeCategories.Sum(c => c.Amount);

            foreach (var cat in topExpenseCategories)
            {
                cat.Percentage = (double)(cat.Amount / topExpense) * 100;
            }

            foreach (var cat in topIncomeCategories)
            {
                cat.Percentage = (double)(cat.Amount / topIncome) * 100;
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
