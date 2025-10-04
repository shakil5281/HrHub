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
  IconShield, 
  IconPlus, 
  IconSearch, 
  IconRefresh, 
  IconEdit, 
  IconTrash, 
  IconEye,
  IconFilter,
  IconCode,
  IconActivity,
  IconCheck,
  IconX
} from "@tabler/icons-react"
import { cn } from "@/lib/utils"
import { 
  getPermissions,
  getPermissionById,
  createPermission,
  updatePermission,
  deletePermission,
  getPermissionsByModule,
  getPermissionsByAction,
  checkPermission,
  type Permission,
  type PermissionCreateRequest,
  type PermissionUpdateRequest,
  type PermissionCheckRequest,
  type PermissionCheckResponse
} from "@/lib/api/permission"
import { toast } from "sonner"

export default function PermissionManagementPage() {
  const [permissions, setPermissions] = useState<Permission[]>([])
  const [filteredPermissions, setFilteredPermissions] = useState<Permission[]>([])
  const [loading, setLoading] = useState(false)
  const [searchTerm, setSearchTerm] = useState("")
  const [moduleFilter, setModuleFilter] = useState<string>("all")
  const [actionFilter, setActionFilter] = useState<string>("all")
  const [statusFilter, setStatusFilter] = useState<string>("all")
  const [selectedPermission, setSelectedPermission] = useState<Permission | null>(null)
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false)
  const [isEditDialogOpen, setIsEditDialogOpen] = useState(false)
  const [isViewDialogOpen, setIsViewDialogOpen] = useState(false)
  const [isCheckDialogOpen, setIsCheckDialogOpen] = useState(false)
  const [permissionCheck, setPermissionCheck] = useState<PermissionCheckResponse | null>(null)
  const [checkUserId, setCheckUserId] = useState<string>("")
  const [checkPermissionCode, setCheckPermissionCode] = useState<string>("")

  // Mock modules and actions for filters
  const modules = ["User", "Employee", "Attendance", "Company", "Department", "System", "Backup", "Report"]
  const actions = ["Create", "Read", "Update", "Delete", "Export", "Import", "Manage", "View"]

  const loadPermissions = useCallback(async () => {
    setLoading(true)
    try {
      const response = await getPermissions({
        page: 1,
        pageSize: 1000 // Get all permissions for now
      })
      setPermissions(response.data)
      setFilteredPermissions(response.data)
    } catch (error) {
      console.error("Error loading permissions:", error)
      toast.error("Failed to load permissions")
    } finally {
      setLoading(false)
    }
  }, [])

  const handleCreatePermission = async (data: PermissionCreateRequest) => {
    try {
      await createPermission(data)
      toast.success("Permission created successfully")
      setIsCreateDialogOpen(false)
      loadPermissions()
    } catch (error) {
      console.error("Error creating permission:", error)
      toast.error("Failed to create permission")
    }
  }

  const handleUpdatePermission = async (id: number, data: PermissionUpdateRequest) => {
    try {
      await updatePermission(id, data)
      toast.success("Permission updated successfully")
      setIsEditDialogOpen(false)
      setSelectedPermission(null)
      loadPermissions()
    } catch (error) {
      console.error("Error updating permission:", error)
      toast.error("Failed to update permission")
    }
  }

  const handleDeletePermission = async (id: number) => {
    try {
      await deletePermission(id)
      toast.success("Permission deleted successfully")
      loadPermissions()
    } catch (error) {
      console.error("Error deleting permission:", error)
      toast.error("Failed to delete permission")
    }
  }

  const handleCheckPermission = async () => {
    if (!checkUserId || !checkPermissionCode) {
      toast.error("Please enter both User ID and Permission Code")
      return
    }

    try {
      const response = await checkPermission({
        userId: parseInt(checkUserId),
        permissionCode: checkPermissionCode
      })
      setPermissionCheck(response)
    } catch (error) {
      console.error("Error checking permission:", error)
      toast.error("Failed to check permission")
    }
  }

  const handleFilter = () => {
    let filtered = permissions || []

    if (searchTerm) {
      filtered = filtered.filter(permission =>
        permission.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        permission.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
        permission.description?.toLowerCase().includes(searchTerm.toLowerCase())
      )
    }

    if (moduleFilter && moduleFilter !== "all") {
      filtered = filtered.filter(permission => permission.module === moduleFilter)
    }

    if (actionFilter && actionFilter !== "all") {
      filtered = filtered.filter(permission => permission.action === actionFilter)
    }

    if (statusFilter && statusFilter !== "all") {
      const isActive = statusFilter === "active"
      filtered = filtered.filter(permission => permission.isActive === isActive)
    }

    setFilteredPermissions(filtered)
  }

  const clearFilters = () => {
    setSearchTerm("")
    setModuleFilter("all")
    setActionFilter("all")
    setStatusFilter("all")
    setFilteredPermissions(permissions || [])
  }

  useEffect(() => {
    loadPermissions()
  }, [loadPermissions])

  useEffect(() => {
    handleFilter()
  }, [searchTerm, moduleFilter, actionFilter, statusFilter, permissions])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Permission Management</h1>
          <p className="text-muted-foreground">
            Manage system permissions and access controls
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            onClick={loadPermissions}
            disabled={loading}
          >
            <IconRefresh className={cn("w-4 h-4 mr-2", loading && "animate-spin")} />
            Refresh
          </Button>
          <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
            <DialogTrigger asChild>
              <Button>
                <IconPlus className="w-4 h-4 mr-2" />
                Create Permission
              </Button>
            </DialogTrigger>
            <DialogContent>
              <DialogHeader>
                <DialogTitle>Create New Permission</DialogTitle>
                <DialogDescription>
                  Add a new permission to the system
                </DialogDescription>
              </DialogHeader>
              <PermissionCreateForm
                onSubmit={handleCreatePermission}
                onCancel={() => setIsCreateDialogOpen(false)}
              />
            </DialogContent>
          </Dialog>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <IconFilter className="h-5 w-5" />
            <span>Filters</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
            <div>
              <div className="text-sm font-medium mb-2">Search</div>
              <div className="relative">
                <IconSearch className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
                <Input
                  placeholder="Search permissions..."
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                  className="pl-10"
                />
              </div>
            </div>
            <div>
              <div className="text-sm font-medium mb-2">Module</div>
              <Select value={moduleFilter} onValueChange={setModuleFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All modules" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All modules</SelectItem>
                  {modules.map((module) => (
                    <SelectItem key={module} value={module}>{module}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div>
              <div className="text-sm font-medium mb-2">Action</div>
              <Select value={actionFilter} onValueChange={setActionFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All actions" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All actions</SelectItem>
                  {actions.map((action) => (
                    <SelectItem key={action} value={action}>{action}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            <div>
              <div className="text-sm font-medium mb-2">Status</div>
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger>
                  <SelectValue placeholder="All status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All status</SelectItem>
                  <SelectItem value="active">Active</SelectItem>
                  <SelectItem value="inactive">Inactive</SelectItem>
                </SelectContent>
              </Select>
            </div>
            <div className="flex items-end">
              <Button variant="outline" onClick={clearFilters} className="w-full">
                Clear Filters
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Permissions Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <IconShield className="h-5 w-5" />
            <span>Permissions ({filteredPermissions?.length || 0})</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="rounded-md border">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Code</TableHead>
                  <TableHead>Name</TableHead>
                  <TableHead>Module</TableHead>
                  <TableHead>Action</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Created</TableHead>
                  <TableHead>Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {(filteredPermissions || []).map((permission) => (
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
                      <Badge variant={permission.isActive ? "default" : "secondary"}>
                        {permission.isActive ? "Active" : "Inactive"}
                      </Badge>
                    </TableCell>
                    <TableCell className="text-sm text-muted-foreground">
                      {new Date(permission.createdAt).toLocaleDateString()}
                    </TableCell>
                    <TableCell>
                      <div className="flex items-center space-x-2">
                        <Dialog open={isViewDialogOpen} onOpenChange={setIsViewDialogOpen}>
                          <DialogTrigger asChild>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => setSelectedPermission(permission)}
                            >
                              <IconEye className="h-4 w-4" />
                            </Button>
                          </DialogTrigger>
                          <DialogContent>
                            <DialogHeader>
                              <DialogTitle>Permission Details</DialogTitle>
                              <DialogDescription>
                                View detailed information about this permission
                              </DialogDescription>
                            </DialogHeader>
                            {selectedPermission && (
                              <PermissionDetails permission={selectedPermission} />
                            )}
                          </DialogContent>
                        </Dialog>
                        <Dialog open={isEditDialogOpen} onOpenChange={setIsEditDialogOpen}>
                          <DialogTrigger asChild>
                            <Button
                              variant="ghost"
                              size="sm"
                              onClick={() => setSelectedPermission(permission)}
                            >
                              <IconEdit className="h-4 w-4" />
                            </Button>
                          </DialogTrigger>
                          <DialogContent>
                            <DialogHeader>
                              <DialogTitle>Edit Permission</DialogTitle>
                              <DialogDescription>
                                Update permission information
                              </DialogDescription>
                            </DialogHeader>
                            <PermissionUpdateForm
                              permission={selectedPermission}
                              onSubmit={(data) => handleUpdatePermission(selectedPermission!.id, data)}
                              onCancel={() => {
                                setIsEditDialogOpen(false)
                                setSelectedPermission(null)
                              }}
                            />
                          </DialogContent>
                        </Dialog>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => handleDeletePermission(permission.id)}
                        >
                          <IconTrash className="h-4 w-4" />
                        </Button>
                      </div>
                    </TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        </CardContent>
      </Card>

      {/* Permission Check Tool */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <IconCheck className="h-5 w-5" />
            <span>Permission Check Tool</span>
          </CardTitle>
          <CardDescription>
            Check if a user has a specific permission
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div>
              <div className="text-sm font-medium mb-2">User ID</div>
              <Input
                placeholder="Enter user ID"
                value={checkUserId}
                onChange={(e) => setCheckUserId(e.target.value)}
              />
            </div>
            <div>
              <div className="text-sm font-medium mb-2">Permission Code</div>
              <Input
                placeholder="Enter permission code"
                value={checkPermissionCode}
                onChange={(e) => setCheckPermissionCode(e.target.value)}
              />
            </div>
            <div className="flex items-end">
              <Button onClick={handleCheckPermission} className="w-full">
                Check Permission
              </Button>
            </div>
          </div>
          {permissionCheck && (
            <div className="mt-4 p-4 border rounded-lg">
              <div className="flex items-center space-x-2">
                {permissionCheck.hasPermission ? (
                  <IconCheck className="h-5 w-5 text-green-600" />
                ) : (
                  <IconX className="h-5 w-5 text-red-600" />
                )}
                <span className="font-medium">
                  User {permissionCheck.userId} {permissionCheck.hasPermission ? "has" : "does not have"} permission {permissionCheck.permissionCode}
                </span>
              </div>
              <div className="text-sm text-muted-foreground mt-1">
                Checked at: {new Date(permissionCheck.checkedAt).toLocaleString()}
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      {loading && (
        <div className="flex items-center justify-center py-8">
          <IconRefresh className="w-6 h-6 animate-spin mr-2" />
          <span>Loading permissions...</span>
        </div>
      )}
    </div>
  )
}

// Permission Create Form Component
function PermissionCreateForm({ 
  onSubmit, 
  onCancel 
}: { 
  onSubmit: (data: PermissionCreateRequest) => Promise<void>
  onCancel: () => void
}) {
  const [formData, setFormData] = useState({
    code: "",
    name: "",
    description: "",
    module: "",
    action: "",
    isActive: true
  })

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    await onSubmit(formData)
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="text-sm font-medium mb-2">Code *</div>
          <Input
            value={formData.code}
            onChange={(e) => setFormData({ ...formData, code: e.target.value })}
            placeholder="e.g., USER_CREATE"
            required
          />
        </div>
        <div>
          <div className="text-sm font-medium mb-2">Name *</div>
          <Input
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="e.g., Create User"
            required
          />
        </div>
      </div>
      <div>
        <div className="text-sm font-medium mb-2">Description</div>
        <Input
          value={formData.description}
          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
          placeholder="Permission description"
        />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="text-sm font-medium mb-2">Module *</div>
          <Select value={formData.module} onValueChange={(value) => setFormData({ ...formData, module: value })}>
            <SelectTrigger>
              <SelectValue placeholder="Select module" />
            </SelectTrigger>
            <SelectContent>
              {["User", "Employee", "Attendance", "Company", "Department", "System", "Backup", "Report"].map((module) => (
                <SelectItem key={module} value={module}>{module}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div>
          <div className="text-sm font-medium mb-2">Action *</div>
          <Select value={formData.action} onValueChange={(value) => setFormData({ ...formData, action: value })}>
            <SelectTrigger>
              <SelectValue placeholder="Select action" />
            </SelectTrigger>
            <SelectContent>
              {["Create", "Read", "Update", "Delete", "Export", "Import", "Manage", "View"].map((action) => (
                <SelectItem key={action} value={action}>{action}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>
      <div className="flex items-center space-x-2">
        <input
          type="checkbox"
          id="isActive"
          checked={formData.isActive}
          onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
          className="rounded"
        />
        <label htmlFor="isActive" className="text-sm font-medium">
          Active
        </label>
      </div>
      <div className="flex justify-end space-x-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit">
          Create Permission
        </Button>
      </div>
    </form>
  )
}

// Permission Update Form Component
function PermissionUpdateForm({ 
  permission, 
  onSubmit, 
  onCancel 
}: { 
  permission: Permission | null
  onSubmit: (data: PermissionUpdateRequest) => Promise<void>
  onCancel: () => void
}) {
  const [formData, setFormData] = useState({
    code: permission?.code || "",
    name: permission?.name || "",
    description: permission?.description || "",
    module: permission?.module || "",
    action: permission?.action || "",
    isActive: permission?.isActive ?? true
  })

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    await onSubmit(formData)
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="text-sm font-medium mb-2">Code *</div>
          <Input
            value={formData.code}
            onChange={(e) => setFormData({ ...formData, code: e.target.value })}
            placeholder="e.g., USER_CREATE"
            required
          />
        </div>
        <div>
          <div className="text-sm font-medium mb-2">Name *</div>
          <Input
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="e.g., Create User"
            required
          />
        </div>
      </div>
      <div>
        <div className="text-sm font-medium mb-2">Description</div>
        <Input
          value={formData.description}
          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
          placeholder="Permission description"
        />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="text-sm font-medium mb-2">Module *</div>
          <Select value={formData.module} onValueChange={(value) => setFormData({ ...formData, module: value })}>
            <SelectTrigger>
              <SelectValue placeholder="Select module" />
            </SelectTrigger>
            <SelectContent>
              {["User", "Employee", "Attendance", "Company", "Department", "System", "Backup", "Report"].map((module) => (
                <SelectItem key={module} value={module}>{module}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div>
          <div className="text-sm font-medium mb-2">Action *</div>
          <Select value={formData.action} onValueChange={(value) => setFormData({ ...formData, action: value })}>
            <SelectTrigger>
              <SelectValue placeholder="Select action" />
            </SelectTrigger>
            <SelectContent>
              {["Create", "Read", "Update", "Delete", "Export", "Import", "Manage", "View"].map((action) => (
                <SelectItem key={action} value={action}>{action}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>
      <div className="flex items-center space-x-2">
        <input
          type="checkbox"
          id="isActive"
          checked={formData.isActive}
          onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
          className="rounded"
        />
        <label htmlFor="isActive" className="text-sm font-medium">
          Active
        </label>
      </div>
      <div className="flex justify-end space-x-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit">
          Update Permission
        </Button>
      </div>
    </form>
  )
}

// Permission Form Component
function PermissionForm({ 
  permission, 
  onSubmit, 
  onCancel 
}: { 
  permission?: Permission | null
  onSubmit: (data: PermissionCreateRequest | PermissionUpdateRequest) => Promise<void>
  onCancel: () => void
}) {
  const [formData, setFormData] = useState({
    code: permission?.code || "",
    name: permission?.name || "",
    description: permission?.description || "",
    module: permission?.module || "",
    action: permission?.action || "",
    isActive: permission?.isActive ?? true
  })

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    onSubmit(formData)
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="text-sm font-medium mb-2">Code *</div>
          <Input
            value={formData.code}
            onChange={(e) => setFormData({ ...formData, code: e.target.value })}
            placeholder="e.g., USER_CREATE"
            required
          />
        </div>
        <div>
          <div className="text-sm font-medium mb-2">Name *</div>
          <Input
            value={formData.name}
            onChange={(e) => setFormData({ ...formData, name: e.target.value })}
            placeholder="e.g., Create User"
            required
          />
        </div>
      </div>
      <div>
        <div className="text-sm font-medium mb-2">Description</div>
        <Input
          value={formData.description}
          onChange={(e) => setFormData({ ...formData, description: e.target.value })}
          placeholder="Permission description"
        />
      </div>
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="text-sm font-medium mb-2">Module *</div>
          <Select value={formData.module} onValueChange={(value) => setFormData({ ...formData, module: value })}>
            <SelectTrigger>
              <SelectValue placeholder="Select module" />
            </SelectTrigger>
            <SelectContent>
              {["User", "Employee", "Attendance", "Company", "Department", "System", "Backup", "Report"].map((module) => (
                <SelectItem key={module} value={module}>{module}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div>
          <div className="text-sm font-medium mb-2">Action *</div>
          <Select value={formData.action} onValueChange={(value) => setFormData({ ...formData, action: value })}>
            <SelectTrigger>
              <SelectValue placeholder="Select action" />
            </SelectTrigger>
            <SelectContent>
              {["Create", "Read", "Update", "Delete", "Export", "Import", "Manage", "View"].map((action) => (
                <SelectItem key={action} value={action}>{action}</SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
      </div>
      <div className="flex items-center space-x-2">
        <input
          type="checkbox"
          id="isActive"
          checked={formData.isActive}
          onChange={(e) => setFormData({ ...formData, isActive: e.target.checked })}
          className="rounded"
        />
        <label htmlFor="isActive" className="text-sm font-medium">
          Active
        </label>
      </div>
      <div className="flex justify-end space-x-2">
        <Button type="button" variant="outline" onClick={onCancel}>
          Cancel
        </Button>
        <Button type="submit">
          {permission ? "Update" : "Create"} Permission
        </Button>
      </div>
    </form>
  )
}

// Permission Details Component
function PermissionDetails({ permission }: { permission: Permission }) {
  return (
    <div className="space-y-4">
      <div className="grid grid-cols-2 gap-4">
        <div>
          <div className="text-sm font-medium text-muted-foreground">Code</div>
          <div className="font-mono text-sm">{permission.code}</div>
        </div>
        <div>
          <div className="text-sm font-medium text-muted-foreground">Name</div>
          <div className="font-medium">{permission.name}</div>
        </div>
        <div>
          <div className="text-sm font-medium text-muted-foreground">Module</div>
          <Badge variant="outline">{permission.module}</Badge>
        </div>
        <div>
          <div className="text-sm font-medium text-muted-foreground">Action</div>
          <Badge variant="secondary">{permission.action}</Badge>
        </div>
        <div>
          <div className="text-sm font-medium text-muted-foreground">Status</div>
          <Badge variant={permission.isActive ? "default" : "secondary"}>
            {permission.isActive ? "Active" : "Inactive"}
          </Badge>
        </div>
        <div>
          <div className="text-sm font-medium text-muted-foreground">Created</div>
          <div className="text-sm">{new Date(permission.createdAt).toLocaleString()}</div>
        </div>
      </div>
      {permission.description && (
        <div>
          <div className="text-sm font-medium text-muted-foreground">Description</div>
          <div className="text-sm">{permission.description}</div>
        </div>
      )}
    </div>
  )
}