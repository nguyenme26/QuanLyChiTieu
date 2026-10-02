namespace QuanLyChiTieuAPI.DTOs
{
    public class WalletDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "VND";
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateWalletDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Currency { get; set; } = "VND";
        public bool IsDefault { get; set; } = false;
    }

    public class UpdateWalletDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Currency { get; set; } = "VND";
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; }
    }

    public class WalletBalanceDto
    {
        public int WalletId { get; set; }
        public string WalletName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "VND";
    }

    public class TransferDto
    {
        public int FromWalletId { get; set; }
        public int ToWalletId { get; set; }
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }
}
