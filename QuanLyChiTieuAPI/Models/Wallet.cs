using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChiTieuAPI.Models
{
    public class Wallet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Balance { get; set; } = 0;

        [MaxLength(10)]
        public string Currency { get; set; } = "VND";

        public bool IsDefault { get; set; } = false;

        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; } = null!;

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}
