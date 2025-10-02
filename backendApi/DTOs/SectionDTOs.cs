using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateSectionDto
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of section name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }
    }

    public class UpdateSectionDto
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of section name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class SectionDto
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // Bangla version of section name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class SectionListDto
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // Bangla version of section name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
