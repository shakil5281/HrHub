"use client"

import { useState } from "react"
import { format } from "date-fns"
import {
  IconEdit,
  IconTrash,
  IconEye,
  IconBuilding,
  IconStar,
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
import { Badge } from "@/components/ui/badge"
import { Card, CardContent } from "@/components/ui/card"
import { deleteDesignation, type Designation } from "@/lib/api/designation"

interface DesignationTableProps {
  designations: Designation[]
  loading: boolean
  onEdit: (designation: Designation) => void
  onRefresh: () => void
}

export function DesignationTable({ designations, loading, onEdit, onRefresh }: DesignationTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [designationToDelete, setDesignationToDelete] = useState<Designation | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [viewDesignation, setViewDesignation] = useState<Designation | null>(null)

  const handleDelete = async () => {
    const designationId = designationToDelete?.id
    if (!designationId) return

    setDeleting(true)
    try {
      await deleteDesignation(designationId)
      onRefresh()
      setDeleteDialogOpen(false)
      setDesignationToDelete(null)
    } catch (error) {
      console.error('Error deleting designation:', error)
    } finally {
      setDeleting(false)
    }
  }

  const openDeleteDialog = (designation: Designation) => {
    setDesignationToDelete(designation)
    setDeleteDialogOpen(true)
  }

  const getGradeBadgeColor = (grade: string) => {
    const upperGrade = grade.toUpperCase()
    if (upperGrade.includes('A') || upperGrade.includes('1ST')) return "bg-purple-100 text-purple-800"
    if (upperGrade.includes('B') || upperGrade.includes('2ND')) return "bg-blue-100 text-blue-800"
    if (upperGrade.includes('C') || upperGrade.includes('3RD')) return "bg-green-100 text-green-800"
    if (upperGrade.includes('D') || upperGrade.includes('4TH')) return "bg-yellow-100 text-yellow-800"
    return "bg-gray-100 text-gray-800"
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

  if (designations.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <IconBuilding className="h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">No designations found</h3>
          <p className="text-gray-500 text-center max-w-sm">
            Get started by creating your first designation. Designations define roles and hierarchy within departments.
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
              <TableHead>Designation</TableHead>
              <TableHead>Grade</TableHead>
              <TableHead>Section</TableHead>
              <TableHead>Attendance Bonus</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {designations.map((designation) => (
              <TableRow key={designation.id}>
                <TableCell>
                  <div>
                    <div className="font-medium">{designation.name}</div>
                    {designation.nameBangla && (
                      <div className="text-sm text-blue-600 font-sutonnymj">{designation.nameBangla}</div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <Badge className={getGradeBadgeColor(designation.grade)}>
                    <IconStar className="h-3 w-3 mr-1" />
                    {designation.grade}
                  </Badge>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    <div>{designation.sectionName || `Section ${designation.sectionId}`}</div>
                    {designation.departmentName && (
                      <div className="text-xs text-gray-500">{designation.departmentName}</div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm font-medium">
                    {designation.attendanceBonus.toLocaleString()}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    {designation.isActive ? (
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
                      <DropdownMenuItem onClick={() => setViewDesignation(designation)}>
                        <IconEye className="mr-2 h-4 w-4" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onEdit(designation)}>
                        <IconEdit className="mr-2 h-4 w-4" />
                        Edit
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => openDeleteDialog(designation)}
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

      {/* Designation Details Modal */}
      {viewDesignation && (
        <AlertDialog open={!!viewDesignation} onOpenChange={() => setViewDesignation(null)}>
          <AlertDialogContent className="max-w-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center space-x-3">
                <IconBuilding className="h-6 w-6" />
                <div>
                  <div>{viewDesignation.name}</div>
                  {viewDesignation.nameBangla && (
                    <div className="text-blue-600 text-sm font-sutonnymj">{viewDesignation.nameBangla}</div>
                  )}
                </div>
              </AlertDialogTitle>
            </AlertDialogHeader>
            
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Grade</h4>
                  <Badge className={getGradeBadgeColor(viewDesignation.grade)}>
                    <IconStar className="h-3 w-3 mr-1" />
                    {viewDesignation.grade}
                  </Badge>
                </div>

                <div>
                  <h4 className="font-semibold mb-2">Attendance Bonus</h4>
                  <div className="text-sm text-gray-600">
                    {viewDesignation.attendanceBonus.toLocaleString()}
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Section</h4>
                  <div className="text-sm text-gray-600">
                    {viewDesignation.sectionName || `Section ${viewDesignation.sectionId}`}
                  </div>
                </div>

                <div>
                  <h4 className="font-semibold mb-2">Department</h4>
                  <div className="text-sm text-gray-600">
                    {viewDesignation.departmentName || 'N/A'}
                  </div>
                </div>
              </div>

              <div>
                <h4 className="font-semibold mb-2">Status</h4>
                <div className="text-sm">
                  {viewDesignation.isActive ? (
                    <span className="text-green-600">Active</span>
                  ) : (
                    <span className="text-red-600">Inactive</span>
                  )}
                </div>
              </div>

              {viewDesignation.createdAt && (
                <div>
                  <h4 className="font-semibold mb-2">Created</h4>
                  <div className="text-sm text-gray-600">
                    {format(new Date(viewDesignation.createdAt), 'PPP')}
                  </div>
                </div>
              )}
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Close</AlertDialogCancel>
              <AlertDialogAction onClick={() => onEdit(viewDesignation)}>
                Edit Designation
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Designation</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{designationToDelete?.name}&quot;? This action cannot be undone.
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
