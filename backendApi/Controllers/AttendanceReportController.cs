using HrHubAPI.DTOs;
using HrHubAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AttendanceReportController : ControllerBase
    {
        private readonly IAttendanceReportService _attendanceReportService;
        private readonly ILogger<AttendanceReportController> _logger;

        public AttendanceReportController(IAttendanceReportService attendanceReportService, ILogger<AttendanceReportController> logger)
        {
            _attendanceReportService = attendanceReportService;
            _logger = logger;
        }

        #region Daily Attendance Reports

        /// <summary>
        /// Get daily attendance report for a specific date
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Daily attendance report</returns>
        [HttpGet("daily")]
        public async Task<ActionResult<AttendanceReportResponseDto>> GetDailyAttendanceReport([FromQuery] DailyAttendanceReportQueryDto query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _attendanceReportService.GetDailyAttendanceReportAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily attendance report for date {ReportDate}", query.ReportDate);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get daily attendance report for all employees on a specific date
        /// </summary>
        /// <param name="reportDate">The date for the report</param>
        /// <param name="companyId">Optional company ID filter</param>
        /// <returns>List of daily attendance reports</returns>
        [HttpGet("daily/all-employees")]
        public async Task<ActionResult<List<DailyAttendanceReportDto>>> GetDailyAttendanceReportForAllEmployees(
            [FromQuery] DateTime reportDate, 
            [FromQuery] int? companyId = null)
        {
            try
            {
                var result = await _attendanceReportService.GetDailyAttendanceReportForAllEmployeesAsync(reportDate, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting daily attendance report for all employees on {ReportDate}", reportDate);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export daily attendance report to CSV
        /// </summary>
        /// <param name="query">Query parameters for filtering</param>
        /// <returns>CSV file</returns>
        [HttpGet("daily/export")]
        public async Task<IActionResult> ExportDailyAttendanceReport([FromQuery] DailyAttendanceReportQueryDto query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var csvData = await _attendanceReportService.ExportDailyAttendanceReportAsync(query);
                var fileName = $"Daily_Attendance_Report_{query.ReportDate:yyyyMMdd}.csv";
                
                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting daily attendance report");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Employee Attendance Reports

        /// <summary>
        /// Get employee attendance report for a date range
        /// </summary>
        /// <param name="query">Query parameters for filtering and pagination</param>
        /// <returns>Employee attendance report</returns>
        [HttpGet("employee")]
        public async Task<ActionResult<EmployeeAttendanceReportResponseDto>> GetEmployeeAttendanceReport([FromQuery] AttendanceReportQueryDto query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _attendanceReportService.GetEmployeeAttendanceReportAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee attendance report");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get attendance report for a specific employee
        /// </summary>
        /// <param name="employeeId">Employee ID</param>
        /// <param name="startDate">Start date for the report</param>
        /// <param name="endDate">End date for the report</param>
        /// <returns>Employee attendance report</returns>
        [HttpGet("employee/{employeeId}")]
        public async Task<ActionResult<EmployeeAttendanceReportDto>> GetEmployeeAttendanceReportById(
            int employeeId, 
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _attendanceReportService.GetEmployeeAttendanceReportByIdAsync(employeeId, startDate, endDate);
                if (result == null)
                    return NotFound(new { message = "Employee not found or no attendance data available" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting employee attendance report for employee {EmployeeId}", employeeId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export employee attendance report to CSV
        /// </summary>
        /// <param name="query">Query parameters for filtering</param>
        /// <returns>CSV file</returns>
        [HttpGet("employee/export")]
        public async Task<IActionResult> ExportEmployeeAttendanceReport([FromQuery] AttendanceReportQueryDto query)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var csvData = await _attendanceReportService.ExportEmployeeAttendanceReportAsync(query);
                var fileName = $"Employee_Attendance_Report_{query.StartDate:yyyyMMdd}_{query.EndDate:yyyyMMdd}.csv";
                
                return File(csvData, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting employee attendance report");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Attendance Summary

        /// <summary>
        /// Get attendance summary for a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="companyId">Optional company ID filter</param>
        /// <returns>Attendance summary</returns>
        [HttpGet("summary")]
        public async Task<ActionResult<AttendanceReportSummaryDto>> GetAttendanceSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] int? companyId = null)
        {
            try
            {
                var result = await _attendanceReportService.GetAttendanceSummaryAsync(startDate, endDate, companyId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance summary");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get attendance log summary for a date range
        /// </summary>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        /// <param name="employeeId">Optional employee ID filter</param>
        /// <returns>Attendance log summary</returns>
        [HttpGet("log-summary")]
        public async Task<ActionResult<List<AttendanceLogSummaryDto>>> GetAttendanceLogSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] string? employeeId = null)
        {
            try
            {
                var result = await _attendanceReportService.GetAttendanceLogSummaryAsync(startDate, endDate, employeeId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance log summary");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Anonymous Test Endpoints

        /// <summary>
        /// Test endpoint - Get daily attendance report (Anonymous)
        /// </summary>
        [HttpGet("test/daily")]
        [AllowAnonymous]
        public async Task<ActionResult<AttendanceReportResponseDto>> TestGetDailyAttendanceReport([FromQuery] DateTime reportDate)
        {
            try
            {
                var query = new DailyAttendanceReportQueryDto
                {
                    ReportDate = reportDate,
                    Page = 1,
                    PageSize = 10
                };
                var result = await _attendanceReportService.GetDailyAttendanceReportAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test getting daily attendance report");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint - Get employee attendance report (Anonymous)
        /// </summary>
        [HttpGet("test/employee")]
        [AllowAnonymous]
        public async Task<ActionResult<EmployeeAttendanceReportResponseDto>> TestGetEmployeeAttendanceReport(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var query = new AttendanceReportQueryDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Page = 1,
                    PageSize = 10
                };
                var result = await _attendanceReportService.GetEmployeeAttendanceReportAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test getting employee attendance report");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint - Get attendance summary (Anonymous)
        /// </summary>
        [HttpGet("test/summary")]
        [AllowAnonymous]
        public async Task<ActionResult<AttendanceReportSummaryDto>> TestGetAttendanceSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _attendanceReportService.GetAttendanceSummaryAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test getting attendance summary");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion
    }
}
