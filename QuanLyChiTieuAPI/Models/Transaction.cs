using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChiTieuAPI.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [MaxLength(500)]
        public string? Note { get; set; }

        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        public bool IsIncome { get; set; } = false;

        public int WalletId { get; set; }

        public int CategoryId { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [ForeignKey(nameof(WalletId))]
        public virtual Wallet Wallet { get; set; } = null!;

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;
    }
}
