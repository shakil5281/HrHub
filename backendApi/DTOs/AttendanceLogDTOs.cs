using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class AttendanceLogDto
    {
        public int Id { get; set; }
        public int ZkDeviceId { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string EmployeeId { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime LogTime { get; set; }
        public string LogType { get; set; } = string.Empty;
        public string VerificationMode { get; set; } = string.Empty;
        public string WorkCode { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public bool IsProcessed { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AttendanceLogQueryDto
    {
        public int? DeviceId { get; set; }
        public string? EmployeeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LogType { get; set; }
        public string? VerificationMode { get; set; }
        public bool? IsProcessed { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? SortBy { get; set; } = "LogTime";
        public string? SortOrder { get; set; } = "desc";
    }

    public class AttendanceLogResponseDto
    {
        public List<AttendanceLogDto> Logs { get; set; } = new List<AttendanceLogDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
