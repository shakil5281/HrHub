import { LoginForm } from "@/components/form/login-form"

export default function Page() {
  return (
    <div className="min-h-screen flex items-center justify-center relative overflow-hidden">
      {/* Background with gradient overlay */}
      <div className="" />
      {/* Main content */}
      <div className="relative z-10 flex flex-col items-center justify-center min-h-screen p-6 w-full max-w-md">
        {/* Login form */}
        <LoginForm />
        {/* Footer */}
        <div className="mt-8 text-center">
          <p className="text-xs text-gray-500 dark:text-gray-400">
            © 2024 HR Hub. Streamlining your workforce management.
          </p>
        </div>
      </div>
    </div>
  )
}
