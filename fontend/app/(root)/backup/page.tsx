"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { 
  getBackupStatus,
  getBackupHealth,
  getScheduledBackups,
  type BackupStatusResponse,
  type BackupHealthResponse,
  type BackupSchedule
} from "@/lib/api/backup"
import { 
  IconDatabase, 
  IconShield, 
  IconDownload,
  IconRefresh,
  IconSettings,
  IconAlertCircle,
  IconCheck,
  IconX,
  IconPlus,
  IconCalendar,
  IconTrash
} from "@tabler/icons-react"
import { toast } from "sonner"
import { BackupCreateForm } from "@/components/backup/backup-create-form"
import { BackupTable } from "@/components/backup/backup-table"
import { BackupScheduleForm } from "@/components/backup/backup-schedule-form"
import { BackupRestoreDialog } from "@/components/backup/backup-restore-dialog"

export default function BackupPage() {
  const [backupStatus, setBackupStatus] = useState<BackupStatusResponse['data'] | null>(null)
  const [backupHealth, setBackupHealth] = useState<BackupHealthResponse['data'] | null>(null)
  const [scheduledBackups, setScheduledBackups] = useState<BackupSchedule[]>([])
  const [loading, setLoading] = useState(true)
  const [showCreateForm, setShowCreateForm] = useState(false)
  const [showScheduleForm, setShowScheduleForm] = useState(false)
  const [showRestoreDialog, setShowRestoreDialog] = useState(false)

  const loadBackupData = async () => {
    try {
      setLoading(true)
      const [statusResponse, healthResponse, scheduledResponse] = await Promise.all([
        getBackupStatus(),
        getBackupHealth(),
        getScheduledBackups()
      ])

      if (statusResponse.success) {
        setBackupStatus(statusResponse.data)
      }
      if (healthResponse.success) {
        setBackupHealth(healthResponse.data)
      }
      if (scheduledResponse.success) {
        setScheduledBackups(scheduledResponse.data)
      }
    } catch (error) {
      console.error('Failed to load backup data:', error)
      toast.error('Failed to load backup data')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadBackupData()
  }, [])

  const getSystemStatusBadge = (status: string) => {
    const statusConfig = {
      'HEALTHY': { variant: 'default' as const, label: 'Healthy', icon: IconCheck, className: 'bg-green-100 text-green-800' },
      'WARNING': { variant: 'secondary' as const, label: 'Warning', icon: IconAlertCircle, className: 'bg-yellow-100 text-yellow-800' },
      'CRITICAL': { variant: 'destructive' as const, label: 'Critical', icon: IconX, className: 'bg-red-100 text-red-800' },
    }
    
    const config = statusConfig[status as keyof typeof statusConfig] || statusConfig.HEALTHY
    const Icon = config.icon
    
    return (
      <Badge variant={config.variant} className={config.className || ""}>
        <Icon className="mr-1 h-3 w-3" />
        {config.label}
      </Badge>
    )
  }

  const getHealthBadge = (health: string) => {
    const healthConfig = {
      'EXCELLENT': { variant: 'default' as const, label: 'Excellent', className: 'bg-green-100 text-green-800' },
      'GOOD': { variant: 'default' as const, label: 'Good', className: 'bg-blue-100 text-blue-800' },
      'FAIR': { variant: 'secondary' as const, label: 'Fair', className: 'bg-yellow-100 text-yellow-800' },
      'POOR': { variant: 'destructive' as const, label: 'Poor', className: 'bg-red-100 text-red-800' },
    }
    
    const config = healthConfig[health as keyof typeof healthConfig] || healthConfig.GOOD
    
    return (
      <Badge variant={config.variant} className={config.className || ""}>
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

  const formatPercentage = (value: number) => {
    return `${Math.round(value)}%`
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="text-center">
          <IconRefresh className="mx-auto h-8 w-8 animate-spin text-gray-400" />
          <p className="mt-2 text-sm text-gray-500">Loading backup data...</p>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Backup Management</h1>
          <p className="text-muted-foreground">
            Manage database backups, schedules, and system health
          </p>
        </div>
        <div className="flex gap-2">
          <Button onClick={() => setShowCreateForm(true)}>
            <IconPlus className="mr-2 h-4 w-4" />
            Create Backup
          </Button>
          <Button variant="outline" onClick={() => setShowScheduleForm(true)}>
            <IconCalendar className="mr-2 h-4 w-4" />
            Schedule Backup
          </Button>
          <Button variant="outline" onClick={loadBackupData}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
        </div>
      </div>

      {/* Status Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">System Status</CardTitle>
            <IconShield className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {backupStatus ? getSystemStatusBadge(backupStatus.systemStatus) : 'Unknown'}
            </div>
            <p className="text-xs text-muted-foreground">
              {backupStatus?.lastBackup ? `Last backup: ${new Date(backupStatus.lastBackup).toLocaleDateString()}` : 'No recent backups'}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Backups</CardTitle>
            <IconDatabase className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">{backupStatus?.totalBackups || 0}</div>
            <p className="text-xs text-muted-foreground">
              {backupStatus ? formatFileSize(backupStatus.totalSize) : '0 Bytes'} total size
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">System Health</CardTitle>
            <IconCheck className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {backupHealth ? getHealthBadge(backupHealth.systemHealth) : 'Unknown'}
            </div>
            <p className="text-xs text-muted-foreground">
              {backupHealth ? `${formatPercentage(backupHealth.backupSuccessRate)} success rate` : 'No data available'}
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Available Space</CardTitle>
            <IconDownload className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {backupStatus ? formatFileSize(backupStatus.availableSpace) : '0 Bytes'}
            </div>
            <p className="text-xs text-muted-foreground">
              {backupStatus?.backupLocation || 'No location set'}
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Main Content Tabs */}
      <Tabs defaultValue="backups" className="space-y-4">
        <TabsList>
          <TabsTrigger value="backups">Backups</TabsTrigger>
          <TabsTrigger value="schedules">Schedules</TabsTrigger>
          <TabsTrigger value="health">Health & Statistics</TabsTrigger>
        </TabsList>

        <TabsContent value="backups" className="space-y-4">
          <BackupTable />
        </TabsContent>

        <TabsContent value="schedules" className="space-y-4">
          <div className="flex items-center justify-between">
            <h3 className="text-lg font-medium">Scheduled Backups</h3>
            <Button onClick={() => setShowScheduleForm(true)}>
              <IconPlus className="mr-2 h-4 w-4" />
              New Schedule
            </Button>
          </div>
          
          <div className="grid gap-4">
            {scheduledBackups.length === 0 ? (
              <Card>
                <CardContent className="flex flex-col items-center justify-center py-8">
                  <IconCalendar className="h-12 w-12 text-gray-400 mb-4" />
                  <h3 className="text-lg font-medium text-gray-900 mb-2">No Scheduled Backups</h3>
                  <p className="text-gray-500 text-center mb-4">
                    Create a schedule to automatically backup your database
                  </p>
                  <Button onClick={() => setShowScheduleForm(true)}>
                    <IconPlus className="mr-2 h-4 w-4" />
                    Create Schedule
                  </Button>
                </CardContent>
              </Card>
            ) : (
              scheduledBackups.map((schedule) => (
                <Card key={schedule.id}>
                  <CardContent className="p-4">
                    <div className="flex items-center justify-between">
                      <div className="space-y-1">
                        <h4 className="font-medium">{schedule.name}</h4>
                        <p className="text-sm text-gray-500">{schedule.description}</p>
                        <div className="flex items-center gap-2 text-xs text-gray-500">
                          <span>{schedule.scheduleType} at {schedule.scheduleTime}</span>
                          <span>•</span>
                          <span>{schedule.backupType} backup</span>
                          <span>•</span>
                          <span>Retention: {schedule.retentionDays} days</span>
                        </div>
                      </div>
                      <div className="flex items-center gap-2">
                        <Badge variant={schedule.isActive ? "default" : "secondary"}>
                          {schedule.isActive ? "Active" : "Inactive"}
                        </Badge>
                        <Button variant="outline" size="sm">
                          <IconSettings className="h-4 w-4" />
                        </Button>
                        <Button variant="outline" size="sm">
                          <IconTrash className="h-4 w-4" />
                        </Button>
                      </div>
                    </div>
                  </CardContent>
                </Card>
              ))
            )}
          </div>
        </TabsContent>

        <TabsContent value="health" className="space-y-4">
          {backupHealth && (
            <div className="grid gap-4 md:grid-cols-2">
              <Card>
                <CardHeader>
                  <CardTitle>Performance Metrics</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex justify-between">
                    <span className="text-sm font-medium">Success Rate</span>
                    <span className="text-sm">{formatPercentage(backupHealth.backupSuccessRate)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-sm font-medium">Avg Backup Time</span>
                    <span className="text-sm">{backupHealth.averageBackupTime}s</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-sm font-medium">Avg Restore Time</span>
                    <span className="text-sm">{backupHealth.averageRestoreTime}s</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-sm font-medium">Compression Efficiency</span>
                    <span className="text-sm">{formatPercentage(backupHealth.compressionEfficiency)}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-sm font-medium">Storage Utilization</span>
                    <span className="text-sm">{formatPercentage(backupHealth.storageUtilization)}</span>
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>Recommendations</CardTitle>
                </CardHeader>
                <CardContent>
                  {backupHealth.recommendations.length === 0 ? (
                    <p className="text-sm text-gray-500">No recommendations at this time.</p>
                  ) : (
                    <ul className="space-y-2">
                      {backupHealth.recommendations.map((recommendation, index) => (
                        <li key={index} className="flex items-start gap-2 text-sm">
                          <IconAlertCircle className="h-4 w-4 text-blue-500 mt-0.5 flex-shrink-0" />
                          <span>{recommendation}</span>
                        </li>
                      ))}
                    </ul>
                  )}
                </CardContent>
              </Card>
            </div>
          )}
        </TabsContent>
      </Tabs>

      {/* Modals */}
      {showCreateForm && (
        <BackupCreateForm 
          open={showCreateForm} 
          onOpenChange={setShowCreateForm}
          onSuccess={() => {
            setShowCreateForm(false)
            loadBackupData()
          }}
        />
      )}

      {showScheduleForm && (
        <BackupScheduleForm 
          open={showScheduleForm} 
          onOpenChange={setShowScheduleForm}
          onSuccess={() => {
            setShowScheduleForm(false)
            loadBackupData()
          }}
        />
      )}

      {showRestoreDialog && (
        <BackupRestoreDialog 
          open={showRestoreDialog} 
          onOpenChange={setShowRestoreDialog}
          onSuccess={() => {
            setShowRestoreDialog(false)
            loadBackupData()
          }}
        />
      )}
    </div>
  )
}