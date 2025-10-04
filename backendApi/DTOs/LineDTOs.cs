using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateLineDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of line name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }

    public class UpdateLineDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of line name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class LineDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla version of line name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class LineListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
