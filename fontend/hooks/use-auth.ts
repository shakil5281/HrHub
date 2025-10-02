'use client'

import { useState, useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { 
  getStoredUser, 
  getStoredToken, 
  isAuthenticated, 
  type User 
} from '@/lib/api/auth'

interface UseAuthReturn {
  user: User | null
  token: string | null
  isLoading: boolean
  isAuthenticated: boolean
  logout: () => void
  checkAuth: () => boolean
}

export function useAuth(): UseAuthReturn {
  const [user, setUser] = useState<User | null>(null)
  const [token, setToken] = useState<string | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const router = useRouter()

  useEffect(() => {
    checkAuthStatus()
  }, [])

  const checkAuthStatus = () => {
    setIsLoading(true)
    try {
      const storedUser = getStoredUser()
      const storedToken = getStoredToken()
      const authenticated = isAuthenticated()

      setUser(storedUser)
      setToken(storedToken)
      
      if (!authenticated && storedToken) {
        // Token exists but is expired, clear storage
        logout()
      }
    } catch (error) {
      console.error('Error checking auth status:', error)
      logout()
    } finally {
      setIsLoading(false)
    }
  }

  const logout = () => {
    setUser(null)
    setToken(null)
    
    // Clear localStorage
    if (typeof window !== 'undefined') {
      localStorage.removeItem('token')
      localStorage.removeItem('refreshToken')
      localStorage.removeItem('user')
      localStorage.removeItem('tokenExpiration')
    }
    
    router.push('/login')
  }

  const checkAuth = (): boolean => {
    return isAuthenticated()
  }

  return {
    user,
    token,
    isLoading,
    isAuthenticated: isAuthenticated(),
    logout,
    checkAuth
  }
}
