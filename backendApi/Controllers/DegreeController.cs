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
    public class DegreeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DegreeController> _logger;

        public DegreeController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<DegreeController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get degrees based on user role (Admin gets all degrees, others get their assigned company degrees)
        /// </summary>
        /// <param name="companyId">Optional company ID to filter degrees (Admin only)</param>
        /// <param name="includeInactive">Include inactive degrees (Admin only)</param>
        /// <param name="level">Filter by degree level (e.g., SSC, HSC, Bachelor, Master, PhD)</param>
        /// <returns>List of degrees based on user role</returns>
        /// <response code="200">Degrees retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DegreeListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllDegrees([FromQuery] int? companyId = null, [FromQuery] bool includeInactive = false, [FromQuery] string? level = null)
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

                List<DegreeListDto> degrees;

                if (isAdmin)
                {
                    // Admin users can see all degrees
                    var query = _context.Degrees
                        .Include(d => d.Company)
                        .AsQueryable();

                    if (companyId.HasValue)
                    {
                        query = query.Where(d => d.CompanyId == companyId.Value);
                    }

                    if (!string.IsNullOrEmpty(level))
                    {
                        query = query.Where(d => d.Level.ToLower() == level.ToLower());
                    }

                    if (!includeInactive)
                    {
                        query = query.Where(d => d.IsActive);
                    }

                    degrees = await query
                        .OrderBy(d => d.Company.Name)
                        .ThenBy(d => d.Level)
                        .ThenBy(d => d.Name)
                        .Select(d => new DegreeListDto
                        {
                            Id = d.Id,
                            Name = d.Name,
                            NameBangla = d.NameBangla,
                            Level = d.Level,
                            LevelBangla = d.LevelBangla,
                            InstitutionType = d.InstitutionType,
                            InstitutionTypeBangla = d.InstitutionTypeBangla,
                            CompanyId = d.CompanyId,
                            CompanyName = d.Company.Name,
                            IsActive = d.IsActive
                        })
                        .ToListAsync();

                    _logger.LogInformation("Admin user {UserId} retrieved all degrees", userId);
                }
                else
                {
                    // Non-admin users can only see degrees from their assigned companies
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
                        degrees = new List<DegreeListDto>();
                    }
                    else
                    {
                        var query = _context.Degrees
                            .Include(d => d.Company)
                            .Where(d => userCompanyIds.Contains(d.CompanyId) && d.IsActive);

                        if (!string.IsNullOrEmpty(level))
                        {
                            query = query.Where(d => d.Level.ToLower() == level.ToLower());
                        }

                        degrees = await query
                            .OrderBy(d => d.Company.Name)
                            .ThenBy(d => d.Level)
                            .ThenBy(d => d.Name)
                            .Select(d => new DegreeListDto
                            {
                                Id = d.Id,
                                Name = d.Name,
                                NameBangla = d.NameBangla,
                                Level = d.Level,
                                LevelBangla = d.LevelBangla,
                                InstitutionType = d.InstitutionType,
                                InstitutionTypeBangla = d.InstitutionTypeBangla,
                                CompanyId = d.CompanyId,
                                CompanyName = d.Company.Name,
                                IsActive = d.IsActive
                            })
                            .ToListAsync();
                    }

                    _logger.LogInformation("User {UserId} retrieved {Count} degrees from assigned companies", userId, degrees.Count);
                }

                return Ok(new ApiResponse<IEnumerable<DegreeListDto>>
                {
                    Success = true,
                    Message = isAdmin ? "All degrees retrieved successfully" : "Assigned company degrees retrieved successfully",
                    Data = degrees
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving degrees");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving degrees",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get degree by ID (Admin gets any degree, others only from their assigned companies)
        /// </summary>
        /// <param name="id">Degree ID</param>
        /// <returns>Degree details</returns>
        /// <response code="200">Degree retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Degree not accessible to user</response>
        /// <response code="404">Degree not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<DegreeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDegreeById(int id)
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

                var degree = await _context.Degrees
                    .Include(d => d.Company)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (degree == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Degree not found",
                        Errors = new List<string> { $"Degree with ID {id} does not exist" }
                    });
                }

                // Check if user has access to this degree
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

                    if (!userCompanyIds.Contains(degree.CompanyId))
                    {
                        return Forbid();
                    }
                }

                var degreeDto = new DegreeDto
                {
                    Id = degree.Id,
                    Name = degree.Name,
                    NameBangla = degree.NameBangla,
                    Level = degree.Level,
                    LevelBangla = degree.LevelBangla,
                    InstitutionType = degree.InstitutionType,
                    InstitutionTypeBangla = degree.InstitutionTypeBangla,
                    CompanyId = degree.CompanyId,
                    CompanyName = degree.Company.Name,
                    CreatedAt = degree.CreatedAt,
                    UpdatedAt = degree.UpdatedAt,
                    IsActive = degree.IsActive,
                    CreatedBy = degree.CreatedBy,
                    UpdatedBy = degree.UpdatedBy
                };

                return Ok(new ApiResponse<DegreeDto>
                {
                    Success = true,
                    Message = "Degree retrieved successfully",
                    Data = degreeDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving degree with ID {DegreeId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the degree",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get common Bangladesh degrees and certificates
        /// </summary>
        /// <returns>List of common Bangladesh degrees</returns>
        /// <response code="200">Common degrees retrieved successfully</response>
        [HttpGet("common")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DegreeTemplate>>), 200)]
        public IActionResult GetCommonBangladeshDegrees()
        {
            try
            {
                return Ok(new ApiResponse<IEnumerable<DegreeTemplate>>
                {
                    Success = true,
                    Message = "Common Bangladesh degrees retrieved successfully",
                    Data = BangladeshDegrees.CommonDegrees
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving common Bangladesh degrees");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving common degrees",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new degree (Admin and HR roles only)
        /// </summary>
        /// <param name="createDegreeDto">Degree creation data</param>
        /// <returns>Created degree details</returns>
        /// <response code="201">Degree created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <response code="409">Conflict - Degree already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,HR,HR Manager")]
        [ProducesResponseType(typeof(ApiResponse<DegreeDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateDegree([FromBody] CreateDegreeDto createDegreeDto)
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

                    if (!userCompanyIds.Contains(createDegreeDto.CompanyId))
                    {
                        return Forbid();
                    }
                }

                // Check if company exists
                var company = await _context.Companies.FindAsync(createDegreeDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {createDegreeDto.CompanyId} does not exist" }
                    });
                }

                // Check if degree with same name already exists in the company
                var existingDegree = await _context.Degrees
                    .FirstOrDefaultAsync(d => d.CompanyId == createDegreeDto.CompanyId && 
                                            d.Name.ToLower() == createDegreeDto.Name.ToLower());

                if (existingDegree != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Degree already exists",
                        Errors = new List<string> { $"A degree with name '{createDegreeDto.Name}' already exists in this company" }
                    });
                }

                var degree = new Degree
                {
                    Name = createDegreeDto.Name,
                    NameBangla = createDegreeDto.NameBangla,
                    Level = createDegreeDto.Level,
                    LevelBangla = createDegreeDto.LevelBangla,
                    InstitutionType = createDegreeDto.InstitutionType,
                    InstitutionTypeBangla = createDegreeDto.InstitutionTypeBangla,
                    CompanyId = createDegreeDto.CompanyId,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Degrees.Add(degree);
                await _context.SaveChangesAsync();

                // Load the company for the response
                await _context.Entry(degree)
                    .Reference(d => d.Company)
                    .LoadAsync();

                var degreeDto = new DegreeDto
                {
                    Id = degree.Id,
                    Name = degree.Name,
                    NameBangla = degree.NameBangla,
                    Level = degree.Level,
                    LevelBangla = degree.LevelBangla,
                    InstitutionType = degree.InstitutionType,
                    InstitutionTypeBangla = degree.InstitutionTypeBangla,
                    CompanyId = degree.CompanyId,
                    CompanyName = degree.Company.Name,
                    CreatedAt = degree.CreatedAt,
                    UpdatedAt = degree.UpdatedAt,
                    IsActive = degree.IsActive,
                    CreatedBy = degree.CreatedBy,
                    UpdatedBy = degree.UpdatedBy
                };

                _logger.LogInformation("Degree '{DegreeName}' created successfully by user {UserId} for company {CompanyId}", 
                    degree.Name, userId, degree.CompanyId);

                return CreatedAtAction(nameof(GetDegreeById), new { id = degree.Id }, new ApiResponse<DegreeDto>
                {
                    Success = true,
                    Message = "Degree created successfully",
                    Data = degreeDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating degree");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the degree",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing degree (Admin and HR roles only)
        /// </summary>
        /// <param name="id">Degree ID</param>
        /// <param name="updateDegreeDto">Degree update data</param>
        /// <returns>Updated degree details</returns>
        /// <response code="200">Degree updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <response code="404">Degree not found</response>
        /// <response code="409">Conflict - Degree name already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR,HR Manager")]
        [ProducesResponseType(typeof(ApiResponse<DegreeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateDegree(int id, [FromBody] UpdateDegreeDto updateDegreeDto)
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

                var degree = await _context.Degrees
                    .Include(d => d.Company)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (degree == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Degree not found",
                        Errors = new List<string> { $"Degree with ID {id} does not exist" }
                    });
                }

                // Check if user has access to this degree
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

                    if (!userCompanyIds.Contains(degree.CompanyId))
                    {
                        return Forbid();
                    }
                }

                // Check if company exists
                var company = await _context.Companies.FindAsync(updateDegreeDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {updateDegreeDto.CompanyId} does not exist" }
                    });
                }

                // Check if degree with same name already exists in the company (excluding current degree)
                var existingDegree = await _context.Degrees
                    .FirstOrDefaultAsync(d => d.CompanyId == updateDegreeDto.CompanyId && 
                                            d.Name.ToLower() == updateDegreeDto.Name.ToLower() &&
                                            d.Id != id);

                if (existingDegree != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Degree name already exists",
                        Errors = new List<string> { $"A degree with name '{updateDegreeDto.Name}' already exists in this company" }
                    });
                }

                // Update degree properties
                degree.Name = updateDegreeDto.Name;
                degree.NameBangla = updateDegreeDto.NameBangla;
                degree.Level = updateDegreeDto.Level;
                degree.LevelBangla = updateDegreeDto.LevelBangla;
                degree.InstitutionType = updateDegreeDto.InstitutionType;
                degree.InstitutionTypeBangla = updateDegreeDto.InstitutionTypeBangla;
                degree.CompanyId = updateDegreeDto.CompanyId;
                degree.IsActive = updateDegreeDto.IsActive;
                degree.UpdatedBy = userId;
                degree.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Reload the company for the response
                await _context.Entry(degree)
                    .Reference(d => d.Company)
                    .LoadAsync();

                var degreeDto = new DegreeDto
                {
                    Id = degree.Id,
                    Name = degree.Name,
                    NameBangla = degree.NameBangla,
                    Level = degree.Level,
                    LevelBangla = degree.LevelBangla,
                    InstitutionType = degree.InstitutionType,
                    InstitutionTypeBangla = degree.InstitutionTypeBangla,
                    CompanyId = degree.CompanyId,
                    CompanyName = degree.Company.Name,
                    CreatedAt = degree.CreatedAt,
                    UpdatedAt = degree.UpdatedAt,
                    IsActive = degree.IsActive,
                    CreatedBy = degree.CreatedBy,
                    UpdatedBy = degree.UpdatedBy
                };

                _logger.LogInformation("Degree '{DegreeName}' updated successfully by user {UserId}", degree.Name, userId);

                return Ok(new ApiResponse<DegreeDto>
                {
                    Success = true,
                    Message = "Degree updated successfully",
                    Data = degreeDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating degree with ID {DegreeId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the degree",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a degree (Admin only)
        /// </summary>
        /// <param name="id">Degree ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Degree deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Degree not found</response>
        /// <response code="409">Conflict - Degree cannot be deleted due to dependencies</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteDegree(int id)
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

                var degree = await _context.Degrees.FindAsync(id);
                if (degree == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Degree not found",
                        Errors = new List<string> { $"Degree with ID {id} does not exist" }
                    });
                }

                // Check if degree is being used by any employees
                var hasEmployees = await _context.Employees.AnyAsync(e => e.DepartmentId == id);
                if (hasEmployees)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete degree",
                        Errors = new List<string> { "This degree is currently assigned to employees and cannot be deleted" }
                    });
                }

                _context.Degrees.Remove(degree);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Degree '{DegreeName}' deleted successfully by user {UserId}", degree.Name, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Degree deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting degree with ID {DegreeId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the degree",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
