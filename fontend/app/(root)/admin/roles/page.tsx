"use client"

import { useState, useEffect, useCallback } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { 
  IconUsers, 
  IconPlus, 
  IconSearch, 
  IconRefresh, 
  IconEdit, 
  IconTrash, 
  IconEye,
  IconFilter,
  IconShield,
  IconUser,
  IconCopy,
  IconSettings,
  IconCheck,
  IconX
} from "@tabler/icons-react"
import { cn } from "@/lib/utils"
import { 
  getRolePermissions,
  getRolePermissionsList,
  getRoleUsers,
  getUserRoles,
  assignPermissionToRole,
  removePermissionFromRole,
  bulkAssignPermissionsToRole,
  bulkRemovePermissionsFromRole,
  assignUserToRole,
  removeUserFromRole,
  syncRolePermissions,
  copyRolePermissions,
  type Permission,
  type RoleUsersResponse,
  type UserRolesResponse
} from "@/lib/api/permission"
import { getAllUsers, type User } from "@/lib/api/user"
import { toast } from "sonner"

// Mock roles data - in real app, this would come from API
const mockRoles = [
  { id: 1, name: "Super Admin", description: "Full system access", isActive: true, createdAt: "2024-01-01", updatedAt: "2024-01-01" },
  { id: 2, name: "Admin", description: "Administrative access", isActive: true, createdAt: "2024-01-01", updatedAt: "2024-01-01" },
  { id: 3, name: "HR Manager", description: "HR management access", isActive: true, createdAt: "2024-01-01", updatedAt: "2024-01-01" },
  { id: 4, name: "Employee", description: "Basic employee access", isActive: true, createdAt: "2024-01-01", updatedAt: "2024-01-01" },
  { id: 5, name: "Viewer", description: "Read-only access", isActive: true, createdAt: "2024-01-01", updatedAt: "2024-01-01" }
]

export default function RoleManagementPage() {
  const [roles] = useState(mockRoles)
  const [selectedRole, setSelectedRole] = useState<typeof mockRoles[0] | null>(null)
  const [rolePermissions, setRolePermissions] = useState<Permission[]>([])
  const [roleUsers, setRoleUsers] = useState<RoleUsersResponse | null>(null)
  const [allUsers, setAllUsers] = useState<User[]>([])
  const [loading, setLoading] = useState(false)
  const [searchTerm, setSearchTerm] = useState("")
  const [isPermissionDialogOpen, setIsPermissionDialogOpen] = useState(false)
  const [isUserDialogOpen, setIsUserDialogOpen] = useState(false)
  const [isCopyDialogOpen, setIsCopyDialogOpen] = useState(false)
  const [copySourceRole, setCopySourceRole] = useState<string>("")
  const [copyTargetRole, setCopyTargetRole] = useState<string>("")

  const loadRoleData = useCallback(async (roleId: number) => {
    if (!roleId) return
    
    setLoading(true)
    try {
      const [permissionsData, usersData] = await Promise.all([
        getRolePermissionsList(roleId),
        getRoleUsers(roleId)
      ])
      setRolePermissions(permissionsData)
      setRoleUsers(usersData)
    } catch (error) {
      console.error("Error loading role data:", error)
      toast.error("Failed to load role data")
    } finally {
      setLoading(false)
    }
  }, [])

  const loadUsers = useCallback(async () => {
    try {
      const response = await getAllUsers()
      setAllUsers(response.data.users)
    } catch (error) {
      console.error("Error loading users:", error)
      toast.error("Failed to load users")
    }
  }, [])

  const handleAssignPermission = async (roleId: number, permissionId: number) => {
    try {
      await assignPermissionToRole({ roleId, permissionId, assignedAt: new Date().toISOString(), assignedBy: 1 })
      toast.success("Permission assigned successfully")
      loadRoleData(roleId)
    } catch (error) {
      console.error("Error assigning permission:", error)
      toast.error("Failed to assign permission")
    }
  }

  const handleRemovePermission = async (roleId: number, permissionId: number) => {
    try {
      await removePermissionFromRole(roleId, permissionId)
      toast.success("Permission removed successfully")
      loadRoleData(roleId)
    } catch (error) {
      console.error("Error removing permission:", error)
      toast.error("Failed to remove permission")
    }
  }

  const handleAssignUser = async (roleId: number, userId: number) => {
    try {
      await assignUserToRole({ userId, roleId, assignedAt: new Date().toISOString(), assignedBy: 1 })
      toast.success("User assigned to role successfully")
      loadRoleData(roleId)
    } catch (error) {
      console.error("Error assigning user:", error)
      toast.error("Failed to assign user to role")
    }
  }

  const handleRemoveUser = async (userId: number, roleId: number) => {
    try {
      await removeUserFromRole(userId, roleId)
      toast.success("User removed from role successfully")
      loadRoleData(roleId)
    } catch (error) {
      console.error("Error removing user:", error)
      toast.error("Failed to remove user from role")
    }
  }

  const handleCopyPermissions = async () => {
    if (!copySourceRole || !copyTargetRole) {
      toast.error("Please select both source and target roles")
      return
    }

    try {
      await copyRolePermissions(parseInt(copySourceRole), parseInt(copyTargetRole))
      toast.success("Permissions copied successfully")
      setIsCopyDialogOpen(false)
      setCopySourceRole("")
      setCopyTargetRole("")
    } catch (error) {
      console.error("Error copying permissions:", error)
      toast.error("Failed to copy permissions")
    }
  }

  const filteredRoles = roles.filter(role =>
    role.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    role.description?.toLowerCase().includes(searchTerm.toLowerCase())
  )

  useEffect(() => {
    loadUsers()
  }, [loadUsers])

  useEffect(() => {
    if (selectedRole) {
      loadRoleData(selectedRole.id)
    }
  }, [selectedRole, loadRoleData])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Role Management</h1>
          <p className="text-muted-foreground">
            Manage user roles and their permissions
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            onClick={() => selectedRole && loadRoleData(selectedRole.id)}
            disabled={loading || !selectedRole}
          >
            <IconRefresh className={cn("w-4 h-4 mr-2", loading && "animate-spin")} />
            Refresh
          </Button>
          <Dialog open={isCopyDialogOpen} onOpenChange={setIsCopyDialogOpen}>
            <DialogTrigger asChild>
              <Button variant="outline">
                <IconCopy className="w-4 h-4 mr-2" />
                Copy Permissions
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Copy Role Permissions</DialogTitle>
                <DialogDescription>
                  Copy all permissions from one role to another
                </DialogDescription>
              </DialogHeader>
              <div className="space-y-4">
                <div>
                  <div className="text-sm font-medium mb-2">Source Role</div>
                  <Select value={copySourceRole} onValueChange={setCopySourceRole}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select source role" />
                    </SelectTrigger>
                    <SelectContent>
                      {roles.map((role) => (
                        <SelectItem key={role.id} value={role.id.toString()}>{role.name}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div>
                  <div className="text-sm font-medium mb-2">Target Role</div>
                  <Select value={copyTargetRole} onValueChange={setCopyTargetRole}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select target role" />
                    </SelectTrigger>
                    <SelectContent>
                      {roles.map((role) => (
                        <SelectItem key={role.id} value={role.id.toString()}>{role.name}</SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                </div>
                <div className="flex justify-end space-x-2">
                  <Button variant="outline" onClick={() => setIsCopyDialogOpen(false)}>
                    Cancel
                  </Button>
                  <Button onClick={handleCopyPermissions}>
                    Copy Permissions
                  </Button>
                </div>
              </div>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        {/* Roles List */}
        <Card className="lg:col-span-1">
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <IconUsers className="h-5 w-5" />
              <span>Roles</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="relative">
                <IconSearch className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Search roles..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
              <div className="space-y-2">
                {filteredRoles.map((role) => (
                  <div
                    key={role.id}
                    className={cn(
                      "p-3 border rounded-lg cursor-pointer transition-colors",
                      selectedRole?.id === role.id ? "bg-primary text-primary-foreground" : "hover:bg-muted"
                    )}
                    onClick={() => setSelectedRole(role)}
                  >
                    <div className="font-medium">{role.name}</div>
                    <div className="text-sm opacity-70">{role.description}</div>
                    <Badge variant={role.isActive ? "default" : "secondary"} className="mt-1">
                      {role.isActive ? "Active" : "Inactive"}
                    </Badge>
                  </div>
                ))}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Role Details */}
        <Card className="lg:col-span-2">
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <IconShield className="h-5 w-5" />
              <span>
                {selectedRole ? `${selectedRole.name} Details` : "Select a Role"}
              </span>
            </CardTitle>
            <CardDescription>
              {selectedRole ? `Manage permissions and users for ${selectedRole.name}` : "Choose a role to view and manage its details"}
            </CardDescription>
          </CardHeader>
          <CardContent>
            {selectedRole ? (
              <Tabs defaultValue="permissions" className="space-y-4">
                <TabsList>
                  <TabsTrigger value="permissions">Permissions</TabsTrigger>
                  <TabsTrigger value="users">Users</TabsTrigger>
                </TabsList>

                {/* Permissions Tab */}
                <TabsContent value="permissions" className="space-y-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="text-lg font-semibold">Role Permissions</h3>
                      <p className="text-sm text-muted-foreground">
                        {rolePermissions.length} permissions assigned
                      </p>
                    </div>
                    <Dialog open={isPermissionDialogOpen} onOpenChange={setIsPermissionDialogOpen}>
                      <DialogTrigger asChild>
                        <Button size="sm">
                          <IconPlus className="w-4 h-4 mr-2" />
                          Assign Permission
                        </Button>
                      </DialogTrigger>
                      <DialogContent>
                        <DialogHeader>
                          <DialogTitle>Assign Permission to Role</DialogTitle>
                          <DialogDescription>
                            Add a new permission to {selectedRole.name}
                          </DialogDescription>
                        </DialogHeader>
                        <PermissionAssignmentForm
                          roleId={selectedRole.id}
                          onAssign={handleAssignPermission}
                          onCancel={() => setIsPermissionDialogOpen(false)}
                        />
                      </DialogContent>
                    </Dialog>
                  </div>
                  <div className="rounded-md border">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Code</TableHead>
                          <TableHead>Name</TableHead>
                          <TableHead>Module</TableHead>
                          <TableHead>Action</TableHead>
                          <TableHead>Actions</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {rolePermissions.map((permission) => (
                          <TableRow key={permission.id}>
                            <TableCell className="font-mono text-sm">{permission.code}</TableCell>
                            <TableCell className="font-medium">{permission.name}</TableCell>
                            <TableCell>
                              <Badge variant="outline">{permission.module}</Badge>
                            </TableCell>
                            <TableCell>
                              <Badge variant="secondary">{permission.action}</Badge>
                            </TableCell>
                            <TableCell>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleRemovePermission(selectedRole.id, permission.id)}
                              >
                                <IconTrash className="h-4 w-4" />
                              </Button>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                </TabsContent>

                {/* Users Tab */}
                <TabsContent value="users" className="space-y-4">
                  <div className="flex items-center justify-between">
                    <div>
                      <h3 className="text-lg font-semibold">Role Users</h3>
                      <p className="text-sm text-muted-foreground">
                        {roleUsers?.users.length || 0} users assigned
                      </p>
                    </div>
                    <Dialog open={isUserDialogOpen} onOpenChange={setIsUserDialogOpen}>
                      <DialogTrigger asChild>
                        <Button size="sm">
                          <IconPlus className="w-4 h-4 mr-2" />
                          Assign User
                        </Button>
                      </DialogTrigger>
                      <DialogContent>
                        <DialogHeader>
                          <DialogTitle>Assign User to Role</DialogTitle>
                          <DialogDescription>
                            Add a user to {selectedRole.name}
                          </DialogDescription>
                        </DialogHeader>
                        <UserAssignmentForm
                          roleId={selectedRole.id}
                          users={allUsers}
                          onAssign={handleAssignUser}
                          onCancel={() => setIsUserDialogOpen(false)}
                        />
                      </DialogContent>
                    </Dialog>
                  </div>
                  <div className="rounded-md border">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Name</TableHead>
                          <TableHead>Email</TableHead>
                          <TableHead>Assigned</TableHead>
                          <TableHead>Actions</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {roleUsers?.users.map((user) => (
                          <TableRow key={user.id}>
                            <TableCell className="font-medium">
                              {user.firstName} {user.lastName}
                            </TableCell>
                            <TableCell>{user.email}</TableCell>
                            <TableCell className="text-sm text-muted-foreground">
                              {new Date(user.assignedAt).toLocaleDateString()}
                            </TableCell>
                            <TableCell>
                              <Button
                                variant="ghost"
                                size="sm"
                                onClick={() => handleRemoveUser(user.id, selectedRole.id)}
                              >
                                <IconTrash className="h-4 w-4" />
                              </Button>
                            </TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                </TabsContent>
              </Tabs>
            ) : (
              <div className="text-center py-8 text-muted-foreground">
                <IconUsers className="w-12 h-12 mx-auto mb-4 opacity-50" />
                <p>Select a role to view and manage its details</p>
              </div>
            )}
          </CardContent>
        </Card>
      </div>

      {loading && (
        <div className="flex items-center justify-center py-8">
          <IconRefresh className="w-6 h-6 animate-spin mr-2" />
          <span>Loading role data...</span>
        </div>
      )}
    </div>
  )
}

// Permission Assignment Form Component
function PermissionAssignmentForm({ 
  roleId, 
  onAssign, 
  onCancel 
}: { 
  roleId: number
  onAssign: (roleId: number, permissionId: number) => void
  onCancel: () => void
}) {
  const [selectedPermissionId, setSelectedPermissionId] = useState<string>("")
  const [permissions, setPermissions] = useState<Permission[]>([])

  useEffect(() => {
    // Load available permissions
    // In real app, this would filter out already assigned permissions
    const loadPermissions = async () => {
      try {
        // Mock permissions - in real app, get from API
        setPermissions([
          { id: 1, code: "USER_CREATE", name: "Create User", module: "User", action: "Create", isActive: true, createdAt: "", updatedAt: "" },
          { id: 2, code: "USER_READ", name: "Read User", module: "User", action: "Read", isActive: true, createdAt: "", updatedAt: "" },
          { id: 3, code: "USER_UPDATE", name: "Update User", module: "User", action: "Update", isActive: true, createdAt: "", updatedAt: "" },
          { id: 4, code: "USER_DELETE", name: "Delete User", module: "User", action: "Delete", isActive: true, createdAt: "", updatedAt: "" },
        ])
      } catch (error) {
        console.error("Error loading permissions:", error)
      }
    }
    loadPermissions()
  }, [])

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (selectedPermissionId) {
      onAssign(roleId, parseInt(selectedPermissionId))
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <div className="text-sm font-medium mb-2">Select Permission</div>
        <Select value={selectedPermissionId} onValueChange={setSelectedPermissionId}>
          <SelectTrigger>
            <SelectValue placeholder="Choose a permission" />
          </SelectTrigger>
          <SelectContent>
            {permissions.map((permission) => (
              <SelectItem key={permission.id} value={permission.id.toString()}>
                {permission.name} ({permission.code})
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
      <div className="flex justify-end space-x-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit" disabled={!selectedPermissionId}>
          Assign Permission
        </Button>
      </div>
    </form>
  )
}

// User Assignment Form Component
function UserAssignmentForm({ 
  roleId, 
  users, 
  onAssign, 
  onCancel 
}: { 
  roleId: number
  users: User[]
  onAssign: (roleId: number, userId: number) => void
  onCancel: () => void
}) {
  const [selectedUserId, setSelectedUserId] = useState<string>("")

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    if (selectedUserId) {
      onAssign(roleId, parseInt(selectedUserId))
    }
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div>
        <div className="text-sm font-medium mb-2">Select User</div>
        <Select value={selectedUserId} onValueChange={setSelectedUserId}>
          <SelectTrigger>
            <SelectValue placeholder="Choose a user" />
          </SelectTrigger>
          <SelectContent>
            {users.map((user) => (
              <SelectItem key={user.id} value={user.id.toString()}>
                {user.firstName} {user.lastName} ({user.email})
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>
      <div className="flex justify-end space-x-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit" disabled={!selectedUserId}>
          Assign User
        </Button>
      </div>
    </form>
  )
}
