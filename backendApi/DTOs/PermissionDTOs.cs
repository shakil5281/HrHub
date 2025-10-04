using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreatePermissionRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Code { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Module { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Resource { get; set; } = string.Empty;
    }

    public class UpdatePermissionRequest
    {
        [StringLength(100, MinimumLength = 2)]
        public string? Name { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string? Code { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Module { get; set; }

        [StringLength(50)]
        public string? Action { get; set; }

        [StringLength(100)]
        public string? Resource { get; set; }

        public bool? IsActive { get; set; }
    }

    public class PermissionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class PermissionListResponse
    {
        public List<PermissionResponse> Permissions { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class PermissionQueryRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public string? Module { get; set; }
        public string? Action { get; set; }
        public string? Resource { get; set; }
        public bool? IsActive { get; set; }
        public string? SortBy { get; set; } = "Name";
        public string? SortDirection { get; set; } = "asc";
    }

    // Role Permission DTOs
    public class AssignRolePermissionRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public string PermissionId { get; set; } = string.Empty;

        public bool IsGranted { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }
    }

    public class BulkAssignRolePermissionRequest
    {
        [Required]
        public string RoleId { get; set; } = string.Empty;

        [Required]
        public List<string> PermissionIds { get; set; } = new();

        public bool IsGranted { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }
    }

    public class RolePermissionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class RolePermissionListResponse
    {
        public List<RolePermissionResponse> RolePermissions { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    // User Permission DTOs
    public class AssignUserPermissionRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string PermissionId { get; set; } = string.Empty;

        public bool IsGranted { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }
    }

    public class BulkAssignUserPermissionRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public List<string> PermissionIds { get; set; } = new();

        public bool IsGranted { get; set; } = true;

        public DateTime? ExpiresAt { get; set; }

        [StringLength(200)]
        public string? Reason { get; set; }
    }

    public class UserPermissionResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string PermissionId { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public string PermissionCode { get; set; } = string.Empty;
        public string Module { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Resource { get; set; } = string.Empty;
        public bool IsGranted { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Reason { get; set; }
    }

    public class UserPermissionListResponse
    {
        public List<UserPermissionResponse> UserPermissions { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    // Permission Group DTOs
    public class CreatePermissionGroupRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Module { get; set; } = string.Empty;
    }

    public class PermissionGroupResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Module { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public List<PermissionResponse> Permissions { get; set; } = new();
    }

    // User Role DTOs
    public class AssignUserRoleRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string RoleId { get; set; } = string.Empty;

        public DateTime? ExpiresAt { get; set; }
    }

    public class UserRoleResponse
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string RoleId { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime AssignedAt { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    // Permission Check DTOs
    public class CheckPermissionRequest
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string PermissionCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Resource { get; set; }
    }

    public class CheckPermissionResponse
    {
        public bool HasPermission { get; set; }
        public string PermissionCode { get; set; } = string.Empty;
        public string? Resource { get; set; }
        public string? Source { get; set; } // "Role" or "User"
        public string? Reason { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    public class UserPermissionsSummaryResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
        public List<PermissionResponse> DirectPermissions { get; set; } = new();
        public List<PermissionResponse> RolePermissions { get; set; } = new();
        public List<PermissionResponse> AllPermissions { get; set; } = new();
        public Dictionary<string, List<PermissionResponse>> PermissionsByModule { get; set; } = new();
    }

    public class PermissionStatisticsResponse
    {
        public int TotalPermissions { get; set; }
        public int ActivePermissions { get; set; }
        public int InactivePermissions { get; set; }
        public Dictionary<string, int> PermissionsByModule { get; set; } = new();
        public Dictionary<string, int> PermissionsByAction { get; set; } = new();
        public int TotalRolePermissions { get; set; }
        public int TotalUserPermissions { get; set; }
        public List<string> MostUsedPermissions { get; set; } = new();
    }
}
