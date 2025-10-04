"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { 
  IconPlus, 
  IconDatabase, 
  IconClock, 
  IconShield, 
  IconRefresh,
  IconSettings,
  IconTrash,
  IconTrendingUp
} from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Badge } from "@/components/ui/badge"
import { Progress } from "@/components/ui/progress"
import { 
  BackupTable, 
  BackupCreateForm, 
  BackupRestoreDialog, 
  BackupScheduleForm 
} from "@/components/backup"
import { 
  getBackups, 
  getBackupStatus, 
  getBackupHealth, 
  getScheduledBackups,
  cancelScheduledBackup,
  cleanupBackups,
  type Backup,
  type BackupSchedule
} from "@/lib/api/backup"
import { toast } from "sonner"

export default function BackupPage() {
  const [backups, setBackups] = useState<Backup[]>([])
  const [schedules, setSchedules] = useState<BackupSchedule[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const [activeTab, setActiveTab] = useState("backups")
  const [showCreateForm, setShowCreateForm] = useState(false)
  const [showScheduleForm, setShowScheduleForm] = useState(false)
  const [restoreDialogOpen, setRestoreDialogOpen] = useState(false)
  const [selectedBackup, setSelectedBackup] = useState<Backup | null>(null)
  const [backupStatus, setBackupStatus] = useState<{ isRunning: boolean; lastRun?: string; nextRun?: string } | null>(null)
  const [backupHealth, setBackupHealth] = useState<{ status: string; message: string } | null>(null)
  const router = useRouter()

  const fetchData = async () => {
    try {
      setLoading(true)
      const [backupsResponse, schedulesResponse, statusResponse, healthResponse] = await Promise.all([
        getBackups(),
        getScheduledBackups(),
        getBackupStatus(),
        getBackupHealth()
      ])

      if (backupsResponse.success) {
        setBackups(backupsResponse.data.backups)
      }
      if (schedulesResponse.success) {
        setSchedules(schedulesResponse.data)
      }
      if (statusResponse.success) {
        setBackupStatus(statusResponse.data)
      }
      if (healthResponse.success) {
        setBackupHealth(healthResponse.data)
      }
    } catch (error) {
      console.error('Error fetching backup data:', error)
      toast.error("Failed to load backup data")
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchData()
  }, [])

  const handleRefresh = () => {
    fetchData()
  }

  const handleRestore = (backup: Backup) => {
    setSelectedBackup(backup)
    setRestoreDialogOpen(true)
  }

  const handleRestoreSuccess = () => {
    fetchData()
  }

  const handleCreateSuccess = () => {
    setShowCreateForm(false)
    fetchData()
  }

  const handleScheduleSuccess = () => {
    setShowScheduleForm(false)
    fetchData()
  }

  const handleCancelSchedule = async (scheduleId: string) => {
    try {
      const response = await cancelScheduledBackup(scheduleId)
      if (response.success) {
        toast.success("Schedule cancelled successfully")
        fetchData()
      } else {
        toast.error("Failed to cancel schedule")
      }
    } catch (error) {
      console.error('Error cancelling schedule:', error)
      toast.error("Failed to cancel schedule")
    }
  }

  const handleCleanup = async () => {
    try {
      const response = await cleanupBackups({ dryRun: false })
      if (response.success) {
        toast.success(`Cleanup completed: ${response.data.deletedBackups} backups deleted, ${(response.data.freedSpace / (1024 * 1024)).toFixed(2)} MB freed`)
        fetchData()
      } else {
        toast.error("Failed to cleanup backups")
      }
    } catch (error) {
      console.error('Error cleaning up backups:', error)
      toast.error("Failed to cleanup backups")
    }
  }

  const filteredBackups = backups.filter(backup =>
    backup.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (backup.description && backup.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
    backup.backupType.toLowerCase().includes(searchTerm.toLowerCase()) ||
    backup.status.toLowerCase().includes(searchTerm.toLowerCase())
  )

  const formatFileSize = (bytes: number) => {
    if (bytes === 0) return '0 Bytes'
    const k = 1024
    const sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB']
    const i = Math.floor(Math.log(bytes) / Math.log(k))
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i]
  }

  if (showCreateForm) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Create Backup</h1>
            <p className="text-muted-foreground">
              Create a new database backup with custom settings.
            </p>
          </div>
          <Button variant="outline" onClick={() => setShowCreateForm(false)}>
            Back to Backups
          </Button>
        </div>
        <BackupCreateForm onSuccess={handleCreateSuccess} onCancel={() => setShowCreateForm(false)} />
      </div>
    )
  }

  if (showScheduleForm) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Schedule Backup</h1>
            <p className="text-muted-foreground">
              Create an automatic backup schedule.
            </p>
          </div>
          <Button variant="outline" onClick={() => setShowScheduleForm(false)}>
            Back to Backups
          </Button>
        </div>
        <BackupScheduleForm onSuccess={handleScheduleSuccess} onCancel={() => setShowScheduleForm(false)} />
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
            Manage database backups, schedules, and system health.
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button variant="outline" onClick={handleRefresh} disabled={loading}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button variant="outline" onClick={handleCleanup} disabled={loading}>
            <IconTrash className="mr-2 h-4 w-4" />
            Cleanup
          </Button>
          <Button variant="outline" onClick={() => setShowScheduleForm(true)}>
            <IconClock className="mr-2 h-4 w-4" />
            Schedule
          </Button>
          <Button onClick={() => setShowCreateForm(true)}>
            <IconPlus className="mr-2 h-4 w-4" />
            Create Backup
          </Button>
        </div>
      </div>

      {/* System Status Cards */}
      {backupStatus && (
        <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">System Status</CardTitle>
              <IconShield className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">
                <Badge variant={backupStatus.systemStatus === 'HEALTHY' ? 'default' : 'destructive'}>
                  {backupStatus.systemStatus}
                </Badge>
              </div>
              <p className="text-xs text-muted-foreground">
                {backupStatus.isBackupInProgress ? 'Backup in progress' : 'System ready'}
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Backups</CardTitle>
              <IconDatabase className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">
                {loading ? "..." : backupStatus.totalBackups.toLocaleString()}
              </div>
              <p className="text-xs text-muted-foreground">
                {formatFileSize(backupStatus.totalSize)} total size
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Available Space</CardTitle>
              <IconSettings className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">
                {loading ? "..." : formatFileSize(backupStatus.availableSpace)}
              </div>
              <p className="text-xs text-muted-foreground">
                Free disk space
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Health Score</CardTitle>
              <IconTrendingUp className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">
                {loading ? "..." : backupHealth?.healthScore || 0}%
              </div>
              <Progress value={backupHealth?.healthScore || 0} className="mt-2" />
            </CardContent>
          </Card>
        </div>
      )}

      {/* Main Content Tabs */}
      <Tabs value={activeTab} onValueChange={setActiveTab} className="space-y-4">
        <TabsList>
          <TabsTrigger value="backups">Backups</TabsTrigger>
          <TabsTrigger value="schedules">Schedules</TabsTrigger>
          <TabsTrigger value="health">Health</TabsTrigger>
        </TabsList>

        <TabsContent value="backups" className="space-y-4">
          {/* Search */}
          <div className="flex items-center space-x-4">
            <div className="flex-1">
              <Input
                placeholder="Search backups by name, type, or status..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="max-w-sm"
              />
            </div>
            <div className="text-sm text-muted-foreground">
              {filteredBackups.length} of {backups.length} backups
            </div>
          </div>

          {/* Backups Table */}
          <BackupTable
            backups={filteredBackups}
            loading={loading}
            onRefresh={handleRefresh}
            onRestore={handleRestore}
          />
        </TabsContent>

        <TabsContent value="schedules" className="space-y-4">
          <div className="rounded-md border">
            <div className="p-4">
              <h3 className="text-lg font-semibold mb-4">Scheduled Backups</h3>
              {schedules.length === 0 ? (
                <div className="text-center py-8 text-muted-foreground">
                  No scheduled backups found
                </div>
              ) : (
                <div className="space-y-4">
                  {schedules.map((schedule) => (
                    <Card key={schedule.id}>
                      <CardContent className="p-4">
                        <div className="flex items-center justify-between">
                          <div className="space-y-1">
                            <div className="font-medium">{schedule.name}</div>
                            {schedule.description && (
                              <div className="text-sm text-muted-foreground">{schedule.description}</div>
                            )}
                            <div className="flex items-center gap-4 text-sm text-muted-foreground">
                              <span>Type: {schedule.backupType}</span>
                              <span>Schedule: {schedule.scheduleType}</span>
                              <span>Next: {schedule.nextRun ? new Date(schedule.nextRun).toLocaleString() : 'N/A'}</span>
                            </div>
                          </div>
                          <div className="flex items-center gap-2">
                            <Badge variant={schedule.isActive ? 'default' : 'secondary'}>
                              {schedule.isActive ? 'Active' : 'Inactive'}
                            </Badge>
                            <Button
                              variant="outline"
                              size="sm"
                              onClick={() => handleCancelSchedule(schedule.id)}
                            >
                              Cancel
                            </Button>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  ))}
                </div>
              )}
            </div>
          </div>
        </TabsContent>

        <TabsContent value="health" className="space-y-4">
          {backupHealth && (
            <div className="grid gap-4 md:grid-cols-2">
              <Card>
                <CardHeader>
                  <CardTitle>Backup Statistics</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex justify-between">
                    <span>Total Backups:</span>
                    <span className="font-medium">{backupHealth.totalBackups}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Successful:</span>
                    <span className="font-medium text-green-600">{backupHealth.successfulBackups}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Failed:</span>
                    <span className="font-medium text-red-600">{backupHealth.failedBackups}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Success Rate:</span>
                    <span className="font-medium">{backupHealth.backupSuccessRate}%</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Total Size:</span>
                    <span className="font-medium">{formatFileSize(backupHealth.totalSize)}</span>
                  </div>
                </CardContent>
              </Card>

              <Card>
                <CardHeader>
                  <CardTitle>System Health</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex justify-between">
                    <span>Health Score:</span>
                    <span className="font-medium">{backupHealth.healthScore}%</span>
                  </div>
                  <Progress value={backupHealth.healthScore} className="w-full" />
                  <div className="flex justify-between">
                    <span>Storage Utilization:</span>
                    <span className="font-medium">{backupHealth.storageUtilization}%</span>
                  </div>
                  <Progress value={backupHealth.storageUtilization} className="w-full" />
                  <div className="flex justify-between">
                    <span>Average Backup Time:</span>
                    <span className="font-medium">{backupHealth.averageBackupTime}s</span>
                  </div>
                  {backupHealth.lastBackupDate && (
                    <div className="flex justify-between">
                      <span>Last Backup:</span>
                      <span className="font-medium">{new Date(backupHealth.lastBackupDate).toLocaleString()}</span>
                    </div>
                  )}
                </CardContent>
              </Card>
            </div>
          )}
        </TabsContent>
      </Tabs>

      {/* Restore Dialog */}
      <BackupRestoreDialog
        backup={selectedBackup}
        open={restoreDialogOpen}
        onOpenChange={setRestoreDialogOpen}
        onSuccess={handleRestoreSuccess}
      />
    </div>
  )
}
