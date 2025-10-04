using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateRosterScheduleDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ShiftId { get; set; }

        [Required]
        public DateTime ScheduleDate { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        // Bangla version of status
        [MaxLength(20)]
        public string? StatusBangla { get; set; }

        // Additional notes
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Bangla version of notes
        [MaxLength(500)]
        public string? NotesBangla { get; set; }
    }

    public class UpdateRosterScheduleDto
    {
        [Required]
        public int EmployeeId { get; set; }

        [Required]
        public int ShiftId { get; set; }

        [Required]
        public DateTime ScheduleDate { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

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

        public bool IsActive { get; set; } = true;
    }

    public class RosterScheduleDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? EmployeeNameBangla { get; set; }
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string? ShiftNameBangla { get; set; }
        public TimeSpan ShiftStartTime { get; set; }
        public TimeSpan ShiftEndTime { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? StatusBangla { get; set; }
        public string? Notes { get; set; }
        public string? NotesBangla { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public TimeSpan? OvertimeHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        
        // Calculated properties
        public TimeSpan? WorkedHours => CheckInTime.HasValue && CheckOutTime.HasValue 
            ? CheckOutTime.Value - CheckInTime.Value 
            : null;
        public TimeSpan? LateArrival => CheckInTime.HasValue 
            ? CheckInTime.Value.TimeOfDay - ShiftStartTime 
            : null;
        public bool IsLate => LateArrival.HasValue && LateArrival.Value > TimeSpan.Zero;
        public bool IsOvertime => OvertimeHours.HasValue && OvertimeHours.Value > TimeSpan.Zero;
    }

    public class RosterScheduleListDto
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? EmployeeNameBangla { get; set; }
        public int ShiftId { get; set; }
        public string ShiftName { get; set; } = string.Empty;
        public string? ShiftNameBangla { get; set; }
        public TimeSpan ShiftStartTime { get; set; }
        public TimeSpan ShiftEndTime { get; set; }
        public DateTime ScheduleDate { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? StatusBangla { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public TimeSpan? OvertimeHours { get; set; }
        public bool IsActive { get; set; }
        
        // Calculated properties
        public TimeSpan? WorkedHours => CheckInTime.HasValue && CheckOutTime.HasValue 
            ? CheckOutTime.Value - CheckInTime.Value 
            : null;
        public TimeSpan? LateArrival => CheckInTime.HasValue 
            ? CheckInTime.Value.TimeOfDay - ShiftStartTime 
            : null;
        public bool IsLate => LateArrival.HasValue && LateArrival.Value > TimeSpan.Zero;
        public bool IsOvertime => OvertimeHours.HasValue && OvertimeHours.Value > TimeSpan.Zero;
    }

    public class BulkCreateRosterScheduleDto
    {
        [Required]
        public List<int> EmployeeIds { get; set; } = new();

        [Required]
        public int ShiftId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int CompanyId { get; set; }

        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        // Bangla version of status
        [MaxLength(20)]
        public string? StatusBangla { get; set; }

        // Additional notes
        [MaxLength(500)]
        public string? Notes { get; set; }

        // Bangla version of notes
        [MaxLength(500)]
        public string? NotesBangla { get; set; }

        // Days of week to schedule (0 = Sunday, 1 = Monday, etc.)
        public List<int> DaysOfWeek { get; set; } = new() { 1, 2, 3, 4, 5 }; // Monday to Friday by default
    }

    public class RosterScheduleFilterDto
    {
        public int? EmployeeId { get; set; }
        public int? ShiftId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public bool? HasCheckIn { get; set; }
        public bool? HasCheckOut { get; set; }
        public bool? IsLate { get; set; }
        public bool? HasOvertime { get; set; }
    }

    public class CheckInOutDto
    {
        [Required]
        public int RosterScheduleId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? NotesBangla { get; set; }
    }

    public class RosterScheduleSummaryDto
    {
        public int TotalSchedules { get; set; }
        public int CompletedSchedules { get; set; }
        public int PendingSchedules { get; set; }
        public int CancelledSchedules { get; set; }
        public int AbsentSchedules { get; set; }
        public int LateArrivals { get; set; }
        public int OvertimeSchedules { get; set; }
        public TimeSpan TotalWorkedHours { get; set; }
        public TimeSpan TotalOvertimeHours { get; set; }
        public double AttendanceRate { get; set; }
        public double PunctualityRate { get; set; }
    }
}
