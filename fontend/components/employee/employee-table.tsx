"use client"

import { useState } from "react"
import { format } from "date-fns"
import {
  IconEdit,
  IconTrash,
  IconEye,
  IconUser,
  IconBuilding,
  IconBriefcase,
  IconCash,
  IconToggleLeft,
  IconToggleRight,
} from "@tabler/icons-react"
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog"
import { Button } from "@/components/ui/button"
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { deleteEmployee, type Employee } from "@/lib/api/employee"

interface EmployeeTableProps {
  employees: Employee[]
  loading: boolean
  onEdit: (employee: Employee) => void
  onRefresh: () => void
}

export function EmployeeTable({ employees, loading, onEdit, onRefresh }: EmployeeTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [employeeToDelete, setEmployeeToDelete] = useState<Employee | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [viewEmployee, setViewEmployee] = useState<Employee | null>(null)

  const handleDelete = async () => {
    if (!employeeToDelete?.id) return

    setDeleting(true)
    try {
      await deleteEmployee(employeeToDelete.id)
      onRefresh()
      setDeleteDialogOpen(false)
      setEmployeeToDelete(null)
    } catch (error) {
      console.error('Error deleting employee:', error)
    } finally {
      setDeleting(false)
    }
  }

  const openDeleteDialog = (employee: Employee) => {
    setEmployeeToDelete(employee)
    setDeleteDialogOpen(true)
  }

  if (loading) {
    return (
      <div className="space-y-4">
        {[...Array(5)].map((_, i) => (
          <div key={i} className="h-16 bg-gray-100 animate-pulse rounded-md" />
        ))}
      </div>
    )
  }

  if (employees.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <IconUser className="h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">No employees found</h3>
          <p className="text-gray-500 text-center max-w-sm">
            Get started by adding your first employee. Employees are the backbone of your organization.
          </p>
        </CardContent>
      </Card>
    )
  }

  return (
    <>
      <div className="rounded-md border">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead>Employee</TableHead>
              <TableHead>Personal Info</TableHead>
              <TableHead>Work Details</TableHead>
              <TableHead>Salary</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {employees.map((employee) => (
              <TableRow key={employee.id}>
                <TableCell>
                  <div className="flex items-center space-x-3">
                    <Avatar className="h-10 w-10">
                      <AvatarFallback>
                        {employee.name?.charAt(0) || ''}{employee.name?.split(' ')[1]?.charAt(0) || ''}
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <div className="font-medium">{employee.name || 'N/A'}</div>
                      {employee.nameBangla && (
                        <div className="text-sm text-blue-600 font-sutonnymj">{employee.nameBangla}</div>
                      )}
                      <div className="text-sm text-gray-500">ID: {employee.id}</div>
                    </div>
                  </div>
                </TableCell>
                <TableCell>
                  <div className="space-y-1">
                    <div className="text-sm">
                      <span className="font-medium">NID:</span> {employee.nidNo || 'N/A'}
                    </div>
                    {employee.dateOfBirth && (
                      <div className="text-sm text-gray-500">
                        DOB: {format(new Date(employee.dateOfBirth), 'dd/MM/yyyy')}
                      </div>
                    )}
                    {employee.fatherName && (
                      <div className="text-sm text-gray-500">
                        Father: {employee.fatherName || 'N/A'}
                      </div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="space-y-1">
                    <div className="flex items-center text-sm">
                      <IconBuilding className="h-3 w-3 mr-1" />
                      {employee.departmentName || 'N/A'}
                    </div>
                    <div className="text-sm text-gray-500">
                      Section: {employee.sectionName || 'N/A'}
                    </div>
                    <div className="flex items-center text-sm">
                      <IconBriefcase className="h-3 w-3 mr-1" />
                      {employee.designationName || 'N/A'}
                    </div>
                    {employee.joiningDate && (
                      <div className="text-sm text-gray-500">
                        Joined: {format(new Date(employee.joiningDate), 'dd/MM/yyyy')}
                      </div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="space-y-1">
                    <div className="flex items-center text-sm">
                      <IconCash className="h-3 w-3 mr-1" />
                      <span className="font-medium">৳{employee.grossSalary?.toLocaleString() || 0}</span>
                    </div>
                    <div className="text-sm text-gray-500">
                      Basic: ৳{employee.basicSalary?.toLocaleString() || 0}
                    </div>
                    {employee.bankAccountNo && (
                      <div className="text-sm text-gray-500">
                        Account: {employee.bankAccountNo}
                      </div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="flex items-center">
                    <Badge variant={employee.isActive ? "default" : "secondary"}>
                      {employee.isActive ? "Active" : "Inactive"}
                    </Badge>
                  </div>
                </TableCell>
                <TableCell className="text-right">
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <span className="sr-only">Open menu</span>
                        <IconEye className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuLabel>Actions</DropdownMenuLabel>
                      <DropdownMenuItem onClick={() => setViewEmployee(employee)}>
                        <IconEye className="mr-2 h-4 w-4" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onEdit(employee)}>
                        <IconEdit className="mr-2 h-4 w-4" />
                        Edit
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => openDeleteDialog(employee)}
                        className="text-red-600"
                      >
                        <IconTrash className="mr-2 h-4 w-4" />
                        Delete
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </div>

      {/* Employee Details Modal */}
      {viewEmployee && (
        <AlertDialog open={!!viewEmployee} onOpenChange={() => setViewEmployee(null)}>
          <AlertDialogContent className="max-w-4xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center space-x-3">
                <Avatar className="h-12 w-12">
                  <AvatarFallback>
                    {viewEmployee.name?.charAt(0) || ''}{viewEmployee.name?.split(' ')[1]?.charAt(0) || ''}
                  </AvatarFallback>
                </Avatar>
                <div>
                  <div>{viewEmployee.name || 'N/A'}</div>
                  {viewEmployee.nameBangla && (
                    <div className="text-blue-600 text-sm font-sutonnymj">{viewEmployee.nameBangla}</div>
                  )}
                  <div className="text-sm text-gray-500">Employee ID: {viewEmployee.id}</div>
                </div>
              </AlertDialogTitle>
            </AlertDialogHeader>
            
            <div className="space-y-6">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6 gap-y-6">
                {/* Personal Information */}
                <div>
                  <h4 className="font-semibold mb-3">Personal Information</h4>
                  <div className="space-y-2 text-sm">
                    <div><strong>Name:</strong> {viewEmployee.name || 'N/A'}</div>
                    <div><strong>Name (Bangla):</strong> {viewEmployee.nameBangla || 'N/A'}</div>
                    <div><strong>NID:</strong> {viewEmployee.nidNo || 'N/A'}</div>
                    <div><strong>Father:</strong> {viewEmployee.fatherName || 'N/A'}</div>
                    <div><strong>Mother:</strong> {viewEmployee.motherName || 'N/A'}</div>
                    <div><strong>DOB:</strong> {viewEmployee.dateOfBirth ? format(new Date(viewEmployee.dateOfBirth), 'PPP') : 'N/A'}</div>
                    <div><strong>Address:</strong> {viewEmployee.address || 'N/A'}</div>
                  </div>
                </div>

                {/* Work Information */}
                <div>
                  <h4 className="font-semibold mb-3">Work Information</h4>
                  <div className="space-y-2 text-sm">
                    <div><strong>Department:</strong> {viewEmployee.departmentName || 'N/A'}</div>
                    <div><strong>Section:</strong> {viewEmployee.sectionName || 'N/A'}</div>
                    <div><strong>Designation:</strong> {viewEmployee.designationName || 'N/A'}</div>
                    <div><strong>Grade:</strong> {viewEmployee.designationGrade || 'N/A'}</div>
                    <div><strong>Joining Date:</strong> {viewEmployee.joiningDate ? format(new Date(viewEmployee.joiningDate), 'PPP') : 'N/A'}</div>
                    <div><strong>Bank Account:</strong> {viewEmployee.bankAccountNo || 'N/A'}</div>
                  </div>
                </div>

                {/* Salary Information */}
                <div>
                  <h4 className="font-semibold mb-3">Salary Information</h4>
                  <div className="space-y-2 text-sm">
                    <div><strong>Gross Salary:</strong> ৳{viewEmployee.grossSalary?.toLocaleString() || 0}</div>
                    <div><strong>Basic Salary:</strong> ৳{viewEmployee.basicSalary?.toLocaleString() || 0}</div>
                    <div><strong>Status:</strong> 
                      <Badge variant={viewEmployee.isActive ? "default" : "secondary"} className="ml-2">
                        {viewEmployee.isActive ? "Active" : "Inactive"}
                      </Badge>
                    </div>
                  </div>
                </div>

                {/* Additional Information */}
                <div>
                  <h4 className="font-semibold mb-3">Additional Information</h4>
                  <div className="space-y-2 text-sm">
                    <div><strong>Father (Bangla):</strong> {viewEmployee.fatherNameBangla || 'N/A'}</div>
                    <div><strong>Mother (Bangla):</strong> {viewEmployee.motherNameBangla || 'N/A'}</div>
                    <div><strong>Created:</strong> {format(new Date(viewEmployee.createdAt), 'PPP')}</div>
                  </div>
                </div>
              </div>
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Close</AlertDialogCancel>
              <AlertDialogAction onClick={() => onEdit(viewEmployee)}>
                Edit Employee
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Employee</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{employeeToDelete?.name}&quot;? 
              This action will soft delete the employee. The employee record will be marked as inactive.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel disabled={deleting}>Cancel</AlertDialogCancel>
            <AlertDialogAction 
              onClick={handleDelete}
              disabled={deleting}
              className="bg-red-600 hover:bg-red-700"
            >
              {deleting ? "Deleting..." : "Delete"}
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  )
}
