using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChiTieuAPI.Models
{
    public class Budget
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SpentAmount { get; set; } = 0;

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int UserId { get; set; }

        public int CategoryId { get; set; }

        public int? WalletId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        [ForeignKey(nameof(WalletId))]
        public virtual Wallet? Wallet { get; set; }
    }
}
