"use client"
import { useState, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form"
import { login, type LoginResponse } from "@/lib/api/auth"

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
    <div className={cn("flex flex-col gap-6", className)} {...props}>
      <Card>
        <CardHeader>
          <CardTitle>Login to your account</CardTitle>
          <CardDescription>
            Enter your email below to login to your account
          </CardDescription>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col gap-6">
              <div className="grid gap-3">
                <FormField
                  control={form.control}
                  name="email"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel htmlFor="email">Email</FormLabel>
                      <FormControl>
                        <Input
                          id="email"
                          type="email"
                          placeholder="m@example.com"
                          autoComplete="email"
                          disabled={loading}
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              <div className="grid gap-3">
                <div className="flex items-center">
                  <FormLabel htmlFor="password">Password</FormLabel>
                  <a
                    href="#"
                    className="ml-auto inline-block text-sm underline-offset-4 hover:underline"
                  >
                    Forgot your password?
                  </a>
                </div>
                <FormField
                  control={form.control}
                  name="password"
                  render={({ field }) => (
                    <FormItem>
                      <FormControl>
                        <Input
                          id="password"
                          type="password"
                          autoComplete="current-password"
                          disabled={loading}
                          {...field}
                        />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>
              {error && (
                <div className="text-sm text-red-500">{error}</div>
              )}
              <div className="flex flex-col gap-3">
                <Button type="submit" className="w-full" disabled={loading}>
                  {loading ? "Logging in..." : "Login"}
                </Button>
                <Button variant="outline" className="w-full" type="button" disabled={loading}>
                  Login with Google
                </Button>
              </div>
              <div className="mt-4 text-center text-sm">
                Don&apos;t have an account?{" "}
                <a href="#" className="underline underline-offset-4">
                  Sign up
                </a>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
