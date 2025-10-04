"use client"

import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { updateUserRoles, getAvailableRoles, type User, type Role } from "@/lib/api/user"

interface UserRolesDialogProps {
  user: User | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess: () => void
}

export function UserRolesDialog({ user, open, onOpenChange, onSuccess }: UserRolesDialogProps) {
  const [selectedRoles, setSelectedRoles] = useState<string[]>([])
  const [availableRoles, setAvailableRoles] = useState<Role[]>([])
  const [loading, setLoading] = useState(false)
  const [loadingRoles, setLoadingRoles] = useState(false)
  const [error, setError] = useState<string | null>(null)

  // Initialize roles when user changes
  useEffect(() => {
    if (user) {
      setSelectedRoles(user.roles || [])
    }
  }, [user])

  // Load available roles when dialog opens
  useEffect(() => {
    if (open) {
      loadAvailableRoles()
    }
  }, [open])

  const loadAvailableRoles = async () => {
    setLoadingRoles(true)
    try {
      const response = await getAvailableRoles()
      if (response.success) {
        setAvailableRoles(response.data)
      }
    } catch (error) {
      console.error('Error loading available roles:', error)
      setError('Failed to load available roles')
    } finally {
      setLoadingRoles(false)
    }
  }

  const handleRoleChange = (roleName: string, isSelected: boolean) => {
    if (isSelected) {
      // Add role if not already selected
      if (!selectedRoles.includes(roleName)) {
        setSelectedRoles([...selectedRoles, roleName])
      }
    } else {
      // Remove role
      setSelectedRoles(selectedRoles.filter(role => role !== roleName))
    }
  }

  const handleSubmit = async () => {
    if (!user?.id) return

    setLoading(true)
    setError(null)

    try {
      await updateUserRoles(user.id, selectedRoles)
      onSuccess()
      onOpenChange(false)
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while updating user roles."
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  if (!user) return null

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle>Manage User Roles</DialogTitle>
          <DialogDescription>
            Update roles for {user.firstName} {user.lastName}
          </DialogDescription>
        </DialogHeader>

        <div className="space-y-6">
          {/* Current Roles Display */}
          <div>
            <h4 className="text-sm font-medium mb-3">Current Roles</h4>
            <div className="space-y-2">
              {selectedRoles.length > 0 ? (
                selectedRoles.map((role) => (
                  <div key={role} className="flex items-center justify-between p-2 bg-gray-50 rounded-md">
                    <span className="font-medium">{role}</span>
                    <Button
                      variant="ghost"
                      size="sm"
                      onClick={() => handleRoleChange(role, false)}
                      className="text-red-600 hover:text-red-700"
                    >
                      Remove
                    </Button>
                  </div>
                ))
              ) : (
                <div className="p-4 text-center text-gray-500 bg-gray-50 rounded-md">
                  No roles assigned
                </div>
              )}
            </div>
          </div>

          {/* Available Roles Selection */}
          <div>
            <h4 className="text-sm font-medium mb-3">Available Roles</h4>
            {loadingRoles ? (
              <div className="p-4 text-center text-gray-500">
                Loading available roles...
              </div>
            ) : (
              <div className="space-y-2 max-h-60 overflow-y-auto">
                {availableRoles.map((role) => (
                  <div key={role.id} className="flex items-center justify-between p-3 border rounded-md">
                    <div className="flex-1">
                      <div className="font-medium">{role.name}</div>
                      <div className="text-sm text-gray-500">{role.description}</div>
                      <div className="text-xs text-gray-400">
                        {role.userCount} users ({role.activeUserCount} active)
                      </div>
                    </div>
                    <Button
                      variant={selectedRoles.includes(role.name) ? "default" : "outline"}
                      size="sm"
                      onClick={() => handleRoleChange(role.name, !selectedRoles.includes(role.name))}
                      disabled={selectedRoles.includes(role.name)}
                    >
                      {selectedRoles.includes(role.name) ? "Selected" : "Add"}
                    </Button>
                  </div>
                ))}
              </div>
            )}
          </div>

          {error && (
            <div className="text-sm text-red-500 bg-red-50 p-3 rounded-md border border-red-200">
              {error}
            </div>
          )}
        </div>

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => onOpenChange(false)}
            disabled={loading}
          >
            Cancel
          </Button>
          <Button
            type="button"
            onClick={handleSubmit}
            disabled={loading || loadingRoles}
          >
            {loading ? "Updating..." : "Update Roles"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  )
}
