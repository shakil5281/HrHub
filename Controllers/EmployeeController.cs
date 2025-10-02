using HrHubAPI.DTOs;
using HrHubAPI.Models;
using HrHubAPI.Data;
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
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<EmployeeController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all employees by company (Admin and Manager only)
        /// </summary>
        /// <param name="companyId">Company ID to filter employees</param>
        /// <returns>List of employees in the specified company</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllEmployees([FromQuery] int? companyId = null)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "Invalid user" }
                    });
                }
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                
                if (currentUser == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "Invalid user" }
                    });
                }

                // If user is Manager, they can only see employees from their company
                var isManager = await _userManager.IsInRoleAsync(currentUser, "Manager");
                if (isManager && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                {
                    companyId = currentUser.CompanyId;
                }

                var query = _userManager.Users
                    .Include(u => u.Company)
                    .Where(u => u.IsActive);

                if (companyId.HasValue)
                {
                    query = query.Where(u => u.CompanyId == companyId.Value);
                }

                var users = await query.ToListAsync();
                var employees = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    employees.Add(new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Department = user.Department,
                        Position = user.Position,
                        CompanyId = user.CompanyId,
                        CompanyName = user.Company?.Name,
                        Roles = roles.ToList()
                    });
                }

                return Ok(new ApiResponse<List<UserDto>>
                {
                    Success = true,
                    Message = companyId.HasValue ? $"Employees for company {companyId} retrieved successfully" : "All employees retrieved successfully",
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employees");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving employees",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get employee by ID (Admin and Manager only)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEmployeeById(string id)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "Invalid user" }
                    });
                }
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                
                if (currentUser == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "Invalid user" }
                    });
                }

                var user = await _userManager.Users
                    .Include(u => u.Company)
                    .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);

                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee not found",
                        Errors = new List<string> { "Employee does not exist or is inactive" }
                    });
                }

                // If user is Manager, they can only see employees from their company
                var isManager = await _userManager.IsInRoleAsync(currentUser, "Manager");
                if (isManager && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                {
                    if (user.CompanyId != currentUser.CompanyId)
                    {
                        return Forbid("You can only view employees from your company");
                    }
                }

                var roles = await _userManager.GetRolesAsync(user);
                var employee = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Position = user.Position,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company?.Name,
                    Roles = roles.ToList()
                };

                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Message = "Employee retrieved successfully",
                    Data = employee
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employee with ID {EmployeeId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving employee",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get employees by department (Manager only)
        /// </summary>
        /// <param name="department">Department name</param>
        /// <param name="companyId">Company ID to filter employees</param>
        /// <returns>List of employees in the department</returns>
        [HttpGet("department/{department}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetEmployeesByDepartment(string department, [FromQuery] int? companyId = null)
        {
            try
            {
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "Invalid user" }
                    });
                }
                var currentUser = await _userManager.FindByIdAsync(currentUserId);
                
                if (currentUser == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "Invalid user" }
                    });
                }

                // If user is Manager, they can only see employees from their company
                var isManager = await _userManager.IsInRoleAsync(currentUser, "Manager");
                if (isManager && !await _userManager.IsInRoleAsync(currentUser, "Admin"))
                {
                    companyId = currentUser.CompanyId;
                }

                var query = _userManager.Users
                    .Include(u => u.Company)
                    .Where(u => u.IsActive && u.Department != null && u.Department.ToLower() == department.ToLower());

                if (companyId.HasValue)
                {
                    query = query.Where(u => u.CompanyId == companyId.Value);
                }

                var users = await query.ToListAsync();
                var employees = new List<UserDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    employees.Add(new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email!,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Department = user.Department,
                        Position = user.Position,
                        CompanyId = user.CompanyId,
                        CompanyName = user.Company?.Name,
                        Roles = roles.ToList()
                    });
                }

                return Ok(new ApiResponse<List<UserDto>>
                {
                    Success = true,
                    Message = $"Employees in {department} department retrieved successfully",
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employees for department {Department}", department);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving employees",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update employee information (Admin only)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="updateDto">Updated employee information</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateEmployee(string id, [FromBody] UpdateEmployeeDto updateDto)
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

                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee not found",
                        Errors = new List<string> { "Employee does not exist" }
                    });
                }

                user.FirstName = updateDto.FirstName;
                user.LastName = updateDto.LastName;
                user.Department = updateDto.Department;
                user.Position = updateDto.Position;
                if (updateDto.CompanyId.HasValue)
                {
                    user.CompanyId = updateDto.CompanyId.Value;
                }
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to update employee",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                _logger.LogInformation("Employee {EmployeeId} updated successfully", id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Employee updated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating employee with ID {EmployeeId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating employee",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Deactivate employee (Admin only)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateEmployee(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee not found",
                        Errors = new List<string> { "Employee does not exist" }
                    });
                }

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to deactivate employee",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                _logger.LogInformation("Employee {EmployeeId} deactivated successfully", id);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Employee deactivated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deactivating employee with ID {EmployeeId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deactivating employee",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }

    public class UpdateEmployeeDto
    {
        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.Required]
        [System.ComponentModel.DataAnnotations.MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string? Department { get; set; }

        [System.ComponentModel.DataAnnotations.MaxLength(200)]
        public string? Position { get; set; }

        public int? CompanyId { get; set; }
    }
}
