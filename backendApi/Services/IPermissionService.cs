using HrHubAPI.DTOs;

namespace HrHubAPI.Services
{
    public interface IPermissionService
    {
        // Permission Management
        Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request, string userId);
        Task<PermissionResponse?> GetPermissionByIdAsync(string permissionId);
        Task<PermissionResponse?> GetPermissionByCodeAsync(string code);
        Task<PermissionListResponse> GetPermissionsAsync(PermissionQueryRequest request);
        Task<PermissionResponse> UpdatePermissionAsync(string permissionId, UpdatePermissionRequest request, string userId);
        Task<bool> DeletePermissionAsync(string permissionId, string userId);
        Task<List<PermissionResponse>> GetPermissionsByModuleAsync(string module);
        Task<List<PermissionResponse>> GetPermissionsByActionAsync(string action);

        // Role Permission Management
        Task<RolePermissionResponse> AssignRolePermissionAsync(AssignRolePermissionRequest request, string assignedBy);
        Task<bool> RemoveRolePermissionAsync(string roleId, string permissionId, string removedBy);
        Task<RolePermissionListResponse> GetRolePermissionsAsync(string roleId, int pageNumber = 1, int pageSize = 10);
        Task<List<PermissionResponse>> GetRolePermissionsListAsync(string roleId);
        Task<bool> BulkAssignRolePermissionsAsync(BulkAssignRolePermissionRequest request, string assignedBy);
        Task<bool> BulkRemoveRolePermissionsAsync(string roleId, List<string> permissionIds, string removedBy);

        // User Permission Management
        Task<UserPermissionResponse> AssignUserPermissionAsync(AssignUserPermissionRequest request, string assignedBy);
        Task<bool> RemoveUserPermissionAsync(string userId, string permissionId, string removedBy);
        Task<UserPermissionListResponse> GetUserPermissionsAsync(string userId, int pageNumber = 1, int pageSize = 10);
        Task<List<PermissionResponse>> GetUserPermissionsListAsync(string userId);
        Task<bool> BulkAssignUserPermissionsAsync(BulkAssignUserPermissionRequest request, string assignedBy);
        Task<bool> BulkRemoveUserPermissionsAsync(string userId, List<string> permissionIds, string removedBy);

        // User Role Management
        Task<UserRoleResponse> AssignUserRoleAsync(AssignUserRoleRequest request, string assignedBy);
        Task<bool> RemoveUserRoleAsync(string userId, string roleId, string removedBy);
        Task<List<UserRoleResponse>> GetUserRolesAsync(string userId);
        Task<List<UserRoleResponse>> GetRoleUsersAsync(string roleId);

        // Permission Checking
        Task<CheckPermissionResponse> CheckUserPermissionAsync(CheckPermissionRequest request);
        Task<bool> HasPermissionAsync(string userId, string permissionCode, string? resource = null);
        Task<UserPermissionsSummaryResponse> GetUserPermissionsSummaryAsync(string userId);
        Task<List<PermissionResponse>> GetUserEffectivePermissionsAsync(string userId);

        // Permission Group Management
        Task<PermissionGroupResponse> CreatePermissionGroupAsync(CreatePermissionGroupRequest request, string userId);
        Task<PermissionGroupResponse?> GetPermissionGroupByIdAsync(string groupId);
        Task<List<PermissionGroupResponse>> GetPermissionGroupsAsync();
        Task<PermissionGroupResponse> UpdatePermissionGroupAsync(string groupId, CreatePermissionGroupRequest request, string userId);
        Task<bool> DeletePermissionGroupAsync(string groupId, string userId);

        // Statistics and Reporting
        Task<PermissionStatisticsResponse> GetPermissionStatisticsAsync();
        Task<List<string>> GetAvailableModulesAsync();
        Task<List<string>> GetAvailableActionsAsync();
        Task<List<string>> GetAvailableResourcesAsync();

        // Bulk Operations
        Task<bool> SyncRolePermissionsAsync(string roleId, List<string> permissionIds, string syncedBy);
        Task<bool> SyncUserPermissionsAsync(string userId, List<string> permissionIds, string syncedBy);
        Task<bool> CopyRolePermissionsAsync(string sourceRoleId, string targetRoleId, string copiedBy);
        Task<bool> CopyUserPermissionsAsync(string sourceUserId, string targetUserId, string copiedBy);

        // Cleanup and Maintenance
        Task<bool> CleanupExpiredPermissionsAsync();
        Task<bool> ValidatePermissionIntegrityAsync();
        Task<List<string>> GetOrphanedPermissionsAsync();
    }
}
