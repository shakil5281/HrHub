"use client"

import { useState } from "react"
import { format } from "date-fns"
import {
  IconEdit,
  IconTrash,
  IconEye,
  IconBuilding,
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
import { deleteSection, type Section } from "@/lib/api/section"

interface SectionTableProps {
  sections: Section[]
  loading: boolean
  onEdit: (section: Section) => void
  onRefresh: () => void
}

export function SectionTable({ sections, loading, onEdit, onRefresh }: SectionTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [sectionToDelete, setSectionToDelete] = useState<Section | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [viewSection, setViewSection] = useState<Section | null>(null)

  const handleDelete = async () => {
    const sectionId = sectionToDelete?.id
    if (!sectionId) return

    setDeleting(true)
    try {
      await deleteSection(sectionId)
      onRefresh()
      setDeleteDialogOpen(false)
      setSectionToDelete(null)
    } catch (error) {
      console.error('Error deleting section:', error)
    } finally {
      setDeleting(false)
    }
  }

  const openDeleteDialog = (section: Section) => {
    setSectionToDelete(section)
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

  if (sections.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <IconBuilding className="h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">No sections found</h3>
          <p className="text-gray-500 text-center max-w-sm">
            Get started by creating your first section. Sections help organize departments within your company.
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
              <TableHead>Section</TableHead>
              <TableHead>Department</TableHead>
              <TableHead>Status</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {sections.map((section) => (
              <TableRow key={section.id}>
                <TableCell>
                  <div>
                    <div className="font-medium">{section.name}</div>
                    {section.nameBangla && (
                      <div className="text-sm text-blue-600 font-sutonnymj">{section.nameBangla}</div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    {section.departmentName || `Department ${section.departmentId}`}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    {section.isActive ? (
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
                      <DropdownMenuItem onClick={() => setViewSection(section)}>
                        <IconEye className="mr-2 h-4 w-4" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onEdit(section)}>
                        <IconEdit className="mr-2 h-4 w-4" />
                        Edit
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => openDeleteDialog(section)}
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

      {/* Section Details Modal */}
      {viewSection && (
        <AlertDialog open={!!viewSection} onOpenChange={() => setViewSection(null)}>
          <AlertDialogContent className="max-w-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center space-x-3">
                <IconBuilding className="h-6 w-6" />
                <div>
                  <div>{viewSection.name}</div>
                  {viewSection.nameBangla && (
                    <div className="text-blue-600 text-sm font-sutonnymj">{viewSection.nameBangla}</div>
                  )}
                </div>
              </AlertDialogTitle>
            </AlertDialogHeader>
            
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Department</h4>
                  <div className="text-sm text-gray-600">
                    {viewSection.departmentName || `Department ${viewSection.departmentId}`}
                  </div>
                </div>

                <div>
                  <h4 className="font-semibold mb-2">Status</h4>
                  <div className="text-sm">
                    {viewSection.isActive ? (
                      <span className="text-green-600">Active</span>
                    ) : (
                      <span className="text-red-600">Inactive</span>
                    )}
                  </div>
                </div>
              </div>

              {viewSection.createdAt && (
                <div>
                  <h4 className="font-semibold mb-2">Created</h4>
                  <div className="text-sm text-gray-600">
                    {format(new Date(viewSection.createdAt), 'PPP')}
                  </div>
                </div>
              )}
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Close</AlertDialogCancel>
              <AlertDialogAction onClick={() => onEdit(viewSection)}>
                Edit Section
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Section</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{sectionToDelete?.name}&quot;? This action cannot be undone.
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
