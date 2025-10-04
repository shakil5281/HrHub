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
    public class UserPermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<UserPermissionController> _logger;

        public UserPermissionController(IPermissionService permissionService, ILogger<UserPermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Assign permission to user
        /// </summary>
        /// <param name="request">User permission assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("assign")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<UserPermissionResponse>> AssignUserPermission([FromBody] AssignUserPermissionRequest request)
        {
            try
            {
                var assignedBy = GetCurrentUserId();
                var result = await _permissionService.AssignUserPermissionAsync(request, assignedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user permission");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove permission from user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="permissionId">Permission ID</param>
        /// <returns>Removal result</returns>
        [HttpDelete("{userId}/permission/{permissionId}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> RemoveUserPermission(string userId, string permissionId)
        {
            try
            {
                var removedBy = GetCurrentUserId();
                var result = await _permissionService.RemoveUserPermissionAsync(userId, permissionId, removedBy);
                
                if (!result)
                {
                    return NotFound(new { message = "User permission assignment not found" });
                }

                return Ok(new { message = "User permission removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user permission: User {userId}, Permission {permissionId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user permissions with pagination
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated user permissions</returns>
        [HttpGet("{userId}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<UserPermissionListResponse>> GetUserPermissions(string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _permissionService.GetUserPermissionsAsync(userId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user permissions: {userId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user permissions list (all direct permissions for a user)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of direct permissions for the user</returns>
        [HttpGet("{userId}/list")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<PermissionResponse>>> GetUserPermissionsList(string userId)
        {
            try
            {
                var result = await _permissionService.GetUserPermissionsListAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user permissions list: {userId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Bulk assign permissions to user
        /// </summary>
        /// <param name="request">Bulk assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("bulk-assign")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> BulkAssignUserPermissions([FromBody] BulkAssignUserPermissionRequest request)
        {
            try
            {
                var assignedBy = GetCurrentUserId();
                var result = await _permissionService.BulkAssignUserPermissionsAsync(request, assignedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to assign user permissions" });
                }

                return Ok(new { message = "User permissions assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk assigning user permissions");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Bulk remove permissions from user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="permissionIds">List of permission IDs to remove</param>
        /// <returns>Removal result</returns>
        [HttpPost("{userId}/bulk-remove")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> BulkRemoveUserPermissions(string userId, [FromBody] List<string> permissionIds)
        {
            try
            {
                var removedBy = GetCurrentUserId();
                var result = await _permissionService.BulkRemoveUserPermissionsAsync(userId, permissionIds, removedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to remove user permissions" });
                }

                return Ok(new { message = "User permissions removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk removing user permissions: {userId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user permissions summary (includes roles and effective permissions)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User permissions summary</returns>
        [HttpGet("{userId}/summary")]
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
        /// Get user effective permissions (combines role and direct permissions)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of effective permissions</returns>
        [HttpGet("{userId}/effective")]
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

        /// <summary>
        /// Check if user has specific permission
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="permissionCode">Permission code</param>
        /// <param name="resource">Optional resource identifier</param>
        /// <returns>Permission check result</returns>
        [HttpGet("{userId}/check/{permissionCode}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<CheckPermissionResponse>> CheckUserPermission(string userId, string permissionCode, [FromQuery] string? resource = null)
        {
            try
            {
                var request = new CheckPermissionRequest
                {
                    UserId = userId,
                    PermissionCode = permissionCode,
                    Resource = resource
                };

                var result = await _permissionService.CheckUserPermissionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking user permission: User {userId}, Permission {permissionCode}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Sync user permissions (replace all direct permissions for a user)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="permissionIds">List of permission IDs to sync</param>
        /// <returns>Sync result</returns>
        [HttpPost("{userId}/sync")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> SyncUserPermissions(string userId, [FromBody] List<string> permissionIds)
        {
            try
            {
                var syncedBy = GetCurrentUserId();
                var result = await _permissionService.SyncUserPermissionsAsync(userId, permissionIds, syncedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to sync user permissions" });
                }

                return Ok(new { message = "User permissions synced successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error syncing user permissions: {userId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Copy permissions from one user to another
        /// </summary>
        /// <param name="sourceUserId">Source user ID</param>
        /// <param name="targetUserId">Target user ID</param>
        /// <returns>Copy result</returns>
        [HttpPost("copy/{sourceUserId}/{targetUserId}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CopyUserPermissions(string sourceUserId, string targetUserId)
        {
            try
            {
                var copiedBy = GetCurrentUserId();
                var result = await _permissionService.CopyUserPermissionsAsync(sourceUserId, targetUserId, copiedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to copy user permissions" });
                }

                return Ok(new { message = "User permissions copied successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error copying user permissions: {sourceUserId} to {targetUserId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current user's own permissions
        /// </summary>
        /// <returns>Current user's permissions summary</returns>
        [HttpGet("my-permissions")]
        [Authorize]
        public async Task<ActionResult<UserPermissionsSummaryResponse>> GetMyPermissions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _permissionService.GetUserPermissionsSummaryAsync(userId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving current user permissions: {GetCurrentUserId()}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if current user has specific permission
        /// </summary>
        /// <param name="permissionCode">Permission code</param>
        /// <param name="resource">Optional resource identifier</param>
        /// <returns>Permission check result</returns>
        [HttpGet("my-permissions/check/{permissionCode}")]
        [Authorize]
        public async Task<ActionResult<CheckPermissionResponse>> CheckMyPermission(string permissionCode, [FromQuery] string? resource = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var request = new CheckPermissionRequest
                {
                    UserId = userId,
                    PermissionCode = permissionCode,
                    Resource = resource
                };

                var result = await _permissionService.CheckUserPermissionAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking current user permission: {GetCurrentUserId()}, Permission {permissionCode}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get current user's effective permissions
        /// </summary>
        /// <returns>List of current user's effective permissions</returns>
        [HttpGet("my-permissions/effective")]
        [Authorize]
        public async Task<ActionResult<List<PermissionResponse>>> GetMyEffectivePermissions()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _permissionService.GetUserEffectivePermissionsAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving current user effective permissions: {GetCurrentUserId()}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        }
    }
}
