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
    public class SectionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<SectionController> _logger;

        public SectionController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<SectionController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get sections based on user role (Admin gets all sections, others get sections from their assigned companies)
        /// </summary>
        /// <param name="departmentId">Optional department ID to filter sections (Admin only)</param>
        /// <param name="includeInactive">Include inactive sections (Admin only)</param>
        /// <returns>List of sections based on user role</returns>
        /// <response code="200">Sections retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SectionListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllSections([FromQuery] int? departmentId = null, [FromQuery] bool includeInactive = false)
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

                List<SectionListDto> sections;

                if (isAdmin)
                {
                    // Admin users can see all sections
                    var query = _context.Sections
                        .Include(s => s.Department)
                        .AsQueryable();

                    if (departmentId.HasValue)
                    {
                        query = query.Where(s => s.DepartmentId == departmentId.Value);
                    }

                    if (!includeInactive)
                    {
                        query = query.Where(s => s.IsActive);
                    }

                    sections = await query
                        .OrderBy(s => s.Department.Name)
                        .ThenBy(s => s.Name)
                        .Select(s => new SectionListDto
                        {
                            Id = s.Id,
                            DepartmentId = s.DepartmentId,
                            DepartmentName = s.Department.Name,
                            Name = s.Name,
                            NameBangla = s.NameBangla,
                            IsActive = s.IsActive,
                            CreatedAt = s.CreatedAt
                        })
                        .ToListAsync();

                    _logger.LogInformation("Admin user {UserId} retrieved all sections", userId);
                }
                else
                {
                    // Non-admin users can only see sections from their assigned companies
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
                        sections = new List<SectionListDto>();
                    }
                    else
                    {
                        var query = _context.Sections
                            .Include(s => s.Department)
                            .Where(s => userCompanyIds.Contains(s.Department.CompanyId) && s.IsActive);

                        sections = await query
                            .OrderBy(s => s.Department.Name)
                            .ThenBy(s => s.Name)
                            .Select(s => new SectionListDto
                            {
                                Id = s.Id,
                                DepartmentId = s.DepartmentId,
                                DepartmentName = s.Department.Name,
                                Name = s.Name,
                                NameBangla = s.NameBangla,
                                IsActive = s.IsActive,
                                CreatedAt = s.CreatedAt
                            })
                            .ToListAsync();
                    }

                    _logger.LogInformation("User {UserId} retrieved {Count} sections from assigned companies", userId, sections.Count);
                }

                return Ok(new ApiResponse<IEnumerable<SectionListDto>>
                {
                    Success = true,
                    Message = isAdmin ? "All sections retrieved successfully" : "Assigned company sections retrieved successfully",
                    Data = sections
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving sections");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving sections",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get section by ID (Admin/Manager/HR/IT only)
        /// </summary>
        /// <param name="id">Section ID</param>
        /// <returns>Section details</returns>
        /// <response code="200">Section retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/Manager/HR/IT role required</response>
        /// <response code="404">Section not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<SectionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSectionById(int id)
        {
            try
            {
                var section = await _context.Sections
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (section == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section not found",
                        Errors = new List<string> { $"Section with ID {id} does not exist" }
                    });
                }

                var sectionDto = new SectionDto
                {
                    Id = section.Id,
                    DepartmentId = section.DepartmentId,
                    DepartmentName = section.Department.Name,
                    Name = section.Name,
                    NameBangla = section.NameBangla,
                    CreatedAt = section.CreatedAt,
                    UpdatedAt = section.UpdatedAt,
                    IsActive = section.IsActive,
                    CreatedBy = section.CreatedBy,
                    UpdatedBy = section.UpdatedBy
                };

                return Ok(new ApiResponse<SectionDto>
                {
                    Success = true,
                    Message = "Section retrieved successfully",
                    Data = sectionDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving section {SectionId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the section",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new section (Admin/HR Manager/IT only)
        /// </summary>
        /// <param name="createSectionDto">Section creation data</param>
        /// <returns>Created section details</returns>
        /// <response code="201">Section created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/HR Manager/IT role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<SectionDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateSection([FromBody] CreateSectionDto createSectionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid model state",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Check if department exists
                var department = await _context.Departments.FindAsync(createSectionDto.DepartmentId);
                if (department == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found",
                        Errors = new List<string> { $"Department with ID {createSectionDto.DepartmentId} does not exist" }
                    });
                }

                // Check if section with same name already exists in the department
                var existingSection = await _context.Sections
                    .FirstOrDefaultAsync(s => s.DepartmentId == createSectionDto.DepartmentId && 
                                            s.Name.ToLower() == createSectionDto.Name.ToLower());

                if (existingSection != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section with this name already exists in the department",
                        Errors = new List<string> { "A section with the same name already exists in this department" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var section = new Section
                {
                    DepartmentId = createSectionDto.DepartmentId,
                    Name = createSectionDto.Name,
                    NameBangla = createSectionDto.NameBangla,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = userId
                };

                _context.Sections.Add(section);
                await _context.SaveChangesAsync();

                // Load department for response
                await _context.Entry(section)
                    .Reference(s => s.Department)
                    .LoadAsync();

                var sectionDto = new SectionDto
                {
                    Id = section.Id,
                    DepartmentId = section.DepartmentId,
                    DepartmentName = section.Department.Name,
                    Name = section.Name,
                    NameBangla = section.NameBangla,
                    CreatedAt = section.CreatedAt,
                    UpdatedAt = section.UpdatedAt,
                    IsActive = section.IsActive,
                    CreatedBy = section.CreatedBy,
                    UpdatedBy = section.UpdatedBy
                };

                _logger.LogInformation("Section {SectionName} created successfully in department {DepartmentName} by user {UserId}", 
                    section.Name, section.Department.Name, userId);

                return CreatedAtAction(nameof(GetSectionById), new { id = section.Id }, new ApiResponse<SectionDto>
                {
                    Success = true,
                    Message = "Section created successfully",
                    Data = sectionDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating section");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the section",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing section (Admin/HR Manager/IT only)
        /// </summary>
        /// <param name="id">Section ID</param>
        /// <param name="updateSectionDto">Section update data</param>
        /// <returns>Updated section details</returns>
        /// <response code="200">Section updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/HR Manager/IT role required</response>
        /// <response code="404">Section not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<SectionDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateSection(int id, [FromBody] UpdateSectionDto updateSectionDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid model state",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var section = await _context.Sections
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (section == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section not found",
                        Errors = new List<string> { $"Section with ID {id} does not exist" }
                    });
                }

                // Check if department exists
                var department = await _context.Departments.FindAsync(updateSectionDto.DepartmentId);
                if (department == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found",
                        Errors = new List<string> { $"Department with ID {updateSectionDto.DepartmentId} does not exist" }
                    });
                }

                // Check if another section with same name already exists in the department (excluding current section)
                var existingSection = await _context.Sections
                    .FirstOrDefaultAsync(s => s.DepartmentId == updateSectionDto.DepartmentId && 
                                            s.Name.ToLower() == updateSectionDto.Name.ToLower() && 
                                            s.Id != id);

                if (existingSection != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section with this name already exists in the department",
                        Errors = new List<string> { "Another section with the same name already exists in this department" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Update section properties
                section.DepartmentId = updateSectionDto.DepartmentId;
                section.Name = updateSectionDto.Name;
                section.NameBangla = updateSectionDto.NameBangla;
                section.IsActive = updateSectionDto.IsActive;
                section.UpdatedAt = DateTime.UtcNow;
                section.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                // Reload department if changed
                if (section.DepartmentId != section.Department.Id)
                {
                    await _context.Entry(section)
                        .Reference(s => s.Department)
                        .LoadAsync();
                }

                var sectionDto = new SectionDto
                {
                    Id = section.Id,
                    DepartmentId = section.DepartmentId,
                    DepartmentName = section.Department.Name,
                    Name = section.Name,
                    NameBangla = section.NameBangla,
                    CreatedAt = section.CreatedAt,
                    UpdatedAt = section.UpdatedAt,
                    IsActive = section.IsActive,
                    CreatedBy = section.CreatedBy,
                    UpdatedBy = section.UpdatedBy
                };

                _logger.LogInformation("Section {SectionId} updated successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<SectionDto>
                {
                    Success = true,
                    Message = "Section updated successfully",
                    Data = sectionDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating section {SectionId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the section",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a section (Admin/IT only)
        /// </summary>
        /// <param name="id">Section ID</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">Section deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/IT role required</response>
        /// <response code="404">Section not found</response>
        /// <response code="409">Conflict - Section is in use</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,IT")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteSection(int id)
        {
            try
            {
                var section = await _context.Sections
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (section == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section not found",
                        Errors = new List<string> { $"Section with ID {id} does not exist" }
                    });
                }

                // Note: Add validation here if sections are referenced by other entities (like employees)
                // For now, we'll allow deletion

                _context.Sections.Remove(section);
                await _context.SaveChangesAsync();

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Section {SectionId} ({SectionName}) deleted successfully by user {UserId}", 
                    id, section.Name, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Section deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting section {SectionId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the section",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get sections by department ID (Admin/Manager/HR/IT only)
        /// </summary>
        /// <param name="departmentId">Department ID</param>
        /// <param name="includeInactive">Include inactive sections</param>
        /// <returns>List of sections in the department</returns>
        /// <response code="200">Sections retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/Manager/HR/IT role required</response>
        /// <response code="404">Department not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("department/{departmentId}")]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<SectionListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSectionsByDepartment(int departmentId, [FromQuery] bool includeInactive = false)
        {
            try
            {
                // Check if department exists
                var department = await _context.Departments.FindAsync(departmentId);
                if (department == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found",
                        Errors = new List<string> { $"Department with ID {departmentId} does not exist" }
                    });
                }

                var query = _context.Sections
                    .Include(s => s.Department)
                    .Where(s => s.DepartmentId == departmentId);

                if (!includeInactive)
                {
                    query = query.Where(s => s.IsActive);
                }

                var sections = await query
                    .OrderBy(s => s.Name)
                    .Select(s => new SectionListDto
                    {
                        Id = s.Id,
                        DepartmentId = s.DepartmentId,
                        DepartmentName = s.Department.Name,
                        Name = s.Name,
                        NameBangla = s.NameBangla,
                        IsActive = s.IsActive,
                        CreatedAt = s.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<SectionListDto>>
                {
                    Success = true,
                    Message = $"Sections for department '{department.Name}' retrieved successfully",
                    Data = sections
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving sections for department {DepartmentId}", departmentId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving sections",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get section statistics (Admin/HR Manager/IT only)
        /// </summary>
        /// <returns>Section statistics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/HR Manager/IT role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetSectionStatistics()
        {
            try
            {
                var totalSections = await _context.Sections.CountAsync();
                var activeSections = await _context.Sections.CountAsync(s => s.IsActive);
                var inactiveSections = totalSections - activeSections;

                // Get sections per department
                var sectionsPerDepartment = await _context.Sections
                    .Include(s => s.Department)
                    .Where(s => s.IsActive)
                    .GroupBy(s => new { s.DepartmentId, s.Department.Name })
                    .Select(g => new
                    {
                        DepartmentId = g.Key.DepartmentId,
                        DepartmentName = g.Key.Name,
                        SectionCount = g.Count()
                    })
                    .OrderByDescending(x => x.SectionCount)
                    .Take(10)
                    .ToListAsync();

                var recentSections = await _context.Sections
                    .Include(s => s.Department)
                    .Where(s => s.IsActive)
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(5)
                    .Select(s => new
                    {
                        s.Id,
                        s.Name,
                        s.NameBangla,
                        DepartmentName = s.Department.Name,
                        s.CreatedAt
                    })
                    .ToListAsync();

                var statistics = new
                {
                    Overview = new
                    {
                        TotalSections = totalSections,
                        ActiveSections = activeSections,
                        InactiveSections = inactiveSections
                    },
                    DepartmentBreakdown = sectionsPerDepartment,
                    RecentSections = recentSections
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Section statistics retrieved successfully",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving section statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving section statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
