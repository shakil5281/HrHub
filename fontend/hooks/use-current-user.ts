"use client"

import { useState, useEffect } from 'react'
import { getStoredUser, type User } from '@/lib/api/auth'

export function useCurrentUser() {
  const [user, setUser] = useState<User | null>(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const storedUser = getStoredUser()
    setUser(storedUser)
    setLoading(false)
  }, [])

  return { user, loading }
}
