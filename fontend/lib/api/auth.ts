import api from '../axios'

export interface LoginPayload {
  email: string
  password: string
}

export interface RegisterPayload {
  email: string
  password: string
  confirmPassword: string
  firstName: string
  lastName: string
  department: string
  position: string
  companyId: number
}

export interface User {
  id: number
  email: string
  firstName: string
  lastName: string
  department: string
  position: string
  companyId: number
  companyName: string
  roles: string[]
}

export interface LoginResponse {
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

export interface ValidateTokenRequest {
  token: string
}

export interface ValidateTokenData {
  isValid: boolean
  userId: string
  email: string
  firstName: string
  lastName: string
  roles: string[]
  expirationDate: string
  errorMessage: string
}

export interface ValidateTokenResponse {
  success: boolean
  message: string
  data: ValidateTokenData
  errors: string[]
}

export interface AuthResponse {
  token: string
  refreshToken: string
  expiration: string
  user: User
}

export const login = async (payload: LoginPayload): Promise<LoginResponse> => {
  const response = await api.post<LoginResponse>('/Auth/login', payload)
  return response.data
}

export const register = async (payload: RegisterPayload): Promise<LoginResponse> => {
  const response = await api.post<LoginResponse>('/Auth/register', payload)
  return response.data
}

export const getCurrentUser = async (): Promise<User> => {
  const response = await api.get<{ user: User }>('/Auth/me')
  return response.data.user
}

export const logout = async (): Promise<void> => {
  try {
    // Call revoke-token API to invalidate the token on server
    await api.post('/Auth/revoke-token')
  } catch (error) {
    // Even if API call fails, we should still clear local data
    console.error('Error revoking token:', error)
  } finally {
    // Always clear all stored authentication data
    if (typeof window !== 'undefined') {
      // Clear localStorage
      localStorage.removeItem('token')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
      localStorage.removeItem('tokenExpiration')
      
      // Clear cookies
      document.cookie = 'token=; path=/; expires=Thu, 01 Jan 1970 00:00:01 GMT;'
      
      // Redirect to login page
      window.location.href = '/login'
    }
  }
}

// Utility functions for managing stored user data
export const getStoredUser = (): User | null => {
  if (typeof window === 'undefined') return null
  
  const userStr = localStorage.getItem('user')
  if (!userStr) return null
  
  try {
    return JSON.parse(userStr) as User
  } catch {
    return null
  }
}

export const getStoredToken = (): string | null => {
  if (typeof window === 'undefined') return null
  return localStorage.getItem('token')
}

export const getStoredRefreshToken = (): string | null => {
  if (typeof window === 'undefined') return null
  return localStorage.getItem('refreshToken')
}

export const isTokenExpired = (): boolean => {
  if (typeof window === 'undefined') return true
  
  const expiration = localStorage.getItem('tokenExpiration')
  if (!expiration) return true
  
  return new Date() >= new Date(expiration)
}

export const isAuthenticated = (): boolean => {
  const token = getStoredToken()
  const user = getStoredUser()
  
  return !!(token && user && !isTokenExpired())
}

export const validateToken = async (token: string): Promise<ValidateTokenResponse> => {
  const response = await api.post<ValidateTokenResponse>('/Auth/validate-token', { token })
  return response.data
}
