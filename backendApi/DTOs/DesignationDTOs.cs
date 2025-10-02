using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateDesignationDto
    {
        [Required]
        public int SectionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of designation name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        [MaxLength(50)]
        public string Grade { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Attendance bonus must be a positive value")]
        public decimal AttendanceBonus { get; set; } = 0;
    }

    public class UpdateDesignationDto
    {
        [Required]
        public int SectionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of designation name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        [MaxLength(50)]
        public string Grade { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Attendance bonus must be a positive value")]
        public decimal AttendanceBonus { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }

    public class DesignationDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // Bangla version of designation name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public string Grade { get; set; } = string.Empty;
        public decimal AttendanceBonus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class DesignationListDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // Bangla version of designation name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public string Grade { get; set; } = string.Empty;
        public decimal AttendanceBonus { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
