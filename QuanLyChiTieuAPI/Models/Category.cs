using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyChiTieuAPI.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string Icon { get; set; } = string.Empty;

        [MaxLength(7)]
        public string Color { get; set; } = "#000000";

        public bool IsIncome { get; set; } = false;

        public bool IsSystem { get; set; } = false;

        public int? UserId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        // Navigation properties
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
