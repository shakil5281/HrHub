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
import { getDesignationById, updateDesignation, type Designation } from "@/lib/api/designation"
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

interface DesignationEditFormProps {
  designationId: string
}

export function DesignationEditForm({ designationId }: DesignationEditFormProps) {
  const [loading, setLoading] = useState(false)
  const [initialLoading, setInitialLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [sections, setSections] = useState<Section[]>([])
  const [loadingSections, setLoadingSections] = useState(true)
  const [designation, setDesignation] = useState<Designation | null>(null)
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
    const fetchData = async () => {
      try {
        setInitialLoading(true)
        
        // Fetch sections and designation in parallel
        const [sectionsResponse, designationResponse] = await Promise.all([
          getAllSections(),
          getDesignationById(designationId)
        ])

        if (sectionsResponse.success) {
          setSections(sectionsResponse.data)
        }

        if (designationResponse.success) {
          const desig = designationResponse.data
          setDesignation(desig)
          
          // Set form values
          form.reset({
            name: desig.name,
            nameBangla: desig.nameBangla,
            grade: desig.grade,
            attendanceBonus: desig.attendanceBonus,
            sectionId: desig.sectionId,
          })
        }
      } catch (error) {
        console.error('Error fetching data:', error)
        setError("Failed to load designation data")
      } finally {
        setInitialLoading(false)
        setLoadingSections(false)
      }
    }

    fetchData()
  }, [designationId, form])

  const onSubmit = async (values: DesignationFormData) => {
    setLoading(true)
    setError(null)

    try {
      await updateDesignation(designationId, values)
      router.push("/designation")
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while updating the designation."
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

  if (!designation) {
    return (
      <div className="space-y-6">
        <div className="text-center py-12">
          <h2 className="text-xl font-semibold text-gray-900">Designation not found</h2>
          <p className="text-gray-500 mt-2">The designation you&apos;re looking for doesn&apos;t exist.</p>
          <Button onClick={() => router.push("/designation")} className="mt-4">
            Back to Designations
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
            <h1 className="text-3xl font-bold tracking-tight">Edit Designation</h1>
            <p className="text-muted-foreground">
              Update the designation information below.
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
              <div className="space-y-6">
                <h3 className="text-lg font-semibold border-b pb-2">Designation Information</h3>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
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
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
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
                  {loading ? "Updating Designation..." : "Update Designation"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
