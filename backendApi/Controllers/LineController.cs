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
    public class LineController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LineController> _logger;

        public LineController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<LineController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get lines based on user role (Admin gets all lines, others get their assigned company lines)
        /// </summary>
        /// <param name="companyId">Optional company ID to filter lines (Admin only)</param>
        /// <param name="includeInactive">Include inactive lines (Admin only)</param>
        /// <returns>List of lines based on user role</returns>
        /// <response code="200">Lines retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<LineListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllLines([FromQuery] int? companyId = null, [FromQuery] bool includeInactive = false)
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

                List<LineListDto> lines;

                if (isAdmin)
                {
                    // Admin users can see all lines
                    var query = _context.Lines
                        .Include(l => l.Company)
                        .AsQueryable();

                    if (companyId.HasValue)
                    {
                        query = query.Where(l => l.CompanyId == companyId.Value);
                    }

                    if (!includeInactive)
                    {
                        query = query.Where(l => l.IsActive);
                    }

                    lines = await query
                        .OrderBy(l => l.Company.Name)
                        .ThenBy(l => l.Name)
                        .Select(l => new LineListDto
                        {
                            Id = l.Id,
                            Name = l.Name,
                            NameBangla = l.NameBangla,
                            CompanyId = l.CompanyId,
                            CompanyName = l.Company.Name,
                            IsActive = l.IsActive
                        })
                        .ToListAsync();

                    _logger.LogInformation("Admin user {UserId} retrieved all lines", userId);
                }
                else
                {
                    // Non-admin users can only see lines from their assigned companies
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
                        lines = new List<LineListDto>();
                    }
                    else
                    {
                        var query = _context.Lines
                            .Include(l => l.Company)
                            .Where(l => userCompanyIds.Contains(l.CompanyId) && l.IsActive);

                        lines = await query
                            .OrderBy(l => l.Company.Name)
                            .ThenBy(l => l.Name)
                            .Select(l => new LineListDto
                            {
                                Id = l.Id,
                                Name = l.Name,
                                NameBangla = l.NameBangla,
                                CompanyId = l.CompanyId,
                                CompanyName = l.Company.Name,
                                IsActive = l.IsActive
                            })
                            .ToListAsync();
                    }

                    _logger.LogInformation("User {UserId} retrieved {Count} lines from assigned companies", userId, lines.Count);
                }

                return Ok(new ApiResponse<IEnumerable<LineListDto>>
                {
                    Success = true,
                    Message = isAdmin ? "All lines retrieved successfully" : "Assigned company lines retrieved successfully",
                    Data = lines
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving lines");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving lines",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get line by ID (Admin gets any line, others only from their assigned companies)
        /// </summary>
        /// <param name="id">Line ID</param>
        /// <returns>Line details</returns>
        /// <response code="200">Line retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Line not accessible to user</response>
        /// <response code="404">Line not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<LineDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetLineById(int id)
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

                var line = await _context.Lines
                    .Include(l => l.Company)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (line == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Line not found",
                        Errors = new List<string> { $"Line with ID {id} does not exist" }
                    });
                }

                // Check if user has access to this line
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

                    if (!userCompanyIds.Contains(line.CompanyId))
                    {
                        return Forbid();
                    }
                }

                var lineDto = new LineDto
                {
                    Id = line.Id,
                    Name = line.Name,
                    NameBangla = line.NameBangla,
                    CompanyId = line.CompanyId,
                    CompanyName = line.Company.Name,
                    CreatedAt = line.CreatedAt,
                    UpdatedAt = line.UpdatedAt,
                    IsActive = line.IsActive,
                    CreatedBy = line.CreatedBy,
                    UpdatedBy = line.UpdatedBy
                };

                return Ok(new ApiResponse<LineDto>
                {
                    Success = true,
                    Message = "Line retrieved successfully",
                    Data = lineDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving line with ID {LineId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the line",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new line (Admin and HR roles only)
        /// </summary>
        /// <param name="createLineDto">Line creation data</param>
        /// <returns>Created line details</returns>
        /// <response code="201">Line created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <response code="409">Conflict - Line already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,HR,HR Manager")]
        [ProducesResponseType(typeof(ApiResponse<LineDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateLine([FromBody] CreateLineDto createLineDto)
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

                    if (!userCompanyIds.Contains(createLineDto.CompanyId))
                    {
                        return Forbid();
                    }
                }

                // Check if company exists
                var company = await _context.Companies.FindAsync(createLineDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {createLineDto.CompanyId} does not exist" }
                    });
                }

                // Check if line with same name already exists in the company
                var existingLine = await _context.Lines
                    .FirstOrDefaultAsync(l => l.CompanyId == createLineDto.CompanyId && 
                                            l.Name.ToLower() == createLineDto.Name.ToLower());

                if (existingLine != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Line already exists",
                        Errors = new List<string> { $"A line with name '{createLineDto.Name}' already exists in this company" }
                    });
                }

                var line = new Line
                {
                    Name = createLineDto.Name,
                    NameBangla = createLineDto.NameBangla,
                    CompanyId = createLineDto.CompanyId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Lines.Add(line);
                await _context.SaveChangesAsync();

                // Load the company for the response
                await _context.Entry(line)
                    .Reference(l => l.Company)
                    .LoadAsync();

                var lineDto = new LineDto
                {
                    Id = line.Id,
                    Name = line.Name,
                    NameBangla = line.NameBangla,
                    CompanyId = line.CompanyId,
                    CompanyName = line.Company.Name,
                    CreatedAt = line.CreatedAt,
                    UpdatedAt = line.UpdatedAt,
                    IsActive = line.IsActive,
                    CreatedBy = line.CreatedBy,
                    UpdatedBy = line.UpdatedBy
                };

                _logger.LogInformation("Line '{LineName}' created successfully by user {UserId} for company {CompanyId}", 
                    line.Name, userId, line.CompanyId);

                return CreatedAtAction(nameof(GetLineById), new { id = line.Id }, new ApiResponse<LineDto>
                {
                    Success = true,
                    Message = "Line created successfully",
                    Data = lineDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating line");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the line",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing line (Admin and HR roles only)
        /// </summary>
        /// <param name="id">Line ID</param>
        /// <param name="updateLineDto">Line update data</param>
        /// <returns>Updated line details</returns>
        /// <response code="200">Line updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <response code="404">Line not found</response>
        /// <response code="409">Conflict - Line name already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR,HR Manager")]
        [ProducesResponseType(typeof(ApiResponse<LineDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateLine(int id, [FromBody] UpdateLineDto updateLineDto)
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

                var line = await _context.Lines
                    .Include(l => l.Company)
                    .FirstOrDefaultAsync(l => l.Id == id);

                if (line == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Line not found",
                        Errors = new List<string> { $"Line with ID {id} does not exist" }
                    });
                }

                // Check if user has access to this line
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

                    if (!userCompanyIds.Contains(line.CompanyId))
                    {
                        return Forbid();
                    }
                }

                // Check if company exists
                var company = await _context.Companies.FindAsync(updateLineDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {updateLineDto.CompanyId} does not exist" }
                    });
                }

                // Check if line with same name already exists in the company (excluding current line)
                var existingLine = await _context.Lines
                    .FirstOrDefaultAsync(l => l.CompanyId == updateLineDto.CompanyId && 
                                            l.Name.ToLower() == updateLineDto.Name.ToLower() &&
                                            l.Id != id);

                if (existingLine != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Line name already exists",
                        Errors = new List<string> { $"A line with name '{updateLineDto.Name}' already exists in this company" }
                    });
                }

                // Update line properties
                line.Name = updateLineDto.Name;
                line.NameBangla = updateLineDto.NameBangla;
                line.CompanyId = updateLineDto.CompanyId;
                line.IsActive = updateLineDto.IsActive;
                line.UpdatedBy = userId;
                line.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Reload the company for the response
                await _context.Entry(line)
                    .Reference(l => l.Company)
                    .LoadAsync();

                var lineDto = new LineDto
                {
                    Id = line.Id,
                    Name = line.Name,
                    NameBangla = line.NameBangla,
                    CompanyId = line.CompanyId,
                    CompanyName = line.Company.Name,
                    CreatedAt = line.CreatedAt,
                    UpdatedAt = line.UpdatedAt,
                    IsActive = line.IsActive,
                    CreatedBy = line.CreatedBy,
                    UpdatedBy = line.UpdatedBy
                };

                _logger.LogInformation("Line '{LineName}' updated successfully by user {UserId}", line.Name, userId);

                return Ok(new ApiResponse<LineDto>
                {
                    Success = true,
                    Message = "Line updated successfully",
                    Data = lineDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating line with ID {LineId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the line",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a line (Admin only)
        /// </summary>
        /// <param name="id">Line ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Line deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Line not found</response>
        /// <response code="409">Conflict - Line cannot be deleted due to dependencies</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteLine(int id)
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

                var line = await _context.Lines.FindAsync(id);
                if (line == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Line not found",
                        Errors = new List<string> { $"Line with ID {id} does not exist" }
                    });
                }

                // Check if line is being used by any employees
                var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
                if (hasEmployees)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete line",
                        Errors = new List<string> { "This line is currently assigned to employees and cannot be deleted" }
                    });
                }

                _context.Lines.Remove(line);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Line '{LineName}' deleted successfully by user {UserId}", line.Name, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Line deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting line with ID {LineId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the line",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
