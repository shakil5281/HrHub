using HrHubAPI.Data;
using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DesignationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DesignationController> _logger;

        public DesignationController(ApplicationDbContext context, ILogger<DesignationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all designations (Admin/Manager/HR/IT only)
        /// </summary>
        /// <param name="sectionId">Optional section ID to filter designations</param>
        /// <param name="grade">Optional grade to filter designations</param>
        /// <param name="includeInactive">Include inactive designations</param>
        /// <returns>List of designations</returns>
        /// <response code="200">Designations retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/Manager/HR/IT role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DesignationListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllDesignations([FromQuery] int? sectionId = null, [FromQuery] string? grade = null, [FromQuery] bool includeInactive = false)
        {
            try
            {
                var query = _context.Designations
                    .Include(d => d.Section)
                    .ThenInclude(s => s.Department)
                    .AsQueryable();

                if (sectionId.HasValue)
                {
                    query = query.Where(d => d.SectionId == sectionId.Value);
                }

                if (!string.IsNullOrEmpty(grade))
                {
                    query = query.Where(d => d.Grade.ToLower() == grade.ToLower());
                }

                if (!includeInactive)
                {
                    query = query.Where(d => d.IsActive);
                }

                var designations = await query
                    .OrderBy(d => d.Section.Department.Name)
                    .ThenBy(d => d.Section.Name)
                    .ThenBy(d => d.Grade)
                    .ThenBy(d => d.Name)
                    .Select(d => new DesignationListDto
                    {
                        Id = d.Id,
                        SectionId = d.SectionId,
                        SectionName = d.Section.Name,
                        DepartmentName = d.Section.Department.Name,
                        Name = d.Name,
                        NameBangla = d.NameBangla,
                        Grade = d.Grade,
                        AttendanceBonus = d.AttendanceBonus,
                        IsActive = d.IsActive,
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<DesignationListDto>>
                {
                    Success = true,
                    Message = "Designations retrieved successfully",
                    Data = designations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving designations");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving designations",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get designation by ID (Admin/Manager/HR/IT only)
        /// </summary>
        /// <param name="id">Designation ID</param>
        /// <returns>Designation details</returns>
        /// <response code="200">Designation retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/Manager/HR/IT role required</response>
        /// <response code="404">Designation not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<DesignationDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDesignationById(int id)
        {
            try
            {
                var designation = await _context.Designations
                    .Include(d => d.Section)
                    .ThenInclude(s => s.Department)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (designation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Designation not found",
                        Errors = new List<string> { $"Designation with ID {id} does not exist" }
                    });
                }

                var designationDto = new DesignationDto
                {
                    Id = designation.Id,
                    SectionId = designation.SectionId,
                    SectionName = designation.Section.Name,
                    DepartmentId = designation.Section.DepartmentId,
                    DepartmentName = designation.Section.Department.Name,
                    Name = designation.Name,
                    NameBangla = designation.NameBangla,
                    Grade = designation.Grade,
                    AttendanceBonus = designation.AttendanceBonus,
                    CreatedAt = designation.CreatedAt,
                    UpdatedAt = designation.UpdatedAt,
                    IsActive = designation.IsActive,
                    CreatedBy = designation.CreatedBy,
                    UpdatedBy = designation.UpdatedBy
                };

                return Ok(new ApiResponse<DesignationDto>
                {
                    Success = true,
                    Message = "Designation retrieved successfully",
                    Data = designationDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving designation {DesignationId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the designation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new designation (Admin/HR Manager/IT only)
        /// </summary>
        /// <param name="createDesignationDto">Designation creation data</param>
        /// <returns>Created designation details</returns>
        /// <response code="201">Designation created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/HR Manager/IT role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<DesignationDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateDesignation([FromBody] CreateDesignationDto createDesignationDto)
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

                // Check if section exists
                var section = await _context.Sections
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == createDesignationDto.SectionId);

                if (section == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section not found",
                        Errors = new List<string> { $"Section with ID {createDesignationDto.SectionId} does not exist" }
                    });
                }

                // Check if designation with same name already exists in the section
                var existingDesignation = await _context.Designations
                    .FirstOrDefaultAsync(d => d.SectionId == createDesignationDto.SectionId && 
                                            d.Name.ToLower() == createDesignationDto.Name.ToLower());

                if (existingDesignation != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Designation with this name already exists in the section",
                        Errors = new List<string> { "A designation with the same name already exists in this section" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var designation = new Designation
                {
                    SectionId = createDesignationDto.SectionId,
                    Name = createDesignationDto.Name,
                    NameBangla = createDesignationDto.NameBangla,
                    Grade = createDesignationDto.Grade,
                    AttendanceBonus = createDesignationDto.AttendanceBonus,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = userId
                };

                _context.Designations.Add(designation);
                await _context.SaveChangesAsync();

                // Load section and department for response
                await _context.Entry(designation)
                    .Reference(d => d.Section)
                    .LoadAsync();

                await _context.Entry(designation.Section)
                    .Reference(s => s.Department)
                    .LoadAsync();

                var designationDto = new DesignationDto
                {
                    Id = designation.Id,
                    SectionId = designation.SectionId,
                    SectionName = designation.Section.Name,
                    DepartmentId = designation.Section.DepartmentId,
                    DepartmentName = designation.Section.Department.Name,
                    Name = designation.Name,
                    NameBangla = designation.NameBangla,
                    Grade = designation.Grade,
                    AttendanceBonus = designation.AttendanceBonus,
                    CreatedAt = designation.CreatedAt,
                    UpdatedAt = designation.UpdatedAt,
                    IsActive = designation.IsActive,
                    CreatedBy = designation.CreatedBy,
                    UpdatedBy = designation.UpdatedBy
                };

                _logger.LogInformation("Designation {DesignationName} created successfully in section {SectionName} by user {UserId}", 
                    designation.Name, designation.Section.Name, userId);

                return CreatedAtAction(nameof(GetDesignationById), new { id = designation.Id }, new ApiResponse<DesignationDto>
                {
                    Success = true,
                    Message = "Designation created successfully",
                    Data = designationDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating designation");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the designation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing designation (Admin/HR Manager/IT only)
        /// </summary>
        /// <param name="id">Designation ID</param>
        /// <param name="updateDesignationDto">Designation update data</param>
        /// <returns>Updated designation details</returns>
        /// <response code="200">Designation updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/HR Manager/IT role required</response>
        /// <response code="404">Designation not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<DesignationDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateDesignation(int id, [FromBody] UpdateDesignationDto updateDesignationDto)
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

                var designation = await _context.Designations
                    .Include(d => d.Section)
                    .ThenInclude(s => s.Department)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (designation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Designation not found",
                        Errors = new List<string> { $"Designation with ID {id} does not exist" }
                    });
                }

                // Check if section exists
                var section = await _context.Sections
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == updateDesignationDto.SectionId);

                if (section == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section not found",
                        Errors = new List<string> { $"Section with ID {updateDesignationDto.SectionId} does not exist" }
                    });
                }

                // Check if another designation with same name already exists in the section (excluding current designation)
                var existingDesignation = await _context.Designations
                    .FirstOrDefaultAsync(d => d.SectionId == updateDesignationDto.SectionId && 
                                            d.Name.ToLower() == updateDesignationDto.Name.ToLower() && 
                                            d.Id != id);

                if (existingDesignation != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Designation with this name already exists in the section",
                        Errors = new List<string> { "Another designation with the same name already exists in this section" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Update designation properties
                designation.SectionId = updateDesignationDto.SectionId;
                designation.Name = updateDesignationDto.Name;
                designation.NameBangla = updateDesignationDto.NameBangla;
                designation.Grade = updateDesignationDto.Grade;
                designation.AttendanceBonus = updateDesignationDto.AttendanceBonus;
                designation.IsActive = updateDesignationDto.IsActive;
                designation.UpdatedAt = DateTime.UtcNow;
                designation.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                // Reload section and department if changed
                if (designation.SectionId != designation.Section.Id)
                {
                    await _context.Entry(designation)
                        .Reference(d => d.Section)
                        .LoadAsync();

                    await _context.Entry(designation.Section)
                        .Reference(s => s.Department)
                        .LoadAsync();
                }

                var designationDto = new DesignationDto
                {
                    Id = designation.Id,
                    SectionId = designation.SectionId,
                    SectionName = designation.Section.Name,
                    DepartmentId = designation.Section.DepartmentId,
                    DepartmentName = designation.Section.Department.Name,
                    Name = designation.Name,
                    NameBangla = designation.NameBangla,
                    Grade = designation.Grade,
                    AttendanceBonus = designation.AttendanceBonus,
                    CreatedAt = designation.CreatedAt,
                    UpdatedAt = designation.UpdatedAt,
                    IsActive = designation.IsActive,
                    CreatedBy = designation.CreatedBy,
                    UpdatedBy = designation.UpdatedBy
                };

                _logger.LogInformation("Designation {DesignationId} updated successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<DesignationDto>
                {
                    Success = true,
                    Message = "Designation updated successfully",
                    Data = designationDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating designation {DesignationId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the designation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a designation (Admin/IT only)
        /// </summary>
        /// <param name="id">Designation ID</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">Designation deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/IT role required</response>
        /// <response code="404">Designation not found</response>
        /// <response code="409">Conflict - Designation is in use</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,IT")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteDesignation(int id)
        {
            try
            {
                var designation = await _context.Designations
                    .Include(d => d.Section)
                    .ThenInclude(s => s.Department)
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (designation == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Designation not found",
                        Errors = new List<string> { $"Designation with ID {id} does not exist" }
                    });
                }

                // Note: Add validation here if designations are referenced by other entities (like employees)
                // For now, we'll allow deletion

                _context.Designations.Remove(designation);
                await _context.SaveChangesAsync();

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Designation {DesignationId} ({DesignationName}) deleted successfully by user {UserId}", 
                    id, designation.Name, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Designation deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting designation {DesignationId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the designation",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get designations by section ID (Admin/Manager/HR/IT only)
        /// </summary>
        /// <param name="sectionId">Section ID</param>
        /// <param name="includeInactive">Include inactive designations</param>
        /// <returns>List of designations in the section</returns>
        /// <response code="200">Designations retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/Manager/HR/IT role required</response>
        /// <response code="404">Section not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("section/{sectionId}")]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DesignationListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDesignationsBySection(int sectionId, [FromQuery] bool includeInactive = false)
        {
            try
            {
                // Check if section exists
                var section = await _context.Sections
                    .Include(s => s.Department)
                    .FirstOrDefaultAsync(s => s.Id == sectionId);

                if (section == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Section not found",
                        Errors = new List<string> { $"Section with ID {sectionId} does not exist" }
                    });
                }

                var query = _context.Designations
                    .Include(d => d.Section)
                    .ThenInclude(s => s.Department)
                    .Where(d => d.SectionId == sectionId);

                if (!includeInactive)
                {
                    query = query.Where(d => d.IsActive);
                }

                var designations = await query
                    .OrderBy(d => d.Grade)
                    .ThenBy(d => d.Name)
                    .Select(d => new DesignationListDto
                    {
                        Id = d.Id,
                        SectionId = d.SectionId,
                        SectionName = d.Section.Name,
                        DepartmentName = d.Section.Department.Name,
                        Name = d.Name,
                        NameBangla = d.NameBangla,
                        Grade = d.Grade,
                        AttendanceBonus = d.AttendanceBonus,
                        IsActive = d.IsActive,
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<DesignationListDto>>
                {
                    Success = true,
                    Message = $"Designations for section '{section.Name}' retrieved successfully",
                    Data = designations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving designations for section {SectionId}", sectionId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving designations",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get designation statistics (Admin/HR Manager/IT only)
        /// </summary>
        /// <returns>Designation statistics</returns>
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
        public async Task<IActionResult> GetDesignationStatistics()
        {
            try
            {
                var totalDesignations = await _context.Designations.CountAsync();
                var activeDesignations = await _context.Designations.CountAsync(d => d.IsActive);
                var inactiveDesignations = totalDesignations - activeDesignations;

                // Get designations per grade
                var designationsPerGrade = await _context.Designations
                    .Where(d => d.IsActive)
                    .GroupBy(d => d.Grade)
                    .Select(g => new
                    {
                        Grade = g.Key,
                        Count = g.Count(),
                        AverageAttendanceBonus = g.Average(d => d.AttendanceBonus)
                    })
                    .OrderBy(x => x.Grade)
                    .ToListAsync();

                // Get designations per section
                var designationsPerSection = await _context.Designations
                    .Include(d => d.Section)
                    .ThenInclude(s => s.Department)
                    .Where(d => d.IsActive)
                    .GroupBy(d => new { d.SectionId, d.Section.Name, DepartmentName = d.Section.Department.Name })
                    .Select(g => new
                    {
                        SectionId = g.Key.SectionId,
                        SectionName = g.Key.Name,
                        DepartmentName = g.Key.DepartmentName,
                        DesignationCount = g.Count()
                    })
                    .OrderByDescending(x => x.DesignationCount)
                    .Take(10)
                    .ToListAsync();

                var recentDesignations = await _context.Designations
                    .Include(d => d.Section)
                    .ThenInclude(s => s.Department)
                    .Where(d => d.IsActive)
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(5)
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.NameBangla,
                        d.Grade,
                        SectionName = d.Section.Name,
                        DepartmentName = d.Section.Department.Name,
                        d.AttendanceBonus,
                        d.CreatedAt
                    })
                    .ToListAsync();

                var statistics = new
                {
                    Overview = new
                    {
                        TotalDesignations = totalDesignations,
                        ActiveDesignations = activeDesignations,
                        InactiveDesignations = inactiveDesignations
                    },
                    GradeBreakdown = designationsPerGrade,
                    SectionBreakdown = designationsPerSection,
                    RecentDesignations = recentDesignations
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Designation statistics retrieved successfully",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving designation statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving designation statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
