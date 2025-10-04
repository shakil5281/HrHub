"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { 
  getAttendanceStatistics,
  getDeviceStatus,
  type AttendanceStatistics,
  type DeviceStatus 
} from "@/lib/api/attendance"
import { formatDate } from "@/lib/utils"
import { 
  IconDeviceDesktop, 
  IconClock, 
  IconActivity,
  IconRefresh,
  IconWifi,
  IconWifiOff,
  IconAlertCircle,
  IconCheck,
  IconX,
  IconArrowRight,
  IconSettings,
  IconDownload,
  IconFileText
} from "@tabler/icons-react"

export default function AttendanceDashboardPage() {
  const [statistics, setStatistics] = useState<AttendanceStatistics | null>(null)
  const [deviceStatuses, setDeviceStatuses] = useState<DeviceStatus[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const router = useRouter()

  const loadDashboardData = async () => {
    try {
      setLoading(true)
      const [statsResponse, devicesResponse] = await Promise.all([
        getAttendanceStatistics(),
        getDeviceStatus()
      ])
      
      if (statsResponse.success) {
        setStatistics(statsResponse.data)
      } else {
        setError(statsResponse.message || 'Failed to load statistics')
      }
      
      if (devicesResponse.success) {
        setDeviceStatuses(devicesResponse.data)
      }
      
    } catch (err) {
      console.error('Error loading dashboard data:', err)
      setError('Failed to load dashboard data')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadDashboardData()
  }, [])

  const getSyncStatusBadge = (status: string) => {
    const statusConfig = {
      'IDLE': { variant: 'secondary' as const, label: 'Idle', icon: IconCheck, className: '' },
      'SYNCING': { variant: 'default' as const, label: 'Syncing', icon: IconActivity, className: 'bg-blue-100 text-blue-800' },
      'ERROR': { variant: 'destructive' as const, label: 'Error', icon: IconX, className: '' },
    }
    
    const config = statusConfig[status as keyof typeof statusConfig] || statusConfig.IDLE
    const Icon = config.icon
    
    return (
      <Badge variant={config.variant} className={config.className || ""}>
        <Icon className="mr-1 h-3 w-3" />
        {config.label}
      </Badge>
    )
  }

  const getConnectionStatusBadge = (status: string) => {
    const statusConfig = {
      'ONLINE': { variant: 'default' as const, label: 'Online', icon: IconWifi, className: 'bg-green-100 text-green-800' },
      'OFFLINE': { variant: 'secondary' as const, label: 'Offline', icon: IconWifiOff, className: '' },
      'ERROR': { variant: 'destructive' as const, label: 'Error', icon: IconX, className: '' },
    }
    
    const config = statusConfig[status as keyof typeof statusConfig] || statusConfig.OFFLINE
    const Icon = config.icon
    
    return (
      <Badge variant={config.variant} className={config.className || ""}>
        <Icon className="mr-1 h-3 w-3" />
        {config.label}
      </Badge>
    )
  }

  const quickActions = [
    {
      title: "Manage Devices",
      description: "Add, edit, and configure attendance devices",
      icon: IconDeviceDesktop,
      href: "/attendance/devices",
      color: "bg-blue-500"
    },
    {
      title: "View Logs",
      description: "Browse and manage attendance records",
      icon: IconFileText,
      href: "/attendance/logs",
      color: "bg-green-500"
    },
    {
      title: "Sync Management",
      description: "Control data synchronization",
      icon: IconRefresh,
      href: "/attendance/sync",
      color: "bg-purple-500"
    },
    {
      title: "Export Data",
      description: "Export attendance data in various formats",
      icon: IconDownload,
      href: "/attendance/export",
      color: "bg-orange-500"
    },
    {
      title: "Statistics",
      description: "View detailed attendance analytics",
      icon: IconActivity,
      href: "/attendance/statistics",
      color: "bg-indigo-500"
    },
    {
      title: "Configuration",
      description: "Configure sync and system settings",
      icon: IconSettings,
      href: "/attendance/sync/configuration",
      color: "bg-gray-500"
    }
  ]

  if (loading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Attendance Dashboard</h1>
            <p className="text-muted-foreground">Overview of your attendance management system</p>
          </div>
        </div>
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto mb-4"></div>
              <p className="text-muted-foreground">Loading dashboard...</p>
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Attendance Dashboard</h1>
          <p className="text-muted-foreground">Overview of your attendance management system</p>
        </div>
        <Button
          variant="outline"
          onClick={loadDashboardData}
        >
          <IconRefresh className="mr-2 h-4 w-4" />
          Refresh
        </Button>
      </div>

      {error && (
        <div className="text-sm text-red-500 bg-red-50 p-4 rounded-md border border-red-200">
          <div className="flex items-center">
            <IconAlertCircle className="mr-2 h-4 w-4" />
            {error}
          </div>
        </div>
      )}

      {/* Statistics Overview */}
      {statistics && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Devices</CardTitle>
              <IconDeviceDesktop className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statistics?.TotalDevices || 0}</div>
              <p className="text-xs text-muted-foreground">
                Registered devices
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Logs</CardTitle>
              <IconClock className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statistics?.TotalLogs || 0}</div>
              <p className="text-xs text-muted-foreground">
                Total attendance records
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Sync Logs</CardTitle>
              <IconActivity className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statistics?.TotalSyncLogs || 0}</div>
              <p className="text-xs text-muted-foreground">
                Total sync operations
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Last Sync</CardTitle>
              <IconActivity className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="flex items-center space-x-2">
                {getSyncStatusBadge(statistics?.LastSyncTime === "0001-01-01T00:00:00" ? "IDLE" : "SYNCING")}
              </div>
              <p className="text-xs text-muted-foreground mt-1">
                {statistics?.LastSyncTime && statistics.LastSyncTime !== "0001-01-01T00:00:00" ? 
                  `Last sync: ${formatDate(statistics.LastSyncTime)}` :
                  'No recent sync'
                }
              </p>
            </CardContent>
          </Card>
        </div>
      )}


      {/* Device Status Summary */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center">
            <IconDeviceDesktop className="mr-2 h-5 w-5" />
            Device Status Summary
          </CardTitle>
          <p className="text-sm text-muted-foreground mt-2">
            Status is updated periodically. Use &quot;Test Connection&quot; in device management for real-time connectivity check.
          </p>
        </CardHeader>
        <CardContent>
          {!deviceStatuses || deviceStatuses.length === 0 ? (
            <div className="text-center py-8">
              <IconDeviceDesktop className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">No devices found</h3>
              <p className="text-muted-foreground mb-4">
                Get started by adding your first attendance device
              </p>
              <Button onClick={() => router.push('/attendance/devices/new')}>
                Add Device
              </Button>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {(deviceStatuses || []).slice(0, 6).map((device) => (
                <div key={device.id} className="border rounded-lg p-4">
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center space-x-2">
                      <IconDeviceDesktop className="h-4 w-4 text-gray-400" />
                      <div>
                        <div className="font-medium">{device.deviceName}</div>
                        <div className="text-sm text-muted-foreground">
                          ID: {device.id}
                        </div>
                      </div>
                    </div>
                    {getConnectionStatusBadge(device.status)}
                  </div>
                  
                  <div className="space-y-2">
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">IP Address:</span>
                      <span className="font-mono text-xs">{device.ipAddress}</span>
                    </div>
                    
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">Total Logs:</span>
                      <span>{device.logCount}</span>
                    </div>
                    
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">Pending Logs:</span>
                      <span className={device.userCount > 0 ? "text-orange-600" : "text-green-600"}>
                        {device.userCount}
                      </span>
                    </div>
                    
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">Last Sync:</span>
                      <span className="text-xs">
                        {device.lastLogDownloadTime && device.lastLogDownloadTime !== "0001-01-01T00:00:00" ? 
                          new Date(device.lastLogDownloadTime).toLocaleDateString() :
                          'Never'
                        }
                      </span>
                    </div>
                    
                    {device.errorMessage && (
                      <div className="text-xs text-red-600 bg-red-50 p-2 rounded">
                        {device.errorMessage}
                      </div>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
          
          {deviceStatuses.length > 6 && (
            <div className="mt-4 text-center">
              <Button
                variant="outline"
                onClick={() => router.push('/attendance/devices')}
              >
                View All Devices
                <IconArrowRight className="ml-2 h-4 w-4" />
              </Button>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Quick Actions */}
      <Card>
        <CardHeader>
          <CardTitle>Quick Actions</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {quickActions.map((action, index) => (
              <div
                key={index}
                className="border rounded-lg p-4 hover:shadow-md transition-shadow cursor-pointer"
                onClick={() => router.push(action.href)}
              >
                <div className="flex items-center space-x-3">
                  <div className={`p-2 rounded-lg ${action.color} text-white`}>
                    <action.icon className="h-5 w-5" />
                  </div>
                  <div className="flex-1">
                    <h3 className="font-medium">{action.title}</h3>
                    <p className="text-sm text-muted-foreground">{action.description}</p>
                  </div>
                  <IconArrowRight className="h-4 w-4 text-muted-foreground" />
                </div>
              </div>
            ))}
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
