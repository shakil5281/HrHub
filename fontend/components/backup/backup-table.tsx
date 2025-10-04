"use client"

import { useState, useEffect, useCallback } from "react"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
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
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { 
  getBackupList,
  deleteBackup,
  validateBackup,
  downloadBackup,
  type Backup,
  type BackupListResponse
} from "@/lib/api/backup"
import { 
  IconDots, 
  IconTrash, 
  IconDownload,
  IconRefresh,
  IconFilter,
  IconShield,
  IconCheck,
  IconX,
  IconAlertCircle,
  IconClock,
  IconDatabase
} from "@tabler/icons-react"
import { toast } from "sonner"

export function BackupTable() {
  const [backups, setBackups] = useState<Backup[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [searchTerm, setSearchTerm] = useState("")
  const [statusFilter, setStatusFilter] = useState<string>("all")
  const [typeFilter, setTypeFilter] = useState<string>("all")
  const [currentPage, setCurrentPage] = useState(1)
  const [totalPages, setTotalPages] = useState(1)

  const loadBackups = useCallback(async () => {
    try {
      setLoading(true)
      setError(null)
      
      const params = {
        page: currentPage,
        pageSize: 10,
        ...(statusFilter !== "all" && { status: statusFilter }),
        ...(typeFilter !== "all" && { backupType: typeFilter }),
      }

      const response: BackupListResponse = await getBackupList(params)
      
      if (response.success) {
        setBackups(response.data.backups)
        setTotalPages(response.data.totalPages)
      } else {
        setError(response.message || 'Failed to load backups')
      }
    } catch (err) {
      console.error('Failed to load backups:', err)
      setError('Failed to load backups')
    } finally {
      setLoading(false)
    }
  }, [currentPage, statusFilter, typeFilter])

  useEffect(() => {
    loadBackups()
  }, [loadBackups])

  const handleDelete = async (backupId: number) => {
    if (!confirm('Are you sure you want to delete this backup? This action cannot be undone.')) {
      return
    }

    try {
      const response = await deleteBackup(backupId.toString())
      if (response.success) {
        toast.success('Backup deleted successfully')
        loadBackups()
      } else {
        toast.error(response.message || 'Failed to delete backup')
      }
    } catch (error) {
      console.error('Failed to delete backup:', error)
      toast.error('Failed to delete backup')
    }
  }

  const handleValidate = async (backupId: number) => {
    try {
      const response = await validateBackup(backupId.toString())
      if (response.success) {
        if (response.data.isValid) {
          toast.success('Backup validation successful')
        } else {
          toast.error('Backup validation failed')
        }
        loadBackups()
      } else {
        toast.error(response.message || 'Failed to validate backup')
      }
    } catch (error) {
      console.error('Failed to validate backup:', error)
      toast.error('Failed to validate backup')
    }
  }

  const handleDownload = async (backup: Backup) => {
    try {
      const blob = await downloadBackup(backup.id.toString())
      const url = window.URL.createObjectURL(blob)
      const a = document.createElement('a')
      a.href = url
      a.download = backup.fileName
      document.body.appendChild(a)
      a.click()
      window.URL.revokeObjectURL(url)
      document.body.removeChild(a)
      toast.success('Backup download started')
    } catch (error) {
      console.error('Failed to download backup:', error)
      toast.error('Failed to download backup')
    }
  }

  const getStatusBadge = (status: string) => {
    const statusConfig = {
      'CREATING': { variant: 'secondary' as const, label: 'Creating', icon: IconClock, className: 'bg-blue-100 text-blue-800' },
      'COMPLETED': { variant: 'default' as const, label: 'Completed', icon: IconCheck, className: 'bg-green-100 text-green-800' },
      'FAILED': { variant: 'destructive' as const, label: 'Failed', icon: IconX, className: 'bg-red-100 text-red-800' },
      'VALIDATING': { variant: 'secondary' as const, label: 'Validating', icon: IconShield, className: 'bg-yellow-100 text-yellow-800' },
      'VALIDATED': { variant: 'default' as const, label: 'Validated', icon: IconCheck, className: 'bg-green-100 text-green-800' },
      'INVALID': { variant: 'destructive' as const, label: 'Invalid', icon: IconAlertCircle, className: 'bg-red-100 text-red-800' },
    }
    
    const config = statusConfig[status as keyof typeof statusConfig] || statusConfig.COMPLETED
    const Icon = config.icon
    
    return (
      <Badge variant={config.variant} className={config.className || ""}>
        <Icon className="mr-1 h-3 w-3" />
        {config.label}
      </Badge>
    )
  }

  const getTypeBadge = (type: string) => {
    const typeConfig = {
      'FULL': { variant: 'default' as const, label: 'Full' },
      'INCREMENTAL': { variant: 'secondary' as const, label: 'Incremental' },
      'DIFFERENTIAL': { variant: 'outline' as const, label: 'Differential' },
    }
    
    const config = typeConfig[type as keyof typeof typeConfig] || typeConfig.FULL
    
    return (
      <Badge variant={config.variant}>
        {config.label}
      </Badge>
    )
  }

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleString()
  }

  const filteredBackups = backups.filter(backup => {
    const matchesSearch = backup.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         backup.description?.toLowerCase().includes(searchTerm.toLowerCase())
    return matchesSearch
  })

  if (loading) {
    return (
      <Card>
        <CardContent className="flex items-center justify-center py-8">
          <div className="text-center">
            <IconRefresh className="mx-auto h-8 w-8 animate-spin text-gray-400" />
            <p className="mt-2 text-sm text-gray-500">Loading backups...</p>
          </div>
        </CardContent>
      </Card>
    )
  }

  if (error) {
    return (
      <Card>
        <CardContent className="flex items-center justify-center py-8">
          <div className="text-center">
            <IconAlertCircle className="mx-auto h-8 w-8 text-red-400" />
            <p className="mt-2 text-sm text-red-500">{error}</p>
            <Button onClick={loadBackups} className="mt-4">
              <IconRefresh className="mr-2 h-4 w-4" />
              Retry
            </Button>
          </div>
        </CardContent>
      </Card>
    )
  }

  return (
    <div className="space-y-4">
      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <IconFilter className="h-5 w-5" />
            Backup Management
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <Input
                placeholder="Search backups..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="max-w-sm"
              />
            </div>
            <div className="flex gap-2">
              <Select value={statusFilter} onValueChange={setStatusFilter}>
                <SelectTrigger className="w-[140px]">
                  <SelectValue placeholder="Status" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Status</SelectItem>
                  <SelectItem value="COMPLETED">Completed</SelectItem>
                  <SelectItem value="FAILED">Failed</SelectItem>
                  <SelectItem value="CREATING">Creating</SelectItem>
                  <SelectItem value="VALIDATING">Validating</SelectItem>
                </SelectContent>
              </Select>
              <Select value={typeFilter} onValueChange={setTypeFilter}>
                <SelectTrigger className="w-[140px]">
                  <SelectValue placeholder="Type" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Types</SelectItem>
                  <SelectItem value="FULL">Full</SelectItem>
                  <SelectItem value="INCREMENTAL">Incremental</SelectItem>
                  <SelectItem value="DIFFERENTIAL">Differential</SelectItem>
                </SelectContent>
              </Select>
              <Button onClick={loadBackups} variant="outline">
                <IconRefresh className="h-4 w-4" />
              </Button>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Backups Table */}
      <Card>
        <CardContent className="p-0">
          {filteredBackups.length === 0 ? (
            <div className="flex flex-col items-center justify-center py-8">
              <IconDatabase className="h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-medium text-gray-900 mb-2">No Backups Found</h3>
              <p className="text-gray-500 text-center">
                {searchTerm || statusFilter !== "all" || typeFilter !== "all" 
                  ? "No backups match your current filters" 
                  : "Create your first backup to get started"}
              </p>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Name</TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Size</TableHead>
                  <TableHead>Created</TableHead>
                  <TableHead>Records</TableHead>
                  <TableHead className="text-right">Actions</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredBackups.map((backup) => (
                  <TableRow key={backup.id}>
                    <TableCell>
                      <div className="space-y-1">
                        <div className="font-medium">{backup.name}</div>
                        {backup.description && (
                          <div className="text-sm text-gray-500">{backup.description}</div>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      {getTypeBadge(backup.backupType)}
                    </TableCell>
                    <TableCell>
                      {getStatusBadge(backup.status)}
                    </TableCell>
                    <TableCell>
                      <div className="space-y-1">
                        <div className="text-sm">{formatFileSize(backup.fileSize)}</div>
                        {backup.compressionRatio && (
                          <div className="text-xs text-gray-500">
                            {Math.round(backup.compressionRatio * 100)}% compressed
                          </div>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="space-y-1">
                        <div className="text-sm">{formatDate(backup.createdAt)}</div>
                        {backup.completedAt && (
                          <div className="text-xs text-gray-500">
                            Completed: {formatDate(backup.completedAt)}
                          </div>
                        )}
                      </div>
                    </TableCell>
                    <TableCell>
                      <div className="text-sm">{backup.recordsCount.toLocaleString()}</div>
                    </TableCell>
                    <TableCell className="text-right">
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="ghost" className="h-8 w-8 p-0">
                            <IconDots className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuLabel>Actions</DropdownMenuLabel>
                          <DropdownMenuItem onClick={() => handleDownload(backup)}>
                            <IconDownload className="mr-2 h-4 w-4" />
                            Download
                          </DropdownMenuItem>
                          <DropdownMenuItem onClick={() => handleValidate(backup.id)}>
                            <IconShield className="mr-2 h-4 w-4" />
                            Validate
                          </DropdownMenuItem>
                          <DropdownMenuSeparator />
                          <DropdownMenuItem 
                            onClick={() => handleDelete(backup.id)}
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
          )}
        </CardContent>
      </Card>

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-between">
          <div className="text-sm text-gray-500">
            Page {currentPage} of {totalPages}
          </div>
          <div className="flex gap-2">
            <Button
              variant="outline"
              size="sm"
              onClick={() => setCurrentPage(prev => Math.max(prev - 1, 1))}
              disabled={currentPage === 1}
            >
              Previous
            </Button>
            <Button
              variant="outline"
              size="sm"
              onClick={() => setCurrentPage(prev => Math.min(prev + 1, totalPages))}
              disabled={currentPage === totalPages}
            >
              Next
            </Button>
          </div>
        </div>
      )}
    </div>
  )
}