using HrHubAPI.DTOs;
using HrHubAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<PermissionController> _logger;

        public PermissionController(IPermissionService permissionService, ILogger<PermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new permission
        /// </summary>
        /// <param name="request">Permission creation request</param>
        /// <returns>Created permission details</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<PermissionResponse>> CreatePermission([FromBody] CreatePermissionRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _permissionService.CreatePermissionAsync(request, userId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating permission");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permission by ID
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>Permission details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<PermissionResponse>> GetPermission(string id)
        {
            try
            {
                var result = await _permissionService.GetPermissionByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Permission not found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permission: {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permission by code
        /// </summary>
        /// <param name="code">Permission code</param>
        /// <returns>Permission details</returns>
        [HttpGet("by-code/{code}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<PermissionResponse>> GetPermissionByCode(string code)
        {
            try
            {
                var result = await _permissionService.GetPermissionByCodeAsync(code);
                if (result == null)
                {
                    return NotFound(new { message = "Permission not found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permission by code: {code}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get list of permissions with filtering and pagination
        /// </summary>
        /// <param name="request">Query parameters</param>
        /// <returns>Paginated list of permissions</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<PermissionListResponse>> GetPermissions([FromQuery] PermissionQueryRequest request)
        {
            try
            {
                var result = await _permissionService.GetPermissionsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update permission
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <param name="request">Update request</param>
        /// <returns>Updated permission details</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<PermissionResponse>> UpdatePermission(string id, [FromBody] UpdatePermissionRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _permissionService.UpdatePermissionAsync(id, request, userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating permission: {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete permission
        /// </summary>
        /// <param name="id">Permission ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> DeletePermission(string id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _permissionService.DeletePermissionAsync(id, userId);
                
                if (!result)
                {
                    return NotFound(new { message = "Permission not found" });
                }

                return Ok(new { message = "Permission deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting permission: {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permissions by module
        /// </summary>
        /// <param name="module">Module name</param>
        /// <returns>List of permissions for the module</returns>
        [HttpGet("by-module/{module}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<PermissionResponse>>> GetPermissionsByModule(string module)
        {
            try
            {
                var result = await _permissionService.GetPermissionsByModuleAsync(module);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permissions by module: {module}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get permissions by action
        /// </summary>
        /// <param name="action">Action name</param>
        /// <returns>List of permissions for the action</returns>
        [HttpGet("by-action/{action}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<PermissionResponse>>> GetPermissionsByAction(string action)
        {
            try
            {
                var result = await _permissionService.GetPermissionsByActionAsync(action);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permissions by action: {action}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        /// <param name="request">Permission check request</param>
        /// <returns>Permission check result</returns>
        [HttpPost("check")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<CheckPermissionResponse>> CheckPermission([FromBody] CheckPermissionRequest request)
        {
            try
            {
                var result = await _permissionService.CheckUserPermissionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user permissions summary
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User permissions summary</returns>
        [HttpGet("user/{userId}/summary")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<UserPermissionsSummaryResponse>> GetUserPermissionsSummary(string userId)
        {
            try
            {
                var result = await _permissionService.GetUserPermissionsSummaryAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user permissions summary: {userId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user effective permissions
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of effective permissions</returns>
        [HttpGet("user/{userId}/effective")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<PermissionResponse>>> GetUserEffectivePermissions(string userId)
        {
            try
            {
                var result = await _permissionService.GetUserEffectivePermissionsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user effective permissions: {userId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        }
    }
}
