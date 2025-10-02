"use client"

import { useState } from "react"
import { format } from "date-fns"
import {
  IconEdit,
  IconTrash,
  IconEye,
  IconBuilding,
  IconMail,
  IconPhone,
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
import { deleteCompany, type Company } from "@/lib/api/company"

interface CompanyTableProps {
  companies: Company[]
  loading: boolean
  onEdit: (company: Company) => void
  onRefresh: () => void
}

export function CompanyTable({ companies, loading, onEdit, onRefresh }: CompanyTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [companyToDelete, setCompanyToDelete] = useState<Company | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [viewCompany, setViewCompany] = useState<Company | null>(null)

  const handleDelete = async () => {
    const companyId = companyToDelete?.id || companyToDelete?.companyId
    if (!companyId) return

    setDeleting(true)
    try {
      await deleteCompany(companyId)
      onRefresh()
      setDeleteDialogOpen(false)
      setCompanyToDelete(null)
    } catch (error) {
      console.error('Error deleting company:', error)
    } finally {
      setDeleting(false)
    }
  }

  const openDeleteDialog = (company: Company) => {
    setCompanyToDelete(company)
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

  if (companies.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <IconBuilding className="h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">No companies found</h3>
          <p className="text-gray-500 text-center max-w-sm">
            Get started by creating your first company. Companies help organize your HR data and employees.
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
              <TableHead>Company</TableHead>
              <TableHead>Contact</TableHead>
              <TableHead>Location</TableHead>
              <TableHead>Signature</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {companies.map((company) => (
              <TableRow key={company.id || company.companyId}>
                <TableCell>
                  <div className="flex items-center space-x-3">
                    <Avatar className="h-10 w-10">
                      <AvatarImage src={company.logoUrl} alt={company.name} />
                      <AvatarFallback>
                        {company.name.substring(0, 2).toUpperCase()}
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <div className="font-medium">{company.name}</div>
                      {company.companyNameBangla && (
                        <div className="text-sm text-blue-600 font-sutonnymj">{company.companyNameBangla}</div>
                      )}
                      <div className="text-sm text-gray-500 truncate max-w-[200px]">
                        {company.description}
                      </div>
                    </div>
                  </div>
                </TableCell>
                <TableCell>
                  <div className="space-y-1">
                    <div className="flex items-center text-sm">
                      <IconMail className="h-3 w-3 mr-1" />
                      {company.email}
                    </div>
                    <div className="flex items-center text-sm text-gray-500">
                      <IconPhone className="h-3 w-3 mr-1" />
                      {company.phone}
                    </div>
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    <div>{company.city}, {company.state}</div>
                    <div className="text-gray-500">{company.country}</div>
                    {company.addressBangla && (
                      <div className="text-blue-600 text-xs font-sutonnymj">{company.addressBangla}</div>
                    )}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    {company.authorizedSignature}
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
                      <DropdownMenuItem onClick={() => setViewCompany(company)}>
                        <IconEye className="mr-2 h-4 w-4" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onEdit(company)}>
                        <IconEdit className="mr-2 h-4 w-4" />
                        Edit
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => openDeleteDialog(company)}
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

      {/* Company Details Modal */}
      {viewCompany && (
        <AlertDialog open={!!viewCompany} onOpenChange={() => setViewCompany(null)}>
          <AlertDialogContent className="max-w-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center space-x-3">
                <Avatar className="h-12 w-12">
                  <AvatarImage src={viewCompany.logoUrl} alt={viewCompany.name} />
                  <AvatarFallback>
                    {viewCompany.name.substring(0, 2).toUpperCase()}
                  </AvatarFallback>
                </Avatar>
                <div>
                  <div>{viewCompany.name}</div>
                  {viewCompany.companyNameBangla && (
                    <div className="text-blue-600 text-sm font-sutonnymj">{viewCompany.companyNameBangla}</div>
                  )}
                </div>
              </AlertDialogTitle>
            </AlertDialogHeader>
            
            <div className="space-y-4">
              <div>
                <h4 className="font-semibold mb-2">Description</h4>
                <p className="text-sm text-gray-600">{viewCompany.description}</p>
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Contact Information</h4>
                  <div className="space-y-2 text-sm">
                    <div className="flex items-center">
                      <IconMail className="h-4 w-4 mr-2" />
                      {viewCompany.email}
                    </div>
                    <div className="flex items-center">
                      <IconPhone className="h-4 w-4 mr-2" />
                      {viewCompany.phone}
                    </div>
                  </div>
                </div>

                <div>
                  <h4 className="font-semibold mb-2">Address</h4>
                  <div className="text-sm text-gray-600">
                    <div>{viewCompany.address}</div>
                    {viewCompany.addressBangla && (
                      <div className="text-blue-600 font-sutonnymj">{viewCompany.addressBangla}</div>
                    )}
                    <div>{viewCompany.city}, {viewCompany.state} {viewCompany.postalCode}</div>
                    <div>{viewCompany.country}</div>
                  </div>
                </div>
              </div>

              <div className="grid grid-cols-1 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Additional Details</h4>
                  <div className="space-y-1 text-sm">
                    <div><strong>Authorized Signature:</strong> {viewCompany.authorizedSignature}</div>
                    {viewCompany.createdAt && (
                      <div><strong>Added:</strong> {format(new Date(viewCompany.createdAt), 'PPP')}</div>
                    )}
                  </div>
                </div>
              </div>
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Close</AlertDialogCancel>
              <AlertDialogAction onClick={() => onEdit(viewCompany)}>
                Edit Company
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Company</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{companyToDelete?.name}&quot;? This action cannot be undone.
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
