import api from '../axios'

// Types for user permissions
export interface Permission {
  id: number
  code: string
  name: string
  description?: string
  module?: string
  isActive: boolean
  createdAt?: string
  updatedAt?: string
}

export interface UserPermission {
  id: number
  userId: number
  permissionId: number
  permission: Permission
  assignedAt: string
  assignedBy?: number
  isActive: boolean
}

export interface UserPermissionSummary {
  userId: number
  userName: string
  userEmail: string
  directPermissions: Permission[]
  rolePermissions: Permission[]
  effectivePermissions: Permission[]
  totalPermissions: number
  lastUpdated: string
}

export interface PermissionAssignmentRequest {
  userId: number
  permissionId: number
  assignedBy?: number
}

export interface BulkPermissionAssignmentRequest {
  userId: number
  permissionIds: number[]
  assignedBy?: number
}

export interface PermissionCheckResponse {
  hasPermission: boolean
  permissionCode: string
  userId: number
  source: 'direct' | 'role' | 'none'
}

export interface UserPermissionListResponse {
  success: boolean
  message: string
  data: UserPermission[]
  errors: string[]
}

export interface UserPermissionSummaryResponse {
  success: boolean
  message: string
  data: UserPermissionSummary
  errors: string[]
}

export interface PermissionCheckResponseWrapper {
  success: boolean
  message: string
  data: PermissionCheckResponse
  errors: string[]
}

export interface MyPermissionsResponse {
  success: boolean
  message: string
  data: {
    directPermissions: Permission[]
    rolePermissions: Permission[]
    effectivePermissions: Permission[]
  }
  errors: string[]
}

// API Functions
export const assignPermissionToUser = async (request: PermissionAssignmentRequest): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post('/UserPermission/assign', request)
  return response.data
}

export const removePermissionFromUser = async (userId: number, permissionId: number): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.delete(`/UserPermission/${userId}/permission/${permissionId}`)
  return response.data
}

export const getUserPermissions = async (userId: number, params?: {
  page?: number
  pageSize?: number
  search?: string
}): Promise<UserPermissionListResponse> => {
  const queryParams = new URLSearchParams()
  if (params?.page) queryParams.append('page', params.page.toString())
  if (params?.pageSize) queryParams.append('pageSize', params.pageSize.toString())
  if (params?.search) queryParams.append('search', params.search)
  
  const url = `/UserPermission/${userId}${queryParams.toString() ? `?${queryParams.toString()}` : ''}`
  const response = await api.get(url)
  return response.data
}

export const getUserPermissionsList = async (userId: number): Promise<UserPermissionListResponse> => {
  const response = await api.get(`/UserPermission/${userId}/list`)
  return response.data
}

export const bulkAssignPermissions = async (request: BulkPermissionAssignmentRequest): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post('/UserPermission/bulk-assign', request)
  return response.data
}

export const bulkRemovePermissions = async (userId: number, permissionIds: number[]): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post(`/UserPermission/${userId}/bulk-remove`, { permissionIds })
  return response.data
}

export const getUserPermissionSummary = async (userId: number): Promise<UserPermissionSummaryResponse> => {
  const response = await api.get(`/UserPermission/${userId}/summary`)
  return response.data
}

export const getUserEffectivePermissions = async (userId: number): Promise<UserPermissionListResponse> => {
  const response = await api.get(`/UserPermission/${userId}/effective`)
  return response.data
}

export const checkUserPermission = async (userId: number, permissionCode: string): Promise<PermissionCheckResponseWrapper> => {
  const response = await api.get(`/UserPermission/${userId}/check/${permissionCode}`)
  return response.data
}

export const syncUserPermissions = async (userId: number, permissionIds: number[]): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post(`/UserPermission/${userId}/sync`, { permissionIds })
  return response.data
}

export const copyUserPermissions = async (sourceUserId: number, targetUserId: number): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post(`/UserPermission/copy/${sourceUserId}/${targetUserId}`)
  return response.data
}

export const getMyPermissions = async (): Promise<MyPermissionsResponse> => {
  const response = await api.get('/UserPermission/my-permissions')
  return response.data
}

export const checkMyPermission = async (permissionCode: string): Promise<PermissionCheckResponseWrapper> => {
  const response = await api.get(`/UserPermission/my-permissions/check/${permissionCode}`)
  return response.data
}

export const getMyEffectivePermissions = async (): Promise<UserPermissionListResponse> => {
  const response = await api.get('/UserPermission/my-permissions/effective')
  return response.data
}
