using HrHubAPI.Data;
using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace HrHubAPI.Services
{
    public class AttendanceReportService : IAttendanceReportService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttendanceReportService> _logger;

        public AttendanceReportService(ApplicationDbContext context, ILogger<AttendanceReportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Daily Attendance Reports

        public async Task<AttendanceReportResponseDto> GetDailyAttendanceReportAsync(DailyAttendanceReportQueryDto query)
        {
            try
            {
                var reportDate = query.ReportDate.Date;
                var nextDay = reportDate.AddDays(1);

                // Get all employees with their attendance logs for the specific date
                var employeesQuery = _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Shift)
                    .Include(e => e.Company)
                    .Where(e => e.IsActive);

                // Apply filters
                if (query.EmployeeId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.Id == query.EmployeeId.Value);

                if (!string.IsNullOrEmpty(query.EmpId))
                    employeesQuery = employeesQuery.Where(e => e.EmpId.Contains(query.EmpId));

                if (query.DepartmentId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.DepartmentId == query.DepartmentId.Value);

                if (query.SectionId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.SectionId == query.SectionId.Value);

                if (query.CompanyId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.CompanyId == query.CompanyId.Value);

                var employees = await employeesQuery.ToListAsync();

                var reports = new List<DailyAttendanceReportDto>();

                foreach (var employee in employees)
                {
                    var attendanceLogs = await _context.AttendanceLogs
                        .Where(al => al.EmployeeId == employee.EmpId && 
                                    al.LogTime >= reportDate && 
                                    al.LogTime < nextDay)
                        .OrderBy(al => al.LogTime)
                        .ToListAsync();

                    var report = await GenerateDailyAttendanceReportAsync(employee, attendanceLogs, reportDate);
                    
                    // Apply additional filters
                    if (!string.IsNullOrEmpty(query.AttendanceStatus) && report.AttendanceStatus != query.AttendanceStatus)
                        continue;

                    if (query.IsLate.HasValue && report.IsLate != query.IsLate.Value)
                        continue;

                    if (query.IsEarlyLeave.HasValue && report.IsEarlyLeave != query.IsEarlyLeave.Value)
                        continue;

                    reports.Add(report);
                }

                // Apply sorting
                reports = ApplySorting(reports, query.SortBy, query.SortOrder);

                // Apply pagination
                var totalCount = reports.Count;
                var paginatedReports = reports
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                // Generate summary
                var summary = GenerateAttendanceSummary(reports, reportDate, reportDate);

                return new AttendanceReportResponseDto
                {
                    Reports = paginatedReports,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize),
                    StartDate = reportDate,
                    EndDate = reportDate,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily attendance report for date {ReportDate}", query.ReportDate);
                throw;
            }
        }

        public async Task<List<DailyAttendanceReportDto>> GetDailyAttendanceReportForAllEmployeesAsync(DateTime reportDate, int? companyId = null)
        {
            try
            {
                var nextDay = reportDate.AddDays(1);

                var employeesQuery = _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Shift)
                    .Include(e => e.Company)
                    .Where(e => e.IsActive);

                if (companyId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.CompanyId == companyId.Value);

                var employees = await employeesQuery.ToListAsync();
                var reports = new List<DailyAttendanceReportDto>();

                foreach (var employee in employees)
                {
                    var attendanceLogs = await _context.AttendanceLogs
                        .Where(al => al.EmployeeId == employee.EmpId && 
                                    al.LogTime >= reportDate && 
                                    al.LogTime < nextDay)
                        .OrderBy(al => al.LogTime)
                        .ToListAsync();

                    var report = await GenerateDailyAttendanceReportAsync(employee, attendanceLogs, reportDate);
                    reports.Add(report);
                }

                return reports;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily attendance report for all employees on {ReportDate}", reportDate);
                throw;
            }
        }

        #endregion

        #region Employee Attendance Reports

        public async Task<EmployeeAttendanceReportResponseDto> GetEmployeeAttendanceReportAsync(AttendanceReportQueryDto query)
        {
            try
            {
                var startDate = query.StartDate ?? DateTime.Now.AddDays(-30);
                var endDate = query.EndDate ?? DateTime.Now;

                var employeesQuery = _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Shift)
                    .Include(e => e.Company)
                    .Where(e => e.IsActive);

                // Apply filters
                if (query.EmployeeId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.Id == query.EmployeeId.Value);

                if (!string.IsNullOrEmpty(query.EmpId))
                    employeesQuery = employeesQuery.Where(e => e.EmpId.Contains(query.EmpId));

                if (query.DepartmentId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.DepartmentId == query.DepartmentId.Value);

                if (query.SectionId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.SectionId == query.SectionId.Value);

                if (query.CompanyId.HasValue)
                    employeesQuery = employeesQuery.Where(e => e.CompanyId == query.CompanyId.Value);

                var employees = await employeesQuery.ToListAsync();
                var reports = new List<EmployeeAttendanceReportDto>();

                foreach (var employee in employees)
                {
                    var report = await GenerateEmployeeAttendanceReportAsync(employee, startDate, endDate);
                    if (report != null)
                        reports.Add(report);
                }

                // Apply sorting
                reports = ApplyEmployeeSorting(reports, query.SortBy, query.SortOrder);

                // Apply pagination
                var totalCount = reports.Count;
                var paginatedReports = reports
                    .Skip((query.Page - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .ToList();

                // Generate summary
                var summary = GenerateEmployeeAttendanceSummary(reports, startDate, endDate);

                return new EmployeeAttendanceReportResponseDto
                {
                    Reports = paginatedReports,
                    TotalCount = totalCount,
                    Page = query.Page,
                    PageSize = query.PageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize),
                    StartDate = startDate,
                    EndDate = endDate,
                    Summary = summary
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee attendance report");
                throw;
            }
        }

        public async Task<EmployeeAttendanceReportDto?> GetEmployeeAttendanceReportByIdAsync(int employeeId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Shift)
                    .Include(e => e.Company)
                    .FirstOrDefaultAsync(e => e.Id == employeeId && e.IsActive);

                if (employee == null)
                    return null;

                return await GenerateEmployeeAttendanceReportAsync(employee, startDate, endDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee attendance report for employee {EmployeeId}", employeeId);
                throw;
            }
        }

        #endregion

        #region Attendance Summary

        public Task<AttendanceReportSummaryDto> GetAttendanceSummaryAsync(DateTime startDate, DateTime endDate, int? companyId = null)
        {
            return Task.Run(() =>
            {
                try
                {
                    var employeesQuery = _context.Employees.Where(e => e.IsActive);
                    if (companyId.HasValue)
                        employeesQuery = employeesQuery.Where(e => e.CompanyId == companyId.Value);

                    var employees = employeesQuery.ToList();
                    var totalWorkingDays = CalculateWorkingDays(startDate, endDate);
                    var summary = new AttendanceReportSummaryDto
                    {
                        TotalEmployees = employees.Count,
                        TotalWorkingDays = totalWorkingDays
                    };

                    foreach (var employee in employees)
                    {
                        var dailyReports = GetDailyAttendanceReportForAllEmployeesAsync(startDate, companyId).Result;
                        // Process each day's attendance
                        for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                        {
                            var dayReport = dailyReports.FirstOrDefault(r => r.EmployeeId == employee.Id && r.ReportDate.Date == date);
                            if (dayReport != null)
                            {
                                switch (dayReport.AttendanceStatus)
                                {
                                    case "Present":
                                        summary.PresentEmployees++;
                                        break;
                                    case "Absent":
                                        summary.AbsentEmployees++;
                                        break;
                                    case "Late":
                                        summary.LateEmployees++;
                                        break;
                                    case "Half Day":
                                        summary.HalfDayEmployees++;
                                        break;
                                }
                            }
                        }
                    }

                    summary.OverallAttendancePercentage = summary.TotalEmployees > 0 
                        ? (double)(summary.PresentEmployees + summary.LateEmployees + summary.HalfDayEmployees) / (summary.TotalEmployees * totalWorkingDays) * 100 
                        : 0;

                    return summary;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error getting attendance summary");
                    throw;
                }
            });
        }

        public async Task<List<AttendanceLogSummaryDto>> GetAttendanceLogSummaryAsync(DateTime startDate, DateTime endDate, string? employeeId = null)
        {
            try
            {
                var logsQuery = _context.AttendanceLogs
                    .Include(al => al.ZkDevice)
                    .Where(al => al.LogTime >= startDate && al.LogTime <= endDate);

                if (!string.IsNullOrEmpty(employeeId))
                    logsQuery = logsQuery.Where(al => al.EmployeeId == employeeId);

                var logs = await logsQuery.OrderBy(al => al.LogTime).ToListAsync();

                var summaries = logs
                    .GroupBy(al => new { al.EmployeeId, al.EmployeeName, Date = al.LogTime.Date })
                    .Select(g => new AttendanceLogSummaryDto
                    {
                        EmployeeId = g.Key.EmployeeId,
                        EmployeeName = g.Key.EmployeeName,
                        Date = g.Key.Date,
                        AllLogs = g.Select(al => new AttendanceLogDto
                        {
                            Id = al.Id,
                            ZkDeviceId = al.ZkDeviceId,
                            DeviceName = al.ZkDevice.DeviceName,
                            EmployeeId = al.EmployeeId,
                            EmployeeName = al.EmployeeName,
                            LogTime = al.LogTime,
                            LogType = al.LogType,
                            VerificationMode = al.VerificationMode,
                            WorkCode = al.WorkCode,
                            Remarks = al.Remarks,
                            IsProcessed = al.IsProcessed,
                            CreatedAt = al.CreatedAt
                        }).ToList(),
                        FirstCheckIn = g.Where(al => al.LogType == "IN").Min(al => (DateTime?)al.LogTime),
                        LastCheckOut = g.Where(al => al.LogType == "OUT").Max(al => (DateTime?)al.LogTime),
                        CheckInCount = g.Count(al => al.LogType == "IN"),
                        CheckOutCount = g.Count(al => al.LogType == "OUT"),
                        Status = DetermineAttendanceStatus(g.ToList())
                    })
                    .ToList();

                // Calculate total work time for each summary
                foreach (var summary in summaries)
                {
                    if (summary.FirstCheckIn.HasValue && summary.LastCheckOut.HasValue)
                    {
                        summary.TotalWorkTime = summary.LastCheckOut.Value - summary.FirstCheckIn.Value;
                    }
                }

                return summaries;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance log summary");
                throw;
            }
        }

        #endregion

        #region Export Functionality

        public async Task<byte[]> ExportDailyAttendanceReportAsync(DailyAttendanceReportQueryDto query)
        {
            try
            {
                var report = await GetDailyAttendanceReportAsync(query);
                return GenerateCsvReport(report.Reports, "Daily Attendance Report");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting daily attendance report");
                throw;
            }
        }

        public async Task<byte[]> ExportEmployeeAttendanceReportAsync(AttendanceReportQueryDto query)
        {
            try
            {
                var report = await GetEmployeeAttendanceReportAsync(query);
                return GenerateCsvReport(report.Reports, "Employee Attendance Report");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting employee attendance report");
                throw;
            }
        }

        #endregion

        #region Private Helper Methods

        private async Task<DailyAttendanceReportDto> GenerateDailyAttendanceReportAsync(Employee employee, List<AttendanceLog> logs, DateTime reportDate)
        {
            var report = new DailyAttendanceReportDto
            {
                EmployeeId = employee.Id,
                EmpId = employee.EmpId,
                EmployeeName = employee.Name,
                EmployeeNameBangla = employee.NameBangla,
                DepartmentName = employee.Department?.Name ?? "",
                SectionName = employee.Section?.Name ?? "",
                DesignationName = employee.Designation?.Name ?? "",
                ShiftName = employee.Shift?.Name,
                ReportDate = reportDate
            };

            if (logs.Any())
            {
                var checkInLogs = logs.Where(l => l.LogType == "IN").OrderBy(l => l.LogTime).ToList();
                var checkOutLogs = logs.Where(l => l.LogType == "OUT").OrderByDescending(l => l.LogTime).ToList();

                if (checkInLogs.Any())
                {
                    report.CheckInTime = checkInLogs.First().LogTime;
                }

                if (checkOutLogs.Any())
                {
                    report.CheckOutTime = checkOutLogs.First().LogTime;
                }

                // Calculate work duration
                if (report.CheckInTime.HasValue && report.CheckOutTime.HasValue)
                {
                    report.WorkDuration = report.CheckOutTime.Value - report.CheckInTime.Value;
                }

                // Determine attendance status
                report.AttendanceStatus = DetermineDailyAttendanceStatus(logs, employee.Shift);
                report.IsLate = IsLateArrival(report.CheckInTime, employee.Shift);
                report.IsEarlyLeave = IsEarlyDeparture(report.CheckOutTime, employee.Shift);

                // Calculate late minutes
                if (report.IsLate && employee.Shift != null)
                {
                    var shiftStartTime = reportDate.Date.Add(employee.Shift.StartTime);
                    if (report.CheckInTime.HasValue && report.CheckInTime.Value > shiftStartTime)
                    {
                        report.LateMinutes = report.CheckInTime.Value - shiftStartTime;
                    }
                }

                // Calculate early leave minutes
                if (report.IsEarlyLeave && employee.Shift != null)
                {
                    var shiftEndTime = reportDate.Date.Add(employee.Shift.EndTime);
                    if (report.CheckOutTime.HasValue && report.CheckOutTime.Value < shiftEndTime)
                    {
                        report.EarlyLeaveMinutes = shiftEndTime - report.CheckOutTime.Value;
                    }
                }
            }
            else
            {
                report.AttendanceStatus = "Absent";
            }

            return report;
        }

        private async Task<EmployeeAttendanceReportDto?> GenerateEmployeeAttendanceReportAsync(Employee employee, DateTime startDate, DateTime endDate)
        {
            try
            {
                var dailyReports = new List<DailyAttendanceReportDto>();
                var totalWorkingDays = CalculateWorkingDays(startDate, endDate);

                for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
                {
                    var logs = await _context.AttendanceLogs
                        .Where(al => al.EmployeeId == employee.EmpId && 
                                    al.LogTime >= date && 
                                    al.LogTime < date.AddDays(1))
                        .OrderBy(al => al.LogTime)
                        .ToListAsync();

                    var dailyReport = await GenerateDailyAttendanceReportAsync(employee, logs, date);
                    dailyReports.Add(dailyReport);
                }

                var report = new EmployeeAttendanceReportDto
                {
                    EmployeeId = employee.Id,
                    EmpId = employee.EmpId,
                    EmployeeName = employee.Name,
                    EmployeeNameBangla = employee.NameBangla,
                    DepartmentName = employee.Department?.Name ?? "",
                    SectionName = employee.Section?.Name ?? "",
                    DesignationName = employee.Designation?.Name ?? "",
                    ShiftName = employee.Shift?.Name,
                    StartDate = startDate,
                    EndDate = endDate,
                    TotalWorkingDays = totalWorkingDays,
                    DailyAttendance = dailyReports
                };

                // Calculate summary statistics
                report.PresentDays = dailyReports.Count(r => r.AttendanceStatus == "Present");
                report.AbsentDays = dailyReports.Count(r => r.AttendanceStatus == "Absent");
                report.LateDays = dailyReports.Count(r => r.AttendanceStatus == "Late");
                report.HalfDays = dailyReports.Count(r => r.AttendanceStatus == "Half Day");

                report.TotalWorkHours = TimeSpan.FromTicks(dailyReports
                    .Where(r => r.WorkDuration.HasValue)
                    .Sum(r => r.WorkDuration!.Value.Ticks));

                report.TotalOvertimeHours = TimeSpan.FromTicks(dailyReports
                    .Where(r => r.OvertimeHours.HasValue)
                    .Sum(r => r.OvertimeHours!.Value.Ticks));

                report.AttendancePercentage = totalWorkingDays > 0 
                    ? (double)(report.PresentDays + report.LateDays + report.HalfDays) / totalWorkingDays * 100 
                    : 0;

                return report;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating employee attendance report for employee {EmployeeId}", employee.Id);
                return null;
            }
        }

        private string DetermineDailyAttendanceStatus(List<AttendanceLog> logs, Shift? shift)
        {
            if (!logs.Any())
                return "Absent";

            var hasCheckIn = logs.Any(l => l.LogType == "IN");
            var hasCheckOut = logs.Any(l => l.LogType == "OUT");

            if (hasCheckIn && hasCheckOut)
            {
                // Check if it's a half day based on work duration
                var checkInTime = logs.Where(l => l.LogType == "IN").Min(l => l.LogTime);
                var checkOutTime = logs.Where(l => l.LogType == "OUT").Max(l => l.LogTime);
                var workDuration = checkOutTime - checkInTime;

                if (shift != null && workDuration < TimeSpan.FromHours(4)) // Less than 4 hours considered half day
                    return "Half Day";

                return "Present";
            }
            else if (hasCheckIn)
            {
                return "Half Day"; // Only checked in, didn't check out
            }

            return "Absent";
        }

        private string DetermineAttendanceStatus(List<AttendanceLog> logs)
        {
            if (!logs.Any())
                return "Absent";

            var hasCheckIn = logs.Any(l => l.LogType == "IN");
            var hasCheckOut = logs.Any(l => l.LogType == "OUT");

            if (hasCheckIn && hasCheckOut)
                return "Present";
            else if (hasCheckIn)
                return "Partial";
            else
                return "Absent";
        }

        private bool IsLateArrival(DateTime? checkInTime, Shift? shift)
        {
            if (!checkInTime.HasValue || shift == null)
                return false;

            var shiftStartTime = checkInTime.Value.Date.Add(shift.StartTime);
            return checkInTime.Value > shiftStartTime.AddMinutes(15); // 15 minutes grace period
        }

        private bool IsEarlyDeparture(DateTime? checkOutTime, Shift? shift)
        {
            if (!checkOutTime.HasValue || shift == null)
                return false;

            var shiftEndTime = checkOutTime.Value.Date.Add(shift.EndTime);
            return checkOutTime.Value < shiftEndTime.AddMinutes(-15); // 15 minutes grace period
        }

        private int CalculateWorkingDays(DateTime startDate, DateTime endDate)
        {
            var workingDays = 0;
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }

        private List<DailyAttendanceReportDto> ApplySorting(List<DailyAttendanceReportDto> reports, string? sortBy, string? sortOrder)
        {
            return sortBy?.ToLower() switch
            {
                "employeename" => sortOrder?.ToLower() == "desc" 
                    ? reports.OrderByDescending(r => r.EmployeeName).ToList()
                    : reports.OrderBy(r => r.EmployeeName).ToList(),
                "departmentname" => sortOrder?.ToLower() == "desc"
                    ? reports.OrderByDescending(r => r.DepartmentName).ToList()
                    : reports.OrderBy(r => r.DepartmentName).ToList(),
                "attendancestatus" => sortOrder?.ToLower() == "desc"
                    ? reports.OrderByDescending(r => r.AttendanceStatus).ToList()
                    : reports.OrderBy(r => r.AttendanceStatus).ToList(),
                "checkintime" => sortOrder?.ToLower() == "desc"
                    ? reports.OrderByDescending(r => r.CheckInTime).ToList()
                    : reports.OrderBy(r => r.CheckInTime).ToList(),
                _ => reports.OrderBy(r => r.EmployeeName).ToList()
            };
        }

        private List<EmployeeAttendanceReportDto> ApplyEmployeeSorting(List<EmployeeAttendanceReportDto> reports, string? sortBy, string? sortOrder)
        {
            return sortBy?.ToLower() switch
            {
                "employeename" => sortOrder?.ToLower() == "desc" 
                    ? reports.OrderByDescending(r => r.EmployeeName).ToList()
                    : reports.OrderBy(r => r.EmployeeName).ToList(),
                "departmentname" => sortOrder?.ToLower() == "desc"
                    ? reports.OrderByDescending(r => r.DepartmentName).ToList()
                    : reports.OrderBy(r => r.DepartmentName).ToList(),
                "attendancepercentage" => sortOrder?.ToLower() == "desc"
                    ? reports.OrderByDescending(r => r.AttendancePercentage).ToList()
                    : reports.OrderBy(r => r.AttendancePercentage).ToList(),
                "presentdays" => sortOrder?.ToLower() == "desc"
                    ? reports.OrderByDescending(r => r.PresentDays).ToList()
                    : reports.OrderBy(r => r.PresentDays).ToList(),
                _ => reports.OrderBy(r => r.EmployeeName).ToList()
            };
        }

        private AttendanceReportSummaryDto GenerateAttendanceSummary(List<DailyAttendanceReportDto> reports, DateTime startDate, DateTime endDate)
        {
            return new AttendanceReportSummaryDto
            {
                TotalEmployees = reports.Count,
                PresentEmployees = reports.Count(r => r.AttendanceStatus == "Present"),
                AbsentEmployees = reports.Count(r => r.AttendanceStatus == "Absent"),
                LateEmployees = reports.Count(r => r.AttendanceStatus == "Late"),
                HalfDayEmployees = reports.Count(r => r.AttendanceStatus == "Half Day"),
                OverallAttendancePercentage = reports.Count > 0 
                    ? (double)reports.Count(r => r.AttendanceStatus != "Absent") / reports.Count * 100 
                    : 0,
                TotalWorkHours = TimeSpan.FromTicks(reports
                    .Where(r => r.WorkDuration.HasValue)
                    .Sum(r => r.WorkDuration!.Value.Ticks)),
                TotalOvertimeHours = TimeSpan.FromTicks(reports
                    .Where(r => r.OvertimeHours.HasValue)
                    .Sum(r => r.OvertimeHours!.Value.Ticks)),
                TotalWorkingDays = CalculateWorkingDays(startDate, endDate)
            };
        }

        private AttendanceReportSummaryDto GenerateEmployeeAttendanceSummary(List<EmployeeAttendanceReportDto> reports, DateTime startDate, DateTime endDate)
        {
            return new AttendanceReportSummaryDto
            {
                TotalEmployees = reports.Count,
                PresentEmployees = reports.Sum(r => r.PresentDays),
                AbsentEmployees = reports.Sum(r => r.AbsentDays),
                LateEmployees = reports.Sum(r => r.LateDays),
                HalfDayEmployees = reports.Sum(r => r.HalfDays),
                OverallAttendancePercentage = reports.Count > 0 
                    ? reports.Average(r => r.AttendancePercentage) 
                    : 0,
                TotalWorkHours = TimeSpan.FromTicks(reports.Sum(r => r.TotalWorkHours.Ticks)),
                TotalOvertimeHours = TimeSpan.FromTicks(reports.Sum(r => r.TotalOvertimeHours.Ticks)),
                TotalWorkingDays = CalculateWorkingDays(startDate, endDate)
            };
        }

        private byte[] GenerateCsvReport<T>(List<T> data, string reportTitle)
        {
            var csv = new StringBuilder();
            
            // Add header
            csv.AppendLine($"Report: {reportTitle}");
            csv.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            csv.AppendLine();

            // Add data headers based on type
            if (data.Any())
            {
                var properties = typeof(T).GetProperties();
                var headers = string.Join(",", properties.Select(p => p.Name));
                csv.AppendLine(headers);

                foreach (var item in data)
                {
                    var values = properties.Select(p => 
                    {
                        var value = p.GetValue(item);
                        return value?.ToString()?.Replace(",", ";") ?? "";
                    });
                    csv.AppendLine(string.Join(",", values));
                }
            }

            return Encoding.UTF8.GetBytes(csv.ToString());
        }

        #endregion
    }
}
