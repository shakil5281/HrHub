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
import { createDesignation } from "@/lib/api/designation"
import { getAllSections, type Section } from "@/lib/api/section"
import { IconArrowLeft } from "@tabler/icons-react"

const designationSchema = z.object({
  name: z.string().min(1, "Designation name is required"),
  nameBangla: z.string().min(1, "Designation name in Bangla is required"),
  grade: z.string().min(1, "Grade is required"),
  attendanceBonus: z.number().min(0, "Attendance bonus must be 0 or greater"),
  sectionId: z.number().min(1, "Section is required"),
})

type DesignationFormData = z.infer<typeof designationSchema>

export function DesignationAddForm() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [sections, setSections] = useState<Section[]>([])
  const [loadingSections, setLoadingSections] = useState(true)
  const router = useRouter()

  const form = useForm<DesignationFormData>({
    resolver: zodResolver(designationSchema),
    defaultValues: {
      name: "",
      nameBangla: "",
      grade: "",
      attendanceBonus: 0,
      sectionId: 0,
    },
  })

  useEffect(() => {
    const fetchSections = async () => {
      try {
        setLoadingSections(true)
        
        const sectionsResponse = await getAllSections()

        if (sectionsResponse.success) {
          setSections(sectionsResponse.data)
        }
      } catch (error) {
        console.error('Error fetching sections:', error)
      } finally {
        setLoadingSections(false)
      }
    }

    fetchSections()
  }, [])

  const onSubmit = async (values: DesignationFormData) => {
    setLoading(true)
    setError(null)

    try {
      await createDesignation(values)
      router.push("/designation")
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while saving the designation."
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
            <h1 className="text-3xl font-bold tracking-tight">Add New Designation</h1>
            <p className="text-muted-foreground">
              Fill in the details to create a new designation in your organization.
            </p>
          </div>
        </div>
      </div>

      {/* Form */}
      <Card>
        <CardHeader>
          <CardTitle>Designation Information</CardTitle>
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
                        <FormLabel>Designation Name *</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter designation name" {...field} />
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
                        <FormLabel>Designation Name (Bangla) *</FormLabel>
                        <FormControl>
                          <Input 
                            placeholder="পদবির নাম" 
                            className="font-sutonnymj"
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="grade"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Grade *</FormLabel>
                        <FormControl>
                          <Input 
                            placeholder="e.g., A, B, C, 1st, 2nd" 
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="attendanceBonus"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Attendance Bonus *</FormLabel>
                        <FormControl>
                          <Input 
                            type="number"
                            placeholder="0"
                            {...field}
                            onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                {/* Additional Information */}
                <div className="space-y-6">
                  <h3 className="text-lg font-semibold border-b pb-2">Organization</h3>
                  
                  <FormField
                    control={form.control}
                    name="sectionId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Section *</FormLabel>
                        <Select 
                          onValueChange={(value) => field.onChange(parseInt(value))} 
                          defaultValue={field.value?.toString()}
                          disabled={loadingSections}
                        >
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder={loadingSections ? "Loading sections..." : "Select a section"} />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {sections.map((section) => (
                              <SelectItem 
                                key={section.id} 
                                value={section.id?.toString() || ''}
                              >
                                <div>
                                  <div>{section.name}</div>
                                  {section.departmentName && (
                                    <div className="text-xs text-gray-500">{section.departmentName}</div>
                                  )}
                                </div>
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
                  {loading ? "Creating Designation..." : "Create Designation"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
