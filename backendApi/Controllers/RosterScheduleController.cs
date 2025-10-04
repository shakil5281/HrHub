using HrHubAPI.Data;
using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RosterScheduleController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RosterScheduleController> _logger;

        public RosterScheduleController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<RosterScheduleController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get roster schedules based on user role and filters
        /// </summary>
        /// <param name="employeeId">Filter by employee ID</param>
        /// <param name="shiftId">Filter by shift ID</param>
        /// <param name="companyId">Filter by company ID (Admin only)</param>
        /// <param name="startDate">Filter by start date</param>
        /// <param name="endDate">Filter by end date</param>
        /// <param name="status">Filter by status</param>
        /// <param name="includeInactive">Include inactive schedules (Admin only)</param>
        /// <returns>List of roster schedules</returns>
        /// <response code="200">Roster schedules retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RosterScheduleListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllRosterSchedules(
            [FromQuery] int? employeeId = null,
            [FromQuery] int? shiftId = null,
            [FromQuery] int? companyId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] string? status = null,
            [FromQuery] bool includeInactive = false)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                List<RosterScheduleListDto> schedules;

                if (isAdmin)
                {
                    // Admin users can see all schedules
                    var query = _context.RosterSchedules
                        .Include(rs => rs.Employee)
                        .Include(rs => rs.Shift)
                        .Include(rs => rs.Company)
                        .AsQueryable();

                    if (employeeId.HasValue)
                        query = query.Where(rs => rs.EmployeeId == employeeId.Value);

                    if (shiftId.HasValue)
                        query = query.Where(rs => rs.ShiftId == shiftId.Value);

                    if (companyId.HasValue)
                        query = query.Where(rs => rs.CompanyId == companyId.Value);

                    if (startDate.HasValue)
                        query = query.Where(rs => rs.ScheduleDate >= startDate.Value);

                    if (endDate.HasValue)
                        query = query.Where(rs => rs.ScheduleDate <= endDate.Value);

                    if (!string.IsNullOrEmpty(status))
                        query = query.Where(rs => rs.Status.ToLower() == status.ToLower());

                    if (!includeInactive)
                        query = query.Where(rs => rs.IsActive);

                    schedules = await query
                        .OrderBy(rs => rs.ScheduleDate)
                        .ThenBy(rs => rs.Employee.Name)
                        .Select(rs => new RosterScheduleListDto
                        {
                            Id = rs.Id,
                            EmployeeId = rs.EmployeeId,
                            EmployeeName = rs.Employee.Name,
                            EmployeeNameBangla = rs.Employee.NameBangla,
                            ShiftId = rs.ShiftId,
                            ShiftName = rs.Shift.Name,
                            ShiftNameBangla = rs.Shift.NameBangla,
                            ShiftStartTime = rs.Shift.StartTime,
                            ShiftEndTime = rs.Shift.EndTime,
                            ScheduleDate = rs.ScheduleDate,
                            CompanyId = rs.CompanyId,
                            CompanyName = rs.Company.Name,
                            Status = rs.Status,
                            StatusBangla = rs.StatusBangla,
                            CheckInTime = rs.CheckInTime,
                            CheckOutTime = rs.CheckOutTime,
                            OvertimeHours = rs.OvertimeHours,
                            IsActive = rs.IsActive
                        })
                        .ToListAsync();

                    _logger.LogInformation("Admin user {UserId} retrieved all roster schedules", userId);
                }
                else
                {
                    // Non-admin users can only see schedules from their assigned companies
                    var userCompanyIds = new List<int>();

                    // Get primary company (from CompanyId field)
                    if (user.CompanyId.HasValue)
                    {
                        userCompanyIds.Add(user.CompanyId.Value);
                    }

                    // Get additional companies from UserCompany relationship
                    var additionalCompanies = await _context.UserCompanies
                        .Where(uc => uc.UserId == userId && uc.IsActive)
                        .Select(uc => uc.CompanyId)
                        .ToListAsync();

                    userCompanyIds.AddRange(additionalCompanies);
                    userCompanyIds = userCompanyIds.Distinct().ToList();

                    if (!userCompanyIds.Any())
                    {
                        schedules = new List<RosterScheduleListDto>();
                    }
                    else
                    {
                        var query = _context.RosterSchedules
                            .Include(rs => rs.Employee)
                            .Include(rs => rs.Shift)
                            .Include(rs => rs.Company)
                            .Where(rs => userCompanyIds.Contains(rs.CompanyId) && rs.IsActive);

                        if (employeeId.HasValue)
                            query = query.Where(rs => rs.EmployeeId == employeeId.Value);

                        if (shiftId.HasValue)
                            query = query.Where(rs => rs.ShiftId == shiftId.Value);

                        if (startDate.HasValue)
                            query = query.Where(rs => rs.ScheduleDate >= startDate.Value);

                        if (endDate.HasValue)
                            query = query.Where(rs => rs.ScheduleDate <= endDate.Value);

                        if (!string.IsNullOrEmpty(status))
                            query = query.Where(rs => rs.Status.ToLower() == status.ToLower());

                        schedules = await query
                            .OrderBy(rs => rs.ScheduleDate)
                            .ThenBy(rs => rs.Employee.Name)
                            .Select(rs => new RosterScheduleListDto
                            {
                                Id = rs.Id,
                                EmployeeId = rs.EmployeeId,
                                EmployeeName = rs.Employee.Name,
                                EmployeeNameBangla = rs.Employee.NameBangla,
                                ShiftId = rs.ShiftId,
                                ShiftName = rs.Shift.Name,
                                ShiftNameBangla = rs.Shift.NameBangla,
                                ShiftStartTime = rs.Shift.StartTime,
                                ShiftEndTime = rs.Shift.EndTime,
                                ScheduleDate = rs.ScheduleDate,
                                CompanyId = rs.CompanyId,
                                CompanyName = rs.Company.Name,
                                Status = rs.Status,
                                StatusBangla = rs.StatusBangla,
                                CheckInTime = rs.CheckInTime,
                                CheckOutTime = rs.CheckOutTime,
                                OvertimeHours = rs.OvertimeHours,
                                IsActive = rs.IsActive
                            })
                            .ToListAsync();
                    }

                    _logger.LogInformation("User {UserId} retrieved {Count} roster schedules from assigned companies", userId, schedules.Count);
                }

                return Ok(new ApiResponse<IEnumerable<RosterScheduleListDto>>
                {
                    Success = true,
                    Message = isAdmin ? "All roster schedules retrieved successfully" : "Assigned company roster schedules retrieved successfully",
                    Data = schedules
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving roster schedules");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving roster schedules",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get roster schedule by ID
        /// </summary>
        /// <param name="id">Roster schedule ID</param>
        /// <returns>Roster schedule details</returns>
        /// <response code="200">Roster schedule retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Roster schedule not accessible to user</response>
        /// <response code="404">Roster schedule not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<RosterScheduleDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetRosterScheduleById(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                var schedule = await _context.RosterSchedules
                    .Include(rs => rs.Employee)
                    .Include(rs => rs.Shift)
                    .Include(rs => rs.Company)
                    .FirstOrDefaultAsync(rs => rs.Id == id);

                if (schedule == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Roster schedule not found",
                        Errors = new List<string> { $"Roster schedule with ID {id} does not exist" }
                    });
                }

                // Check if user has access to this schedule
                if (!isAdmin)
                {
                    var userCompanyIds = new List<int>();

                    if (user.CompanyId.HasValue)
                    {
                        userCompanyIds.Add(user.CompanyId.Value);
                    }

                    var additionalCompanies = await _context.UserCompanies
                        .Where(uc => uc.UserId == userId && uc.IsActive)
                        .Select(uc => uc.CompanyId)
                        .ToListAsync();

                    userCompanyIds.AddRange(additionalCompanies);
                    userCompanyIds = userCompanyIds.Distinct().ToList();

                    if (!userCompanyIds.Contains(schedule.CompanyId))
                    {
                        return Forbid();
                    }
                }

                var scheduleDto = new RosterScheduleDto
                {
                    Id = schedule.Id,
                    EmployeeId = schedule.EmployeeId,
                    EmployeeName = schedule.Employee.Name,
                    EmployeeNameBangla = schedule.Employee.NameBangla,
                    ShiftId = schedule.ShiftId,
                    ShiftName = schedule.Shift.Name,
                    ShiftNameBangla = schedule.Shift.NameBangla,
                    ShiftStartTime = schedule.Shift.StartTime,
                    ShiftEndTime = schedule.Shift.EndTime,
                    ScheduleDate = schedule.ScheduleDate,
                    CompanyId = schedule.CompanyId,
                    CompanyName = schedule.Company.Name,
                    Status = schedule.Status,
                    StatusBangla = schedule.StatusBangla,
                    Notes = schedule.Notes,
                    NotesBangla = schedule.NotesBangla,
                    CheckInTime = schedule.CheckInTime,
                    CheckOutTime = schedule.CheckOutTime,
                    OvertimeHours = schedule.OvertimeHours,
                    CreatedAt = schedule.CreatedAt,
                    UpdatedAt = schedule.UpdatedAt,
                    IsActive = schedule.IsActive,
                    CreatedBy = schedule.CreatedBy,
                    UpdatedBy = schedule.UpdatedBy
                };

                return Ok(new ApiResponse<RosterScheduleDto>
                {
                    Success = true,
                    Message = "Roster schedule retrieved successfully",
                    Data = scheduleDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving roster schedule with ID {ScheduleId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the roster schedule",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new roster schedule (Admin and HR roles only)
        /// </summary>
        /// <param name="createDto">Roster schedule creation data</param>
        /// <returns>Created roster schedule details</returns>
        /// <response code="201">Roster schedule created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <response code="409">Conflict - Schedule already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,HR,HR Manager")]
        [ProducesResponseType(typeof(ApiResponse<RosterScheduleDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateRosterSchedule([FromBody] CreateRosterScheduleDto createDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                // Check if user has access to the company
                if (!isAdmin)
                {
                    var userCompanyIds = new List<int>();

                    if (user.CompanyId.HasValue)
                    {
                        userCompanyIds.Add(user.CompanyId.Value);
                    }

                    var additionalCompanies = await _context.UserCompanies
                        .Where(uc => uc.UserId == userId && uc.IsActive)
                        .Select(uc => uc.CompanyId)
                        .ToListAsync();

                    userCompanyIds.AddRange(additionalCompanies);
                    userCompanyIds = userCompanyIds.Distinct().ToList();

                    if (!userCompanyIds.Contains(createDto.CompanyId))
                    {
                        return Forbid();
                    }
                }

                // Validate employee exists and belongs to the company
                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .FirstOrDefaultAsync(e => e.Id == createDto.EmployeeId);

                if (employee == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid employee",
                        Errors = new List<string> { $"Employee with ID {createDto.EmployeeId} does not exist" }
                    });
                }

                // Validate shift exists and belongs to the company
                var shift = await _context.Shifts
                    .FirstOrDefaultAsync(s => s.Id == createDto.ShiftId && s.CompanyId == createDto.CompanyId);

                if (shift == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid shift",
                        Errors = new List<string> { $"Shift with ID {createDto.ShiftId} does not exist or does not belong to the company" }
                    });
                }

                // Check if schedule already exists for this employee on this date
                var existingSchedule = await _context.RosterSchedules
                    .FirstOrDefaultAsync(rs => rs.EmployeeId == createDto.EmployeeId && 
                                            rs.ScheduleDate.Date == createDto.ScheduleDate.Date);

                if (existingSchedule != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Schedule already exists",
                        Errors = new List<string> { $"A schedule already exists for this employee on {createDto.ScheduleDate:yyyy-MM-dd}" }
                    });
                }

                var schedule = new RosterSchedule
                {
                    EmployeeId = createDto.EmployeeId,
                    ShiftId = createDto.ShiftId,
                    ScheduleDate = createDto.ScheduleDate.Date,
                    CompanyId = createDto.CompanyId,
                    Status = createDto.Status,
                    StatusBangla = createDto.StatusBangla,
                    Notes = createDto.Notes,
                    NotesBangla = createDto.NotesBangla,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.RosterSchedules.Add(schedule);
                await _context.SaveChangesAsync();

                // Load related entities for response
                await _context.Entry(schedule)
                    .Reference(rs => rs.Employee)
                    .LoadAsync();
                await _context.Entry(schedule)
                    .Reference(rs => rs.Shift)
                    .LoadAsync();
                await _context.Entry(schedule)
                    .Reference(rs => rs.Company)
                    .LoadAsync();

                var scheduleDto = new RosterScheduleDto
                {
                    Id = schedule.Id,
                    EmployeeId = schedule.EmployeeId,
                    EmployeeName = schedule.Employee.Name,
                    EmployeeNameBangla = schedule.Employee.NameBangla,
                    ShiftId = schedule.ShiftId,
                    ShiftName = schedule.Shift.Name,
                    ShiftNameBangla = schedule.Shift.NameBangla,
                    ShiftStartTime = schedule.Shift.StartTime,
                    ShiftEndTime = schedule.Shift.EndTime,
                    ScheduleDate = schedule.ScheduleDate,
                    CompanyId = schedule.CompanyId,
                    CompanyName = schedule.Company.Name,
                    Status = schedule.Status,
                    StatusBangla = schedule.StatusBangla,
                    Notes = schedule.Notes,
                    NotesBangla = schedule.NotesBangla,
                    CheckInTime = schedule.CheckInTime,
                    CheckOutTime = schedule.CheckOutTime,
                    OvertimeHours = schedule.OvertimeHours,
                    CreatedAt = schedule.CreatedAt,
                    UpdatedAt = schedule.UpdatedAt,
                    IsActive = schedule.IsActive,
                    CreatedBy = schedule.CreatedBy,
                    UpdatedBy = schedule.UpdatedBy
                };

                _logger.LogInformation("Roster schedule created successfully by user {UserId} for employee {EmployeeId} on {ScheduleDate}", 
                    userId, createDto.EmployeeId, createDto.ScheduleDate);

                return CreatedAtAction(nameof(GetRosterScheduleById), new { id = schedule.Id }, new ApiResponse<RosterScheduleDto>
                {
                    Success = true,
                    Message = "Roster schedule created successfully",
                    Data = scheduleDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating roster schedule");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the roster schedule",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Check in for a roster schedule
        /// </summary>
        /// <param name="checkInDto">Check-in data</param>
        /// <returns>Updated roster schedule</returns>
        /// <response code="200">Check-in successful</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="404">Roster schedule not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("checkin")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<RosterScheduleDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CheckIn([FromBody] CheckInOutDto checkInDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var schedule = await _context.RosterSchedules
                    .Include(rs => rs.Employee)
                    .Include(rs => rs.Shift)
                    .Include(rs => rs.Company)
                    .FirstOrDefaultAsync(rs => rs.Id == checkInDto.RosterScheduleId);

                if (schedule == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Roster schedule not found",
                        Errors = new List<string> { $"Roster schedule with ID {checkInDto.RosterScheduleId} does not exist" }
                    });
                }

                if (schedule.CheckInTime.HasValue)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Already checked in",
                        Errors = new List<string> { "Employee has already checked in for this schedule" }
                    });
                }

                schedule.CheckInTime = checkInDto.DateTime;
                schedule.Status = "Confirmed";
                schedule.StatusBangla = "নিশ্চিত";
                schedule.UpdatedBy = userId;
                schedule.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                var scheduleDto = new RosterScheduleDto
                {
                    Id = schedule.Id,
                    EmployeeId = schedule.EmployeeId,
                    EmployeeName = schedule.Employee.Name,
                    EmployeeNameBangla = schedule.Employee.NameBangla,
                    ShiftId = schedule.ShiftId,
                    ShiftName = schedule.Shift.Name,
                    ShiftNameBangla = schedule.Shift.NameBangla,
                    ShiftStartTime = schedule.Shift.StartTime,
                    ShiftEndTime = schedule.Shift.EndTime,
                    ScheduleDate = schedule.ScheduleDate,
                    CompanyId = schedule.CompanyId,
                    CompanyName = schedule.Company.Name,
                    Status = schedule.Status,
                    StatusBangla = schedule.StatusBangla,
                    Notes = schedule.Notes,
                    NotesBangla = schedule.NotesBangla,
                    CheckInTime = schedule.CheckInTime,
                    CheckOutTime = schedule.CheckOutTime,
                    OvertimeHours = schedule.OvertimeHours,
                    CreatedAt = schedule.CreatedAt,
                    UpdatedAt = schedule.UpdatedAt,
                    IsActive = schedule.IsActive,
                    CreatedBy = schedule.CreatedBy,
                    UpdatedBy = schedule.UpdatedBy
                };

                _logger.LogInformation("Check-in successful for roster schedule {ScheduleId} by user {UserId}", 
                    checkInDto.RosterScheduleId, userId);

                return Ok(new ApiResponse<RosterScheduleDto>
                {
                    Success = true,
                    Message = "Check-in successful",
                    Data = scheduleDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during check-in for roster schedule {ScheduleId}", checkInDto.RosterScheduleId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during check-in",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Check out for a roster schedule
        /// </summary>
        /// <param name="checkOutDto">Check-out data</param>
        /// <returns>Updated roster schedule</returns>
        /// <response code="200">Check-out successful</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="404">Roster schedule not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("checkout")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<RosterScheduleDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CheckOut([FromBody] CheckInOutDto checkOutDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var schedule = await _context.RosterSchedules
                    .Include(rs => rs.Employee)
                    .Include(rs => rs.Shift)
                    .Include(rs => rs.Company)
                    .FirstOrDefaultAsync(rs => rs.Id == checkOutDto.RosterScheduleId);

                if (schedule == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Roster schedule not found",
                        Errors = new List<string> { $"Roster schedule with ID {checkOutDto.RosterScheduleId} does not exist" }
                    });
                }

                if (!schedule.CheckInTime.HasValue)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Not checked in",
                        Errors = new List<string> { "Employee must check in before checking out" }
                    });
                }

                if (schedule.CheckOutTime.HasValue)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Already checked out",
                        Errors = new List<string> { "Employee has already checked out for this schedule" }
                    });
                }

                schedule.CheckOutTime = checkOutDto.DateTime;
                schedule.Status = "Completed";
                schedule.StatusBangla = "সম্পন্ন";
                schedule.UpdatedBy = userId;
                schedule.UpdatedAt = DateTime.UtcNow;

                // Calculate overtime if applicable
                var shiftEndTime = schedule.ScheduleDate.Date.Add(schedule.Shift.EndTime);
                if (checkOutDto.DateTime > shiftEndTime)
                {
                    schedule.OvertimeHours = checkOutDto.DateTime - shiftEndTime;
                }

                await _context.SaveChangesAsync();

                var scheduleDto = new RosterScheduleDto
                {
                    Id = schedule.Id,
                    EmployeeId = schedule.EmployeeId,
                    EmployeeName = schedule.Employee.Name,
                    EmployeeNameBangla = schedule.Employee.NameBangla,
                    ShiftId = schedule.ShiftId,
                    ShiftName = schedule.Shift.Name,
                    ShiftNameBangla = schedule.Shift.NameBangla,
                    ShiftStartTime = schedule.Shift.StartTime,
                    ShiftEndTime = schedule.Shift.EndTime,
                    ScheduleDate = schedule.ScheduleDate,
                    CompanyId = schedule.CompanyId,
                    CompanyName = schedule.Company.Name,
                    Status = schedule.Status,
                    StatusBangla = schedule.StatusBangla,
                    Notes = schedule.Notes,
                    NotesBangla = schedule.NotesBangla,
                    CheckInTime = schedule.CheckInTime,
                    CheckOutTime = schedule.CheckOutTime,
                    OvertimeHours = schedule.OvertimeHours,
                    CreatedAt = schedule.CreatedAt,
                    UpdatedAt = schedule.UpdatedAt,
                    IsActive = schedule.IsActive,
                    CreatedBy = schedule.CreatedBy,
                    UpdatedBy = schedule.UpdatedBy
                };

                _logger.LogInformation("Check-out successful for roster schedule {ScheduleId} by user {UserId}", 
                    checkOutDto.RosterScheduleId, userId);

                return Ok(new ApiResponse<RosterScheduleDto>
                {
                    Success = true,
                    Message = "Check-out successful",
                    Data = scheduleDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during check-out for roster schedule {ScheduleId}", checkOutDto.RosterScheduleId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during check-out",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a roster schedule (Admin only)
        /// </summary>
        /// <param name="id">Roster schedule ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Roster schedule deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Roster schedule not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteRosterSchedule(int id)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var schedule = await _context.RosterSchedules.FindAsync(id);
                if (schedule == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Roster schedule not found",
                        Errors = new List<string> { $"Roster schedule with ID {id} does not exist" }
                    });
                }

                _context.RosterSchedules.Remove(schedule);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Roster schedule {ScheduleId} deleted successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Roster schedule deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting roster schedule with ID {ScheduleId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the roster schedule",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
