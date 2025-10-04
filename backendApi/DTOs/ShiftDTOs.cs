using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateShiftDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of shift name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // Break time information (optional)
        public TimeSpan? BreakStartTime { get; set; }

        public TimeSpan? BreakEndTime { get; set; }

        [Required]
        public int CompanyId { get; set; }
    }

    public class UpdateShiftDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of shift name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // Break time information (optional)
        public TimeSpan? BreakStartTime { get; set; }

        public TimeSpan? BreakEndTime { get; set; }

        [Required]
        public int CompanyId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class ShiftDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla version of shift name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        
        // Calculated properties
        public TimeSpan Duration => EndTime - StartTime;
        public TimeSpan? BreakDuration => BreakStartTime.HasValue && BreakEndTime.HasValue 
            ? BreakEndTime.Value - BreakStartTime.Value 
            : null;
        public TimeSpan WorkingHours => BreakDuration.HasValue 
            ? Duration - BreakDuration.Value 
            : Duration;
    }

    public class ShiftListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan? BreakStartTime { get; set; }
        public TimeSpan? BreakEndTime { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        // Calculated properties
        public TimeSpan Duration => EndTime - StartTime;
        public TimeSpan? BreakDuration => BreakStartTime.HasValue && BreakEndTime.HasValue 
            ? BreakEndTime.Value - BreakStartTime.Value 
            : null;
        public TimeSpan WorkingHours => BreakDuration.HasValue 
            ? Duration - BreakDuration.Value 
            : Duration;
    }
}
