using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<DashboardController> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        /// <summary>
        /// Get employee dashboard data (All authenticated users)
        /// </summary>
        /// <returns>Employee dashboard information</returns>
        [HttpGet("employee")]
        public async Task<IActionResult> GetEmployeeDashboard()
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
                        Message = "User not found"
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);

                var dashboardData = new
                {
                    WelcomeMessage = $"Welcome, {user.FirstName} {user.LastName}!",
                    Department = user.Department ?? "Not Assigned",
                    Position = user.Position ?? "Not Assigned",
                    Roles = userRoles.ToList(),
                    LastLogin = DateTime.UtcNow,
                    QuickStats = new
                    {
                        TasksCompleted = 0,
                        PendingRequests = 0,
                        UpcomingEvents = 0
                    }
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Employee dashboard data retrieved successfully",
                    Data = dashboardData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving employee dashboard");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving dashboard data",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get manager dashboard data (Manager and Admin only)
        /// </summary>
        /// <returns>Manager dashboard information</returns>
        [HttpGet("manager")]
        [Authorize(Roles = "Manager,Admin")]
        public IActionResult GetManagerDashboard()
        {
            try
            {
                var totalEmployees = _userManager.Users.Count(u => u.IsActive);
                var totalDepartments = _userManager.Users
                    .Where(u => u.IsActive && !string.IsNullOrEmpty(u.Department))
                    .Select(u => u.Department)
                    .Distinct()
                    .Count();

                var departmentStats = _userManager.Users
                    .Where(u => u.IsActive && !string.IsNullOrEmpty(u.Department))
                    .GroupBy(u => u.Department)
                    .Select(g => new
                    {
                        Department = g.Key,
                        EmployeeCount = g.Count()
                    })
                    .ToList();

                var dashboardData = new
                {
                    TotalEmployees = totalEmployees,
                    TotalDepartments = totalDepartments,
                    DepartmentBreakdown = departmentStats,
                    RecentActivities = new List<object>
                    {
                        new { Activity = "New employee onboarded", Date = DateTime.UtcNow.AddDays(-1) },
                        new { Activity = "Performance review completed", Date = DateTime.UtcNow.AddDays(-2) },
                        new { Activity = "Training session scheduled", Date = DateTime.UtcNow.AddDays(-3) }
                    },
                    PendingApprovals = new
                    {
                        LeaveRequests = 5,
                        ExpenseReports = 3,
                        TimeSheets = 12
                    }
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Manager dashboard data retrieved successfully",
                    Data = dashboardData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving manager dashboard");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving dashboard data",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get admin dashboard data (Admin only)
        /// </summary>
        /// <returns>Admin dashboard information</returns>
        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var totalUsers = _userManager.Users.Count();
                var activeUsers = _userManager.Users.Count(u => u.IsActive);
                var inactiveUsers = totalUsers - activeUsers;

                var roleStats = new List<object>();
                foreach (var role in _roleManager.Roles)
                {
                    var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
                    roleStats.Add(new
                    {
                        Role = role.Name,
                        UserCount = usersInRole.Count
                    });
                }

                var recentRegistrations = _userManager.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new
                    {
                        Name = $"{u.FirstName} {u.LastName}",
                        Email = u.Email,
                        Department = u.Department,
                        RegistrationDate = u.CreatedAt
                    })
                    .ToList();

                var dashboardData = new
                {
                    SystemOverview = new
                    {
                        TotalUsers = totalUsers,
                        ActiveUsers = activeUsers,
                        InactiveUsers = inactiveUsers,
                        TotalRoles = _roleManager.Roles.Count()
                    },
                    RoleDistribution = roleStats,
                    RecentRegistrations = recentRegistrations,
                    SystemHealth = new
                    {
                        DatabaseStatus = "Connected",
                        LastBackup = DateTime.UtcNow.AddDays(-1),
                        SystemUptime = "99.9%",
                        ActiveSessions = 45
                    },
                    SecurityAlerts = new List<object>
                    {
                        new { Alert = "No failed login attempts in last 24 hours", Severity = "Low" },
                        new { Alert = "All user accounts have strong passwords", Severity = "Low" }
                    }
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Admin dashboard data retrieved successfully",
                    Data = dashboardData
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving admin dashboard");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving dashboard data",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get system statistics (Admin only)
        /// </summary>
        /// <returns>System statistics</returns>
        [HttpGet("stats")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetSystemStats()
        {
            try
            {
                var stats = new
                {
                    UserStats = new
                    {
                        TotalUsers = _userManager.Users.Count(),
                        ActiveUsers = _userManager.Users.Count(u => u.IsActive),
                        NewUsersThisMonth = _userManager.Users.Count(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30)),
                        NewUsersToday = _userManager.Users.Count(u => u.CreatedAt.Date == DateTime.UtcNow.Date)
                    },
                    DepartmentStats = _userManager.Users
                        .Where(u => u.IsActive && !string.IsNullOrEmpty(u.Department))
                        .GroupBy(u => u.Department)
                        .Select(g => new
                        {
                            Department = g.Key,
                            Count = g.Count(),
                            Percentage = Math.Round((double)g.Count() / _userManager.Users.Count(u => u.IsActive) * 100, 2)
                        })
                        .ToList(),
                    RoleStats = _roleManager.Roles.Select(r => new
                    {
                        Role = r.Name,
                        IsActive = r.IsActive,
                        Description = r.Description
                    }).ToList()
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "System statistics retrieved successfully",
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving system statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
