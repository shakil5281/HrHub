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
    public class ShiftController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ShiftController> _logger;

        public ShiftController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<ShiftController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get shifts based on user role (Admin gets all shifts, others get their assigned company shifts)
        /// </summary>
        /// <param name="companyId">Optional company ID to filter shifts (Admin only)</param>
        /// <param name="includeInactive">Include inactive shifts (Admin only)</param>
        /// <returns>List of shifts based on user role</returns>
        /// <response code="200">Shifts retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ShiftListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllShifts([FromQuery] int? companyId = null, [FromQuery] bool includeInactive = false)
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

                List<ShiftListDto> shifts;

                if (isAdmin)
                {
                    // Admin users can see all shifts
                    var query = _context.Shifts
                        .Include(s => s.Company)
                        .AsQueryable();

                    if (companyId.HasValue)
                    {
                        query = query.Where(s => s.CompanyId == companyId.Value);
                    }

                    if (!includeInactive)
                    {
                        query = query.Where(s => s.IsActive);
                    }

                    shifts = await query
                        .OrderBy(s => s.Company.Name)
                        .ThenBy(s => s.StartTime)
                        .Select(s => new ShiftListDto
                        {
                            Id = s.Id,
                            Name = s.Name,
                            NameBangla = s.NameBangla,
                            StartTime = s.StartTime,
                            EndTime = s.EndTime,
                            BreakStartTime = s.BreakStartTime,
                            BreakEndTime = s.BreakEndTime,
                            CompanyId = s.CompanyId,
                            CompanyName = s.Company.Name,
                            IsActive = s.IsActive
                        })
                        .ToListAsync();

                    _logger.LogInformation("Admin user {UserId} retrieved all shifts", userId);
                }
                else
                {
                    // Non-admin users can only see shifts from their assigned companies
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
                        // User has no company assigned
                        shifts = new List<ShiftListDto>();
                    }
                    else
                    {
                        var query = _context.Shifts
                            .Include(s => s.Company)
                            .Where(s => userCompanyIds.Contains(s.CompanyId) && s.IsActive);

                        shifts = await query
                            .OrderBy(s => s.Company.Name)
                            .ThenBy(s => s.StartTime)
                            .Select(s => new ShiftListDto
                            {
                                Id = s.Id,
                                Name = s.Name,
                                NameBangla = s.NameBangla,
                                StartTime = s.StartTime,
                                EndTime = s.EndTime,
                                BreakStartTime = s.BreakStartTime,
                                BreakEndTime = s.BreakEndTime,
                                CompanyId = s.CompanyId,
                                CompanyName = s.Company.Name,
                                IsActive = s.IsActive
                            })
                            .ToListAsync();
                    }

                    _logger.LogInformation("User {UserId} retrieved {Count} shifts from assigned companies", userId, shifts.Count);
                }

                return Ok(new ApiResponse<IEnumerable<ShiftListDto>>
                {
                    Success = true,
                    Message = isAdmin ? "All shifts retrieved successfully" : "Assigned company shifts retrieved successfully",
                    Data = shifts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving shifts");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving shifts",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get shift by ID (Admin gets any shift, others only from their assigned companies)
        /// </summary>
        /// <param name="id">Shift ID</param>
        /// <returns>Shift details</returns>
        /// <response code="200">Shift retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Shift not accessible to user</response>
        /// <response code="404">Shift not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<ShiftDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetShiftById(int id)
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

                var shift = await _context.Shifts
                    .Include(s => s.Company)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (shift == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Shift not found",
                        Errors = new List<string> { $"Shift with ID {id} does not exist" }
                    });
                }

                // Check if user has access to this shift
                if (!isAdmin)
                {
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

                    if (!userCompanyIds.Contains(shift.CompanyId))
                    {
                        return Forbid();
                    }
                }

                var shiftDto = new ShiftDto
                {
                    Id = shift.Id,
                    Name = shift.Name,
                    NameBangla = shift.NameBangla,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    BreakStartTime = shift.BreakStartTime,
                    BreakEndTime = shift.BreakEndTime,
                    CompanyId = shift.CompanyId,
                    CompanyName = shift.Company.Name,
                    CreatedAt = shift.CreatedAt,
                    UpdatedAt = shift.UpdatedAt,
                    IsActive = shift.IsActive,
                    CreatedBy = shift.CreatedBy,
                    UpdatedBy = shift.UpdatedBy
                };

                return Ok(new ApiResponse<ShiftDto>
                {
                    Success = true,
                    Message = "Shift retrieved successfully",
                    Data = shiftDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving shift with ID {ShiftId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the shift",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new shift (Admin and HR roles only)
        /// </summary>
        /// <param name="createShiftDto">Shift creation data</param>
        /// <returns>Created shift details</returns>
        /// <response code="201">Shift created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <response code="409">Conflict - Shift already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,HR,HR Manager")]
        [ProducesResponseType(typeof(ApiResponse<ShiftDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateShift([FromBody] CreateShiftDto createShiftDto)
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

                    if (!userCompanyIds.Contains(createShiftDto.CompanyId))
                    {
                        return Forbid();
                    }
                }

                // Validate shift times
                if (createShiftDto.StartTime >= createShiftDto.EndTime)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid shift times",
                        Errors = new List<string> { "Start time must be before end time" }
                    });
                }

                // Validate break times if provided
                if (createShiftDto.BreakStartTime.HasValue && createShiftDto.BreakEndTime.HasValue)
                {
                    if (createShiftDto.BreakStartTime.Value >= createShiftDto.BreakEndTime.Value)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid break times",
                            Errors = new List<string> { "Break start time must be before break end time" }
                        });
                    }

                    if (createShiftDto.BreakStartTime.Value < createShiftDto.StartTime || 
                        createShiftDto.BreakEndTime.Value > createShiftDto.EndTime)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid break times",
                            Errors = new List<string> { "Break times must be within shift hours" }
                        });
                    }
                }

                // Check if company exists
                var company = await _context.Companies.FindAsync(createShiftDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {createShiftDto.CompanyId} does not exist" }
                    });
                }

                // Check if shift with same name already exists in the company
                var existingShift = await _context.Shifts
                    .FirstOrDefaultAsync(s => s.CompanyId == createShiftDto.CompanyId && 
                                            s.Name.ToLower() == createShiftDto.Name.ToLower());

                if (existingShift != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Shift already exists",
                        Errors = new List<string> { $"A shift with name '{createShiftDto.Name}' already exists in this company" }
                    });
                }

                var shift = new Shift
                {
                    Name = createShiftDto.Name,
                    NameBangla = createShiftDto.NameBangla,
                    StartTime = createShiftDto.StartTime,
                    EndTime = createShiftDto.EndTime,
                    BreakStartTime = createShiftDto.BreakStartTime,
                    BreakEndTime = createShiftDto.BreakEndTime,
                    CompanyId = createShiftDto.CompanyId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Shifts.Add(shift);
                await _context.SaveChangesAsync();

                // Load the company for the response
                await _context.Entry(shift)
                    .Reference(s => s.Company)
                    .LoadAsync();

                var shiftDto = new ShiftDto
                {
                    Id = shift.Id,
                    Name = shift.Name,
                    NameBangla = shift.NameBangla,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    BreakStartTime = shift.BreakStartTime,
                    BreakEndTime = shift.BreakEndTime,
                    CompanyId = shift.CompanyId,
                    CompanyName = shift.Company.Name,
                    CreatedAt = shift.CreatedAt,
                    UpdatedAt = shift.UpdatedAt,
                    IsActive = shift.IsActive,
                    CreatedBy = shift.CreatedBy,
                    UpdatedBy = shift.UpdatedBy
                };

                _logger.LogInformation("Shift '{ShiftName}' created successfully by user {UserId} for company {CompanyId}", 
                    shift.Name, userId, shift.CompanyId);

                return CreatedAtAction(nameof(GetShiftById), new { id = shift.Id }, new ApiResponse<ShiftDto>
                {
                    Success = true,
                    Message = "Shift created successfully",
                    Data = shiftDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating shift");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the shift",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing shift (Admin and HR roles only)
        /// </summary>
        /// <param name="id">Shift ID</param>
        /// <param name="updateShiftDto">Shift update data</param>
        /// <returns>Updated shift details</returns>
        /// <response code="200">Shift updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <response code="404">Shift not found</response>
        /// <response code="409">Conflict - Shift name already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR,HR Manager")]
        [ProducesResponseType(typeof(ApiResponse<ShiftDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateShift(int id, [FromBody] UpdateShiftDto updateShiftDto)
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

                var shift = await _context.Shifts
                    .Include(s => s.Company)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (shift == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Shift not found",
                        Errors = new List<string> { $"Shift with ID {id} does not exist" }
                    });
                }

                // Check if user has access to this shift
                if (!isAdmin)
                {
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

                    if (!userCompanyIds.Contains(shift.CompanyId))
                    {
                        return Forbid();
                    }
                }

                // Validate shift times
                if (updateShiftDto.StartTime >= updateShiftDto.EndTime)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid shift times",
                        Errors = new List<string> { "Start time must be before end time" }
                    });
                }

                // Validate break times if provided
                if (updateShiftDto.BreakStartTime.HasValue && updateShiftDto.BreakEndTime.HasValue)
                {
                    if (updateShiftDto.BreakStartTime.Value >= updateShiftDto.BreakEndTime.Value)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid break times",
                            Errors = new List<string> { "Break start time must be before break end time" }
                        });
                    }

                    if (updateShiftDto.BreakStartTime.Value < updateShiftDto.StartTime || 
                        updateShiftDto.BreakEndTime.Value > updateShiftDto.EndTime)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid break times",
                            Errors = new List<string> { "Break times must be within shift hours" }
                        });
                    }
                }

                // Check if company exists
                var company = await _context.Companies.FindAsync(updateShiftDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {updateShiftDto.CompanyId} does not exist" }
                    });
                }

                // Check if shift with same name already exists in the company (excluding current shift)
                var existingShift = await _context.Shifts
                    .FirstOrDefaultAsync(s => s.CompanyId == updateShiftDto.CompanyId && 
                                            s.Name.ToLower() == updateShiftDto.Name.ToLower() &&
                                            s.Id != id);

                if (existingShift != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Shift name already exists",
                        Errors = new List<string> { $"A shift with name '{updateShiftDto.Name}' already exists in this company" }
                    });
                }

                // Update shift properties
                shift.Name = updateShiftDto.Name;
                shift.NameBangla = updateShiftDto.NameBangla;
                shift.StartTime = updateShiftDto.StartTime;
                shift.EndTime = updateShiftDto.EndTime;
                shift.BreakStartTime = updateShiftDto.BreakStartTime;
                shift.BreakEndTime = updateShiftDto.BreakEndTime;
                shift.CompanyId = updateShiftDto.CompanyId;
                shift.IsActive = updateShiftDto.IsActive;
                shift.UpdatedBy = userId;
                shift.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Reload the company for the response
                await _context.Entry(shift)
                    .Reference(s => s.Company)
                    .LoadAsync();

                var shiftDto = new ShiftDto
                {
                    Id = shift.Id,
                    Name = shift.Name,
                    NameBangla = shift.NameBangla,
                    StartTime = shift.StartTime,
                    EndTime = shift.EndTime,
                    BreakStartTime = shift.BreakStartTime,
                    BreakEndTime = shift.BreakEndTime,
                    CompanyId = shift.CompanyId,
                    CompanyName = shift.Company.Name,
                    CreatedAt = shift.CreatedAt,
                    UpdatedAt = shift.UpdatedAt,
                    IsActive = shift.IsActive,
                    CreatedBy = shift.CreatedBy,
                    UpdatedBy = shift.UpdatedBy
                };

                _logger.LogInformation("Shift '{ShiftName}' updated successfully by user {UserId}", shift.Name, userId);

                return Ok(new ApiResponse<ShiftDto>
                {
                    Success = true,
                    Message = "Shift updated successfully",
                    Data = shiftDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating shift with ID {ShiftId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the shift",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a shift (Admin only)
        /// </summary>
        /// <param name="id">Shift ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Shift deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Shift not found</response>
        /// <response code="409">Conflict - Shift cannot be deleted due to dependencies</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteShift(int id)
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

                var shift = await _context.Shifts.FindAsync(id);
                if (shift == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Shift not found",
                        Errors = new List<string> { $"Shift with ID {id} does not exist" }
                    });
                }

                // Check if shift is being used by any employees
                var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
                if (hasEmployees)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete shift",
                        Errors = new List<string> { "This shift is currently assigned to employees and cannot be deleted" }
                    });
                }

                _context.Shifts.Remove(shift);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Shift '{ShiftName}' deleted successfully by user {UserId}", shift.Name, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Shift deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting shift with ID {ShiftId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the shift",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
