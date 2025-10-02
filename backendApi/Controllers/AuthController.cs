using HrHubAPI.DTOs;
using HrHubAPI.Models;
using HrHubAPI.Services;
using HrHubAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService,
            ApplicationDbContext context,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Register a new user account (Admin only)
        /// </summary>
        /// <param name="model">User registration information</param>
        /// <returns>Authentication response with JWT token and user information</returns>
        /// <response code="200">User registered successfully</response>
        /// <response code="400">Invalid input or user already exists</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("register")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
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

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User with this email already exists",
                        Errors = new List<string> { "Email already registered" }
                    });
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Department = model.Department,
                    Position = model.Position,
                    CompanyId = model.CompanyId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User registration failed",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Assign default Employee role
                await _userManager.AddToRoleAsync(user, "Employee");

                var token = await _jwtService.GenerateJwtTokenAsync(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Store refresh token in database
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days expiry
                await _userManager.UpdateAsync(user);

                var userRoles = await _userManager.GetRolesAsync(user);

                var response = new AuthResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(60),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Department = user.Department,
                        Position = user.Position,
                        CompanyId = user.CompanyId,
                        CompanyName = user.Company?.Name,
                        Roles = userRoles.ToList()
                    }
                };

                _logger.LogInformation("User {Email} registered successfully", user.Email);

                return Ok(new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "User registered successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user registration");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during registration",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Authenticate user and generate JWT token
        /// </summary>
        /// <param name="model">User login credentials</param>
        /// <returns>Authentication response with JWT token and user information</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid credentials</response>
        /// <response code="400">Invalid input</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
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

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !user.IsActive)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Errors = new List<string> { "Authentication failed" }
                    });
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (!result.Succeeded)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid email or password",
                        Errors = new List<string> { "Authentication failed" }
                    });
                }

                var token = await _jwtService.GenerateJwtTokenAsync(user);
                var refreshToken = _jwtService.GenerateRefreshToken();

                // Store refresh token in database
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days expiry
                await _userManager.UpdateAsync(user);

                var userRoles = await _userManager.GetRolesAsync(user);

                var response = new AuthResponseDto
                {
                    Token = token,
                    RefreshToken = refreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(60),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Department = user.Department,
                        Position = user.Position,
                        CompanyId = user.CompanyId,
                        CompanyName = user.Company?.Name,
                        Roles = userRoles.ToList()
                    }
                };

                _logger.LogInformation("User {Email} logged in successfully", user.Email);

                return Ok(new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Login successful",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during user login");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during login",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Assign a role to a user (Admin only)
        /// </summary>
        /// <param name="model">Role assignment information</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">Role assigned successfully</response>
        /// <response code="400">Invalid input or role assignment failed</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("assign-role")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto model)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
                    });
                }

                var roleExists = await _roleManager.RoleExistsAsync(model.RoleName);
                if (!roleExists)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Role does not exist",
                        Errors = new List<string> { $"Role '{model.RoleName}' not found" }
                    });
                }

                var result = await _userManager.AddToRoleAsync(user, model.RoleName);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to assign role",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                _logger.LogInformation("Role {RoleName} assigned to user {Email}", model.RoleName, user.Email);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"Role '{model.RoleName}' assigned successfully to user '{user.Email}'"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning role");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while assigning role",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get current user's profile information
        /// </summary>
        /// <returns>User profile information</returns>
        /// <response code="200">Profile retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("profile")]
        [HttpPost("profile")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<UserDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
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
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
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

                return Ok(new ApiResponse<UserDto>
                {
                    Success = true,
                    Message = "Profile retrieved successfully",
                    Data = userDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving user profile");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving profile",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Refresh JWT token using refresh token
        /// </summary>
        /// <param name="model">Token refresh request containing current token and refresh token</param>
        /// <returns>New JWT token and refresh token</returns>
        /// <response code="200">Token refreshed successfully</response>
        /// <response code="400">Invalid tokens or refresh token expired</response>
        /// <response code="401">Unauthorized - Invalid refresh token</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
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

                // Get principal from expired token
                ClaimsPrincipal principal;
                try
                {
                    principal = _jwtService.GetPrincipalFromExpiredToken(model.Token);
                }
                catch (SecurityTokenException)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid access token",
                        Errors = new List<string> { "The provided access token is invalid" }
                    });
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token claims",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid refresh token",
                        Errors = new List<string> { "The provided refresh token is invalid or expired" }
                    });
                }

                // Generate new tokens
                var newJwtToken = await _jwtService.GenerateJwtTokenAsync(user);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

                // Update refresh token in database
                user.RefreshToken = newRefreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days expiry
                await _userManager.UpdateAsync(user);

                var userRoles = await _userManager.GetRolesAsync(user);

                var response = new AuthResponseDto
                {
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken,
                    Expiration = DateTime.UtcNow.AddMinutes(60),
                    User = new UserDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Department = user.Department,
                        Position = user.Position,
                        CompanyId = user.CompanyId,
                        CompanyName = user.Company?.Name,
                        Roles = userRoles.ToList()
                    }
                };

                _logger.LogInformation("Token refreshed successfully for user {Email}", user.Email);

                return Ok(new ApiResponse<AuthResponseDto>
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    Data = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during token refresh");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred during token refresh",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Revoke refresh token (logout)
        /// </summary>
        /// <returns>Success or error response</returns>
        /// <response code="200">Refresh token revoked successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="404">User not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("revoke-token")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> RevokeToken()
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
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
                    });
                }

                // Clear refresh token
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.UtcNow; // Set to past date
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Refresh token revoked for user {Email}", user.Email);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Refresh token revoked successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while revoking refresh token");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while revoking refresh token",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Assign a user to a company (Admin only)
        /// </summary>
        /// <param name="model">Company assignment information</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">User assigned to company successfully</response>
        /// <response code="400">Invalid input or assignment failed</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="404">User or company not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("assign-company")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> AssignCompany([FromBody] AssignCompanyDto model)
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

                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
                    });
                }

                // Check if company exists
                var company = await _context.Companies.FindAsync(model.CompanyId);
                if (company == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {model.CompanyId} does not exist" }
                    });
                }

                user.CompanyId = model.CompanyId;
                user.UpdatedAt = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Failed to assign user to company",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                _logger.LogInformation("User {UserId} assigned to company {CompanyId}", model.UserId, model.CompanyId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = $"User '{user.Email}' assigned successfully to company '{company.Name}'"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning user to company");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while assigning user to company",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Assign multiple users to a company (Admin only)
        /// </summary>
        /// <param name="model">Multiple users company assignment information</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">Users assigned to company successfully</response>
        /// <response code="400">Invalid input or assignment failed</response>
        /// <response code="401">Unauthorized - Admin role required</response>
        /// <response code="404">Company not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("assign-multiple-users-to-company")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> AssignMultipleUsersToCompany([FromBody] AssignMultipleUsersToCompanyDto model)
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

                // Check if company exists
                var company = await _context.Companies.FindAsync(model.CompanyId);
                if (company == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {model.CompanyId} does not exist" }
                    });
                }

                var assignedUsers = new List<string>();
                var failedUsers = new List<string>();

                foreach (var userId in model.UserIds)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        user.CompanyId = model.CompanyId;
                        user.UpdatedAt = DateTime.UtcNow;

                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            assignedUsers.Add(user.Email ?? userId);
                        }
                        else
                        {
                            failedUsers.Add(user.Email ?? userId);
                        }
                    }
                    else
                    {
                        failedUsers.Add(userId);
                    }
                }

                var message = $"Assigned {assignedUsers.Count} users to company '{company.Name}'";
                if (failedUsers.Count > 0)
                {
                    message += $". Failed to assign {failedUsers.Count} users: {string.Join(", ", failedUsers)}";
                }

                _logger.LogInformation("Multiple users assigned to company {CompanyId}. Success: {SuccessCount}, Failed: {FailedCount}", 
                    model.CompanyId, assignedUsers.Count, failedUsers.Count);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = message,
                    Data = new { AssignedUsers = assignedUsers, FailedUsers = failedUsers }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while assigning multiple users to company");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while assigning users to company",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
