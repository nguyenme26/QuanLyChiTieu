namespace QuanLyChiTieuAPI.DTOs
{
    public class TransactionDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsIncome { get; set; }
        public int WalletId { get; set; }
        public string WalletName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string CategoryIcon { get; set; } = string.Empty;
        public string CategoryColor { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTransactionDto
    {
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; } = default!;
        public bool IsIncome { get; set; }
        public int WalletId { get; set; }
        public int CategoryId { get; set; }
    }

    public class UpdateTransactionDto
    {
        public decimal Amount { get; set; }
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsIncome { get; set; }
        public int WalletId { get; set; }
        public int CategoryId { get; set; }
    }

    public class TransactionFilterDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? WalletId { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsIncome { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}
