namespace QuanLyChiTieuAPI.DTOs
{
    public class DashboardSummaryDto
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public int TotalWallets { get; set; }
        public int TotalTransactions { get; set; }
        public int ActiveBudgets { get; set; }
        public List<BudgetAlertDto> BudgetAlerts { get; set; } = new();
        public List<WalletBalanceDto> WalletBalances { get; set; } = new();
    }

    public class CategorySpendingDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
        public double Percentage { get; set; }
    }

    public class MonthlyReportDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public List<CategorySpendingDto> IncomeByCategory { get; set; } = new();
        public List<CategorySpendingDto> ExpenseByCategory { get; set; } = new();
        public List<DailyBalanceDto> DailyBalances { get; set; } = new();
    }

    public class DailyBalanceDto
    {
        public DateTime Date { get; set; }
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
    }

    public class WalletTrendDto
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; } = string.Empty;
        public List<DailyBalanceDto> DailyBalances { get; set; } = new();
    }

    public class YearlyReportDto
    {
        public int Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance { get; set; }
        public List<MonthlySummaryDto> MonthlySummaries { get; set; } = new();
        public List<CategorySpendingDto> TopExpenseCategories { get; set; } = new();
        public List<CategorySpendingDto> TopIncomeCategories { get; set; } = new();
    }

    public class MonthlySummaryDto
    {
        public int Month { get; set; }
        public string MonthName { get; set; } = string.Empty;
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Balance { get; set; }
    }

    public class ExportRequestDto
    {
        public string Format { get; set; } = "excel"; // "excel" or "pdf"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? WalletId { get; set; }
        public ReportType ReportType { get; set; } = ReportType.Transactions;
    }

    public class ExportResultDto
    {
        public ReportType ReportType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? WalletId { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public List<ExportTransactionDto> Transactions { get; set; } = new();
    }

    public class ExportTransactionDto
    {
        public int Id { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public bool IsIncome { get; set; }
        public decimal Amount { get; set; }
        public string WalletName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }

    public enum ReportType
    {
        Transactions,
        MonthlySummary,
        YearlySummary,
        CategoryBreakdown,
        WalletBalances
    }
}
