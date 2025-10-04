using HrHubAPI.Data;
using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HrHubAPI.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<PermissionService> _logger;

        public PermissionService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<PermissionService> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        #region Permission Management

        public async Task<PermissionResponse> CreatePermissionAsync(CreatePermissionRequest request, string userId)
        {
            try
            {
                // Check if permission code already exists
                var existingPermission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Code == request.Code);

                if (existingPermission != null)
                {
                    throw new InvalidOperationException($"Permission with code '{request.Code}' already exists");
                }

                var permission = new Permission
                {
                    Name = request.Name,
                    Code = request.Code,
                    Description = request.Description,
                    Module = request.Module,
                    Action = request.Action,
                    Resource = request.Resource,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Permissions.Add(permission);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Permission created: {permission.Code} by user {userId}");

                return MapToPermissionResponse(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating permission: {request.Code}");
                throw;
            }
        }

        public async Task<PermissionResponse?> GetPermissionByIdAsync(string permissionId)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                return permission != null ? MapToPermissionResponse(permission) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permission: {permissionId}");
                throw;
            }
        }

        public async Task<PermissionResponse?> GetPermissionByCodeAsync(string code)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Code == code);

                return permission != null ? MapToPermissionResponse(permission) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permission by code: {code}");
                throw;
            }
        }

        public async Task<PermissionListResponse> GetPermissionsAsync(PermissionQueryRequest request)
        {
            try
            {
                var query = _context.Permissions.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    query = query.Where(p => p.Name.Contains(request.SearchTerm) ||
                                           p.Code.Contains(request.SearchTerm) ||
                                           p.Description!.Contains(request.SearchTerm));
                }

                if (!string.IsNullOrEmpty(request.Module))
                {
                    query = query.Where(p => p.Module == request.Module);
                }

                if (!string.IsNullOrEmpty(request.Action))
                {
                    query = query.Where(p => p.Action == request.Action);
                }

                if (!string.IsNullOrEmpty(request.Resource))
                {
                    query = query.Where(p => p.Resource == request.Resource);
                }

                if (request.IsActive.HasValue)
                {
                    query = query.Where(p => p.IsActive == request.IsActive.Value);
                }

                // Apply sorting
                query = request.SortBy?.ToLower() switch
                {
                    "name" => request.SortDirection == "asc" ? query.OrderBy(p => p.Name) : query.OrderByDescending(p => p.Name),
                    "code" => request.SortDirection == "asc" ? query.OrderBy(p => p.Code) : query.OrderByDescending(p => p.Code),
                    "module" => request.SortDirection == "asc" ? query.OrderBy(p => p.Module) : query.OrderByDescending(p => p.Module),
                    "createdat" => request.SortDirection == "asc" ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
                    _ => query.OrderBy(p => p.Name)
                };

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

                var permissions = await query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(p => MapToPermissionResponse(p))
                    .ToListAsync();

                return new PermissionListResponse
                {
                    Permissions = permissions,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving permissions");
                throw;
            }
        }

        public async Task<PermissionResponse> UpdatePermissionAsync(string permissionId, UpdatePermissionRequest request, string userId)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                if (permission == null)
                {
                    throw new KeyNotFoundException($"Permission with ID '{permissionId}' not found");
                }

                // Check if new code conflicts with existing permissions
                if (!string.IsNullOrEmpty(request.Code) && request.Code != permission.Code)
                {
                    var existingPermission = await _context.Permissions
                        .FirstOrDefaultAsync(p => p.Code == request.Code && p.Id != permissionId);

                    if (existingPermission != null)
                    {
                        throw new InvalidOperationException($"Permission with code '{request.Code}' already exists");
                    }
                }

                // Update fields
                if (!string.IsNullOrEmpty(request.Name))
                    permission.Name = request.Name;
                if (!string.IsNullOrEmpty(request.Code))
                    permission.Code = request.Code;
                if (request.Description != null)
                    permission.Description = request.Description;
                if (!string.IsNullOrEmpty(request.Module))
                    permission.Module = request.Module;
                if (!string.IsNullOrEmpty(request.Action))
                    permission.Action = request.Action;
                if (!string.IsNullOrEmpty(request.Resource))
                    permission.Resource = request.Resource;
                if (request.IsActive.HasValue)
                    permission.IsActive = request.IsActive.Value;

                permission.UpdatedAt = DateTime.UtcNow;
                permission.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Permission updated: {permission.Code} by user {userId}");

                return MapToPermissionResponse(permission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating permission: {permissionId}");
                throw;
            }
        }

        public async Task<bool> DeletePermissionAsync(string permissionId, string userId)
        {
            try
            {
                var permission = await _context.Permissions
                    .FirstOrDefaultAsync(p => p.Id == permissionId);

                if (permission == null)
                {
                    return false;
                }

                // Check if permission is in use
                var rolePermissionCount = await _context.RolePermissions
                    .CountAsync(rp => rp.PermissionId == permissionId);
                var userPermissionCount = await _context.UserPermissions
                    .CountAsync(up => up.PermissionId == permissionId);

                if (rolePermissionCount > 0 || userPermissionCount > 0)
                {
                    throw new InvalidOperationException("Cannot delete permission that is assigned to roles or users");
                }

                _context.Permissions.Remove(permission);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Permission deleted: {permission.Code} by user {userId}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting permission: {permissionId}");
                throw;
            }
        }

        public async Task<List<PermissionResponse>> GetPermissionsByModuleAsync(string module)
        {
            try
            {
                var permissions = await _context.Permissions
                    .Where(p => p.Module == module && p.IsActive)
                    .OrderBy(p => p.Name)
                    .Select(p => MapToPermissionResponse(p))
                    .ToListAsync();

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permissions by module: {module}");
                throw;
            }
        }

        public async Task<List<PermissionResponse>> GetPermissionsByActionAsync(string action)
        {
            try
            {
                var permissions = await _context.Permissions
                    .Where(p => p.Action == action && p.IsActive)
                    .OrderBy(p => p.Name)
                    .Select(p => MapToPermissionResponse(p))
                    .ToListAsync();

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving permissions by action: {action}");
                throw;
            }
        }

        #endregion

        #region Role Permission Management

        public async Task<RolePermissionResponse> AssignRolePermissionAsync(AssignRolePermissionRequest request, string assignedBy)
        {
            try
            {
                // Check if assignment already exists
                var existingAssignment = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == request.PermissionId);

                if (existingAssignment != null)
                {
                    // Update existing assignment
                    existingAssignment.IsGranted = request.IsGranted;
                    existingAssignment.ExpiresAt = request.ExpiresAt;
                    existingAssignment.AssignedBy = assignedBy;
                    existingAssignment.AssignedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new assignment
                    var rolePermission = new RolePermission
                    {
                        RoleId = request.RoleId,
                        PermissionId = request.PermissionId,
                        IsGranted = request.IsGranted,
                        ExpiresAt = request.ExpiresAt,
                        AssignedBy = assignedBy,
                        AssignedAt = DateTime.UtcNow
                    };

                    _context.RolePermissions.Add(rolePermission);
                }

                await _context.SaveChangesAsync();

                var result = await GetRolePermissionResponseAsync(request.RoleId, request.PermissionId);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to create role permission assignment");
                }

                _logger.LogInformation($"Role permission assigned: Role {request.RoleId}, Permission {request.PermissionId} by user {assignedBy}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning role permission: Role {request.RoleId}, Permission {request.PermissionId}");
                throw;
            }
        }

        public async Task<bool> RemoveRolePermissionAsync(string roleId, string permissionId, string removedBy)
        {
            try
            {
                var rolePermission = await _context.RolePermissions
                    .FirstOrDefaultAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

                if (rolePermission == null)
                {
                    return false;
                }

                _context.RolePermissions.Remove(rolePermission);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Role permission removed: Role {roleId}, Permission {permissionId} by user {removedBy}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing role permission: Role {roleId}, Permission {permissionId}");
                throw;
            }
        }

        public async Task<RolePermissionListResponse> GetRolePermissionsAsync(string roleId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.RolePermissions
                    .Include(rp => rp.Role)
                    .Include(rp => rp.Permission)
                    .Where(rp => rp.RoleId == roleId);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var rolePermissions = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(rp => new RolePermissionResponse
                    {
                        Id = rp.Id,
                        RoleId = rp.RoleId,
                        RoleName = rp.Role != null ? (rp.Role.Name ?? string.Empty) : string.Empty,
                        PermissionId = rp.PermissionId,
                        PermissionName = rp.Permission != null ? (rp.Permission.Name ?? string.Empty) : string.Empty,
                        PermissionCode = rp.Permission != null ? (rp.Permission.Code ?? string.Empty) : string.Empty,
                        Module = rp.Permission != null ? (rp.Permission.Module ?? string.Empty) : string.Empty,
                        Action = rp.Permission != null ? (rp.Permission.Action ?? string.Empty) : string.Empty,
                        Resource = rp.Permission != null ? (rp.Permission.Resource ?? string.Empty) : string.Empty,
                        IsGranted = rp.IsGranted,
                        AssignedAt = rp.AssignedAt,
                        AssignedBy = rp.AssignedBy,
                        ExpiresAt = rp.ExpiresAt
                    })
                    .ToListAsync();

                return new RolePermissionListResponse
                {
                    RolePermissions = rolePermissions,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving role permissions: {roleId}");
                throw;
            }
        }

        public async Task<List<PermissionResponse>> GetRolePermissionsListAsync(string roleId)
        {
            try
            {
                var permissions = await _context.RolePermissions
                    .Include(rp => rp.Permission)
                    .Where(rp => rp.RoleId == roleId && rp.IsGranted)
                    .Select(rp => MapToPermissionResponse(rp.Permission))
                    .ToListAsync();

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving role permissions list: {roleId}");
                throw;
            }
        }

        public async Task<bool> BulkAssignRolePermissionsAsync(BulkAssignRolePermissionRequest request, string assignedBy)
        {
            try
            {
                var rolePermissions = new List<RolePermission>();

                foreach (var permissionId in request.PermissionIds)
                {
                    // Check if assignment already exists
                    var existingAssignment = await _context.RolePermissions
                        .FirstOrDefaultAsync(rp => rp.RoleId == request.RoleId && rp.PermissionId == permissionId);

                    if (existingAssignment != null)
                    {
                        // Update existing assignment
                        existingAssignment.IsGranted = request.IsGranted;
                        existingAssignment.ExpiresAt = request.ExpiresAt;
                        existingAssignment.AssignedBy = assignedBy;
                        existingAssignment.AssignedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create new assignment
                        rolePermissions.Add(new RolePermission
                        {
                            RoleId = request.RoleId,
                            PermissionId = permissionId,
                            IsGranted = request.IsGranted,
                            ExpiresAt = request.ExpiresAt,
                            AssignedBy = assignedBy,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }

                if (rolePermissions.Any())
                {
                    _context.RolePermissions.AddRange(rolePermissions);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Bulk role permissions assigned: Role {request.RoleId}, {request.PermissionIds.Count} permissions by user {assignedBy}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk assigning role permissions: Role {request.RoleId}");
                throw;
            }
        }

        public async Task<bool> BulkRemoveRolePermissionsAsync(string roleId, List<string> permissionIds, string removedBy)
        {
            try
            {
                var rolePermissions = await _context.RolePermissions
                    .Where(rp => rp.RoleId == roleId && permissionIds.Contains(rp.PermissionId))
                    .ToListAsync();

                if (rolePermissions.Any())
                {
                    _context.RolePermissions.RemoveRange(rolePermissions);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"Bulk role permissions removed: Role {roleId}, {permissionIds.Count} permissions by user {removedBy}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk removing role permissions: Role {roleId}");
                throw;
            }
        }

        #endregion

        #region User Permission Management

        public async Task<UserPermissionResponse> AssignUserPermissionAsync(AssignUserPermissionRequest request, string assignedBy)
        {
            try
            {
                // Check if assignment already exists
                var existingAssignment = await _context.UserPermissions
                    .FirstOrDefaultAsync(up => up.UserId == request.UserId && up.PermissionId == request.PermissionId);

                if (existingAssignment != null)
                {
                    // Update existing assignment
                    existingAssignment.IsGranted = request.IsGranted;
                    existingAssignment.ExpiresAt = request.ExpiresAt;
                    existingAssignment.Reason = request.Reason;
                    existingAssignment.AssignedBy = assignedBy;
                    existingAssignment.AssignedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new assignment
                    var userPermission = new UserPermission
                    {
                        UserId = request.UserId,
                        PermissionId = request.PermissionId,
                        IsGranted = request.IsGranted,
                        ExpiresAt = request.ExpiresAt,
                        Reason = request.Reason,
                        AssignedBy = assignedBy,
                        AssignedAt = DateTime.UtcNow
                    };

                    _context.UserPermissions.Add(userPermission);
                }

                await _context.SaveChangesAsync();

                var result = await GetUserPermissionResponseAsync(request.UserId, request.PermissionId);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to create user permission assignment");
                }

                _logger.LogInformation($"User permission assigned: User {request.UserId}, Permission {request.PermissionId} by user {assignedBy}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning user permission: User {request.UserId}, Permission {request.PermissionId}");
                throw;
            }
        }

        public async Task<bool> RemoveUserPermissionAsync(string userId, string permissionId, string removedBy)
        {
            try
            {
                var userPermission = await _context.UserPermissions
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

                if (userPermission == null)
                {
                    return false;
                }

                _context.UserPermissions.Remove(userPermission);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User permission removed: User {userId}, Permission {permissionId} by user {removedBy}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user permission: User {userId}, Permission {permissionId}");
                throw;
            }
        }

        public async Task<UserPermissionListResponse> GetUserPermissionsAsync(string userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var query = _context.UserPermissions
                    .Include(up => up.User)
                    .Include(up => up.Permission)
                    .Where(up => up.UserId == userId);

                var totalCount = await query.CountAsync();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

                var userPermissions = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Select(up => new UserPermissionResponse
                    {
                        Id = up.Id,
                        UserId = up.UserId,
                        UserName = up.User.FirstName + " " + up.User.LastName,
                        UserEmail = up.User.Email!,
                        PermissionId = up.PermissionId,
                        PermissionName = up.Permission.Name,
                        PermissionCode = up.Permission.Code,
                        Module = up.Permission.Module,
                        Action = up.Permission.Action,
                        Resource = up.Permission.Resource,
                        IsGranted = up.IsGranted,
                        AssignedAt = up.AssignedAt,
                        AssignedBy = up.AssignedBy,
                        ExpiresAt = up.ExpiresAt,
                        Reason = up.Reason
                    })
                    .ToListAsync();

                return new UserPermissionListResponse
                {
                    UserPermissions = userPermissions,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user permissions: {userId}");
                throw;
            }
        }

        public async Task<List<PermissionResponse>> GetUserPermissionsListAsync(string userId)
        {
            try
            {
                var permissions = await _context.UserPermissions
                    .Include(up => up.Permission)
                    .Where(up => up.UserId == userId && up.IsGranted)
                    .Select(up => MapToPermissionResponse(up.Permission))
                    .ToListAsync();

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user permissions list: {userId}");
                throw;
            }
        }

        public async Task<bool> BulkAssignUserPermissionsAsync(BulkAssignUserPermissionRequest request, string assignedBy)
        {
            try
            {
                var userPermissions = new List<UserPermission>();

                foreach (var permissionId in request.PermissionIds)
                {
                    // Check if assignment already exists
                    var existingAssignment = await _context.UserPermissions
                        .FirstOrDefaultAsync(up => up.UserId == request.UserId && up.PermissionId == permissionId);

                    if (existingAssignment != null)
                    {
                        // Update existing assignment
                        existingAssignment.IsGranted = request.IsGranted;
                        existingAssignment.ExpiresAt = request.ExpiresAt;
                        existingAssignment.Reason = request.Reason;
                        existingAssignment.AssignedBy = assignedBy;
                        existingAssignment.AssignedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create new assignment
                        userPermissions.Add(new UserPermission
                        {
                            UserId = request.UserId,
                            PermissionId = permissionId,
                            IsGranted = request.IsGranted,
                            ExpiresAt = request.ExpiresAt,
                            Reason = request.Reason,
                            AssignedBy = assignedBy,
                            AssignedAt = DateTime.UtcNow
                        });
                    }
                }

                if (userPermissions.Any())
                {
                    _context.UserPermissions.AddRange(userPermissions);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Bulk user permissions assigned: User {request.UserId}, {request.PermissionIds.Count} permissions by user {assignedBy}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk assigning user permissions: User {request.UserId}");
                throw;
            }
        }

        public async Task<bool> BulkRemoveUserPermissionsAsync(string userId, List<string> permissionIds, string removedBy)
        {
            try
            {
                var userPermissions = await _context.UserPermissions
                    .Where(up => up.UserId == userId && permissionIds.Contains(up.PermissionId))
                    .ToListAsync();

                if (userPermissions.Any())
                {
                    _context.UserPermissions.RemoveRange(userPermissions);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation($"Bulk user permissions removed: User {userId}, {permissionIds.Count} permissions by user {removedBy}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error bulk removing user permissions: User {userId}");
                throw;
            }
        }

        #endregion

        #region User Role Management

        public async Task<UserRoleResponse> AssignUserRoleAsync(AssignUserRoleRequest request, string assignedBy)
        {
            try
            {
                // Check if assignment already exists
                var existingAssignment = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId);

                if (existingAssignment != null)
                {
                    // Update existing assignment
                    existingAssignment.ExpiresAt = request.ExpiresAt;
                    existingAssignment.AssignedBy = assignedBy;
                    existingAssignment.AssignedAt = DateTime.UtcNow;
                }
                else
                {
                    // Create new assignment
                    var userRole = new UserRole
                    {
                        UserId = request.UserId,
                        RoleId = request.RoleId,
                        ExpiresAt = request.ExpiresAt,
                        AssignedBy = assignedBy,
                        AssignedAt = DateTime.UtcNow
                    };

                    _context.UserRoles.Add(userRole);
                }

                await _context.SaveChangesAsync();

                var result = await GetUserRoleResponseAsync(request.UserId, request.RoleId);
                if (result == null)
                {
                    throw new InvalidOperationException("Failed to create user role assignment");
                }

                _logger.LogInformation($"User role assigned: User {request.UserId}, Role {request.RoleId} by user {assignedBy}");

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error assigning user role: User {request.UserId}, Role {request.RoleId}");
                throw;
            }
        }

        public async Task<bool> RemoveUserRoleAsync(string userId, string roleId, string removedBy)
        {
            try
            {
                var userRole = await _context.UserRoles
                    .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

                if (userRole == null)
                {
                    return false;
                }

                _context.UserRoles.Remove(userRole);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User role removed: User {userId}, Role {roleId} by user {removedBy}");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing user role: User {userId}, Role {roleId}");
                throw;
            }
        }

        public async Task<List<UserRoleResponse>> GetUserRolesAsync(string userId)
        {
            try
            {
                var userRoles = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .Where(ur => ur.UserId == userId && ur.IsActive)
                    .Select(ur => new UserRoleResponse
                    {
                        Id = ur.Id,
                        UserId = ur.UserId,
                        UserName = (ur.User != null ? (ur.User.FirstName ?? string.Empty) : string.Empty) + " " + (ur.User != null ? (ur.User.LastName ?? string.Empty) : string.Empty),
                        UserEmail = ur.User != null ? (ur.User.Email ?? string.Empty) : string.Empty,
                        RoleId = ur.RoleId,
                        RoleName = ur.Role != null ? (ur.Role.Name ?? string.Empty) : string.Empty,
                        IsActive = ur.IsActive,
                        AssignedAt = ur.AssignedAt,
                        AssignedBy = ur.AssignedBy,
                        ExpiresAt = ur.ExpiresAt
                    })
                    .ToListAsync();

                return userRoles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user roles: {userId}");
                throw;
            }
        }

        public async Task<List<UserRoleResponse>> GetRoleUsersAsync(string roleId)
        {
            try
            {
                var roleUsers = await _context.UserRoles
                    .Include(ur => ur.User)
                    .Include(ur => ur.Role)
                    .Where(ur => ur.RoleId == roleId && ur.IsActive)
                    .Select(ur => new UserRoleResponse
                    {
                        Id = ur.Id,
                        UserId = ur.UserId,
                        UserName = (ur.User != null ? (ur.User.FirstName ?? string.Empty) : string.Empty) + " " + (ur.User != null ? (ur.User.LastName ?? string.Empty) : string.Empty),
                        UserEmail = ur.User != null ? (ur.User.Email ?? string.Empty) : string.Empty,
                        RoleId = ur.RoleId,
                        RoleName = ur.Role != null ? (ur.Role.Name ?? string.Empty) : string.Empty,
                        IsActive = ur.IsActive,
                        AssignedAt = ur.AssignedAt,
                        AssignedBy = ur.AssignedBy,
                        ExpiresAt = ur.ExpiresAt
                    })
                    .ToListAsync();

                return roleUsers;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving role users: {roleId}");
                throw;
            }
        }

        #endregion

        #region Permission Checking

        public async Task<CheckPermissionResponse> CheckUserPermissionAsync(CheckPermissionRequest request)
        {
            try
            {
                // Check direct user permissions first
                var userPermission = await _context.UserPermissions
                    .Include(up => up.Permission)
                    .FirstOrDefaultAsync(up => up.UserId == request.UserId && 
                                             up.Permission.Code == request.PermissionCode &&
                                             up.IsGranted &&
                                             (up.ExpiresAt == null || up.ExpiresAt > DateTime.UtcNow));

                if (userPermission != null)
                {
                    return new CheckPermissionResponse
                    {
                        HasPermission = true,
                        PermissionCode = request.PermissionCode,
                        Resource = request.Resource,
                        Source = "User",
                        Reason = userPermission.Reason,
                        ExpiresAt = userPermission.ExpiresAt
                    };
                }

                // Check role permissions
                var rolePermission = await _context.UserRoles
                    .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                    .Where(ur => ur.UserId == request.UserId && 
                               ur.IsActive &&
                               (ur.ExpiresAt == null || ur.ExpiresAt > DateTime.UtcNow))
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .FirstOrDefaultAsync(rp => rp.Permission.Code == request.PermissionCode &&
                                             rp.IsGranted &&
                                             (rp.ExpiresAt == null || rp.ExpiresAt > DateTime.UtcNow));

                if (rolePermission != null)
                {
                    return new CheckPermissionResponse
                    {
                        HasPermission = true,
                        PermissionCode = request.PermissionCode,
                        Resource = request.Resource,
                        Source = "Role",
                        ExpiresAt = rolePermission.ExpiresAt
                    };
                }

                return new CheckPermissionResponse
                {
                    HasPermission = false,
                    PermissionCode = request.PermissionCode,
                    Resource = request.Resource
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking user permission: User {request.UserId}, Permission {request.PermissionCode}");
                throw;
            }
        }

        public async Task<bool> HasPermissionAsync(string userId, string permissionCode, string? resource = null)
        {
            try
            {
                var request = new CheckPermissionRequest
                {
                    UserId = userId,
                    PermissionCode = permissionCode,
                    Resource = resource
                };

                var result = await CheckUserPermissionAsync(request);
                return result.HasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking permission: User {userId}, Permission {permissionCode}");
                return false;
            }
        }

        public async Task<UserPermissionsSummaryResponse> GetUserPermissionsSummaryAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    throw new KeyNotFoundException($"User with ID '{userId}' not found");
                }

                // Get user roles
                var userRoles = await GetUserRolesAsync(userId);
                var roleNames = userRoles.Select(ur => ur.RoleName).ToList();

                // Get direct user permissions
                var directPermissions = await GetUserPermissionsListAsync(userId);

                // Get role permissions
                var rolePermissions = new List<PermissionResponse>();
                foreach (var userRole in userRoles)
                {
                    var rolePerms = await GetRolePermissionsListAsync(userRole.RoleId);
                    rolePermissions.AddRange(rolePerms);
                }

                // Combine all permissions
                var allPermissions = directPermissions.Concat(rolePermissions)
                    .GroupBy(p => p.Id)
                    .Select(g => g.First())
                    .ToList();

                // Group by module
                var permissionsByModule = allPermissions
                    .GroupBy(p => p.Module)
                    .ToDictionary(g => g.Key, g => g.ToList());

                return new UserPermissionsSummaryResponse
                {
                    UserId = userId,
                    UserName = user.FirstName + " " + user.LastName,
                    UserEmail = user.Email!,
                    Roles = roleNames,
                    DirectPermissions = directPermissions,
                    RolePermissions = rolePermissions,
                    AllPermissions = allPermissions,
                    PermissionsByModule = permissionsByModule
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user permissions summary: {userId}");
                throw;
            }
        }

        public async Task<List<PermissionResponse>> GetUserEffectivePermissionsAsync(string userId)
        {
            try
            {
                var summary = await GetUserPermissionsSummaryAsync(userId);
                return summary.AllPermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user effective permissions: {userId}");
                throw;
            }
        }

        #endregion

        #region Helper Methods

        private static PermissionResponse MapToPermissionResponse(Permission permission)
        {
            return new PermissionResponse
            {
                Id = permission.Id,
                Name = permission.Name,
                Code = permission.Code,
                Description = permission.Description,
                Module = permission.Module,
                Action = permission.Action,
                Resource = permission.Resource,
                IsActive = permission.IsActive,
                CreatedAt = permission.CreatedAt,
                UpdatedAt = permission.UpdatedAt,
                CreatedBy = permission.CreatedBy,
                UpdatedBy = permission.UpdatedBy
            };
        }

        private async Task<RolePermissionResponse?> GetRolePermissionResponseAsync(string roleId, string permissionId)
        {
            return await _context.RolePermissions
                .Include(rp => rp.Role)
                .Include(rp => rp.Permission)
                .Where(rp => rp.RoleId == roleId && rp.PermissionId == permissionId)
                .Select(rp => new RolePermissionResponse
                {
                    Id = rp.Id,
                    RoleId = rp.RoleId,
                    RoleName = rp.Role != null ? (rp.Role.Name ?? string.Empty) : string.Empty,
                    PermissionId = rp.PermissionId,
                    PermissionName = rp.Permission != null ? (rp.Permission.Name ?? string.Empty) : string.Empty,
                    PermissionCode = rp.Permission != null ? (rp.Permission.Code ?? string.Empty) : string.Empty,
                    Module = rp.Permission != null ? (rp.Permission.Module ?? string.Empty) : string.Empty,
                    Action = rp.Permission != null ? (rp.Permission.Action ?? string.Empty) : string.Empty,
                    Resource = rp.Permission != null ? (rp.Permission.Resource ?? string.Empty) : string.Empty,
                    IsGranted = rp.IsGranted,
                    AssignedAt = rp.AssignedAt,
                    AssignedBy = rp.AssignedBy,
                    ExpiresAt = rp.ExpiresAt
                })
                .FirstOrDefaultAsync();
        }

        private async Task<UserPermissionResponse?> GetUserPermissionResponseAsync(string userId, string permissionId)
        {
            return await _context.UserPermissions
                .Include(up => up.User)
                .Include(up => up.Permission)
                .Where(up => up.UserId == userId && up.PermissionId == permissionId)
                .Select(up => new UserPermissionResponse
                {
                    Id = up.Id,
                    UserId = up.UserId,
                    UserName = up.User.FirstName + " " + up.User.LastName,
                    UserEmail = up.User.Email!,
                    PermissionId = up.PermissionId,
                    PermissionName = up.Permission.Name,
                    PermissionCode = up.Permission.Code,
                    Module = up.Permission.Module,
                    Action = up.Permission.Action,
                    Resource = up.Permission.Resource,
                    IsGranted = up.IsGranted,
                    AssignedAt = up.AssignedAt,
                    AssignedBy = up.AssignedBy,
                    ExpiresAt = up.ExpiresAt,
                    Reason = up.Reason
                })
                .FirstOrDefaultAsync();
        }

        private async Task<UserRoleResponse?> GetUserRoleResponseAsync(string userId, string roleId)
        {
            return await _context.UserRoles
                .Include(ur => ur.User)
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == userId && ur.RoleId == roleId)
                .Select(ur => new UserRoleResponse
                {
                    Id = ur.Id,
                    UserId = ur.UserId,
                    UserName = (ur.User != null ? (ur.User.FirstName ?? string.Empty) : string.Empty) + " " + (ur.User != null ? (ur.User.LastName ?? string.Empty) : string.Empty),
                    UserEmail = ur.User != null ? (ur.User.Email ?? string.Empty) : string.Empty,
                    RoleId = ur.RoleId,
                    RoleName = ur.Role != null ? (ur.Role.Name ?? string.Empty) : string.Empty,
                    IsActive = ur.IsActive,
                    AssignedAt = ur.AssignedAt,
                    AssignedBy = ur.AssignedBy,
                    ExpiresAt = ur.ExpiresAt
                })
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Placeholder Methods (To be implemented)

        public Task<PermissionGroupResponse> CreatePermissionGroupAsync(CreatePermissionGroupRequest request, string userId)
        {
            // Implementation for permission groups
            throw new NotImplementedException("Permission groups feature not yet implemented");
        }

        public Task<PermissionGroupResponse?> GetPermissionGroupByIdAsync(string groupId)
        {
            throw new NotImplementedException("Permission groups feature not yet implemented");
        }

        public Task<List<PermissionGroupResponse>> GetPermissionGroupsAsync()
        {
            throw new NotImplementedException("Permission groups feature not yet implemented");
        }

        public Task<PermissionGroupResponse> UpdatePermissionGroupAsync(string groupId, CreatePermissionGroupRequest request, string userId)
        {
            throw new NotImplementedException("Permission groups feature not yet implemented");
        }

        public Task<bool> DeletePermissionGroupAsync(string groupId, string userId)
        {
            throw new NotImplementedException("Permission groups feature not yet implemented");
        }

        public Task<PermissionStatisticsResponse> GetPermissionStatisticsAsync()
        {
            throw new NotImplementedException("Statistics feature not yet implemented");
        }

        public Task<List<string>> GetAvailableModulesAsync()
        {
            throw new NotImplementedException("Modules feature not yet implemented");
        }

        public Task<List<string>> GetAvailableActionsAsync()
        {
            throw new NotImplementedException("Actions feature not yet implemented");
        }

        public Task<List<string>> GetAvailableResourcesAsync()
        {
            throw new NotImplementedException("Resources feature not yet implemented");
        }

        public Task<bool> SyncRolePermissionsAsync(string roleId, List<string> permissionIds, string syncedBy)
        {
            throw new NotImplementedException("Sync feature not yet implemented");
        }

        public Task<bool> SyncUserPermissionsAsync(string userId, List<string> permissionIds, string syncedBy)
        {
            throw new NotImplementedException("Sync feature not yet implemented");
        }

        public Task<bool> CopyRolePermissionsAsync(string sourceRoleId, string targetRoleId, string copiedBy)
        {
            throw new NotImplementedException("Copy feature not yet implemented");
        }

        public Task<bool> CopyUserPermissionsAsync(string sourceUserId, string targetUserId, string copiedBy)
        {
            throw new NotImplementedException("Copy feature not yet implemented");
        }

        public Task<bool> CleanupExpiredPermissionsAsync()
        {
            throw new NotImplementedException("Cleanup feature not yet implemented");
        }

        public Task<bool> ValidatePermissionIntegrityAsync()
        {
            throw new NotImplementedException("Validation feature not yet implemented");
        }

        public Task<List<string>> GetOrphanedPermissionsAsync()
        {
            throw new NotImplementedException("Orphaned permissions feature not yet implemented");
        }

        #endregion
    }
}
