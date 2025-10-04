"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { createDepartment } from "@/lib/api/department"
import { getAllCompanies, type Company } from "@/lib/api/company"
import { IconArrowLeft } from "@tabler/icons-react"

const departmentSchema = z.object({
  name: z.string().min(1, "Department name is required"),
  nameBangla: z.string().min(1, "Department name in Bangla is required"),
  companyId: z.number().min(1, "Company is required"),
})

type DepartmentFormData = z.infer<typeof departmentSchema>

export function DepartmentAddForm() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [companies, setCompanies] = useState<Company[]>([])
  const [loadingCompanies, setLoadingCompanies] = useState(true)
  const router = useRouter()

  const form = useForm<DepartmentFormData>({
    resolver: zodResolver(departmentSchema),
    defaultValues: {
      name: "",
      nameBangla: "",
      companyId: 0,
    },
  })

  useEffect(() => {
    const fetchCompanies = async () => {
      try {
        setLoadingCompanies(true)
        const response = await getAllCompanies()
        if (response.success) {
          setCompanies(response.data)
        }
      } catch (error) {
        console.error('Error fetching companies:', error)
      } finally {
        setLoadingCompanies(false)
      }
    }

    fetchCompanies()
  }, [])

  const onSubmit = async (values: DepartmentFormData) => {
    setLoading(true)
    setError(null)

    try {
      await createDepartment(values)
      router.push("/department")
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while saving the department."
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="space-y-4">
          <Button
            variant="outline"
            size="sm"
            onClick={() => router.back()}
          >
            <IconArrowLeft className="mr-2 h-4 w-4" />
            Back
          </Button>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Add New Department</h1>
            <p className="text-muted-foreground">
              Fill in the details to create a new department in your organization.
            </p>
          </div>
        </div>
      </div>

      {/* Form */}
      <Card>
        <CardHeader>
          <CardTitle>Department Information</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                {/* Basic Information */}
                <div className="space-y-6">
                  <h3 className="text-lg font-semibold border-b pb-2">Basic Information</h3>
                  
                  <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Department Name *</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter department name" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="nameBangla"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Department Name (Bangla) *</FormLabel>
                        <FormControl>
                          <Input 
                            placeholder="বিভাগের নাম" 
                            className="font-sutonnymj"
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                {/* Additional Information */}
                <div className="space-y-6">
                  <h3 className="text-lg font-semibold border-b pb-2">Additional Details</h3>
                  
                  <FormField
                    control={form.control}
                    name="companyId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Company *</FormLabel>
                        <Select 
                          onValueChange={(value) => field.onChange(parseInt(value))} 
                          defaultValue={field.value?.toString()}
                          disabled={loadingCompanies}
                        >
                          <FormControl className="w-44">
                            <SelectTrigger>
                              <SelectValue placeholder={loadingCompanies ? "Loading companies..." : "Select a company"} />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {companies.map((company) => (
                              <SelectItem 
                                key={company.id || company.companyId} 
                                value={(company.id || company.companyId || '0').toString()}
                              >
                                {company.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>

              {error && (
                <div className="text-sm text-red-500 bg-red-50 p-4 rounded-md border border-red-200">
                  {error}
                </div>
              )}

              <div className="flex items-center justify-end space-x-4 pt-6 border-t">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => router.back()}
                  disabled={loading}
                >
                  Cancel
                </Button>
                <Button type="submit" disabled={loading}>
                  {loading ? "Creating Department..." : "Create Department"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
