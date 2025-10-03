"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
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
import { getSectionById, updateSection, type Section } from "@/lib/api/section"
import { getAllDepartments, type Department } from "@/lib/api/department"
import { IconArrowLeft } from "@tabler/icons-react"

const sectionSchema = z.object({
  name: z.string().min(1, "Section name is required"),
  nameBangla: z.string().min(1, "Section name in Bangla is required"),
  departmentId: z.number().min(1, "Department is required"),
})

type SectionFormData = z.infer<typeof sectionSchema>

interface SectionEditFormProps {
  sectionId: string
}

export function SectionEditForm({ sectionId }: SectionEditFormProps) {
  const [loading, setLoading] = useState(false)
  const [initialLoading, setInitialLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [departments, setDepartments] = useState<Department[]>([])
  const [loadingDepartments, setLoadingDepartments] = useState(true)
  const [section, setSection] = useState<Section | null>(null)
  const router = useRouter()

  const form = useForm<SectionFormData>({
    resolver: zodResolver(sectionSchema),
    defaultValues: {
      name: "",
      nameBangla: "",
      departmentId: 0,
    },
  })

  useEffect(() => {
    const fetchData = async () => {
      try {
        setInitialLoading(true)
        
        // Fetch departments and section in parallel
        const [departmentsResponse, sectionResponse] = await Promise.all([
          getAllDepartments(),
          getSectionById(sectionId)
        ])

        if (departmentsResponse.success) {
          setDepartments(departmentsResponse.data)
        }

        if (sectionResponse.success) {
          const sect = sectionResponse.data
          setSection(sect)
          
          // Set form values
          form.reset({
            name: sect.name,
            nameBangla: sect.nameBangla,
            departmentId: sect.departmentId,
          })
        }
      } catch (error) {
        console.error('Error fetching data:', error)
        setError("Failed to load section data")
      } finally {
        setInitialLoading(false)
        setLoadingDepartments(false)
      }
    }

    fetchData()
  }, [sectionId, form])

  const onSubmit = async (values: SectionFormData) => {
    setLoading(true)
    setError(null)

    try {
      await updateSection(sectionId, values)
      router.push("/section")
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while updating the section."
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  if (initialLoading) {
    return (
      <div className="space-y-6">
        <div className="animate-pulse space-y-4">
          <div className="h-8 bg-gray-200 rounded w-1/4"></div>
          <div className="h-32 bg-gray-200 rounded"></div>
        </div>
      </div>
    )
  }

  if (!section) {
    return (
      <div className="space-y-6">
        <div className="text-center py-12">
          <h2 className="text-xl font-semibold text-gray-900">Section not found</h2>
          <p className="text-gray-500 mt-2">The section you&apos;re looking for doesn&apos;t exist.</p>
          <Button onClick={() => router.push("/section")} className="mt-4">
            Back to Sections
          </Button>
        </div>
      </div>
    )
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
            <h1 className="text-3xl font-bold tracking-tight">Edit Section</h1>
            <p className="text-muted-foreground">
              Update the section information below.
            </p>
          </div>
        </div>
      </div>

      {/* Form */}
      <Card>
        <CardHeader>
          <CardTitle>Section Information</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
              <div className="space-y-6">
                <h3 className="text-lg font-semibold border-b pb-2">Section Information</h3>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                  <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Section Name *</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter section name" {...field} />
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
                        <FormLabel>Section Name (Bangla) *</FormLabel>
                        <FormControl>
                          <Input 
                            placeholder="সেকশনের নাম" 
                            className="font-sutonnymj"
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <FormField
                  control={form.control}
                  name="departmentId"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Department *</FormLabel>
                      <Select 
                        onValueChange={(value) => field.onChange(parseInt(value))} 
                        defaultValue={field.value?.toString()}
                        disabled={loadingDepartments}
                      >
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue 
                              placeholder={loadingDepartments ? "Loading departments..." : "Select a department"} 
                            />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          {departments.map((department) => (
                            <SelectItem 
                              key={department.id} 
                              value={department.id?.toString() || ''}
                            >
                              {department.name}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                      <FormMessage />
                    </FormItem>
                  )}
                />
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
                  {loading ? "Updating Section..." : "Update Section"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
