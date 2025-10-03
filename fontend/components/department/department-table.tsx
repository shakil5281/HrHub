"use client"

import { useState } from "react"
import { format } from "date-fns"
import {
  IconEdit,
  IconTrash,
  IconEye,
  IconBuilding,
  IconCode,
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
import { Card, CardContent } from "@/components/ui/card"
import { deleteDepartment, type Department } from "@/lib/api/department"

interface DepartmentTableProps {
  departments: Department[]
  loading: boolean
  onEdit: (department: Department) => void
  onRefresh: () => void
}

export function DepartmentTable({ departments, loading, onEdit, onRefresh }: DepartmentTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [departmentToDelete, setDepartmentToDelete] = useState<Department | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [viewDepartment, setViewDepartment] = useState<Department | null>(null)

  const handleDelete = async () => {
    const departmentId = departmentToDelete?.id
    if (!departmentId) return

    setDeleting(true)
    try {
      await deleteDepartment(departmentId)
      onRefresh()
      setDeleteDialogOpen(false)
      setDepartmentToDelete(null)
    } catch (error) {
      console.error('Error deleting department:', error)
    } finally {
      setDeleting(false)
    }
  }

  const openDeleteDialog = (department: Department) => {
    setDepartmentToDelete(department)
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

  if (departments.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <IconBuilding className="h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">No departments found</h3>
          <p className="text-gray-500 text-center max-w-sm">
            Get started by creating your first department. Departments help organize your company structure.
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
              <TableHead>Department</TableHead>
              <TableHead>Company</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {departments.map((department) => (
              <TableRow key={department.id}>
                <TableCell>
                  <div>
                    <div className="font-medium">{department.name}</div>
                    {department.nameBangla && (
                      <div className="text-sm text-blue-600 font-sutonnymj">{department.nameBangla}</div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="flex items-center text-sm">
                    <IconCode className="h-3 w-3 mr-1" />
                    {department.companyName || `Company ${department.companyId}`}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    {department.isActive ? (
                      <span className="text-green-600">Active</span>
                    ) : (
                      <span className="text-red-600">Inactive</span>
                    )}
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
                      <DropdownMenuItem onClick={() => setViewDepartment(department)}>
                        <IconEye className="mr-2 h-4 w-4" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onEdit(department)}>
                        <IconEdit className="mr-2 h-4 w-4" />
                        Edit
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => openDeleteDialog(department)}
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

      {/* Department Details Modal */}
      {viewDepartment && (
        <AlertDialog open={!!viewDepartment} onOpenChange={() => setViewDepartment(null)}>
          <AlertDialogContent className="max-w-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center space-x-3">
                <IconBuilding className="h-6 w-6" />
                <div>
                  <div>{viewDepartment.name}</div>
                  {viewDepartment.nameBangla && (
                    <div className="text-blue-600 text-sm font-sutonnymj">{viewDepartment.nameBangla}</div>
                  )}
                </div>
              </AlertDialogTitle>
            </AlertDialogHeader>
            
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Company</h4>
                  <div className="text-sm text-gray-600">
                    {viewDepartment.companyName || `Company ID: ${viewDepartment.companyId}`}
                  </div>
                </div>

                <div>
                  <h4 className="font-semibold mb-2">Status</h4>
                  <div className="text-sm">
                    {viewDepartment.isActive ? (
                      <span className="text-green-600">Active</span>
                    ) : (
                      <span className="text-red-600">Inactive</span>
                    )}
                  </div>
                </div>
              </div>

              {viewDepartment.createdAt && (
                <div>
                  <h4 className="font-semibold mb-2">Created</h4>
                  <div className="text-sm text-gray-600">
                    {format(new Date(viewDepartment.createdAt), 'PPP')}
                  </div>
                </div>
              )}
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Close</AlertDialogCancel>
              <AlertDialogAction onClick={() => onEdit(viewDepartment)}>
                Edit Department
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Department</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{departmentToDelete?.name}&quot;? This action cannot be undone.
              All associated data will be permanently removed.
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
