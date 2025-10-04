import api from '../axios'

// Types for Permission management
export interface Permission {
  id: number
  code: string
  name: string
  description?: string
  module: string
  action: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface PermissionCreateRequest {
  code: string
  name: string
  description?: string
  module: string
  action: string
  isActive?: boolean
}

export interface PermissionUpdateRequest {
  code?: string
  name?: string
  description?: string
  module?: string
  action?: string
  isActive?: boolean
}

export interface PermissionCheckRequest {
  userId: number
  permissionCode: string
}

export interface PermissionCheckResponse {
  hasPermission: boolean
  permissionCode: string
  userId: number
  checkedAt: string
}

export interface UserPermissionSummary {
  userId: number
  userName: string
  directPermissions: Permission[]
  rolePermissions: Permission[]
  effectivePermissions: Permission[]
  totalPermissions: number
}

export interface UserEffectivePermissions {
  userId: number
  userName: string
  permissions: Permission[]
  roles: string[]
  lastUpdated: string
}

export interface PermissionsResponse {
  data: Permission[]
  pagination: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

// Types for Role management
export interface Role {
  id: number
  name: string
  description?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface RolePermissionAssignment {
  roleId: number
  permissionId: number
  assignedAt: string
  assignedBy: number
}

export interface RolePermissionBulkRequest {
  roleId: number
  permissionIds: number[]
}

export interface UserRoleAssignment {
  userId: number
  roleId: number
  assignedAt: string
  assignedBy: number
}

export interface RoleUsersResponse {
  roleId: number
  roleName: string
  users: Array<{
    id: number
    firstName: string
    lastName: string
    email: string
    assignedAt: string
  }>
}

export interface UserRolesResponse {
  userId: number
  userName: string
  roles: Array<{
    id: number
    name: string
    description?: string
    assignedAt: string
  }>
}

export interface RolePermissionSyncRequest {
  roleId: number
  permissionIds: number[]
}

export interface RolePermissionCopyRequest {
  sourceRoleId: number
  targetRoleId: number
}

// Permission API Functions
export const createPermission = async (data: PermissionCreateRequest): Promise<Permission> => {
  const response = await api.post('/Permission', data)
  return response.data
}

export const getPermissions = async (params?: {
  page?: number
  pageSize?: number
  module?: string
  action?: string
  isActive?: boolean
}): Promise<PermissionsResponse> => {
  const response = await api.get('/Permission', { params })
  return response.data
}

export const getPermissionById = async (id: number): Promise<Permission> => {
  const response = await api.get(`/Permission/${id}`)
  return response.data
}

export const updatePermission = async (id: number, data: PermissionUpdateRequest): Promise<Permission> => {
  const response = await api.put(`/Permission/${id}`, data)
  return response.data
}

export const deletePermission = async (id: number): Promise<void> => {
  await api.delete(`/Permission/${id}`)
}

export const getPermissionByCode = async (code: string): Promise<Permission> => {
  const response = await api.get(`/Permission/by-code/${code}`)
  return response.data
}

export const getPermissionsByModule = async (module: string): Promise<Permission[]> => {
  const response = await api.get(`/Permission/by-module/${module}`)
  return response.data
}

export const getPermissionsByAction = async (action: string): Promise<Permission[]> => {
  const response = await api.get(`/Permission/by-action/${action}`)
  return response.data
}

export const checkPermission = async (data: PermissionCheckRequest): Promise<PermissionCheckResponse> => {
  const response = await api.post('/Permission/check', data)
  return response.data
}

export const getUserPermissionSummary = async (userId: number): Promise<UserPermissionSummary> => {
  const response = await api.get(`/Permission/user/${userId}/summary`)
  return response.data
}

export const getUserEffectivePermissions = async (userId: number): Promise<UserEffectivePermissions> => {
  const response = await api.get(`/Permission/user/${userId}/effective`)
  return response.data
}

// Role Permission API Functions
export const assignPermissionToRole = async (data: RolePermissionAssignment): Promise<void> => {
  await api.post('/RolePermission/assign', data)
}

export const removePermissionFromRole = async (roleId: number, permissionId: number): Promise<void> => {
  await api.delete(`/RolePermission/${roleId}/permission/${permissionId}`)
}

export const getRolePermissions = async (roleId: number, params?: {
  page?: number
  pageSize?: number
}): Promise<PermissionsResponse> => {
  const response = await api.get(`/RolePermission/${roleId}`, { params })
  return response.data
}

export const getRolePermissionsList = async (roleId: number): Promise<Permission[]> => {
  const response = await api.get(`/RolePermission/${roleId}/list`)
  return response.data
}

export const bulkAssignPermissionsToRole = async (data: RolePermissionBulkRequest): Promise<void> => {
  await api.post('/RolePermission/bulk-assign', data)
}

export const bulkRemovePermissionsFromRole = async (roleId: number, permissionIds: number[]): Promise<void> => {
  await api.post(`/RolePermission/${roleId}/bulk-remove`, { permissionIds })
}

export const getRoleUsers = async (roleId: number): Promise<RoleUsersResponse> => {
  const response = await api.get(`/RolePermission/${roleId}/users`)
  return response.data
}

export const assignUserToRole = async (data: UserRoleAssignment): Promise<void> => {
  await api.post('/RolePermission/user-role/assign', data)
}

export const removeUserFromRole = async (userId: number, roleId: number): Promise<void> => {
  await api.delete(`/RolePermission/user-role/${userId}/${roleId}`)
}

export const getUserRoles = async (userId: number): Promise<UserRolesResponse> => {
  const response = await api.get(`/RolePermission/user/${userId}/roles`)
  return response.data
}

export const syncRolePermissions = async (data: RolePermissionSyncRequest): Promise<void> => {
  await api.post('/RolePermission/sync', data)
}

export const copyRolePermissions = async (sourceRoleId: number, targetRoleId: number): Promise<void> => {
  await api.post(`/RolePermission/copy/${sourceRoleId}/${targetRoleId}`)
}
