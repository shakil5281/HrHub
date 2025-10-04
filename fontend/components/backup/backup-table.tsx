"use client"

import { useState } from "react"
import { format } from "date-fns"
import {
  IconDownload,
  IconTrash,
  IconEye,
  IconDatabase,
  IconShield,
  IconClock,
  IconCheck,
  IconX,
  IconAlertTriangle,
  IconRefresh,
  IconRestore,
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
import { Progress } from "@/components/ui/progress"
import { deleteBackup, downloadBackup, validateBackup, type Backup } from "@/lib/api/backup"
import { toast } from "sonner"

interface BackupTableProps {
  backups: Backup[]
  loading: boolean
  onRefresh: () => void
  onRestore: (backup: Backup) => void
}

export function BackupTable({ backups, loading, onRefresh, onRestore }: BackupTableProps) {
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false)
  const [backupToDelete, setBackupToDelete] = useState<Backup | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [viewBackup, setViewBackup] = useState<Backup | null>(null)
  const [validating, setValidating] = useState<string | null>(null)
  const [downloading, setDownloading] = useState<string | null>(null)

  const handleDelete = async () => {
    if (!backupToDelete) return

    setDeleting(true)
    try {
      await deleteBackup(backupToDelete.id)
      toast.success("Backup deleted successfully")
      onRefresh()
      setDeleteDialogOpen(false)
      setBackupToDelete(null)
    } catch (error) {
      console.error('Error deleting backup:', error)
      toast.error("Failed to delete backup")
    } finally {
      setDeleting(false)
    }
  }

  const handleDownload = async (backup: Backup) => {
    setDownloading(backup.id)
    try {
      const blob = await downloadBackup(backup.id)
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = backup.fileName
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)
      toast.success("Backup downloaded successfully")
    } catch (error) {
      console.error('Error downloading backup:', error)
      toast.error("Failed to download backup")
    } finally {
      setDownloading(null)
    }
  }

  const handleValidate = async (backup: Backup) => {
    setValidating(backup.id)
    try {
      const response = await validateBackup(backup.id)
      if (response.success) {
        if (response.data.isValid) {
          toast.success("Backup validation successful")
        } else {
          toast.error("Backup validation failed")
        }
        onRefresh() // Refresh to update validation status
      }
    } catch (error) {
      console.error('Error validating backup:', error)
      toast.error("Failed to validate backup")
    } finally {
      setValidating(null)
    }
  }

  const openDeleteDialog = (backup: Backup) => {
    setBackupToDelete(backup)
    setDeleteDialogOpen(true)
  }

  const getStatusBadge = (status: Backup['status']) => {
    const statusConfig = {
      PENDING: { variant: "secondary" as const, icon: IconClock, text: "Pending" },
      IN_PROGRESS: { variant: "default" as const, icon: IconRefresh, text: "In Progress" },
      COMPLETED: { variant: "default" as const, icon: IconCheck, text: "Completed" },
      FAILED: { variant: "destructive" as const, icon: IconX, text: "Failed" },
      CANCELLED: { variant: "outline" as const, icon: IconX, text: "Cancelled" },
    }

    const config = statusConfig[status]
    const Icon = config.icon

    return (
      <Badge variant={config.variant} className="flex items-center gap-1">
        <Icon className="h-3 w-3" />
        {config.text}
      </Badge>
    )
  }

  const getTypeBadge = (type: Backup['backupType']) => {
    const typeConfig = {
      FULL: { variant: "default" as const, text: "Full" },
      INCREMENTAL: { variant: "secondary" as const, text: "Incremental" },
      DIFFERENTIAL: { variant: "outline" as const, text: "Differential" },
    }

    const config = typeConfig[type]
    return <Badge variant={config.variant}>{config.text}</Badge>
  }

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
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

  if (backups.length === 0) {
    return (
      <Card>
        <CardContent className="flex flex-col items-center justify-center py-12">
          <IconDatabase className="h-12 w-12 text-gray-400 mb-4" />
          <h3 className="text-lg font-semibold text-gray-900 mb-2">No backups found</h3>
          <p className="text-gray-500 text-center max-w-sm">
            Create your first backup to protect your data. Backups help ensure data safety and recovery.
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
              <TableHead>Backup</TableHead>
              <TableHead>Type</TableHead>
              <TableHead>Status</TableHead>
              <TableHead>Size</TableHead>
              <TableHead>Created</TableHead>
              <TableHead>Validation</TableHead>
              <TableHead className="text-right">Actions</TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {backups.map((backup) => (
              <TableRow key={backup.id}>
                <TableCell>
                  <div className="space-y-1">
                    <div className="font-medium">{backup.name}</div>
                    {backup.description && (
                      <div className="text-sm text-gray-500 truncate max-w-[200px]">
                        {backup.description}
                      </div>
                    )}
                    <div className="text-xs text-gray-400">
                      {backup.fileName}
                    </div>
                  </div>
                </TableCell>
                <TableCell>
                  {getTypeBadge(backup.backupType)}
                </TableCell>
                <TableCell>
                  {getStatusBadge(backup.status)}
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    {formatFileSize(backup.fileSize)}
                  </div>
                </TableCell>
                <TableCell>
                  <div className="text-sm">
                    <div>{format(new Date(backup.createdAt), 'MMM dd, yyyy')}</div>
                    <div className="text-gray-500">
                      {format(new Date(backup.createdAt), 'HH:mm')}
                    </div>
                  </div>
                </TableCell>
                <TableCell>
                  <div className="flex items-center gap-2">
                    {backup.isValidated ? (
                      <Badge variant="default" className="flex items-center gap-1">
                        <IconCheck className="h-3 w-3" />
                        Valid
                      </Badge>
                    ) : (
                      <Badge variant="outline" className="flex items-center gap-1">
                        <IconAlertTriangle className="h-3 w-3" />
                        Not Validated
                      </Badge>
                    )}
                    {backup.encryptionEnabled && (
                      <Badge variant="secondary" className="flex items-center gap-1">
                        <IconShield className="h-3 w-3" />
                        Encrypted
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
                      <DropdownMenuItem onClick={() => setViewBackup(backup)}>
                        <IconEye className="mr-2 h-4 w-4" />
                        View Details
                      </DropdownMenuItem>
                      <DropdownMenuItem 
                        onClick={() => handleDownload(backup)}
                        disabled={downloading === backup.id || backup.status !== 'COMPLETED'}
                      >
                        <IconDownload className="mr-2 h-4 w-4" />
                        {downloading === backup.id ? "Downloading..." : "Download"}
                      </DropdownMenuItem>
                      <DropdownMenuItem 
                        onClick={() => handleValidate(backup)}
                        disabled={validating === backup.id || backup.status !== 'COMPLETED'}
                      >
                        <IconShield className="mr-2 h-4 w-4" />
                        {validating === backup.id ? "Validating..." : "Validate"}
                      </DropdownMenuItem>
                      <DropdownMenuItem 
                        onClick={() => onRestore(backup)}
                        disabled={backup.status !== 'COMPLETED' || !backup.isValidated}
                      >
                        <IconRestore className="mr-2 h-4 w-4" />
                        Restore
                      </DropdownMenuItem>
                      <DropdownMenuSeparator />
                      <DropdownMenuItem 
                        onClick={() => openDeleteDialog(backup)}
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

      {/* Backup Details Modal */}
      {viewBackup && (
        <AlertDialog open={!!viewBackup} onOpenChange={() => setViewBackup(null)}>
          <AlertDialogContent className="max-w-2xl">
            <AlertDialogHeader>
              <AlertDialogTitle className="flex items-center space-x-3">
                <IconDatabase className="h-8 w-8" />
                <div>
                  <div>{viewBackup.name}</div>
                  <div className="text-sm font-normal text-gray-500">
                    {viewBackup.fileName}
                  </div>
                </div>
              </AlertDialogTitle>
            </AlertDialogHeader>
            
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Backup Information</h4>
                  <div className="space-y-2 text-sm">
                    <div><strong>Type:</strong> {getTypeBadge(viewBackup.backupType)}</div>
                    <div><strong>Status:</strong> {getStatusBadge(viewBackup.status)}</div>
                    <div><strong>Size:</strong> {formatFileSize(viewBackup.fileSize)}</div>
                    <div><strong>Created:</strong> {format(new Date(viewBackup.createdAt), 'PPP p')}</div>
                    {viewBackup.completedAt && (
                      <div><strong>Completed:</strong> {format(new Date(viewBackup.completedAt), 'PPP p')}</div>
                    )}
                  </div>
                </div>

                <div>
                  <h4 className="font-semibold mb-2">Security & Validation</h4>
                  <div className="space-y-2 text-sm">
                    <div className="flex items-center gap-2">
                      <strong>Encryption:</strong>
                      {viewBackup.encryptionEnabled ? (
                        <Badge variant="default" className="flex items-center gap-1">
                          <IconShield className="h-3 w-3" />
                          Enabled
                        </Badge>
                      ) : (
                        <Badge variant="outline">Disabled</Badge>
                      )}
                    </div>
                    <div className="flex items-center gap-2">
                      <strong>Validation:</strong>
                      {viewBackup.isValidated ? (
                        <Badge variant="default" className="flex items-center gap-1">
                          <IconCheck className="h-3 w-3" />
                          Validated
                        </Badge>
                      ) : (
                        <Badge variant="outline" className="flex items-center gap-1">
                          <IconAlertTriangle className="h-3 w-3" />
                          Not Validated
                        </Badge>
                      )}
                    </div>
                    {viewBackup.checksum && (
                      <div><strong>Checksum:</strong> <code className="text-xs">{viewBackup.checksum}</code></div>
                    )}
                    {viewBackup.validationDate && (
                      <div><strong>Validated:</strong> {format(new Date(viewBackup.validationDate), 'PPP p')}</div>
                    )}
                  </div>
                </div>
              </div>

              {viewBackup.description && (
                <div>
                  <h4 className="font-semibold mb-2">Description</h4>
                  <p className="text-sm text-gray-600">{viewBackup.description}</p>
                </div>
              )}

              <div className="grid grid-cols-1 gap-4">
                <div>
                  <h4 className="font-semibold mb-2">Additional Details</h4>
                  <div className="space-y-1 text-sm">
                    <div><strong>Created By:</strong> {viewBackup.createdBy}</div>
                    <div><strong>File Path:</strong> <code className="text-xs">{viewBackup.filePath}</code></div>
                    {viewBackup.compressionType && (
                      <div><strong>Compression:</strong> {viewBackup.compressionType}</div>
                    )}
                    {viewBackup.retentionDays && (
                      <div><strong>Retention:</strong> {viewBackup.retentionDays} days</div>
                    )}
                    {viewBackup.expiresAt && (
                      <div><strong>Expires:</strong> {format(new Date(viewBackup.expiresAt), 'PPP')}</div>
                    )}
                  </div>
                </div>
              </div>
            </div>

            <AlertDialogFooter>
              <AlertDialogCancel>Close</AlertDialogCancel>
              <AlertDialogAction onClick={() => handleDownload(viewBackup)}>
                Download Backup
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      )}

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={deleteDialogOpen} onOpenChange={setDeleteDialogOpen}>
        <AlertDialogContent>
          <AlertDialogHeader>
            <AlertDialogTitle>Delete Backup</AlertDialogTitle>
            <AlertDialogDescription>
              Are you sure you want to delete &quot;{backupToDelete?.name}&quot;? This action cannot be undone.
              The backup file will be permanently removed from the system.
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
