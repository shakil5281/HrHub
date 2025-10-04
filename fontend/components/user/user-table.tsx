"use client"

import { useState } from "react"
import { format } from "date-fns"
import {
  IconEdit,
  IconTrash,
  IconEye,
  IconUser,
  IconMail,
  IconBuilding,
  IconBriefcase,
  IconToggleLeft,
  IconToggleRight,
  IconShield,
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
import { Avatar, AvatarFallback } from "@/components/ui/avatar"
import { Card, CardContent } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { deleteUser, updateUserStatus, type User } from "@/lib/api/user"
import { UserRolesDialog } from "./user-roles-dialog"

interface UserTableProps {
  users: User[]
  loading: boolean
  onEdit: (user: User) => void
  onRefresh: () => void
}

export function UserTable({ users, loading, onEdit, onRefresh }: UserTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [userToDelete, setUserToDelete] = useState<User | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [viewUser, setViewUser] = useState<User | null>(null)
  const [updatingStatus, setUpdatingStatus] = useState<string | null>(null)
  const [rolesDialogOpen, setRolesDialogOpen] = useState(false)
  const [userForRoles, setUserForRoles] = useState<User | null>(null)

  const handleDelete = async () => {
    if (!userToDelete?.id) return

    setDeleting(true)
    try {
      await deleteUser(userToDelete.id)
      onRefresh()
      setDeleteDialogOpen(false)
      setUserToDelete(null)
    } catch (error) {
      console.error('Error deleting user:', error)
    } finally {
      setDeleting(false)
    }
  }

  const openDeleteDialog = (user: User) => {
    setUserToDelete(user)
    setDeleteDialogOpen(true)
  }

  const handleStatusToggle = async (user: User) => {
    setUpdatingStatus(user.id)
    try {
      await updateUserStatus(user.id, !user.isActive)
      onRefresh()
    } catch (error) {
      console.error('Error updating user status:', error)
    } finally {
      setUpdatingStatus(null)
    }
  }

  const openRolesDialog = (user: User) => {
    setUserForRoles(user)
    setRolesDialogOpen(true)
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

  if (users.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <IconUser className="h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">No users found</h3>
          <p className="text-gray-500 text-center max-w-sm">
            Get started by creating your first user. Users can access the system and manage their profiles.
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
              <TableHead>User</TableHead>
              <TableHead>Contact</TableHead>
              <TableHead>Company</TableHead>
              <TableHead>Department</TableHead>
              <TableHead>Position</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Roles</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {users.map((user) => (
              <TableRow key={user.id}>
                <TableCell>
                  <div className="flex items-center space-x-3">
                    <Avatar className="h-10 w-10">
                      <AvatarFallback>
                        {user.firstName?.charAt(0) || ''}{user.lastName?.charAt(0) || ''}
                      </AvatarFallback>
                    </Avatar>
                    <div>
                      <div className="font-medium">{user.firstName || ''} {user.lastName || ''}</div>
                      <div className="text-sm text-gray-500">ID: {user.id}</div>
                    </div>
                  </div>
                </TableCell>
                <TableCell>
                  <div className="flex items-center text-sm">
                    <IconMail className="h-3 w-3 mr-1" />
                    {user.email || 'N/A'}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="flex items-center text-sm">
                    <IconBuilding className="h-3 w-3 mr-1" />
                    {user.companyName || 'N/A'}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">{user.department || 'N/A'}</div>
                </TableCell>
                <TableCell>
                  <div className="flex items-center text-sm">
                    <IconBriefcase className="h-3 w-3 mr-1" />
                    {user.position || 'N/A'}
                  </div>
                </TableCell>
                <TableCell>
                  <Button
                    variant="ghost"
                    size="sm"
                    onClick={() => handleStatusToggle(user)}
                    disabled={updatingStatus === user.id}
                    className="p-0 h-auto"
                  >
                    {user.isActive ? (
                      <IconToggleRight className="h-5 w-5 text-green-600" />
                    ) : (
                      <IconToggleLeft className="h-5 w-5 text-gray-400" />
                    )}
                  </Button>
                  <span className={`ml-2 text-xs ${user.isActive ? 'text-green-600' : 'text-gray-500'}`}>
                    {user.isActive ? 'Active' : 'Inactive'}
                  </span>
                </TableCell>
                <TableCell>
                  <div className="flex flex-wrap gap-1">
                    {user.roles?.slice(0, 2).map((role, index) => (
                      <Badge key={index} variant="secondary" className="text-xs">
                        {role}
                      </Badge>
                    )) || []}
                    {user.roles && user.roles.length > 2 && (
                      <Badge variant="outline" className="text-xs">
                        +{user.roles.length - 2}
                      </Badge>
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
                      <DropdownMenuItem onClick={() => setViewUser(user)}>
                        <IconEye className="mr-2 h-4 w-4" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => onEdit(user)}>
                        <IconEdit className="mr-2 h-4 w-4" />
                        Edit
                      </DropdownMenuItem>
                      <DropdownMenuItem onClick={() => openRolesDialog(user)}>
                        <IconShield className="mr-2 h-4 w-4" />
                        Manage Roles
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => openDeleteDialog(user)}
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

      {/* User Details Modal */}
      {viewUser && (
        <AlertDialog open={!!viewUser} onOpenChange={() => setViewUser(null)}>
          <AlertDialogContent className="max-w-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center space-x-3">
                <Avatar className="h-12 w-12">
                  <AvatarFallback>
                    {viewUser.firstName?.charAt(0) || ''}{viewUser.lastName?.charAt(0) || ''}
                  </AvatarFallback>
                </Avatar>
                <div>
                  <div>{viewUser.firstName || ''} {viewUser.lastName || ''}</div>
                  <div className="text-sm text-gray-500">{viewUser.email || 'N/A'}</div>
                </div>
              </AlertDialogTitle>
            </AlertDialogHeader>
            
              <div className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <h4 className="font-semibold mb-2">Personal Information</h4>
                    <div className="space-y-2 text-sm">
                      <div><strong>First Name:</strong> {viewUser.firstName || 'N/A'}</div>
                      <div><strong>Last Name:</strong> {viewUser.lastName || 'N/A'}</div>
                      <div><strong>Email:</strong> {viewUser.email || 'N/A'}</div>
                      <div><strong>User ID:</strong> {viewUser.id}</div>
                      <div><strong>Status:</strong> 
                        <span className={`ml-1 ${viewUser.isActive ? 'text-green-600' : 'text-gray-500'}`}>
                          {viewUser.isActive ? 'Active' : 'Inactive'}
                        </span>
                      </div>
                    </div>
                  </div>

                  <div>
                    <h4 className="font-semibold mb-2">Work Information</h4>
                    <div className="space-y-2 text-sm">
                      <div><strong>Company:</strong> {viewUser.companyName || 'N/A'}</div>
                      <div><strong>Department:</strong> {viewUser.department || 'N/A'}</div>
                      <div><strong>Position:</strong> {viewUser.position || 'N/A'}</div>
                      <div><strong>Company ID:</strong> {viewUser.companyId || 'N/A'}</div>
                    </div>
                  </div>
                </div>

              <div>
                <h4 className="font-semibold mb-2">Roles & Permissions</h4>
                <div className="flex flex-wrap gap-2">
                  {viewUser.roles?.map((role, index) => (
                    <Badge key={index} variant="secondary">
                      {role}
                    </Badge>
                  )) || <span className="text-sm text-gray-500">No roles assigned</span>}
                </div>
              </div>

              <div>
                <h4 className="font-semibold mb-2">Timestamps</h4>
                <div className="space-y-1 text-sm">
                  <div><strong>Created:</strong> {viewUser.createdAt ? format(new Date(viewUser.createdAt), 'PPP') : 'N/A'}</div>
                  <div><strong>Updated:</strong> {viewUser.updatedAt ? format(new Date(viewUser.updatedAt), 'PPP') : 'N/A'}</div>
                </div>
              </div>
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Close</AlertDialogCancel>
              <AlertDialogAction onClick={() => onEdit(viewUser)}>
                Edit User
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete User</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{userToDelete?.firstName} {userToDelete?.lastName}&quot;? 
              This action cannot be undone. All user data will be permanently removed.
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

      {/* User Roles Management Dialog */}
      <UserRolesDialog
        user={userForRoles}
        open={rolesDialogOpen}
        onOpenChange={setRolesDialogOpen}
        onSuccess={onRefresh}
      />
    </>
  )
}
