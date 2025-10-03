import api from '../axios'

// CreateUserPayload moved to auth.ts as RegisterPayload

export interface UpdateUserPayload {
  email?: string
  firstName?: string
  lastName?: string
  department?: string
  position?: string
  companyId?: number
}

export interface User {
  id: string
  email: string
  firstName: string
  lastName: string
  department: string
  position: string
  companyId: number
  companyName: string
  isActive: boolean
  createdAt: string
  updatedAt: string
  roles: string[]
}

export interface UserResponse {
  success: boolean
  message: string
  data: {
    token: string
    refreshToken: string
    expiration: string
    user: User
  }
  errors: string[]
}

export interface Pagination {
  currentPage: number
  pageSize: number
  totalItems: number
  totalPages: number
  hasNextPage: boolean
  hasPreviousPage: boolean
}

export interface UsersResponse {
  success: boolean
  message: string
  data: {
    users: User[]
    pagination: Pagination
  }
  errors: string[]
}

export interface SingleUserResponse {
  success: boolean
  message: string
  data: User
  errors: string[]
}

export interface DeleteUserResponse {
  success: boolean
  message: string
  errors: string[]
}

// Note: User creation is now handled by the auth register endpoint
// Use register() from @/lib/api/auth instead

// Get all users
export const getAllUsers = async (): Promise<UsersResponse> => {
  const response = await api.get<UsersResponse>('/User')
  return response.data
}

// Get user by ID
export const getUserById = async (id: string): Promise<SingleUserResponse> => {
  const response = await api.get<SingleUserResponse>(`/User/${id}`)
  return response.data
}

// Update user
export const updateUser = async (id: string, payload: UpdateUserPayload): Promise<SingleUserResponse> => {
  const response = await api.put<SingleUserResponse>(`/User/${id}`, payload)
  return response.data
}

// Delete user
export const deleteUser = async (id: string): Promise<DeleteUserResponse> => {
  const response = await api.delete<DeleteUserResponse>(`/User/${id}`)
  return response.data
}

// Get users by company
export const getUsersByCompany = async (companyId: number): Promise<UsersResponse> => {
  const response = await api.get<UsersResponse>(`/User/company/${companyId}`)
  return response.data
}

// Get users by department
export const getUsersByDepartment = async (department: string): Promise<UsersResponse> => {
  const response = await api.get<UsersResponse>(`/User/department/${department}`)
  return response.data
}

// Update user status
export const updateUserStatus = async (id: string, isActive: boolean): Promise<SingleUserResponse> => {
  const response = await api.put<SingleUserResponse>(`/User/${id}/status`, { isActive })
  return response.data
}

// Update user roles
export interface UpdateUserRolesPayload {
  roles: string[]
}

export const updateUserRoles = async (id: string, roles: string[]): Promise<SingleUserResponse> => {
  const response = await api.put<SingleUserResponse>(`/User/${id}/roles`, { roles })
  return response.data
}

// Permanently delete a user
export const permanentlyDeleteUser = async (id: string): Promise<DeleteUserResponse> => {
  const response = await api.delete<DeleteUserResponse>(`/User/${id}/permanent`)
  return response.data
}

// Get user statistics
export interface UserStatistics {
  totalUsers: number
  activeUsers: number
  inactiveUsers: number
  usersByCompany: Record<string, number>
  usersByDepartment: Record<string, number>
  averageUsersPerCompany: number
  recentRegistrations: number
}

export interface UserStatisticsResponse {
  success: boolean
  message: string
  data: UserStatistics
  errors: string[]
}

export const getUserStatistics = async (): Promise<UserStatisticsResponse> => {
  const response = await api.get<UserStatisticsResponse>('/User/statistics')
  return response.data
}

// Get available roles
export interface Role {
  id: string
  name: string
  description: string
  userCount: number
  activeUserCount: number
}

export interface RolesResponse {
  success: boolean
  message: string
  data: Role[]
  errors: string[]
}

export const getAvailableRoles = async (): Promise<RolesResponse> => {
  const response = await api.get<RolesResponse>('/User/roles')
  return response.data
}
