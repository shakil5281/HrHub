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
import { updateEmployee, getEmployeeById, type UpdateEmployeePayload, type Employee } from "@/lib/api/employee"
import { getAllDepartments, type Department } from "@/lib/api/department"
import { getAllSections, getSectionsByDepartment, type Section } from "@/lib/api/section"
import { getAllDesignations, getDesignationsBySection, type Designation } from "@/lib/api/designation"
import { IconArrowLeft } from "@tabler/icons-react"

const employeeEditSchema = z.object({
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

type EmployeeEditFormData = z.infer<typeof employeeEditSchema>

interface EmployeeEditFormProps {
  employeeId: number
}

export function EmployeeEditForm({ employeeId }: EmployeeEditFormProps) {
  const [loading, setLoading] = useState(false)
  const [loadingData, setLoadingData] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [departments, setDepartments] = useState<Department[]>([])
  const [sections, setSections] = useState<Section[]>([])
  const [designations, setDesignations] = useState<Designation[]>([])
  const [filteredSections, setFilteredSections] = useState<Section[]>([])
  const [filteredDesignations, setFilteredDesignations] = useState<Designation[]>([])
  const [loadingSections, setLoadingSections] = useState(false)
  const [loadingDesignations, setLoadingDesignations] = useState(false)
  const [employee, setEmployee] = useState<Employee | null>(null)
  const router = useRouter()

  const form = useForm<EmployeeEditFormData>({
    resolver: zodResolver(employeeEditSchema),
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

  // Load employee data and reference data
  useEffect(() => {
    const loadData = async () => {
      try {
        const [employeeRes, departmentsRes] = await Promise.all([
          getEmployeeById(employeeId),
          getAllDepartments()
        ])
        
        if (employeeRes.success && departmentsRes.success) {
          const employeeData = employeeRes.data
          setEmployee(employeeData)
          setDepartments(departmentsRes.data)
          
          // Find department ID by name
          const department = departmentsRes.data.find(dept => dept.name === employeeData.departmentName)
          const departmentId = department?.id || 0
          
          // Load sections for the department
          if (departmentId > 0) {
            const sectionsRes = await getSectionsByDepartment(departmentId)
            if (sectionsRes.success) {
              setFilteredSections(sectionsRes.data)
              
              // Find section ID by name
              const section = sectionsRes.data.find(sec => sec.name === employeeData.sectionName)
              const sectionId = section?.id || 0
              
              // Load designations for the section
              if (sectionId > 0) {
                const designationsRes = await getDesignationsBySection(sectionId)
                if (designationsRes.success) {
                  setFilteredDesignations(designationsRes.data)
                  
                  // Find designation ID by name
                  const designation = designationsRes.data.find(des => des.name === employeeData.designationName)
                  const designationId = designation?.id || 0
          
          // Format dates for form inputs
          const joiningDate = employeeData.joiningDate ? new Date(employeeData.joiningDate).toISOString().split('T')[0] : ""
          const dateOfBirth = employeeData.dateOfBirth ? new Date(employeeData.dateOfBirth).toISOString().split('T')[0] : ""
          
          form.reset({
            name: employeeData.name || "",
            nameBangla: employeeData.nameBangla || "",
            nidNo: employeeData.nidNo || "",
            fatherName: employeeData.fatherName || "",
            motherName: employeeData.motherName || "",
            fatherNameBangla: employeeData.fatherNameBangla || "",
            motherNameBangla: employeeData.motherNameBangla || "",
            dateOfBirth,
            address: employeeData.address || "",
            joiningDate,
                    departmentId,
                    sectionId,
                    designationId,
            grossSalary: employeeData.grossSalary || 0,
            basicSalary: employeeData.basicSalary || 0,
            bankAccountNo: employeeData.bankAccountNo || "",
          })
        }
              }
        }
        }
        }
      } catch (err) {
        console.error('Error loading data:', err)
        setError('Failed to load employee data')
      } finally {
        setLoadingData(false)
      }
    }

    loadData()
  }, [employeeId, form])

  // Handle department change to load sections
  const handleDepartmentChange = async (departmentId: number) => {
    if (departmentId === 0) {
      setFilteredSections([])
      setFilteredDesignations([])
      form.setValue('sectionId', 0)
      form.setValue('designationId', 0)
      return
    }

    setLoadingSections(true)
    try {
      const sectionsRes = await getSectionsByDepartment(departmentId)
      if (sectionsRes.success) {
        setFilteredSections(sectionsRes.data)
        setFilteredDesignations([])
        form.setValue('sectionId', 0)
        form.setValue('designationId', 0)
      }
    } catch (err) {
      console.error('Error loading sections:', err)
      setError('Failed to load sections for selected department')
    } finally {
      setLoadingSections(false)
    }
  }

  // Handle section change to load designations
  const handleSectionChange = async (sectionId: number) => {
    if (sectionId === 0) {
      setFilteredDesignations([])
      form.setValue('designationId', 0)
      return
    }

    setLoadingDesignations(true)
    try {
      const designationsRes = await getDesignationsBySection(sectionId)
      if (designationsRes.success) {
        setFilteredDesignations(designationsRes.data)
        form.setValue('designationId', 0)
      }
    } catch (err) {
      console.error('Error loading designations:', err)
      setError('Failed to load designations for selected section')
    } finally {
      setLoadingDesignations(false)
    }
  }

  const onSubmit = async (values: EmployeeEditFormData) => {
    setLoading(true)
    setError(null)

    try {
      const employeeData: UpdateEmployeePayload = {
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

      await updateEmployee(employeeId, employeeData)
      router.push("/employee")
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while updating the employee."
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
              <h1 className="text-3xl font-bold tracking-tight">Edit Employee</h1>
              <p className="text-muted-foreground">
                Loading employee information...
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

  if (!employee) {
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
              <h1 className="text-3xl font-bold tracking-tight">Edit Employee</h1>
              <p className="text-muted-foreground">
                Employee not found.
              </p>
            </div>
          </div>
        </div>
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="text-center">
              <p className="text-muted-foreground">Employee not found</p>
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
            <h1 className="text-3xl font-bold tracking-tight">Edit Employee</h1>
            <p className="text-muted-foreground">
              Update employee information for {employee.name}.
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
                        <Select 
                          onValueChange={(value) => {
                            const deptId = parseInt(value)
                            field.onChange(deptId)
                            handleDepartmentChange(deptId)
                          }} 
                          value={field.value.toString()}
                        >
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
                        <Select 
                          onValueChange={(value) => {
                            const sectionId = parseInt(value)
                            field.onChange(sectionId)
                            handleSectionChange(sectionId)
                          }} 
                          value={field.value.toString()}
                          disabled={!form.watch('departmentId') || form.watch('departmentId') === 0 || loadingSections}
                        >
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder={
                                !form.watch('departmentId') || form.watch('departmentId') === 0 
                                  ? "Select department first" 
                                  : loadingSections 
                                    ? "Loading sections..." 
                                    : "Select section"
                              } />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {filteredSections.map((section) => (
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
                        <Select 
                          onValueChange={(value) => field.onChange(parseInt(value))} 
                          value={field.value.toString()}
                          disabled={!form.watch('sectionId') || form.watch('sectionId') === 0 || loadingDesignations}
                        >
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder={
                                !form.watch('sectionId') || form.watch('sectionId') === 0 
                                  ? "Select section first" 
                                  : loadingDesignations 
                                    ? "Loading designations..." 
                                    : "Select designation"
                              } />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {filteredDesignations.map((designation) => (
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
                  {loading ? "Updating Employee..." : "Update Employee"}
                </Button>
          </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
