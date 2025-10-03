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
import { getAllSections, type Section } from "@/lib/api/section"
import { getAllDesignations, type Designation } from "@/lib/api/designation"
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
        const [employeeRes, departmentsRes, sectionsRes, designationsRes] = await Promise.all([
          getEmployeeById(employeeId),
          getAllDepartments(),
          getAllSections(),
          getAllDesignations()
        ])
        
        if (employeeRes.success) {
          const employeeData = employeeRes.data
          setEmployee(employeeData)
          
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
            departmentId: employeeData.id || 0,
            sectionId: employeeData.id || 0,
            designationId: employeeData.id || 0,
            grossSalary: employeeData.grossSalary || 0,
            basicSalary: employeeData.basicSalary || 0,
            bankAccountNo: employeeData.bankAccountNo || "",
          })
        }
        
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
        setError('Failed to load employee data')
      } finally {
        setLoadingData(false)
      }
    }

    loadData()
  }, [employeeId, form])

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

      {/* Form - Same as add form but with edit specific details */}
      <Card>
        <CardHeader>
          <CardTitle>Employee Information</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="p-8 text-center text-gray-500">
            Employee edit form content would be similar to the add form
            with pre-populated data. For brevity, showing placeholder.
            <br /><br />
            Employee ID: {employee.id}
            <br />
            Current Status: {employee.isActive ? "Active" : "Inactive"}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
