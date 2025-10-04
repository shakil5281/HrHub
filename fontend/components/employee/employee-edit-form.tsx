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
import { updateEmployee, getEmployeeById, type UpdateEmployeePayload, type Employee } from "@/lib/api/employee"
import { getAllDepartments, type Department } from "@/lib/api/department"
import { getSectionsByDepartment, type Section } from "@/lib/api/section"
import { getDesignationsBySection, type Designation } from "@/lib/api/designation"
import { IconArrowLeft } from "@tabler/icons-react"
import { toast } from "sonner"

const employeeSchema = z.object({
  empId: z.string().min(1, "Employee ID is required"),
  name: z.string().min(1, "Name is required"),
  nameBangla: z.string().min(1, "Bangla name is required"),
  nidNo: z.string().min(1, "NID number is required"),
  fatherName: z.string().optional(),
  motherName: z.string().optional(),
  fatherNameBangla: z.string().optional(),
  motherNameBangla: z.string().optional(),
  dateOfBirth: z.string().optional(),
  permanentAddress: z.string().optional(),
  permanentDivision: z.string().optional(),
  permanentDistrict: z.string().optional(),
  permanentUpazila: z.string().optional(),
  permanentPostalCode: z.string().optional(),
  presentAddress: z.string().optional(),
  presentDivision: z.string().optional(),
  presentDistrict: z.string().optional(),
  presentUpazila: z.string().optional(),
  presentPostalCode: z.string().optional(),
  joiningDate: z.string().min(1, "Joining date is required"),
  bloodGroup: z.string().optional(),
  gender: z.string().optional(),
  religion: z.string().optional(),
  maritalStatus: z.string().optional(),
  education: z.string().optional(),
  companyId: z.number().min(1, "Please select a company"),
  departmentId: z.number().min(1, "Please select a department"),
  sectionId: z.number().min(1, "Please select a section"),
  designationId: z.number().min(1, "Please select a designation"),
  lineId: z.number().optional(),
  shiftId: z.number().optional(),
  degreeId: z.number().optional(),
  floor: z.string().optional(),
  empType: z.string().optional(),
  group: z.string().optional(),
  house: z.number().optional(),
  rentMedical: z.number().optional(),
  food: z.number().optional(),
  conveyance: z.number().optional(),
  transport: z.number().optional(),
  nightBill: z.number().optional(),
  mobileBill: z.number().optional(),
  otherAllowance: z.number().optional(),
  grossSalary: z.number().min(0, "Gross salary must be positive"),
  basicSalary: z.number().min(0, "Basic salary must be positive"),
  salaryType: z.string().optional(),
  bankAccountNo: z.string().optional(),
  bank: z.string().optional(),
})

type EmployeeFormData = z.infer<typeof employeeSchema>

interface EmployeeEditFormProps {
  employeeId: number
}

export function EmployeeEditForm({ employeeId }: EmployeeEditFormProps) {
  const router = useRouter()
  const [loading, setLoading] = useState(false)
  const [employee, setEmployee] = useState<Employee | null>(null)
  const [departments, setDepartments] = useState<Department[]>([])
  const [sections, setSections] = useState<Section[]>([])
  const [designations, setDesignations] = useState<Designation[]>([])

  const form = useForm<EmployeeFormData>({
    resolver: zodResolver(employeeSchema),
    defaultValues: {
      empId: "",
      name: "",
      nameBangla: "",
      nidNo: "",
      fatherName: "",
      motherName: "",
      fatherNameBangla: "",
      motherNameBangla: "",
      dateOfBirth: "",
      permanentAddress: "",
      permanentDivision: "",
      permanentDistrict: "",
      permanentUpazila: "",
      permanentPostalCode: "",
      presentAddress: "",
      presentDivision: "",
      presentDistrict: "",
      presentUpazila: "",
      presentPostalCode: "",
      joiningDate: "",
      bloodGroup: "",
      gender: "",
      religion: "",
      maritalStatus: "",
      education: "",
      companyId: 0,
      departmentId: 0,
      sectionId: 0,
      designationId: 0,
      lineId: 0,
      shiftId: 0,
      degreeId: 0,
      floor: "",
      empType: "",
      group: "",
      house: 0,
      rentMedical: 0,
      food: 0,
      conveyance: 0,
      transport: 0,
      nightBill: 0,
      mobileBill: 0,
      otherAllowance: 0,
      grossSalary: 0,
      basicSalary: 0,
      salaryType: "",
      bankAccountNo: "",
      bank: "",
    },
  })

  const watchedDepartmentId = form.watch("departmentId")
  const watchedSectionId = form.watch("sectionId")

  useEffect(() => {
    const loadEmployee = async () => {
      try {
        const response = await getEmployeeById(employeeId)
        if (response.success) {
          const emp = response.data
          setEmployee(emp)
          
          // Reset form with employee data
          form.reset({
            empId: emp.empId || "",
            name: emp.name,
            nameBangla: emp.nameBangla,
            nidNo: emp.nidNo,
            fatherName: emp.fatherName || "",
            motherName: emp.motherName || "",
            fatherNameBangla: emp.fatherNameBangla || "",
            motherNameBangla: emp.motherNameBangla || "",
            dateOfBirth: emp.dateOfBirth || "",
            permanentAddress: emp.permanentAddress || "",
            permanentDivision: emp.permanentDivision || "",
            permanentDistrict: emp.permanentDistrict || "",
            permanentUpazila: emp.permanentUpazila || "",
            permanentPostalCode: emp.permanentPostalCode || "",
            presentAddress: emp.presentAddress || "",
            presentDivision: emp.presentDivision || "",
            presentDistrict: emp.presentDistrict || "",
            presentUpazila: emp.presentUpazila || "",
            presentPostalCode: emp.presentPostalCode || "",
            joiningDate: emp.joiningDate,
            bloodGroup: emp.bloodGroup || "",
            gender: emp.gender || "",
            religion: emp.religion || "",
            maritalStatus: emp.maritalStatus || "",
            education: emp.education || "",
            companyId: emp.companyId,
            departmentId: emp.departmentId,
            sectionId: emp.sectionId,
            designationId: emp.designationId,
            lineId: emp.lineId || 0,
            shiftId: emp.shiftId || 0,
            degreeId: emp.degreeId || 0,
            floor: emp.floor || "",
            empType: emp.empType || "",
            group: emp.group || "",
            house: emp.house || 0,
            rentMedical: emp.rentMedical || 0,
            food: emp.food || 0,
            conveyance: emp.conveyance || 0,
            transport: emp.transport || 0,
            nightBill: emp.nightBill || 0,
            mobileBill: emp.mobileBill || 0,
            otherAllowance: emp.otherAllowance || 0,
            grossSalary: emp.grossSalary,
            basicSalary: emp.basicSalary,
            salaryType: emp.salaryType || "",
            bankAccountNo: emp.bankAccountNo || "",
            bank: emp.bank || "",
          })
        }
      } catch (error) {
        console.error("Error loading employee:", error)
        toast.error("Failed to load employee data")
      }
    }

    loadEmployee()
  }, [employeeId, form])

  useEffect(() => {
    const loadDepartments = async () => {
      try {
        const response = await getAllDepartments()
        if (response.success) {
          setDepartments(response.data)
        }
      } catch (error) {
        console.error("Error loading departments:", error)
      }
    }

    loadDepartments()
  }, [])

  useEffect(() => {
    if (watchedDepartmentId && watchedDepartmentId > 0) {
      const loadSections = async () => {
        try {
          const response = await getSectionsByDepartment(watchedDepartmentId)
          if (response.success) {
            setSections(response.data)
            // Reset designation when department changes
            form.setValue("designationId", 0)
            setDesignations([])
          }
        } catch (error) {
          console.error("Error loading sections:", error)
        }
      }

      loadSections()
    } else {
      setSections([])
      setDesignations([])
    }
  }, [watchedDepartmentId, form])

  useEffect(() => {
    if (watchedSectionId && watchedSectionId > 0) {
      const loadDesignations = async () => {
        try {
          const response = await getDesignationsBySection(watchedSectionId)
          if (response.success) {
            setDesignations(response.data)
          }
        } catch (error) {
          console.error("Error loading designations:", error)
        }
      }

      loadDesignations()
    } else {
      setDesignations([])
    }
  }, [watchedSectionId, form])

  const onSubmit = async (values: EmployeeFormData) => {
    setLoading(true)
    try {
      const employeeData: UpdateEmployeePayload = {
        empId: values.empId,
        name: values.name,
        nameBangla: values.nameBangla,
        nidNo: values.nidNo,
        fatherName: values.fatherName,
        motherName: values.motherName,
        fatherNameBangla: values.fatherNameBangla,
        motherNameBangla: values.motherNameBangla,
        dateOfBirth: values.dateOfBirth,
        permanentAddress: values.permanentAddress,
        permanentDivision: values.permanentDivision,
        permanentDistrict: values.permanentDistrict,
        permanentUpazila: values.permanentUpazila,
        permanentPostalCode: values.permanentPostalCode,
        presentAddress: values.presentAddress,
        presentDivision: values.presentDivision,
        presentDistrict: values.presentDistrict,
        presentUpazila: values.presentUpazila,
        presentPostalCode: values.presentPostalCode,
        joiningDate: values.joiningDate,
        bloodGroup: values.bloodGroup,
        gender: values.gender,
        religion: values.religion,
        maritalStatus: values.maritalStatus,
        education: values.education,
        companyId: values.companyId,
        departmentId: values.departmentId,
        sectionId: values.sectionId,
        designationId: values.designationId,
        lineId: values.lineId,
        shiftId: values.shiftId,
        degreeId: values.degreeId,
        floor: values.floor,
        empType: values.empType,
        group: values.group,
        house: values.house,
        rentMedical: values.rentMedical,
        food: values.food,
        conveyance: values.conveyance,
        transport: values.transport,
        nightBill: values.nightBill,
        mobileBill: values.mobileBill,
        otherAllowance: values.otherAllowance,
        grossSalary: values.grossSalary,
        basicSalary: values.basicSalary,
        salaryType: values.salaryType,
        bankAccountNo: values.bankAccountNo,
        bank: values.bank,
      }

      const response = await updateEmployee(employeeId, employeeData)
      if (response.success) {
        toast.success("Employee updated successfully")
        router.push("/employee")
      } else {
        toast.error(response.message || "Failed to update employee")
      }
    } catch (error) {
      console.error("Error updating employee:", error)
      toast.error("Failed to update employee")
    } finally {
      setLoading(false)
    }
  }

  if (!employee) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
          <p className="mt-2 text-muted-foreground">Loading employee data...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div className="flex items-center space-x-4">
        <Button
          variant="outline"
          size="sm"
          onClick={() => router.back()}
        >
          <IconArrowLeft className="h-4 w-4 mr-2" />
          Back
        </Button>
        <div>
          <h1 className="text-2xl font-bold">Edit Employee</h1>
          <p className="text-muted-foreground">
            Update employee information for {employee.name}
          </p>
        </div>
      </div>

      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
          {/* Basic Information */}
          <Card>
            <CardHeader>
              <CardTitle>Basic Information</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <FormField
                control={form.control}
                name="empId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Employee ID *</FormLabel>
                    <FormControl>
                      <Input placeholder="EMP001" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Full Name *</FormLabel>
                    <FormControl>
                      <Input placeholder="John Doe" {...field} />
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
                    <FormLabel>Name (Bangla) *</FormLabel>
                    <FormControl>
                      <Input placeholder="জন ডো" {...field} />
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
                      <Input placeholder="1234567890123" {...field} />
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
            </CardContent>
          </Card>

          {/* Family Information */}
          <Card>
            <CardHeader>
              <CardTitle>Family Information</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="fatherName"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Father's Name</FormLabel>
                    <FormControl>
                      <Input placeholder="Father's Name" {...field} />
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
                      <Input placeholder="পিতার নাম" {...field} />
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
                      <Input placeholder="Mother's Name" {...field} />
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
                      <Input placeholder="মাতার নাম" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
          </Card>

          {/* Personal Details */}
          <Card>
            <CardHeader>
              <CardTitle>Personal Details</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <FormField
                control={form.control}
                name="gender"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Gender</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select gender" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="Male">Male</SelectItem>
                        <SelectItem value="Female">Female</SelectItem>
                        <SelectItem value="Other">Other</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="bloodGroup"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Blood Group</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select blood group" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="A+">A+</SelectItem>
                        <SelectItem value="A-">A-</SelectItem>
                        <SelectItem value="B+">B+</SelectItem>
                        <SelectItem value="B-">B-</SelectItem>
                        <SelectItem value="AB+">AB+</SelectItem>
                        <SelectItem value="AB-">AB-</SelectItem>
                        <SelectItem value="O+">O+</SelectItem>
                        <SelectItem value="O-">O-</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="religion"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Religion</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select religion" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="Islam">Islam</SelectItem>
                        <SelectItem value="Hinduism">Hinduism</SelectItem>
                        <SelectItem value="Christianity">Christianity</SelectItem>
                        <SelectItem value="Buddhism">Buddhism</SelectItem>
                        <SelectItem value="Other">Other</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="maritalStatus"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Marital Status</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select marital status" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="Single">Single</SelectItem>
                        <SelectItem value="Married">Married</SelectItem>
                        <SelectItem value="Divorced">Divorced</SelectItem>
                        <SelectItem value="Widowed">Widowed</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="education"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Education</FormLabel>
                    <FormControl>
                      <Input placeholder="Education level" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
          </Card>

          {/* Address Information */}
          <Card>
            <CardHeader>
              <CardTitle>Address Information</CardTitle>
            </CardHeader>
            <CardContent className="space-y-6">
              <div>
                <h4 className="text-lg font-medium mb-4">Permanent Address</h4>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  <FormField
                    control={form.control}
                    name="permanentAddress"
                    render={({ field }) => (
                      <FormItem className="md:col-span-2 lg:col-span-3">
                        <FormLabel>Address</FormLabel>
                        <FormControl>
                          <Textarea placeholder="Full permanent address" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="permanentDivision"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Division</FormLabel>
                        <FormControl>
                          <Input placeholder="Division" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="permanentDistrict"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>District</FormLabel>
                        <FormControl>
                          <Input placeholder="District" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="permanentUpazila"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Upazila</FormLabel>
                        <FormControl>
                          <Input placeholder="Upazila" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="permanentPostalCode"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Postal Code</FormLabel>
                        <FormControl>
                          <Input placeholder="Postal Code" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>

              <div>
                <h4 className="text-lg font-medium mb-4">Present Address</h4>
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  <FormField
                    control={form.control}
                    name="presentAddress"
                    render={({ field }) => (
                      <FormItem className="md:col-span-2 lg:col-span-3">
                        <FormLabel>Address</FormLabel>
                        <FormControl>
                          <Textarea placeholder="Full present address" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="presentDivision"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Division</FormLabel>
                        <FormControl>
                          <Input placeholder="Division" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="presentDistrict"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>District</FormLabel>
                        <FormControl>
                          <Input placeholder="District" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="presentUpazila"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Upazila</FormLabel>
                        <FormControl>
                          <Input placeholder="Upazila" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  <FormField
                    control={form.control}
                    name="presentPostalCode"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Postal Code</FormLabel>
                        <FormControl>
                          <Input placeholder="Postal Code" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Employment Information */}
          <Card>
            <CardHeader>
              <CardTitle>Employment Information</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <FormField
                control={form.control}
                name="companyId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Company *</FormLabel>
                    <Select onValueChange={(value) => field.onChange(parseInt(value))} defaultValue={field.value?.toString()}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select company" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="1">Acme Inc.</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="departmentId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Department *</FormLabel>
                    <Select onValueChange={(value) => field.onChange(parseInt(value))} defaultValue={field.value?.toString()}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select department" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {departments.map((dept) => (
                          <SelectItem key={dept.id} value={dept.id.toString()}>
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
                    <Select onValueChange={(value) => field.onChange(parseInt(value))} defaultValue={field.value?.toString()}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select section" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {sections.map((section) => (
                          <SelectItem key={section.id} value={section.id.toString()}>
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
                    <Select onValueChange={(value) => field.onChange(parseInt(value))} defaultValue={field.value?.toString()}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select designation" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {designations.map((designation) => (
                          <SelectItem key={designation.id} value={designation.id.toString()}>
                            {designation.name}
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
                name="empType"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Employee Type</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select type" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="Permanent">Permanent</SelectItem>
                        <SelectItem value="Contract">Contract</SelectItem>
                        <SelectItem value="Temporary">Temporary</SelectItem>
                        <SelectItem value="Intern">Intern</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="floor"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Floor</FormLabel>
                    <FormControl>
                      <Input placeholder="Floor" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
          </Card>

          {/* Salary Information */}
          <Card>
            <CardHeader>
              <CardTitle>Salary Information</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              <FormField
                control={form.control}
                name="grossSalary"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Gross Salary *</FormLabel>
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
              <FormField
                control={form.control}
                name="basicSalary"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Basic Salary *</FormLabel>
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
              <FormField
                control={form.control}
                name="salaryType"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Salary Type</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select salary type" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="Monthly">Monthly</SelectItem>
                        <SelectItem value="Daily">Daily</SelectItem>
                        <SelectItem value="Hourly">Hourly</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="house"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>House Allowance</FormLabel>
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
              <FormField
                control={form.control}
                name="rentMedical"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Rent & Medical</FormLabel>
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
              <FormField
                control={form.control}
                name="food"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Food Allowance</FormLabel>
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
              <FormField
                control={form.control}
                name="conveyance"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Conveyance</FormLabel>
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
              <FormField
                control={form.control}
                name="transport"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Transport</FormLabel>
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
              <FormField
                control={form.control}
                name="nightBill"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Night Bill</FormLabel>
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
              <FormField
                control={form.control}
                name="mobileBill"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Mobile Bill</FormLabel>
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
              <FormField
                control={form.control}
                name="otherAllowance"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Other Allowance</FormLabel>
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
            </CardContent>
          </Card>

          {/* Bank Information */}
          <Card>
            <CardHeader>
              <CardTitle>Bank Information</CardTitle>
            </CardHeader>
            <CardContent className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="bankAccountNo"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Bank Account Number</FormLabel>
                    <FormControl>
                      <Input placeholder="Account number" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="bank"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Bank Name</FormLabel>
                    <FormControl>
                      <Input placeholder="Bank name" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </CardContent>
          </Card>

          {/* Form Actions */}
          <div className="flex justify-end space-x-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => router.back()}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={loading}>
              {loading ? "Updating..." : "Update Employee"}
            </Button>
          </div>
        </form>
      </Form>
    </div>
  )
}