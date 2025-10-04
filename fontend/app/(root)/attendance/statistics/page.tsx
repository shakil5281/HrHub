"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { 
  getAttendanceStatistics,
  getDeviceStatus,
  type AttendanceStatistics,
  type DeviceStatus 
} from "@/lib/api/attendance"
import { 
  IconUsers, 
  IconDeviceDesktop, 
  IconClock, 
  IconActivity,
  IconRefresh,
  IconWifi,
  IconWifiOff,
  IconBattery,
  IconSignalE,
  IconAlertCircle,
  IconCheck,
  IconX
} from "@tabler/icons-react"
import { toast } from "sonner"

export default function AttendanceStatisticsPage() {
  const [statistics, setStatistics] = useState<AttendanceStatistics | null>(null)
  const [deviceStatuses, setDeviceStatuses] = useState<DeviceStatus[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const loadStatistics = async () => {
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
      console.error('Error loading statistics:', err)
      setError('Failed to load statistics')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadStatistics()
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
      'CONNECTED': { variant: 'default' as const, label: 'Connected', icon: IconWifi, className: 'bg-green-100 text-green-800' },
      'DISCONNECTED': { variant: 'secondary' as const, label: 'Disconnected', icon: IconWifiOff, className: '' },
      'ERROR': { variant: 'destructive' as const, label: 'Error', icon: IconX, className: '' },
    }
    
    const config = statusConfig[status as keyof typeof statusConfig] || statusConfig.DISCONNECTED
    const Icon = config.icon
    
    return (
      <Badge variant={config.variant} className={config.className || ""}>
        <Icon className="mr-1 h-3 w-3" />
        {config.label}
      </Badge>
    )
  }

  if (loading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Attendance Statistics</h1>
            <p className="text-muted-foreground">Overview of attendance system performance</p>
          </div>
        </div>
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto mb-4"></div>
              <p className="text-muted-foreground">Loading statistics...</p>
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
          <h1 className="text-3xl font-bold tracking-tight">Attendance Statistics</h1>
          <p className="text-muted-foreground">Overview of attendance system performance</p>
        </div>
        <Button
          variant="outline"
          onClick={loadStatistics}
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
              <div className="text-2xl font-bold">{statistics.TotalDevices}</div>
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
              <div className="text-2xl font-bold">{statistics.TotalLogs.toLocaleString()}</div>
              <p className="text-xs text-muted-foreground">
                All time attendance records
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Sync Logs</CardTitle>
              <IconActivity className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{statistics.TotalSyncLogs}</div>
              <p className="text-xs text-muted-foreground">
                Total sync operations
              </p>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Database Size</CardTitle>
              <IconActivity className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{(statistics.DatabaseSize / 1024 / 1024).toFixed(1)} MB</div>
              <p className="text-xs text-muted-foreground">
                Database storage used
              </p>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Last Sync Information */}
      {statistics && (
        <Card>
          <CardHeader>
            <CardTitle className="text-lg">Last Sync Information</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Last Sync Time:</span>
                <span className="text-sm text-muted-foreground">
                  {statistics.LastSyncTime && statistics.LastSyncTime !== "0001-01-01T00:00:00" ? 
                    new Date(statistics.LastSyncTime).toLocaleString() :
                    'Never'
                  }
                </span>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-sm font-medium">Database Size:</span>
                <span className="text-sm text-muted-foreground">
                  {(statistics.DatabaseSize / 1024 / 1024).toFixed(2)} MB
                </span>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {/* Device Status */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center">
            <IconDeviceDesktop className="mr-2 h-5 w-5" />
            Device Status
          </CardTitle>
        </CardHeader>
        <CardContent>
          {deviceStatuses.length === 0 ? (
            <div className="text-center py-8">
              <IconDeviceDesktop className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">No devices found</h3>
              <p className="text-muted-foreground">
                Device status information will appear here
              </p>
            </div>
          ) : (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {deviceStatuses.map((device) => (
                <div key={device.deviceId} className="border rounded-lg p-4">
                  <div className="flex items-center justify-between mb-3">
                    <div className="flex items-center space-x-2">
                      <IconDeviceDesktop className="h-4 w-4 text-gray-400" />
                      <div>
                        <div className="font-medium">{device.deviceName}</div>
                        <div className="text-sm text-muted-foreground">
                          ID: {device.deviceId}
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
                      <span>{device.totalLogs}</span>
                    </div>
                    
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-muted-foreground">Pending Logs:</span>
                      <span className={device.pendingLogs > 0 ? "text-orange-600" : "text-green-600"}>
                        {device.pendingLogs}
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
        </CardContent>
      </Card>
    </div>
  )
}
