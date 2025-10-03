"use client"

import { useEffect } from 'react'
import { useRouter } from 'next/navigation'

export default function CreateUserPage() {
  const router = useRouter()

  useEffect(() => {
    // Redirect to the new user management system
    router.replace('/admin/users/add')
  }, [router])

  return (
    <div className="flex items-center justify-center min-h-[400px]">
      <div className="text-center">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto mb-4"></div>
        <p className="text-muted-foreground">Redirecting to user management...</p>
      </div>
    </div>
  )
}
