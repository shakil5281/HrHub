import { NextResponse } from 'next/server'
import type { NextRequest } from 'next/server'
import { 
  getTokenFromRequest, 
  validateTokenWithAPI 
} from './lib/middleware'

export async function middleware(request: NextRequest) {
  const { pathname } = request.nextUrl
  
  // Allow access to login page without authentication
  if (pathname === '/login') {
    // Get token from request
    const token = getTokenFromRequest(request)
    
    // If accessing login page with a valid token, redirect to dashboard
    if (token) {
      try {
        const validation = await validateTokenWithAPI(token)
        if (validation.success && validation.data.isValid) {
          return NextResponse.redirect(new URL('/dashboard', request.url))
        }
      } catch (error) {
        // If validation fails, allow access to login page
        console.error('Token validation error on login page:', error)
      }
    }
    return NextResponse.next()
  }
  
  // All other routes are protected - require authentication
  const token = getTokenFromRequest(request)
  
  // No token provided - redirect to login
  if (!token) {
    const loginUrl = new URL('/login', request.url)
    loginUrl.searchParams.set('redirect', pathname)
    return NextResponse.redirect(loginUrl)
  }
  
  // Validate token with API
  try {
    const validation = await validateTokenWithAPI(token)
    
    if (!validation.success || !validation.data.isValid) {
      const loginUrl = new URL('/login', request.url)
      loginUrl.searchParams.set('redirect', pathname)
      loginUrl.searchParams.set('expired', 'true')
      return NextResponse.redirect(loginUrl)
    }
    
    // Add validated user info to headers for downstream use
    const response = NextResponse.next()
    response.headers.set('x-user-id', validation.data.userId)
    response.headers.set('x-user-email', validation.data.email)
    response.headers.set('x-user-roles', JSON.stringify(validation.data.roles))
    return response
    
  } catch (error) {
    console.error('Token validation error:', error)
    const loginUrl = new URL('/login', request.url)
    loginUrl.searchParams.set('redirect', pathname)
    loginUrl.searchParams.set('error', 'validation_failed')
    return NextResponse.redirect(loginUrl)
  }
}

// Configure which paths the middleware should run on
export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     * - public folder files
     */
    '/((?!api|_next/static|_next/image|favicon.ico|.*\\.(?:svg|png|jpg|jpeg|gif|webp)$).*)',
  ],
}
