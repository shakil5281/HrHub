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
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            ILogger<UserController> logger)
        {
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        /// <param name="includeInactive">Include inactive users in the results</param>
        /// <param name="page">Page number for pagination (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 50, max: 100)</param>
        /// <param name="search">Search term for filtering by name or email</param>
        /// <param name="companyId">Filter by company ID</param>
        /// <param name="role">Filter by role</param>
        /// <returns>List of users with pagination</returns>
        /// <response code="200">Users retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserListResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllUsers(
            [FromQuery] bool includeInactive = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null,
            [FromQuery] int? companyId = null,
            [FromQuery] string? role = null)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 50;
                if (pageSize > 100) pageSize = 100;

                var query = _userManager.Users.AsQueryable();

                // Filter by active status
                if (!includeInactive)
                {
                    query = query.Where(u => u.IsActive);
                }

                // Filter by company
                if (companyId.HasValue)
                {
                    query = query.Where(u => u.CompanyId == companyId.Value);
                }

                // Search filter
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var searchLower = search.ToLower();
                    query = query.Where(u => 
                        u.FirstName.ToLower().Contains(searchLower) ||
                        u.LastName.ToLower().Contains(searchLower) ||
                        u.Email!.ToLower().Contains(searchLower) ||
                        (u.Department != null && u.Department.ToLower().Contains(searchLower)) ||
                        (u.Position != null && u.Position.ToLower().Contains(searchLower)));
                }

                // Get total count for pagination
                var totalCount = await query.CountAsync();

                // Apply pagination
                var users = await query
                    .Include(u => u.Company)
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserListDto
                    {
                        Id = u.Id,
                        Email = u.Email!,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Department = u.Department,
                        Position = u.Position,
                        CompanyId = u.CompanyId,
                        CompanyName = u.Company!.Name,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt
                    })
                    .ToListAsync();

                // Get roles for each user
                var userListWithRoles = new List<UserListDto>();
                foreach (var user in users)
                {
                    var userEntity = await _userManager.FindByIdAsync(user.Id);
                    if (userEntity != null)
                    {
                        var userRoles = await _userManager.GetRolesAsync(userEntity);
                        
                        // Apply role filter if specified
                        if (!string.IsNullOrWhiteSpace(role))
                        {
                            if (!userRoles.Contains(role))
                                continue;
                        }

                        user.Roles = userRoles.ToList();
                        userListWithRoles.Add(user);
                    }
                }

                var response = new UserListResponseDto
                {
                    Users = userListWithRoles,
                    Pagination = new PaginationDto
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalCount,
                        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                        HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                        HasPreviousPage = page > 1
                    }
                };

                return Ok(new ApiResponse<UserListResponseDto>
                {
                    Success = true,
                    Message = "Users retrieved successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving users");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving users",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User details</returns>
        /// <response code="200">User retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var userDetail = new UserDetailDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Position = user.Position,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company?.Name,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = userRoles.ToList(),
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd
                };

                return Ok(new ApiResponse<UserDetailDto>
                {
                    Success = true,
                    Message = "User retrieved successfully",
                    Data = userDetail
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update user status (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserStatusDto">User status update data</param>
        /// <returns>Updated user details</returns>
        /// <response code="200">User status updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateUserStatus(string id, [FromBody] UpdateUserStatusDto updateUserStatusDto)
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
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                // Update user properties
                user.IsActive = updateUserStatusDto.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to update user status",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Position = user.Position,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company?.Name,
                    Roles = userRoles.ToList()
                };

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User {UserId} status updated to {Status} by admin {AdminId}", id, updateUserStatusDto.IsActive ? "Active" : "Inactive", userId);

                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Message = $"User status updated to {(updateUserStatusDto.IsActive ? "Active" : "Inactive")}",
                    Data = userDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user status for user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating user status",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update user information (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserDto">User update data</param>
        /// <returns>Updated user details</returns>
        /// <response code="200">User updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserDetailDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UpdateUserDto updateUserDto)
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
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                // Validate company exists if provided
                if (updateUserDto.CompanyId.HasValue)
                {
                    var companyExists = await _context.Companies
                        .AnyAsync(c => c.Id == updateUserDto.CompanyId.Value && c.IsActive);
                    
                    if (!companyExists)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Company not found or inactive",
                            Errors = new List<string> { $"Company with ID {updateUserDto.CompanyId} does not exist or is inactive" }
                        });
                    }
                }

                // Update user properties
                user.FirstName = updateUserDto.FirstName;
                user.LastName = updateUserDto.LastName;
                user.Department = updateUserDto.Department;
                user.Position = updateUserDto.Position;
                user.CompanyId = updateUserDto.CompanyId;
                user.PhoneNumber = updateUserDto.PhoneNumber;
                user.IsActive = updateUserDto.IsActive;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to update user",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var userDetail = new UserDetailDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Department = user.Department,
                    Position = user.Position,
                    CompanyId = user.CompanyId,
                    CompanyName = user.Company?.Name,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt,
                    Roles = userRoles.ToList(),
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    LockoutEnabled = user.LockoutEnabled,
                    LockoutEnd = user.LockoutEnd
                };

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User {UserId} updated successfully by admin {AdminId}", id, userId);

                return Ok(new ApiResponse<UserDetailDto>
                {
                    Success = true,
                    Message = "User updated successfully",
                    Data = userDetail
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">User deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="409">Conflict - Cannot delete user with active sessions</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                // Check if user is the current admin (prevent self-deletion)
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == id)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete your own account",
                        Errors = new List<string> { "You cannot delete your own user account" }
                    });
                }

                // Check if user has active refresh token (has active sessions)
                if (!string.IsNullOrEmpty(user.RefreshToken) && user.RefreshTokenExpiryTime > DateTime.UtcNow)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete user with active sessions",
                        Errors = new List<string> { "User has active sessions. Please ask the user to logout first or wait for sessions to expire." }
                    });
                }

                // Get user roles to check if they're the last admin
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Admin"))
                {
                    var adminCount = await _userManager.GetUsersInRoleAsync("Admin");
                    var activeAdminCount = adminCount.Count(u => u.IsActive && u.Id != id);
                    
                    if (activeAdminCount == 0)
                    {
                        return Conflict(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Cannot delete the last active admin",
                            Errors = new List<string> { "At least one active admin user must remain in the system" }
                        });
                    }
                }

                // Soft delete: Deactivate user instead of hard delete
                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                user.RefreshToken = null; // Clear any refresh tokens
                user.RefreshTokenExpiryTime = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to delete user",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User {UserId} (Email: {Email}) deactivated successfully by admin {AdminId}", 
                    id, user.Email, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"User '{user.Email}' has been deactivated successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Permanently delete a user (Admin only) - Use with caution
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">User permanently deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="409">Conflict - Cannot delete user with active sessions or last admin</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}/permanent")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> PermanentlyDeleteUser(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                // Check if user is the current admin (prevent self-deletion)
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentUserId == id)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete your own account",
                        Errors = new List<string> { "You cannot delete your own user account" }
                    });
                }

                // Check if user has active refresh token (has active sessions)
                if (!string.IsNullOrEmpty(user.RefreshToken) && user.RefreshTokenExpiryTime > DateTime.UtcNow)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete user with active sessions",
                        Errors = new List<string> { "User has active sessions. Please ask the user to logout first or wait for sessions to expire." }
                    });
                }

                // Get user roles to check if they're the last admin
                var userRoles = await _userManager.GetRolesAsync(user);
                if (userRoles.Contains("Admin"))
                {
                    var adminCount = await _userManager.GetUsersInRoleAsync("Admin");
                    var activeAdminCount = adminCount.Count(u => u.IsActive && u.Id != id);
                    
                    if (activeAdminCount == 0)
                    {
                        return Conflict(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Cannot delete the last active admin",
                            Errors = new List<string> { "At least one active admin user must remain in the system" }
                        });
                    }
                }

                // Permanently delete the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to permanently delete user",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogWarning("User {UserId} (Email: {Email}) permanently deleted by admin {AdminId}", 
                    id, user.Email, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"User '{user.Email}' has been permanently deleted from the system"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while permanently deleting user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while permanently deleting the user",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get all available roles (Admin only)
        /// </summary>
        /// <returns>List of all available roles with user counts</returns>
        /// <response code="200">Roles retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<RoleDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var roles = await _context.Roles.ToListAsync();
                var roleDtos = new List<RoleDto>();

                foreach (var role in roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                    var activeUsersInRole = usersInRole.Count(u => u.IsActive);

                    roleDtos.Add(new RoleDto
                    {
                        Id = role.Id,
                        Name = role.Name!,
                        Description = role.Description,
                        UserCount = usersInRole.Count,
                        ActiveUserCount = activeUsersInRole
                    });
                }

                return Ok(new ApiResponse<IEnumerable<RoleDto>>
                {
                    Success = true,
                    Message = "Roles retrieved successfully",
                    Data = roleDtos.OrderBy(r => r.Name)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving roles");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving roles",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get user roles (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User's current roles and available roles</returns>
        /// <response code="200">User roles retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserRolesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetUserRoles(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                var currentRoles = await _userManager.GetRolesAsync(user);
                var allRoles = await _context.Roles.Select(r => r.Name!).ToListAsync();

                var userRolesDto = new UserRolesDto
                {
                    UserId = user.Id,
                    UserEmail = user.Email!,
                    UserName = $"{user.FirstName} {user.LastName}",
                    CurrentRoles = currentRoles.ToList(),
                    AvailableRoles = allRoles.Except(currentRoles).OrderBy(r => r).ToList()
                };

                return Ok(new ApiResponse<UserRolesDto>
                {
                    Success = true,
                    Message = "User roles retrieved successfully",
                    Data = userRolesDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user roles for user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user roles",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update user roles (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <param name="updateUserRolesDto">New roles for the user</param>
        /// <returns>Updated user roles</returns>
        /// <response code="200">User roles updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserRolesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateUserRoles(string id, [FromBody] UpdateUserRolesDto updateUserRolesDto)
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
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                // Validate that all provided roles exist
                var existingRoles = await _context.Roles.Select(r => r.Name!).ToListAsync();
                var invalidRoles = updateUserRolesDto.Roles.Except(existingRoles).ToList();
                
                if (invalidRoles.Any())
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid roles provided",
                        Errors = new List<string> { $"Roles not found: {string.Join(", ", invalidRoles)}" }
                    });
                }

                // Get current roles
                var currentRoles = await _userManager.GetRolesAsync(user);
                
                // Check if trying to remove the last admin
                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (currentRoles.Contains("Admin") && !updateUserRolesDto.Roles.Contains("Admin"))
                {
                    var adminCount = await _userManager.GetUsersInRoleAsync("Admin");
                    var activeAdminCount = adminCount.Count(u => u.IsActive && u.Id != id);
                    
                    if (activeAdminCount == 0)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Cannot remove admin role from the last active admin",
                            Errors = new List<string> { "At least one active admin user must remain in the system" }
                        });
                    }
                }

                // Remove all current roles
                if (currentRoles.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
                    if (!removeResult.Succeeded)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Failed to remove existing roles",
                            Errors = removeResult.Errors.Select(e => e.Description).ToList()
                        });
                    }
                }

                // Add new roles
                if (updateUserRolesDto.Roles.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, updateUserRolesDto.Roles);
                    if (!addResult.Succeeded)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Failed to assign new roles",
                            Errors = addResult.Errors.Select(e => e.Description).ToList()
                        });
                    }
                }

                // Get updated roles
                var updatedRoles = await _userManager.GetRolesAsync(user);
                var allRoles = await _context.Roles.Select(r => r.Name!).ToListAsync();

                var userRolesDto = new UserRolesDto
                {
                    UserId = user.Id,
                    UserEmail = user.Email!,
                    UserName = $"{user.FirstName} {user.LastName}",
                    CurrentRoles = updatedRoles.ToList(),
                    AvailableRoles = allRoles.Except(updatedRoles).OrderBy(r => r).ToList()
                };

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("User {UserId} roles updated by admin {AdminId}. New roles: {Roles}", 
                    id, adminId, string.Join(", ", updatedRoles));

                return Ok(new ApiResponse<UserRolesDto>
                {
                    Success = true,
                    Message = "User roles updated successfully",
                    Data = userRolesDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating user roles for user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating user roles",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get all IT role users (Admin only)
        /// </summary>
        /// <param name="companyId">Filter by company ID (optional)</param>
        /// <param name="includeInactive">Include inactive users (optional)</param>
        /// <param name="page">Page number for pagination (default: 1)</param>
        /// <param name="pageSize">Number of items per page (default: 50, max: 100)</param>
        /// <returns>List of IT users with pagination</returns>
        /// <response code="200">IT users retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("it-users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserListResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetITUsers(
            [FromQuery] int? companyId = null,
            [FromQuery] bool includeInactive = false,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                // Validate pagination parameters
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 50;
                if (pageSize > 100) pageSize = 100;

                // Get all users with IT role
                var itUsers = await _userManager.GetUsersInRoleAsync("IT");
                
                // Filter by active status
                if (!includeInactive)
                {
                    itUsers = itUsers.Where(u => u.IsActive).ToList();
                }

                // Filter by company
                if (companyId.HasValue)
                {
                    itUsers = itUsers.Where(u => u.CompanyId == companyId.Value).ToList();
                }

                // Get total count for pagination
                var totalCount = itUsers.Count;

                // Apply pagination and ordering
                var users = itUsers
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(u => new UserListDto
                    {
                        Id = u.Id,
                        Email = u.Email!,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Department = u.Department,
                        Position = u.Position,
                        CompanyId = u.CompanyId,
                        CompanyName = u.Company?.Name,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        Roles = new List<string> { "IT" } // IT users will have IT role
                    })
                    .ToList();

                var response = new UserListResponseDto
                {
                    Users = users,
                    Pagination = new PaginationDto
                    {
                        CurrentPage = page,
                        PageSize = pageSize,
                        TotalItems = totalCount,
                        TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                        HasNextPage = page < (int)Math.Ceiling((double)totalCount / pageSize),
                        HasPreviousPage = page > 1
                    }
                };

                return Ok(new ApiResponse<UserListResponseDto>
                {
                    Success = true,
                    Message = "IT users retrieved successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving IT users");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving IT users",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get IT users by company (Admin only)
        /// </summary>
        /// <param name="companyId">Company ID</param>
        /// <param name="includeInactive">Include inactive users (optional)</param>
        /// <returns>List of IT users for the specified company</returns>
        /// <response code="200">Company IT users retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Company not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("companies/{companyId}/it-users")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetCompanyITUsers(int companyId, [FromQuery] bool includeInactive = false)
        {
            try
            {
                // Check if company exists
                var company = await _context.Companies.FindAsync(companyId);
                if (company == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {companyId} does not exist" }
                    });
                }

                // Get all users with IT role
                var itUsers = await _userManager.GetUsersInRoleAsync("IT");
                var companyItUsers = itUsers.Where(u => u.CompanyId == companyId).ToList();

                // Filter by active status
                if (!includeInactive)
                {
                    companyItUsers = companyItUsers.Where(u => u.IsActive).ToList();
                }

                var users = companyItUsers
                    .OrderBy(u => u.FirstName)
                    .ThenBy(u => u.LastName)
                    .Select(u => new UserListDto
                    {
                        Id = u.Id,
                        Email = u.Email!,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Department = u.Department,
                        Position = u.Position,
                        CompanyId = u.CompanyId,
                        CompanyName = company.Name,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        UpdatedAt = u.UpdatedAt,
                        Roles = new List<string> { "IT" }
                    })
                    .ToList();

                return Ok(new ApiResponse<IEnumerable<UserListDto>>
                {
                    Success = true,
                    Message = $"IT users for company '{company.Name}' retrieved successfully",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving IT users for company {CompanyId}", companyId);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving company IT users",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update user to IT role (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Updated user with IT role</returns>
        /// <response code="200">User assigned IT role successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}/assign-it-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserRolesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> AssignITRole(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                // Check if user already has IT role
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (currentRoles.Contains("IT"))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User already has IT role",
                        Errors = new List<string> { "User already assigned IT role" }
                    });
                }

                // Add IT role to user
                var result = await _userManager.AddToRoleAsync(user, "IT");
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to assign IT role",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Get updated roles
                var updatedRoles = await _userManager.GetRolesAsync(user);
                var allRoles = await _context.Roles.Select(r => r.Name!).ToListAsync();

                var userRolesDto = new UserRolesDto
                {
                    UserId = user.Id,
                    UserEmail = user.Email!,
                    UserName = $"{user.FirstName} {user.LastName}",
                    CurrentRoles = updatedRoles.ToList(),
                    AvailableRoles = allRoles.Except(updatedRoles).OrderBy(r => r).ToList()
                };

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("IT role assigned to user {UserId} by admin {AdminId}", id, adminId);

                return Ok(new ApiResponse<UserRolesDto>
                {
                    Success = true,
                    Message = "IT role assigned successfully",
                    Data = userRolesDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning IT role to user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while assigning IT role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Remove IT role from user (Admin only)
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>Updated user without IT role</returns>
        /// <response code="200">IT role removed successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}/remove-it-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<UserRolesDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> RemoveITRole(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { $"User with ID {id} does not exist" }
                    });
                }

                // Check if user has IT role
                var currentRoles = await _userManager.GetRolesAsync(user);
                if (!currentRoles.Contains("IT"))
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User does not have IT role",
                        Errors = new List<string> { "User is not assigned IT role" }
                    });
                }

                // Remove IT role from user
                var result = await _userManager.RemoveFromRoleAsync(user, "IT");
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to remove IT role",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Get updated roles
                var updatedRoles = await _userManager.GetRolesAsync(user);
                var allRoles = await _context.Roles.Select(r => r.Name!).ToListAsync();

                var userRolesDto = new UserRolesDto
                {
                    UserId = user.Id,
                    UserEmail = user.Email!,
                    UserName = $"{user.FirstName} {user.LastName}",
                    CurrentRoles = updatedRoles.ToList(),
                    AvailableRoles = allRoles.Except(updatedRoles).OrderBy(r => r).ToList()
                };

                var adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("IT role removed from user {UserId} by admin {AdminId}", id, adminId);

                return Ok(new ApiResponse<UserRolesDto>
                {
                    Success = true,
                    Message = "IT role removed successfully",
                    Data = userRolesDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing IT role from user {UserId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while removing IT role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get user statistics (Admin only)
        /// </summary>
        /// <returns>User statistics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetUserStatistics()
        {
            try
            {
                var totalUsers = await _userManager.Users.CountAsync();
                var activeUsers = await _userManager.Users.CountAsync(u => u.IsActive);
                var inactiveUsers = totalUsers - activeUsers;

                // Get role statistics
                var roleStats = new List<object>();
                var roles = new[] { "Admin", "HR", "HRManager", "IT", "Employee" };
                
                foreach (var role in roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                    var activeUsersInRole = usersInRole.Count(u => u.IsActive);
                    
                    roleStats.Add(new
                    {
                        Role = role,
                        TotalUsers = usersInRole.Count,
                        ActiveUsers = activeUsersInRole,
                        InactiveUsers = usersInRole.Count - activeUsersInRole
                    });
                }

                // Get company statistics
                var companyStats = await _userManager.Users
                    .Where(u => u.CompanyId.HasValue)
                    .GroupBy(u => u.CompanyId)
                    .Select(g => new
                    {
                        CompanyId = g.Key,
                        TotalUsers = g.Count(),
                        ActiveUsers = g.Count(u => u.IsActive)
                    })
                    .OrderByDescending(x => x.TotalUsers)
                    .Take(10)
                    .ToListAsync();

                // Get recent users
                var recentUsers = await _userManager.Users
                    .Where(u => u.IsActive)
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new
                    {
                        u.Id,
                        u.Email,
                        u.FirstName,
                        u.LastName,
                        u.Company!.Name,
                        u.CreatedAt
                    })
                    .ToListAsync();

                var statistics = new
                {
                    Overview = new
                    {
                        TotalUsers = totalUsers,
                        ActiveUsers = activeUsers,
                        InactiveUsers = inactiveUsers
                    },
                    RoleBreakdown = roleStats,
                    CompanyBreakdown = companyStats,
                    RecentUsers = recentUsers
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "User statistics retrieved successfully",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving user statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
