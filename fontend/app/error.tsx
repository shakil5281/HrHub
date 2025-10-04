"use client"

import { useEffect } from "react"
import { useRouter } from "next/navigation"
import { IconRefresh, IconHome, IconAlertCircle } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

interface ErrorProps {
  error: Error & { digest?: string }
  reset: () => void
}

export default function Error({ error, reset }: ErrorProps) {
  const router = useRouter()

  useEffect(() => {
    // Log the error to an error reporting service
    console.error('Application error:', error)
  }, [error])

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="max-w-md w-full space-y-6">
        <Card>
          <CardHeader className="text-center">
            <div className="mx-auto mb-4 flex h-20 w-20 items-center justify-center rounded-full bg-red-100">
              <IconAlertCircle className="h-10 w-10 text-red-600" />
            </div>
            <CardTitle className="text-2xl font-bold text-gray-900">Something went wrong!</CardTitle>
            <CardDescription className="text-gray-600">
              An unexpected error occurred while loading this page.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="text-center text-sm text-gray-500">
              We apologize for the inconvenience. Please try again or contact support if the problem persists.
            </div>
            
            {process.env.NODE_ENV === 'development' && (
              <div className="rounded-md bg-red-50 p-3">
                <div className="text-sm text-red-800 font-medium mb-1">Development Error Details:</div>
                <div className="text-xs text-red-700 font-mono break-words">
                  {error.message}
                </div>
                {error.digest && (
                  <div className="text-xs text-red-600 mt-1">
                    Error ID: {error.digest}
                  </div>
                )}
              </div>
            )}
            
            <div className="flex flex-col space-y-2">
              <Button 
                onClick={reset}
                className="w-full"
              >
                <IconRefresh className="mr-2 h-4 w-4" />
                Try Again
              </Button>
              
              <Button 
                variant="outline" 
                onClick={() => router.push('/dashboard')}
                className="w-full"
              >
                <IconHome className="mr-2 h-4 w-4" />
                Go to Dashboard
              </Button>
            </div>
            
            <div className="text-center">
              <Button 
                variant="ghost" 
                onClick={() => router.push('/')}
                className="text-sm text-gray-500 hover:text-gray-700"
              >
                Return to Home
              </Button>
            </div>
          </CardContent>
        </Card>
        
        <div className="text-center text-xs text-gray-400">
          If this error continues, please contact support
        </div>
      </div>
    </div>
  )
}
