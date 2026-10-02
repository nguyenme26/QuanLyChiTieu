namespace QuanLyChiTieuAPI.DTOs
{
    public class BudgetDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount => Amount - SpentAmount;
        public double PercentageUsed => Amount > 0 ? (double)(SpentAmount / Amount * 100) : 0;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public int? WalletId { get; set; }
        public string? WalletName { get; set; }
        public bool IsOverBudget => SpentAmount > Amount;
        public bool IsNearLimit => SpentAmount >= Amount * 0.8m && !IsOverBudget;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateBudgetDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public int? WalletId { get; set; }
    }

    public class UpdateBudgetDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int CategoryId { get; set; }
        public int? WalletId { get; set; }
        public bool IsActive { get; set; }
    }

    public class BudgetAlertDto
    {
        public int BudgetId { get; set; }
        public string BudgetName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal SpentAmount { get; set; }
        public double PercentageUsed { get; set; }
        public string AlertType { get; set; } = string.Empty; // "WARNING" or "OVER_BUDGET"
        public string Message { get; set; } = string.Empty;
    }
}
