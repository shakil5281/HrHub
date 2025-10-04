using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class RosterSchedule
    {
        [Key]
        public int Id { get; set; }

        // Employee relationship
        [Required]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

        // Shift relationship
        [Required]
        public int ShiftId { get; set; }

        [ForeignKey("ShiftId")]
        public virtual Shift Shift { get; set; } = null!;

        // Schedule date
        [Required]
        public DateTime ScheduleDate { get; set; }

        // Company relationship (for easier querying and access control)
        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        // Schedule status
        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, Completed, Cancelled, Absent

        // Bangla version of status
        [MaxLength(20)]
        public string? StatusBangla { get; set; }

        // Additional notes
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Bangla version of notes
        [MaxLength(500)]
        public string? NotesBangla { get; set; }

        // Check-in and check-out times
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        // Overtime information
        public TimeSpan? OvertimeHours { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }
    }
}
