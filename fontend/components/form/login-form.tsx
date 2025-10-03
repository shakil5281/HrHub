"use client"
import { useState, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { login, type LoginResponse } from "@/lib/api/auth"
import { Eye, EyeOff, Mail, Lock, AlertCircle, Loader2, Chrome } from "lucide-react"

const formSchema = z.object({
  email: z.string().email({ message: "Please enter a valid email address" }),
  password: z.string().min(6, { message: "Password must be at least 6 characters" }),
})

export function LoginForm({
  className,
  ...props
}: React.ComponentProps<"div">) {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [showPassword, setShowPassword] = useState(false)

  const form = useForm<z.infer<typeof formSchema>>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  })

  // Check for error parameters on component mount
  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search)
    const expired = urlParams.get('expired')
    const errorParam = urlParams.get('error')
    
    if (expired === 'true') {
      setError('Your session has expired. Please log in again.')
    } else if (errorParam === 'validation_failed') {
      setError('Authentication failed. Please log in again.')
    }
  }, [])

  const onSubmit = async (values: z.infer<typeof formSchema>) => {
    setLoading(true)
    setError(null)
    
    try {
      const response: LoginResponse = await login(values)

      if (response.success) {
        // Store authentication data in localStorage
        localStorage.setItem("token", response.data.token)
        localStorage.setItem("refreshToken", response.data.refreshToken)
        localStorage.setItem("tokenExpiration", response.data.expiration)
        localStorage.setItem("user", JSON.stringify(response.data.user))

        // Also set token as cookie for middleware
        document.cookie = `token=${response.data.token}; path=/; max-age=${60 * 60 * 24 * 7}; SameSite=Lax`

        console.log("Login successful:", response.message)
        console.log("User:", response.data.user)

        // Check for redirect parameter
        const urlParams = new URLSearchParams(window.location.search)
        const redirectTo = urlParams.get('redirect') || '/dashboard'

        // Redirect to intended page or dashboard
        window.location.href = redirectTo
      } else {
        // Handle unsuccessful login
        setError(response.message || "Login failed. Please try again.")
        if (response.errors && response.errors.length > 0) {
          setError(response.errors.join(", "))
        }
      }
    } catch (err: any) {
      // Handle API errors
      const errorMessage = err?.response?.data?.message ||
        err?.response?.data?.errors?.join(", ") ||
        err?.message ||
        "Login failed. Please try again."
      setError(errorMessage)
      console.error("Login error:", err)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className={cn("w-full md:max-w-md mx-auto", className)} {...props}>
      <Card className="md:shadow-lg border-0 bg-gradient-to-br from-white to-gray-50/50 border-none shadow-none">
        <CardHeader className="space-y-4 pb-8">
          <div className="flex flex-col items-center space-y-2">
            <div className="w-12 h-12 bg-primary/10 rounded-full flex items-center justify-center">
              <Lock className="w-6 h-6 text-primary" />
            </div>
            <CardTitle className="text-2xl font-bold text-center">Welcome back</CardTitle>
            <CardDescription className="text-center text-muted-foreground">
              Sign in to your account to continue
            </CardDescription>
          </div>
        </CardHeader>
        <CardContent className="space-y-6">
          {error && (
            <Alert variant="destructive" className="border-red-200 bg-red-50">
              <AlertCircle className="h-4 w-4" />
              <AlertDescription className="text-red-800">
                {error}
              </AlertDescription>
            </Alert>
          )}
          
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              <FormField
                control={form.control}
                name="email"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <FormLabel htmlFor="email" className="text-sm font-medium text-gray-700">
                      Email Address
                    </FormLabel>
                    <FormControl>
                      <div className="relative">
                        <Mail className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
                        <Input
                          id="email"
                          type="email"
                          placeholder="Enter your email"
                          autoComplete="email"
                          disabled={loading}
                          className="pl-10 h-11 border-gray-200 focus:border-primary focus:ring-primary/20"
                          {...field}
                        />
                      </div>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="password"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <div className="flex items-center justify-between">
                      <FormLabel htmlFor="password" className="text-sm font-medium text-gray-700">
                        Password
                      </FormLabel>
                      <button
                        type="button"
                        className="text-sm text-primary hover:text-primary/80 font-medium transition-colors"
                        onClick={() => {/* TODO: Implement forgot password */}}
                      >
                        Forgot password?
                      </button>
                    </div>
                    <FormControl>
                      <div className="relative">
                        <Lock className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
                        <Input
                          id="password"
                          type={showPassword ? "text" : "password"}
                          placeholder="Enter your password"
                          autoComplete="current-password"
                          disabled={loading}
                          className="pl-10 pr-10 h-11 border-gray-200 focus:border-primary focus:ring-primary/20"
                          {...field}
                        />
                        <button
                          type="button"
                          className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-400 hover:text-gray-600 transition-colors"
                          onClick={() => setShowPassword(!showPassword)}
                          disabled={loading}
                          aria-label={showPassword ? "Hide password" : "Show password"}
                        >
                          {showPassword ? (
                            <EyeOff className="h-4 w-4" />
                          ) : (
                            <Eye className="h-4 w-4" />
                          )}
                        </button>
                      </div>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <div className="space-y-4">
                <Button 
                  type="submit" 
                  className="w-full h-11 font-medium transition-all duration-200 hover:shadow-md" 
                  disabled={loading}
                >
                  {loading ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                      Signing in...
                    </>
                  ) : (
                    "Sign In"
                  )}
                </Button>

                <div className="relative">
                  <div className="absolute inset-0 flex items-center">
                    <span className="w-full border-t border-gray-200" />
                  </div>
                  <div className="relative flex justify-center text-xs uppercase">
                    <span className="bg-white px-2 text-gray-500">Or continue with</span>
                  </div>
                </div>
              </div>
            </form>
          </Form>

          <div className="text-center">
            <p className="text-sm text-gray-600">
              Don&apos;t have an account?{" "}
              <button
                type="button"
                className="font-medium text-primary hover:text-primary/80 transition-colors"
                onClick={() => {/* TODO: Navigate to sign up */}}
              >
                Sign up
              </button>
            </p>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
