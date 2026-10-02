namespace QuanLyChiTieuAPI.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsIncome { get; set; }
        public bool IsSystem { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsIncome { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Icon { get; set; }
        public string? Color { get; set; }
        public bool IsIncome { get; set; }
        public bool IsActive { get; set; }
    }
}
