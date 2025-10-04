"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Checkbox } from "@/components/ui/checkbox"
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import { IconShield, IconPlus, IconSearch, IconCheck, IconX } from "@tabler/icons-react"
import { cn } from "@/lib/utils"
import { 
  assignPermissionToUser,
  removePermissionFromUser,
  checkUserPermission,
  type Permission,
  type PermissionAssignmentRequest
} from "@/lib/api/user-permission"
import { toast } from "sonner"

interface PermissionAssignmentDialogProps {
  userId: number
  userName: string
  onPermissionChange: () => void
}

export function PermissionAssignmentDialog({ userId, userName, onPermissionChange }: PermissionAssignmentDialogProps) {
  const [open, setOpen] = useState(false)
  const [permissions, setPermissions] = useState<Permission[]>([])
  const [selectedPermissions, setSelectedPermissions] = useState<number[]>([])
  const [searchTerm, setSearchTerm] = useState("")
  const [loading, setLoading] = useState(false)

  // Mock permissions data - in real app, this would come from an API
  const mockPermissions: Permission[] = [
    { id: 1, code: "USER_READ", name: "Read Users", description: "View user information", module: "Users", isActive: true },
    { id: 2, code: "USER_WRITE", name: "Write Users", description: "Create and edit users", module: "Users", isActive: true },
    { id: 3, code: "USER_DELETE", name: "Delete Users", description: "Delete users", module: "Users", isActive: true },
    { id: 4, code: "ATTENDANCE_READ", name: "Read Attendance", description: "View attendance records", module: "Attendance", isActive: true },
    { id: 5, code: "ATTENDANCE_WRITE", name: "Write Attendance", description: "Create and edit attendance", module: "Attendance", isActive: true },
    { id: 6, code: "REPORT_READ", name: "Read Reports", description: "View reports", module: "Reports", isActive: true },
    { id: 7, code: "REPORT_EXPORT", name: "Export Reports", description: "Export reports to files", module: "Reports", isActive: true },
    { id: 8, code: "ADMIN_ACCESS", name: "Admin Access", description: "Full administrative access", module: "Admin", isActive: true },
  ]

  const filteredPermissions = mockPermissions.filter(permission =>
    permission.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    permission.code.toLowerCase().includes(searchTerm.toLowerCase()) ||
    permission.module?.toLowerCase().includes(searchTerm.toLowerCase())
  )

  const handleAssignPermissions = async () => {
    if (selectedPermissions.length === 0) return

    setLoading(true)
    try {
      for (const permissionId of selectedPermissions) {
        await assignPermissionToUser({
          userId,
          permissionId
        })
      }
      toast.success(`${selectedPermissions.length} permissions assigned successfully`)
      setOpen(false)
      setSelectedPermissions([])
      onPermissionChange()
    } catch (error) {
      console.error("Error assigning permissions:", error)
      toast.error("Failed to assign permissions")
    } finally {
      setLoading(false)
    }
  }

  const handleRemovePermission = async (permissionId: number) => {
    try {
      await removePermissionFromUser(userId, permissionId)
      toast.success("Permission removed successfully")
      onPermissionChange()
    } catch (error) {
      console.error("Error removing permission:", error)
      toast.error("Failed to remove permission")
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button>
          <IconPlus className="w-4 h-4 mr-2" />
          Assign Permissions
        </Button>
      </DialogTrigger>
      <DialogContent className="max-w-4xl max-h-[80vh] overflow-hidden flex flex-col">
        <DialogHeader>
          <DialogTitle>Assign Permissions to {userName}</DialogTitle>
          <DialogDescription>
            Select permissions to assign to this user
          </DialogDescription>
        </DialogHeader>
        
        <div className="flex-1 overflow-hidden flex flex-col space-y-4">
          {/* Search */}
          <div className="relative">
            <IconSearch className="absolute left-2 top-2.5 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Search permissions..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-8"
            />
          </div>

          {/* Permissions List */}
          <div className="flex-1 overflow-auto">
            <div className="space-y-2">
              {filteredPermissions.map((permission) => (
                <Card key={permission.id} className="p-3">
                  <div className="flex items-center space-x-3">
                    <Checkbox
                      checked={selectedPermissions.includes(permission.id)}
                      onCheckedChange={(checked) => {
                        if (checked) {
                          setSelectedPermissions([...selectedPermissions, permission.id])
                        } else {
                          setSelectedPermissions(selectedPermissions.filter(id => id !== permission.id))
                        }
                      }}
                    />
                    <div className="flex-1">
                      <div className="flex items-center space-x-2">
                        <h4 className="font-medium">{permission.name}</h4>
                        <Badge variant="outline" className="text-xs">
                          {permission.code}
                        </Badge>
                        {permission.module && (
                          <Badge variant="secondary" className="text-xs">
                            {permission.module}
                          </Badge>
                        )}
                      </div>
                      {permission.description && (
                        <p className="text-sm text-muted-foreground mt-1">
                          {permission.description}
                        </p>
                      )}
                    </div>
                  </div>
                </Card>
              ))}
            </div>
          </div>

          {/* Actions */}
          <div className="flex items-center justify-between pt-4 border-t">
            <div className="text-sm text-muted-foreground">
              {selectedPermissions.length} permission(s) selected
            </div>
            <div className="flex space-x-2">
              <Button variant="outline" onClick={() => setOpen(false)}>
                Cancel
              </Button>
              <Button 
                onClick={handleAssignPermissions}
                disabled={selectedPermissions.length === 0 || loading}
              >
                {loading ? "Assigning..." : `Assign ${selectedPermissions.length} Permission(s)`}
              </Button>
            </div>
          </div>
        </div>
      </DialogContent>
    </Dialog>
  )
}

interface PermissionCheckDialogProps {
  userId: number
  userName: string
}

export function PermissionCheckDialog({ userId, userName }: PermissionCheckDialogProps) {
  const [open, setOpen] = useState(false)
  const [permissionCode, setPermissionCode] = useState("")
  const [checkResult, setCheckResult] = useState<{ hasPermission: boolean; source: string } | null>(null)
  const [loading, setLoading] = useState(false)

  const handleCheckPermission = async () => {
    if (!permissionCode.trim()) return

    setLoading(true)
    try {
      const response = await checkUserPermission(userId, permissionCode)
      setCheckResult({
        hasPermission: response.data.hasPermission,
        source: response.data.source
      })
    } catch (error) {
      console.error("Error checking permission:", error)
      toast.error("Failed to check permission")
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button variant="outline">
          <IconShield className="w-4 h-4 mr-2" />
          Check Permission
        </Button>
      </DialogTrigger>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Check Permission for {userName}</DialogTitle>
          <DialogDescription>
            Check if this user has a specific permission
          </DialogDescription>
        </DialogHeader>
        
        <div className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="permissionCode">Permission Code</Label>
            <Input
              id="permissionCode"
              placeholder="e.g., USER_READ"
              value={permissionCode}
              onChange={(e) => setPermissionCode(e.target.value)}
            />
          </div>

          <Button 
            onClick={handleCheckPermission}
            disabled={!permissionCode.trim() || loading}
            className="w-full"
          >
            {loading ? "Checking..." : "Check Permission"}
          </Button>

          {checkResult && (
            <Card className="p-4">
              <div className="flex items-center space-x-2">
                {checkResult.hasPermission ? (
                  <IconCheck className="h-5 w-5 text-green-600" />
                ) : (
                  <IconX className="h-5 w-5 text-red-600" />
                )}
                <div>
                  <div className="font-medium">
                    {checkResult.hasPermission ? "Has Permission" : "No Permission"}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    Source: {checkResult.source}
                  </div>
                </div>
              </div>
            </Card>
          )}
        </div>
      </DialogContent>
    </Dialog>
  )
}
