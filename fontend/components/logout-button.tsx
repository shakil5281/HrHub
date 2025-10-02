"use client"

import { useState } from "react"
import { IconLogout } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { logout } from "@/lib/api/auth"

interface LogoutButtonProps {
  variant?: "default" | "ghost" | "outline" | "secondary" | "destructive" | "link"
  size?: "default" | "sm" | "lg" | "icon"
  showIcon?: boolean
  showConfirmation?: boolean
  className?: string
  children?: React.ReactNode
}

export function LogoutButton({ 
  variant = "ghost", 
  size = "sm", 
  showIcon = true, 
  showConfirmation = true,
  className = "",
  children 
}: LogoutButtonProps) {
  const [isLoggingOut, setIsLoggingOut] = useState(false)

  const handleLogout = async () => {
    if (isLoggingOut) return
    
    setIsLoggingOut(true)
    try {
      await logout()
    } catch (error) {
      console.error('Logout error:', error)
      // Even if there's an error, the logout function will still clear data and redirect
    }
  }

  const LogoutButtonContent = (
    <Button 
      variant={variant}
      size={size}
      disabled={isLoggingOut}
      className={`flex items-center gap-2 ${className}`}
    >
      {showIcon && <IconLogout className="h-4 w-4" />}
      {children || (isLoggingOut ? 'Logging out...' : 'Logout')}
    </Button>
  )

  if (showConfirmation) {
    return (
      <AlertDialog>
        <AlertDialogTrigger asChild>
          {LogoutButtonContent}
        </AlertDialogTrigger>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Confirm Logout</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to log out? You will need to sign in again to access your account.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel>Cancel</AlertDialogCancel>
            <AlertDialogAction onClick={handleLogout} disabled={isLoggingOut}>
              {isLoggingOut ? 'Logging out...' : 'Logout'}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    )
  }

  return (
    <div onClick={handleLogout}>
      {LogoutButtonContent}
    </div>
  )
}
