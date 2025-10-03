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
import { createEmployee, type CreateEmployeePayload } from "@/lib/api/employee"
import { getAllDepartments, type Department } from "@/lib/api/department"
import { getAllSections, type Section } from "@/lib/api/section"
import { getAllDesignations, type Designation } from "@/lib/api/designation"
import { IconArrowLeft } from "@tabler/icons-react"

const employeeSchema = z.object({
  name: z.string().min(1, "Name is required"),
  nameBangla: z.string().min(1, "Bangla name is required"),
  nidNo: z.string().min(1, "NID number is required"),
  fatherName: z.string().optional(),
  motherName: z.string().optional(),
  fatherNameBangla: z.string().optional(),
  motherNameBangla: z.string().optional(),
  dateOfBirth: z.string().optional(),
  address: z.string().optional(),
  joiningDate: z.string().min(1, "Joining date is required"),
  departmentId: z.number().min(1, "Please select a department"),
  sectionId: z.number().min(1, "Please select a section"),
  designationId: z.number().min(1, "Please select a designation"),
  grossSalary: z.number().min(0, "Gross salary must be positive"),
  basicSalary: z.number().min(0, "Basic salary must be positive"),
  bankAccountNo: z.string().optional(),
})

type EmployeeFormData = z.infer<typeof employeeSchema>

export function EmployeeAddForm() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [departments, setDepartments] = useState<Department[]>([])
  const [sections, setSections] = useState<Section[]>([])
  const [designations, setDesignations] = useState<Designation[]>([])
  const [loadingData, setLoadingData] = useState(true)
  const router = useRouter()

  const form = useForm<EmployeeFormData>({
    resolver: zodResolver(employeeSchema),
    defaultValues: {
      name: "",
      nameBangla: "",
      nidNo: "",
      fatherName: "",
      motherName: "",
      fatherNameBangla: "",
      motherNameBangla: "",
      dateOfBirth: "",
      address: "",
      joiningDate: "",
      departmentId: 0,
      sectionId: 0,
      designationId: 0,
      grossSalary: 0,
      basicSalary: 0,
      bankAccountNo: "",
    },
  })

  // Load departments, sections, and designations
  useEffect(() => {
    const loadData = async () => {
      try {
        const [departmentsRes, sectionsRes, designationsRes] = await Promise.all([
          getAllDepartments(),
          getAllSections(),
          getAllDesignations()
        ])
        
        if (departmentsRes.success) {
          setDepartments(departmentsRes.data)
        }
        
        if (sectionsRes.success) {
          setSections(sectionsRes.data)
        }
        
        if (designationsRes.success) {
          setDesignations(designationsRes.data)
        }
      } catch (err) {
        console.error('Error loading data:', err)
        setError('Failed to load departments, sections, and designations')
      } finally {
        setLoadingData(false)
      }
    }

    loadData()
  }, [])

  const onSubmit = async (values: EmployeeFormData) => {
    setLoading(true)
    setError(null)

    try {
      const employeeData: CreateEmployeePayload = {
        name: values.name,
        nameBangla: values.nameBangla,
        nidNo: values.nidNo,
        fatherName: values.fatherName || undefined,
        motherName: values.motherName || undefined,
        fatherNameBangla: values.fatherNameBangla || undefined,
        motherNameBangla: values.motherNameBangla || undefined,
        dateOfBirth: values.dateOfBirth || undefined,
        address: values.address || undefined,
        joiningDate: values.joiningDate,
        departmentId: values.departmentId,
        sectionId: values.sectionId,
        designationId: values.designationId,
        grossSalary: values.grossSalary,
        basicSalary: values.basicSalary,
        bankAccountNo: values.bankAccountNo || undefined,
      }

      await createEmployee(employeeData)
      router.push("/employee")
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while creating the employee."
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  if (loadingData) {
    return (
      <div className="space-y-6">
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
              <h1 className="text-3xl font-bold tracking-tight">Add New Employee</h1>
              <p className="text-muted-foreground">
                Loading employee creation form...
              </p>
            </div>
          </div>
        </div>
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto mb-4"></div>
              <p className="text-muted-foreground">Loading...</p>
            </div>
          </CardContent>
        </Card>
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
            <h1 className="text-3xl font-bold tracking-tight">Add New Employee</h1>
            <p className="text-muted-foreground">
              Fill in the details to create a new employee record.
            </p>
          </div>
        </div>
      </div>

      {/* Form */}
      <Card>
        <CardHeader>
          <CardTitle>Employee Information</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
              {/* Personal Information */}
              <div className="space-y-6">
                <h3 className="text-lg font-semibold border-b pb-2">Personal Information</h3>
                
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Full Name *</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter full name" {...field} />
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
                        <FormLabel>Full Name (Bangla) *</FormLabel>
                        <FormControl>
                          <Input placeholder="বাংলায় পূর্ণ নাম" className="font-sutonnymj" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="nidNo"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>NID Number *</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter NID number" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="dateOfBirth"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Date of Birth</FormLabel>
                        <FormControl>
                          <Input type="date" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  <FormField
                    control={form.control}
                    name="fatherName"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Father's Name</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter father's name" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="motherName"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Mother's Name</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter mother's name" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="fatherNameBangla"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Father's Name (Bangla)</FormLabel>
                        <FormControl>
                          <Input placeholder="পিতার নাম" className="font-sutonnymj" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="motherNameBangla"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Mother's Name (Bangla)</FormLabel>
                        <FormControl>
                          <Input placeholder="মাতার নাম" className="font-sutonnymj" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                <FormField
                  control={form.control}
                  name="address"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Address</FormLabel>
                      <FormControl>
                        <Input placeholder="Enter address" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              {/* Work Information */}
              <div className="space-y-6">
                <h3 className="text-lg font-semibold border-b pb-2">Work Information</h3>
                
                <FormField
                  control={form.control}
                  name="joiningDate"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Joining Date *</FormLabel>
                      <FormControl>
                        <Input type="date" {...field} />
                      </FormControl>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                  <FormField
                    control={form.control}
                    name="departmentId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Department *</FormLabel>
                        <Select onValueChange={(value) => field.onChange(parseInt(value))} value={field.value.toString()}>
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder="Select department" />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {departments.map((dept) => (
                              <SelectItem key={dept.id} value={dept.id?.toString() || '0'}>
                                {dept.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="sectionId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Section *</FormLabel>
                        <Select onValueChange={(value) => field.onChange(parseInt(value))} value={field.value.toString()}>
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder="Select section" />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {sections.map((section) => (
                              <SelectItem key={section.id} value={section.id?.toString() || '0'}>
                                {section.name}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="designationId"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Designation *</FormLabel>
                        <Select onValueChange={(value) => field.onChange(parseInt(value))} value={field.value.toString()}>
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder="Select designation" />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {designations.map((designation) => (
                              <SelectItem key={designation.id} value={designation.id?.toString() || '0'}>
                                {designation.name}
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

              {/* Salary Information */}
              <div className="space-y-6">
                <h3 className="text-lg font-semibold border-b pb-2">Salary Information</h3>
                
                <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
                  <FormField
                    control={form.control}
                    name="grossSalary"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Gross Salary *</FormLabel>
                        <FormControl>
                          <Input 
                            type="number" 
                            placeholder="0.00" 
                            {...field} 
                            onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="basicSalary"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Basic Salary *</FormLabel>
                        <FormControl>
                          <Input 
                            type="number" 
                            placeholder="0.00" 
                            {...field} 
                            onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="bankAccountNo"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Bank Account Number</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter account number" {...field} />
                        </FormControl>
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
                  {loading ? "Creating Employee..." : "Create Employee"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
