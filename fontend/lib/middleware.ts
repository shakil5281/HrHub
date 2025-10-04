import { NextRequest } from 'next/server'

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

export interface DecodedToken {
  exp: number
  iat: number
  jti: string
  iss: string
  aud: string
  [key: string]: unknown
}

/**
 * Decode JWT token without verification (for middleware use)
 * Note: This is for reading token data only, not for security validation
 */
export function decodeToken(token: string): DecodedToken | null {
  try {
    const parts = token.split('.')
    if (parts.length !== 3) return null
    
    const payload = parts[1]
    const decoded = JSON.parse(atob(payload.replace(/-/g, '+').replace(/_/g, '/')))
    return decoded
  } catch {
    return null
  }
}

/**
 * Check if token is expired
 */
export function isTokenExpired(token: string): boolean {
  const decoded = decodeToken(token)
  if (!decoded || !decoded.exp) return true
  
  const currentTime = Math.floor(Date.now() / 1000)
  return decoded.exp < currentTime
}

/**
 * Get token from request (cookies or headers)
 */
export function getTokenFromRequest(request: NextRequest): string | null {
  // First try to get from cookies
  const cookieToken = request.cookies.get('token')?.value
  if (cookieToken) return cookieToken
  
  // Then try from Authorization header
  const authHeader = request.headers.get('authorization')
  if (authHeader && authHeader.startsWith('Bearer ')) {
    return authHeader.substring(7)
  }
  
  return null
}

/**
 * Check if user has required role
 */
export function hasRequiredRole(token: string, requiredRoles: string[]): boolean {
  const decoded = decodeToken(token)
  if (!decoded) return false
  
  // Get user roles from token (adjust based on your token structure)
  const userRoles = decoded['http://schemas.microsoft.com/ws/2008/06/identity/claims/role']
  
  if (!userRoles) return false
  
  // Handle single role as string or multiple roles as array
  const roles = Array.isArray(userRoles) ? userRoles : [userRoles]
  
  return requiredRoles.some(role => roles.includes(role))
}

/**
 * Route configuration interface
 */
export interface RouteConfig {
  path: string
  roles?: string[]
  requireAuth?: boolean
}

/**
 * Default route configurations
 */
export const defaultRouteConfigs: RouteConfig[] = [
  { path: '/dashboard', requireAuth: true },
  { path: '/admin', requireAuth: true, roles: ['Admin'] },
  { path: '/hr', requireAuth: true, roles: ['HR', 'HRManager', 'Admin'] },
  { path: '/login', requireAuth: false },
  { path: '/register', requireAuth: false },
  { path: '/', requireAuth: false },
]

/**
 * Check if route matches configuration
 */
export function getRouteConfig(pathname: string, configs: RouteConfig[] = defaultRouteConfigs): RouteConfig | null {
  return configs.find(config => 
    pathname === config.path || pathname.startsWith(config.path + '/')
  ) || null
}

/**
 * Validate token using the backend API
 */
export async function validateTokenWithAPI(token: string): Promise<ValidateTokenResponse> {
  try {
    const backendUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api'
    
    const response = await fetch(`${backendUrl}/Auth/validate-token`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      },
      body: JSON.stringify({ token })
    })

    if (!response.ok) {
      return {
        success: false,
        message: 'Token validation failed',
        data: {
          isValid: false,
          userId: '',
          email: '',
          firstName: '',
          lastName: '',
          roles: [],
          expirationDate: '',
          errorMessage: 'Token validation failed'
        },
        errors: ['Token validation failed']
      }
    }

    const result: ValidateTokenResponse = await response.json()
    return result
  } catch (error) {
    console.error('Token validation error:', error)
    return {
      success: false,
      message: 'Token validation error',
      data: {
        isValid: false,
        userId: '',
        email: '',
        firstName: '',
        lastName: '',
        roles: [],
        expirationDate: '',
        errorMessage: 'Token validation error'
      },
      errors: ['Token validation error']
    }
  }
}
