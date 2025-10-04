using HrHubAPI.DTOs;

namespace HrHubAPI.Services
{
    public interface IAttendanceReportService
    {
        // Daily Attendance Reports
        Task<AttendanceReportResponseDto> GetDailyAttendanceReportAsync(DailyAttendanceReportQueryDto query);
        Task<List<DailyAttendanceReportDto>> GetDailyAttendanceReportForAllEmployeesAsync(DateTime reportDate, int? companyId = null);
        
        // Employee Attendance Reports
        Task<EmployeeAttendanceReportResponseDto> GetEmployeeAttendanceReportAsync(AttendanceReportQueryDto query);
        Task<EmployeeAttendanceReportDto?> GetEmployeeAttendanceReportByIdAsync(int employeeId, DateTime startDate, DateTime endDate);
        
        // Attendance Summary
        Task<AttendanceReportSummaryDto> GetAttendanceSummaryAsync(DateTime startDate, DateTime endDate, int? companyId = null);
        Task<List<AttendanceLogSummaryDto>> GetAttendanceLogSummaryAsync(DateTime startDate, DateTime endDate, string? employeeId = null);
        
        // Export functionality
        Task<byte[]> ExportDailyAttendanceReportAsync(DailyAttendanceReportQueryDto query);
        Task<byte[]> ExportEmployeeAttendanceReportAsync(AttendanceReportQueryDto query);
    }
}
