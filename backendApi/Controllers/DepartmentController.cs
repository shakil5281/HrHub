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
    public class DepartmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<DepartmentController> _logger;

        public DepartmentController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<DepartmentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get departments based on user role (Admin gets all departments, others get their assigned company departments)
        /// </summary>
        /// <param name="companyId">Optional company ID to filter departments (Admin only)</param>
        /// <param name="includeInactive">Include inactive departments (Admin only)</param>
        /// <returns>List of departments based on user role</returns>
        /// <response code="200">Departments retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartmentListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllDepartments([FromQuery] int? companyId = null, [FromQuery] bool includeInactive = false)
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

                List<DepartmentListDto> departments;

                if (isAdmin)
                {
                    // Admin users can see all departments
                    var query = _context.Departments
                        .Include(d => d.Company)
                        .AsQueryable();

                    if (companyId.HasValue)
                    {
                        query = query.Where(d => d.CompanyId == companyId.Value);
                    }

                    if (!includeInactive)
                    {
                        query = query.Where(d => d.IsActive);
                    }

                    departments = await query
                        .OrderBy(d => d.Company.Name)
                        .ThenBy(d => d.Name)
                        .Select(d => new DepartmentListDto
                        {
                            Id = d.Id,
                            Name = d.Name,
                            NameBangla = d.NameBangla,
                            CompanyId = d.CompanyId,
                            CompanyName = d.Company.Name,
                            IsActive = d.IsActive,
                            CreatedAt = d.CreatedAt
                        })
                        .ToListAsync();

                    _logger.LogInformation("Admin user {UserId} retrieved all departments", userId);
                }
                else
                {
                    // Non-admin users can only see departments from their assigned companies
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
                        departments = new List<DepartmentListDto>();
                    }
                    else
                    {
                        var query = _context.Departments
                            .Include(d => d.Company)
                            .Where(d => userCompanyIds.Contains(d.CompanyId) && d.IsActive);

                        departments = await query
                            .OrderBy(d => d.Company.Name)
                            .ThenBy(d => d.Name)
                            .Select(d => new DepartmentListDto
                            {
                                Id = d.Id,
                                Name = d.Name,
                                NameBangla = d.NameBangla,
                                CompanyId = d.CompanyId,
                                CompanyName = d.Company.Name,
                                IsActive = d.IsActive,
                                CreatedAt = d.CreatedAt
                            })
                            .ToListAsync();
                    }

                    _logger.LogInformation("User {UserId} retrieved {Count} departments from assigned companies", userId, departments.Count);
                }

                return Ok(new ApiResponse<IEnumerable<DepartmentListDto>>
                {
                    Success = true,
                    Message = isAdmin ? "All departments retrieved successfully" : "Assigned company departments retrieved successfully",
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
        /// Get department by ID (Admin gets any department, others only from their assigned companies)
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Department details</returns>
        /// <response code="200">Department retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Department not accessible to user</response>
        /// <response code="404">Department not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDepartmentById(int id)
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

                var department = await _context.Departments
                    .Include(d => d.Company)
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

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                if (!isAdmin)
                {
                    // Non-admin users can only access departments from their assigned companies
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

                    if (!userCompanyIds.Contains(department.CompanyId))
                    {
                        return Forbid("Access denied. You can only access departments from your assigned companies.");
                    }
                }

                var departmentDto = new DepartmentDto
                {
                    Id = department.Id,
                    Name = department.Name,
                    NameBangla = department.NameBangla,
                    CompanyId = department.CompanyId,
                    CompanyName = department.Company.Name,
                    CreatedAt = department.CreatedAt,
                    UpdatedAt = department.UpdatedAt,
                    IsActive = department.IsActive,
                    CreatedBy = department.CreatedBy,
                    UpdatedBy = department.UpdatedBy
                };

                _logger.LogInformation("User {UserId} retrieved department {DepartmentId}", userId, id);

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
        /// Create a new department (Admin can create in any company, others only in their assigned companies)
        /// </summary>
        /// <param name="createDepartmentDto">Department creation data</param>
        /// <returns>Created department details</returns>
        /// <response code="201">Department created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Cannot create department in this company</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<DepartmentDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateDepartment([FromBody] CreateDepartmentDto createDepartmentDto)
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

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid model state",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Validate that the company exists
                var company = await _context.Companies.FindAsync(createDepartmentDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {createDepartmentDto.CompanyId} does not exist" }
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                if (!isAdmin)
                {
                    // Non-admin users can only create departments in their assigned companies
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

                    if (!userCompanyIds.Contains(createDepartmentDto.CompanyId))
                    {
                        return Forbid("Access denied. You can only create departments in your assigned companies.");
                    }
                }

                // Check if department with same name already exists in the same company
                var existingDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Name.ToLower() == createDepartmentDto.Name.ToLower() && d.CompanyId == createDepartmentDto.CompanyId);

                if (existingDepartment != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department with this name already exists in this company",
                        Errors = new List<string> { "A department with the same name is already registered in this company" }
                    });
                }

                var department = new Department
                {
                    Name = createDepartmentDto.Name,
                    NameBangla = createDepartmentDto.NameBangla,
                    CompanyId = createDepartmentDto.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = userId
                };

                _context.Departments.Add(department);
                await _context.SaveChangesAsync();

                // Reload the department with company information
                var createdDepartment = await _context.Departments
                    .Include(d => d.Company)
                    .FirstAsync(d => d.Id == department.Id);

                var departmentDto = new DepartmentDto
                {
                    Id = createdDepartment.Id,
                    Name = createdDepartment.Name,
                    NameBangla = createdDepartment.NameBangla,
                    CompanyId = createdDepartment.CompanyId,
                    CompanyName = createdDepartment.Company.Name,
                    CreatedAt = createdDepartment.CreatedAt,
                    UpdatedAt = createdDepartment.UpdatedAt,
                    IsActive = createdDepartment.IsActive,
                    CreatedBy = createdDepartment.CreatedBy,
                    UpdatedBy = createdDepartment.UpdatedBy
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
        /// Update an existing department (Admin can update any department, others only from their assigned companies)
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <param name="updateDepartmentDto">Department update data</param>
        /// <returns>Updated department details</returns>
        /// <response code="200">Department updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Cannot update this department</response>
        /// <response code="404">Department not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize] // Allow all authenticated users
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

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                if (!isAdmin)
                {
                    // Non-admin users can only update departments from their assigned companies
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

                    if (!userCompanyIds.Contains(department.CompanyId))
                    {
                        return Forbid("Access denied. You can only update departments from your assigned companies.");
                    }

                    // Also check if the new company is accessible to the user
                    if (!userCompanyIds.Contains(updateDepartmentDto.CompanyId))
                    {
                        return Forbid("Access denied. You can only move departments to your assigned companies.");
                    }
                }

                // Validate that the company exists
                var company = await _context.Companies.FindAsync(updateDepartmentDto.CompanyId);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {updateDepartmentDto.CompanyId} does not exist" }
                    });
                }

                // Check if another department with same name already exists in the same company (excluding current department)
                var existingDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Name.ToLower() == updateDepartmentDto.Name.ToLower() && d.CompanyId == updateDepartmentDto.CompanyId && d.Id != id);

                if (existingDepartment != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Department with this name already exists in this company",
                        Errors = new List<string> { "Another department with the same name is already registered in this company" }
                    });
                }

                // Update department properties
                department.Name = updateDepartmentDto.Name;
                department.NameBangla = updateDepartmentDto.NameBangla;
                department.CompanyId = updateDepartmentDto.CompanyId;
                department.IsActive = updateDepartmentDto.IsActive;
                department.UpdatedAt = DateTime.UtcNow;
                department.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                // Reload the department with company information
                var updatedDepartment = await _context.Departments
                    .Include(d => d.Company)
                    .FirstAsync(d => d.Id == department.Id);

                var departmentDto = new DepartmentDto
                {
                    Id = updatedDepartment.Id,
                    Name = updatedDepartment.Name,
                    NameBangla = updatedDepartment.NameBangla,
                    CompanyId = updatedDepartment.CompanyId,
                    CompanyName = updatedDepartment.Company.Name,
                    CreatedAt = updatedDepartment.CreatedAt,
                    UpdatedAt = updatedDepartment.UpdatedAt,
                    IsActive = updatedDepartment.IsActive,
                    CreatedBy = updatedDepartment.CreatedBy,
                    UpdatedBy = updatedDepartment.UpdatedBy
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
        /// Delete a department (Admin can delete any department, others only from their assigned companies)
        /// </summary>
        /// <param name="id">Department ID</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">Department deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Cannot delete this department</response>
        /// <response code="404">Department not found</response>
        /// <response code="409">Conflict - Department is in use</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize] // Allow all authenticated users
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

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                if (!isAdmin)
                {
                    // Non-admin users can only delete departments from their assigned companies
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

                    if (!userCompanyIds.Contains(department.CompanyId))
                    {
                        return Forbid("Access denied. You can only delete departments from your assigned companies.");
                    }
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
        /// Get department statistics (Admin gets all statistics, others get statistics for their assigned companies)
        /// </summary>
        /// <returns>Department statistics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("statistics")]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDepartmentStatistics()
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

                object statistics;

                if (isAdmin)
                {
                    // Admin users can see all department statistics
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

                    statistics = new
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

                    _logger.LogInformation("Admin user {UserId} retrieved all department statistics", userId);
                }
                else
                {
                    // Non-admin users can only see statistics for their assigned companies
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
                        statistics = new
                        {
                            Overview = new
                            {
                                TotalDepartments = 0,
                                ActiveDepartments = 0,
                                InactiveDepartments = 0
                            },
                            UserDistribution = new List<object>(),
                            RecentDepartments = new List<object>()
                        };
                    }
                    else
                    {
                        var totalDepartments = await _context.Departments
                            .CountAsync(d => userCompanyIds.Contains(d.CompanyId));
                        var activeDepartments = await _context.Departments
                            .CountAsync(d => userCompanyIds.Contains(d.CompanyId) && d.IsActive);
                        var inactiveDepartments = totalDepartments - activeDepartments;

                        // Get user count per department (only for assigned companies)
                        var departmentUserCounts = await _context.Users
                            .Where(u => !string.IsNullOrEmpty(u.Department) && userCompanyIds.Contains(u.CompanyId ?? 0))
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
                            .Where(d => userCompanyIds.Contains(d.CompanyId) && d.IsActive)
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

                        statistics = new
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
                    }

                    _logger.LogInformation("User {UserId} retrieved department statistics for {Count} assigned companies", userId, userCompanyIds.Count);
                }

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = isAdmin ? "All department statistics retrieved successfully" : "Assigned company department statistics retrieved successfully",
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
