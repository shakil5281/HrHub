using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateDepartmentDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of department name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }
    }

    public class UpdateDepartmentDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of department name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class DepartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla version of department name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DepartmentListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla version of department name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
