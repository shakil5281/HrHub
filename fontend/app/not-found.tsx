"use client"

import { useRouter } from "next/navigation"
import { IconHome, IconArrowLeft, IconSearch } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"

export default function NotFound() {
  const router = useRouter()

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 px-4">
      <div className="max-w-md w-full space-y-6">
        <Card>
          <CardHeader className="text-center">
            <div className="mx-auto mb-4 flex h-20 w-20 items-center justify-center rounded-full bg-red-100">
              <IconSearch className="h-10 w-10 text-red-600" />
            </div>
            <CardTitle className="text-2xl font-bold text-gray-900">Page Not Found</CardTitle>
            <CardDescription className="text-gray-600">
              Sorry, we couldn't find the page you're looking for.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="text-center text-sm text-gray-500">
              The page you requested might have been moved, deleted, or doesn't exist.
            </div>
            
            <div className="flex flex-col space-y-2">
              <Button 
                onClick={() => router.push('/dashboard')}
                className="w-full"
              >
                <IconHome className="mr-2 h-4 w-4" />
                Go to Dashboard
              </Button>
              
              <Button 
                variant="outline" 
                onClick={() => router.back()}
                className="w-full"
              >
                <IconArrowLeft className="mr-2 h-4 w-4" />
                Go Back
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
          Error 404
        </div>
      </div>
    </div>
  )
}
