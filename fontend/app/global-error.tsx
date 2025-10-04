"use client"

import { useEffect } from "react"
import { IconRefresh, IconHome, IconAlertTriangle } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

interface GlobalErrorProps {
  error: Error & { digest?: string }
  reset: () => void
}

export default function GlobalError({ error, reset }: GlobalErrorProps) {
  useEffect(() => {
    // Log the error to an error reporting service
    console.error('Global application error:', error)
  }, [error])

  return (
    <html>
      <body>
        <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
          <div className="max-w-md w-full space-y-6">
            <Card>
              <CardHeader className="text-center">
                <div className="mx-auto mb-4 flex h-20 w-20 items-center justify-center rounded-full bg-red-100">
                  <IconAlertTriangle className="h-10 w-10 text-red-600" />
                </div>
                <CardTitle className="text-2xl font-bold text-gray-900">Application Error</CardTitle>
                <CardDescription className="text-gray-600">
                  A critical error occurred in the application.
                </CardDescription>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="text-center text-sm text-gray-500">
                  The application encountered an unexpected error. Please refresh the page or contact support.
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
                    onClick={() => window.location.href = '/dashboard'}
                    className="w-full"
                  >
                    <IconHome className="mr-2 h-4 w-4" />
                    Go to Dashboard
                  </Button>
                </div>
                
                <div className="text-center">
                  <Button 
                    variant="ghost" 
                    onClick={() => window.location.href = '/'}
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
      </body>
    </html>
  )
}
