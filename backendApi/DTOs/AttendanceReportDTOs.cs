using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class DailyAttendanceReportDto
    {
        public int EmployeeId { get; set; }
        public string EmpId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? EmployeeNameBangla { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string? ShiftName { get; set; }
        public DateTime ReportDate { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public TimeSpan? WorkDuration { get; set; }
        public TimeSpan? OvertimeHours { get; set; }
        public string AttendanceStatus { get; set; } = string.Empty; // Present, Absent, Late, Half Day
        public string? Remarks { get; set; }
        public bool IsLate { get; set; }
        public bool IsEarlyLeave { get; set; }
        public TimeSpan? LateMinutes { get; set; }
        public TimeSpan? EarlyLeaveMinutes { get; set; }
    }

    public class EmployeeAttendanceReportDto
    {
        public int EmployeeId { get; set; }
        public string EmpId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string? EmployeeNameBangla { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string? ShiftName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalWorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int HalfDays { get; set; }
        public TimeSpan TotalWorkHours { get; set; }
        public TimeSpan TotalOvertimeHours { get; set; }
        public double AttendancePercentage { get; set; }
        public List<DailyAttendanceReportDto> DailyAttendance { get; set; } = new List<DailyAttendanceReportDto>();
    }

    public class AttendanceReportQueryDto
    {
        public int? EmployeeId { get; set; }
        public string? EmpId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
        public int? CompanyId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AttendanceStatus { get; set; } // Present, Absent, Late, Half Day
        public bool? IsLate { get; set; }
        public bool? IsEarlyLeave { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; } = "EmployeeName";
        public string? SortOrder { get; set; } = "asc";
    }

    public class DailyAttendanceReportQueryDto
    {
        public DateTime ReportDate { get; set; }
        public int? EmployeeId { get; set; }
        public string? EmpId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SectionId { get; set; }
        public int? CompanyId { get; set; }
        public string? AttendanceStatus { get; set; }
        public bool? IsLate { get; set; }
        public bool? IsEarlyLeave { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; } = "EmployeeName";
        public string? SortOrder { get; set; } = "asc";
    }

    public class AttendanceReportResponseDto
    {
        public List<DailyAttendanceReportDto> Reports { get; set; } = new List<DailyAttendanceReportDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AttendanceReportSummaryDto Summary { get; set; } = new AttendanceReportSummaryDto();
    }

    public class EmployeeAttendanceReportResponseDto
    {
        public List<EmployeeAttendanceReportDto> Reports { get; set; } = new List<EmployeeAttendanceReportDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public AttendanceReportSummaryDto Summary { get; set; } = new AttendanceReportSummaryDto();
    }

    public class AttendanceReportSummaryDto
    {
        public int TotalEmployees { get; set; }
        public int PresentEmployees { get; set; }
        public int AbsentEmployees { get; set; }
        public int LateEmployees { get; set; }
        public int HalfDayEmployees { get; set; }
        public double OverallAttendancePercentage { get; set; }
        public TimeSpan TotalWorkHours { get; set; }
        public TimeSpan TotalOvertimeHours { get; set; }
        public int TotalWorkingDays { get; set; }
    }

    public class AttendanceLogSummaryDto
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime? FirstCheckIn { get; set; }
        public DateTime? LastCheckOut { get; set; }
        public List<AttendanceLogDto> AllLogs { get; set; } = new List<AttendanceLogDto>();
        public TimeSpan? TotalWorkTime { get; set; }
        public int CheckInCount { get; set; }
        public int CheckOutCount { get; set; }
        public string Status { get; set; } = string.Empty; // Present, Absent, Partial
    }
}
