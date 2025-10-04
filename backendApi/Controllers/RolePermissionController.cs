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
    public class RolePermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;
        private readonly ILogger<RolePermissionController> _logger;

        public RolePermissionController(IPermissionService permissionService, ILogger<RolePermissionController> logger)
        {
            _permissionService = permissionService;
            _logger = logger;
        }

        /// <summary>
        /// Assign permission to role
        /// </summary>
        /// <param name="request">Role permission assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("assign")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<RolePermissionResponse>> AssignRolePermission([FromBody] AssignRolePermissionRequest request)
        {
            try
            {
                var assignedBy = GetCurrentUserId();
                var result = await _permissionService.AssignRolePermissionAsync(request, assignedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning role permission");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove permission from role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="permissionId">Permission ID</param>
        /// <returns>Removal result</returns>
        [HttpDelete("{roleId}/permission/{permissionId}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> RemoveRolePermission(string roleId, string permissionId)
        {
            try
            {
                var removedBy = GetCurrentUserId();
                var result = await _permissionService.RemoveRolePermissionAsync(roleId, permissionId, removedBy);
                
                if (!result)
                {
                    return NotFound(new { message = "Role permission assignment not found" });
                }

                return Ok(new { message = "Role permission removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing role permission: Role {roleId}, Permission {permissionId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get role permissions with pagination
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paginated role permissions</returns>
        [HttpGet("{roleId}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<RolePermissionListResponse>> GetRolePermissions(string roleId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _permissionService.GetRolePermissionsAsync(roleId, pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving role permissions: {roleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get role permissions list (all permissions for a role)
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>List of permissions for the role</returns>
        [HttpGet("{roleId}/list")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<PermissionResponse>>> GetRolePermissionsList(string roleId)
        {
            try
            {
                var result = await _permissionService.GetRolePermissionsListAsync(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving role permissions list: {roleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Bulk assign permissions to role
        /// </summary>
        /// <param name="request">Bulk assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("bulk-assign")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> BulkAssignRolePermissions([FromBody] BulkAssignRolePermissionRequest request)
        {
            try
            {
                var assignedBy = GetCurrentUserId();
                var result = await _permissionService.BulkAssignRolePermissionsAsync(request, assignedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to assign role permissions" });
                }

                return Ok(new { message = "Role permissions assigned successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error bulk assigning role permissions");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Bulk remove permissions from role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="permissionIds">List of permission IDs to remove</param>
        /// <returns>Removal result</returns>
        [HttpPost("{roleId}/bulk-remove")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> BulkRemoveRolePermissions(string roleId, [FromBody] List<string> permissionIds)
        {
            try
            {
                var removedBy = GetCurrentUserId();
                var result = await _permissionService.BulkRemoveRolePermissionsAsync(roleId, permissionIds, removedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to remove role permissions" });
                }

                return Ok(new { message = "Role permissions removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk removing role permissions: {roleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get users assigned to a role
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <returns>List of users with the role</returns>
        [HttpGet("{roleId}/users")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<UserRoleResponse>>> GetRoleUsers(string roleId)
        {
            try
            {
                var result = await _permissionService.GetRoleUsersAsync(roleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving role users: {roleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Assign user to role
        /// </summary>
        /// <param name="request">User role assignment request</param>
        /// <returns>Assignment result</returns>
        [HttpPost("user-role/assign")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<UserRoleResponse>> AssignUserRole([FromBody] AssignUserRoleRequest request)
        {
            try
            {
                var assignedBy = GetCurrentUserId();
                var result = await _permissionService.AssignUserRoleAsync(request, assignedBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning user role");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Remove user from role
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="roleId">Role ID</param>
        /// <returns>Removal result</returns>
        [HttpDelete("user-role/{userId}/{roleId}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> RemoveUserRole(string userId, string roleId)
        {
            try
            {
                var removedBy = GetCurrentUserId();
                var result = await _permissionService.RemoveUserRoleAsync(userId, roleId, removedBy);
                
                if (!result)
                {
                    return NotFound(new { message = "User role assignment not found" });
                }

                return Ok(new { message = "User role removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user role: User {userId}, Role {roleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get user roles
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>List of user roles</returns>
        [HttpGet("user/{userId}/roles")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<UserRoleResponse>>> GetUserRoles(string userId)
        {
            try
            {
                var result = await _permissionService.GetUserRolesAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user roles: {userId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Sync role permissions (replace all permissions for a role)
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="permissionIds">List of permission IDs to sync</param>
        /// <returns>Sync result</returns>
        [HttpPost("{roleId}/sync")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> SyncRolePermissions(string roleId, [FromBody] List<string> permissionIds)
        {
            try
            {
                var syncedBy = GetCurrentUserId();
                var result = await _permissionService.SyncRolePermissionsAsync(roleId, permissionIds, syncedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to sync role permissions" });
                }

                return Ok(new { message = "Role permissions synced successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error syncing role permissions: {roleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Copy permissions from one role to another
        /// </summary>
        /// <param name="sourceRoleId">Source role ID</param>
        /// <param name="targetRoleId">Target role ID</param>
        /// <returns>Copy result</returns>
        [HttpPost("copy/{sourceRoleId}/{targetRoleId}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CopyRolePermissions(string sourceRoleId, string targetRoleId)
        {
            try
            {
                var copiedBy = GetCurrentUserId();
                var result = await _permissionService.CopyRolePermissionsAsync(sourceRoleId, targetRoleId, copiedBy);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to copy role permissions" });
                }

                return Ok(new { message = "Role permissions copied successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error copying role permissions: {sourceRoleId} to {targetRoleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        }
    }
}
