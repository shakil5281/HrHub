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
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(ApplicationDbContext context, ILogger<DepartmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all departments (Admin/Manager/HR/IT only)
        /// </summary>
        /// <returns>List of departments</returns>
        /// <response code="200">Departments retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/Manager/HR/IT role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllDepartments([FromQuery] bool includeInactive = false)
        {
            try
            {
                var query = _context.Departments.AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(d => d.IsActive);
                }

                var departments = await query
                    .OrderBy(d => d.Name)
                    .Select(d => new DepartmentListDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        NameBangla = d.NameBangla,
                        IsActive = d.IsActive,
                        CreatedAt = d.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<DepartmentListDto>>
                {
                    Success = true,
                    Message = "Departments retrieved successfully",
                    Data = departments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving departments");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving departments",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get department by ID (Admin/Manager/HR/IT only)
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department details</returns>
        /// <response code="200">Department retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/Manager/HR/IT role required</response>
        /// <response code="404">Department not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            try
            {
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (department == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found",
                        Errors = new List<string> { $"Department with ID {id} does not exist" }
                    });
                }

                var departmentDto = new DepartmentDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    NameBangla = department.NameBangla,
                    CreatedAt = department.CreatedAt,
                    UpdatedAt = department.UpdatedAt,
                    IsActive = department.IsActive,
                    CreatedBy = department.CreatedBy,
                    UpdatedBy = department.UpdatedBy
                };

                return Ok(new ApiResponse<DepartmentDto>
                {
                    Success = true,
                    Message = "Department retrieved successfully",
                    Data = departmentDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving department {DepartmentId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the department",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new department (Admin/HR Manager/IT only)
        /// </summary>
        /// <param name="createDepartmentDto">Department creation data</param>
        /// <returns>Created department details</returns>
        /// <response code="201">Department created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/HR Manager/IT role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto createDepartmentDto)
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

                // Check if department with same name already exists
                var existingDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Name.ToLower() == createDepartmentDto.Name.ToLower());

                if (existingDepartment != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department with this name already exists",
                        Errors = new List<string> { "A department with the same name is already registered" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var department = new Department
                {
                    Name = createDepartmentDto.Name,
                    NameBangla = createDepartmentDto.NameBangla,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = userId
                };

                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                var departmentDto = new DepartmentDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    NameBangla = department.NameBangla,
                    CreatedAt = department.CreatedAt,
                    UpdatedAt = department.UpdatedAt,
                    IsActive = department.IsActive,
                    CreatedBy = department.CreatedBy,
                    UpdatedBy = department.UpdatedBy
                };

                _logger.LogInformation("Department {DepartmentName} created successfully by user {UserId}", department.Name, userId);

                return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, new ApiResponse<DepartmentDto>
                {
                    Success = true,
                    Message = "Department created successfully",
                    Data = departmentDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating department");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the department",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing department (Admin/HR Manager/IT only)
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="updateDepartmentDto">Department update data</param>
        /// <returns>Updated department details</returns>
        /// <response code="200">Department updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/HR Manager/IT role required</response>
        /// <response code="404">Department not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] UpdateDepartmentDto updateDepartmentDto)
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

                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found",
                        Errors = new List<string> { $"Department with ID {id} does not exist" }
                    });
                }

                // Check if another department with same name already exists (excluding current department)
                var existingDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Name.ToLower() == updateDepartmentDto.Name.ToLower() && d.Id != id);

                if (existingDepartment != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department with this name already exists",
                        Errors = new List<string> { "Another department with the same name is already registered" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Update department properties
                department.Name = updateDepartmentDto.Name;
                department.NameBangla = updateDepartmentDto.NameBangla;
                department.IsActive = updateDepartmentDto.IsActive;
                department.UpdatedAt = DateTime.UtcNow;
                department.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                var departmentDto = new DepartmentDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    NameBangla = department.NameBangla,
                    CreatedAt = department.CreatedAt,
                    UpdatedAt = department.UpdatedAt,
                    IsActive = department.IsActive,
                    CreatedBy = department.CreatedBy,
                    UpdatedBy = department.UpdatedBy
                };

                _logger.LogInformation("Department {DepartmentId} updated successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<DepartmentDto>
                {
                    Success = true,
                    Message = "Department updated successfully",
                    Data = departmentDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating department {DepartmentId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the department",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a department (Admin/IT only)
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">Department deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin/IT role required</response>
        /// <response code="404">Department not found</response>
        /// <response code="409">Conflict - Department is in use</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,IT")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);

                if (department == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department not found",
                        Errors = new List<string> { $"Department with ID {id} does not exist" }
                    });
                }

                // Check if department is being used by any users
                var usersInDepartment = await _context.Users
                    .AnyAsync(u => u.Department == department.Name);

                if (usersInDepartment)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete department that is assigned to users",
                        Errors = new List<string> { "This department is currently assigned to one or more users. Please reassign users before deleting the department." }
                    });
                }

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Department {DepartmentId} deleted successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Department deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting department {DepartmentId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the department",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get department statistics (Admin/HR Manager/IT only)
        /// </summary>
        /// <returns>Department statistics</returns>
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
        public async Task<IActionResult> GetDepartmentStatistics()
        {
            try
            {
                var totalDepartments = await _context.Departments.CountAsync();
                var activeDepartments = await _context.Departments.CountAsync(d => d.IsActive);
                var inactiveDepartments = totalDepartments - activeDepartments;

                // Get user count per department
                var departmentUserCounts = await _context.Users
                    .Where(u => !string.IsNullOrEmpty(u.Department))
                    .GroupBy(u => u.Department)
                    .Select(g => new
                    {
                        DepartmentName = g.Key,
                        UserCount = g.Count()
                    })
                    .OrderByDescending(x => x.UserCount)
                    .Take(10)
                    .ToListAsync();

                var recentDepartments = await _context.Departments
                    .Where(d => d.IsActive)
                    .OrderByDescending(d => d.CreatedAt)
                    .Take(5)
                    .Select(d => new
                    {
                        d.Id,
                        d.Name,
                        d.NameBangla,
                        d.CreatedAt
                    })
                    .ToListAsync();

                var statistics = new
                {
                    Overview = new
                    {
                        TotalDepartments = totalDepartments,
                        ActiveDepartments = activeDepartments,
                        InactiveDepartments = inactiveDepartments
                    },
                    UserDistribution = departmentUserCounts,
                    RecentDepartments = recentDepartments
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Department statistics retrieved successfully",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving department statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving department statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
