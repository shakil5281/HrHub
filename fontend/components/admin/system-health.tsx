"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Progress } from "@/components/ui/progress"
import { 
  IconHeartbeat, 
  IconCircleCheck, 
  IconAlertCircle, 
  IconCircleX, 
  IconRefresh, 
  IconActivity,
  IconServer,
  IconDatabase,
  IconCpu
} from "@tabler/icons-react"
import { cn } from "@/lib/utils"
import { 
  getSystemHealth,
  getSystemStatistics,
  getDatabaseStatus,
  type SystemHealth,
  type SystemStatistics
} from "@/lib/api/system-management"
import { toast } from "sonner"

export function SystemHealthMonitoring() {
  const [systemHealth, setSystemHealth] = useState<SystemHealth | null>(null)
  const [systemStats, setSystemStats] = useState<SystemStatistics | null>(null)
  const [dbStatus, setDbStatus] = useState<{ status: 'online' | 'offline'; responseTime: number } | null>(null)
  const [loading, setLoading] = useState(false)
  const [lastCheck, setLastCheck] = useState<Date>(new Date())

  const loadHealthData = async () => {
    setLoading(true)
    try {
      const [healthData, statsData, statusData] = await Promise.all([
        getSystemHealth(),
        getSystemStatistics(),
        getDatabaseStatus()
      ])

      setSystemHealth(healthData)
      setSystemStats(statsData)
      setDbStatus(statusData)
      setLastCheck(new Date())
    } catch (error) {
      console.error("Error loading health data:", error)
      toast.error("Failed to load health data")
    } finally {
      setLoading(false)
    }
  }

  const getHealthStatusColor = (status: string) => {
    switch (status) {
      case 'pass': return 'text-green-600 bg-green-100'
      case 'warning': return 'text-yellow-600 bg-yellow-100'
      case 'fail': return 'text-red-600 bg-red-100'
      default: return 'text-gray-600 bg-gray-100'
    }
  }

  const getHealthStatusIcon = (status: string) => {
    switch (status) {
      case 'pass': return IconCircleCheck
      case 'warning': return IconAlertCircle
      case 'fail': return IconCircleX
      default: return IconAlertCircle
    }
  }

  const getOverallHealthColor = (status: string) => {
    switch (status) {
      case 'healthy': return 'text-green-600'
      case 'warning': return 'text-yellow-600'
      case 'critical': return 'text-red-600'
      default: return 'text-gray-600'
    }
  }

  const getResourceStatus = (percentage: number) => {
    if (percentage < 70) return { status: 'healthy', color: 'text-green-600' }
    if (percentage < 90) return { status: 'warning', color: 'text-yellow-600' }
    return { status: 'critical', color: 'text-red-600' }
  }

  useEffect(() => {
    loadHealthData()
    // Auto-refresh every 30 seconds
    const interval = setInterval(loadHealthData, 30000)
    return () => clearInterval(interval)
  }, [])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">System Health Monitoring</h2>
          <p className="text-muted-foreground">
            Real-time system health checks and resource monitoring
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <div className="text-sm text-muted-foreground">
            Last check: {lastCheck.toLocaleTimeString()}
          </div>
          <Button
            variant="outline"
            onClick={loadHealthData}
            disabled={loading}
          >
            <IconRefresh className={cn("w-4 h-4 mr-2", loading && "animate-spin")} />
            Refresh
          </Button>
        </div>
      </div>

      {/* Overall Health Status */}
      {systemHealth && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <IconHeartbeat className={cn("h-5 w-5", getOverallHealthColor(systemHealth.status))} />
              <span>Overall System Health</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-between mb-4">
              <div>
                <div className="text-3xl font-bold mb-2">{systemHealth.overallScore}%</div>
                <Badge className={cn(
                  "text-sm",
                  systemHealth.status === 'healthy' ? 'bg-green-100 text-green-800' :
                  systemHealth.status === 'warning' ? 'bg-yellow-100 text-yellow-800' :
                  'bg-red-100 text-red-800'
                )}>
                  {systemHealth.status?.toUpperCase() || 'UNKNOWN'}
                </Badge>
              </div>
              <div className="text-right">
                <div className="text-sm text-muted-foreground">Last Checked</div>
                <div className="font-medium">{new Date(systemHealth.lastChecked).toLocaleTimeString()}</div>
              </div>
            </div>
            <Progress value={systemHealth.overallScore} className="h-3" />
          </CardContent>
        </Card>
      )}

      {/* Health Checks */}
      {systemHealth && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <IconActivity className="h-5 w-5" />
              <span>Health Checks</span>
            </CardTitle>
            <CardDescription>
              Individual system component health status
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
              {(systemHealth.checks || []).map((check, index) => {
                const Icon = getHealthStatusIcon(check.status)
                return (
                  <div key={index} className="p-4 border rounded-lg">
                    <div className="flex items-center justify-between mb-2">
                      <div className="flex items-center space-x-2">
                        <Icon className={cn(
                          "h-4 w-4",
                          check.status === 'pass' ? 'text-green-600' :
                          check.status === 'warning' ? 'text-yellow-600' : 'text-red-600'
                        )} />
                        <span className="font-medium">{check.name}</span>
                      </div>
                      <Badge className={getHealthStatusColor(check.status)}>
                        {check.status?.toUpperCase() || 'UNKNOWN'}
                      </Badge>
                    </div>
                    <div className="text-sm text-muted-foreground mb-2">
                      {check.message}
                    </div>
                    {check.responseTime && (
                      <div className="text-xs text-muted-foreground">
                        Response time: {check.responseTime}ms
                      </div>
                    )}
                    <div className="text-xs text-muted-foreground mt-2">
                      {new Date(check.lastChecked).toLocaleTimeString()}
                    </div>
                  </div>
                )
              })}
            </div>
          </CardContent>
        </Card>
      )}

      {/* Resource Monitoring */}
      {systemStats && (
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* CPU Usage */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconCpu className="h-5 w-5" />
                <span>CPU Usage</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div>
                  <div className="flex justify-between text-sm mb-1">
                    <span>Current Usage</span>
                    <span className={getResourceStatus(systemStats.systemLoad?.cpuUsage || 0).color}>
                      {systemStats.systemLoad?.cpuUsage || 0}%
                    </span>
                  </div>
                  <Progress value={systemStats.systemLoad?.cpuUsage || 0} className="h-2" />
                </div>
                <div className="grid grid-cols-3 gap-2 text-center">
                  <div>
                    <div className="text-lg font-semibold">{systemStats.systemLoad?.loadAverage1m || 0}</div>
                    <div className="text-xs text-muted-foreground">1m</div>
                  </div>
                  <div>
                    <div className="text-lg font-semibold">{systemStats.systemLoad?.loadAverage5m || 0}</div>
                    <div className="text-xs text-muted-foreground">5m</div>
                  </div>
                  <div>
                    <div className="text-lg font-semibold">{systemStats.systemLoad?.loadAverage15m || 0}</div>
                    <div className="text-xs text-muted-foreground">15m</div>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Memory Usage */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconActivity className="h-5 w-5" />
                <span>Memory Usage</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div>
                  <div className="flex justify-between text-sm mb-1">
                    <span>Memory Usage</span>
                    <span className={getResourceStatus(systemStats.memoryUsage?.percentage || 0).color}>
                      {systemStats.memoryUsage?.percentage || 0}%
                    </span>
                  </div>
                  <Progress value={systemStats.memoryUsage?.percentage || 0} className="h-2" />
                </div>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span>Used</span>
                    <span className="font-medium">{systemStats.memoryUsage?.used || '0'}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Free</span>
                    <span className="font-medium">{systemStats.memoryUsage?.free || '0'}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Total</span>
                    <span className="font-medium">{systemStats.memoryUsage?.total || '0'}</span>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>

          {/* Disk Usage */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconServer className="h-5 w-5" />
                <span>Disk Usage</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                <div>
                  <div className="flex justify-between text-sm mb-1">
                    <span>Disk Usage</span>
                    <span className={getResourceStatus(systemStats.diskUsage?.percentage || 0).color}>
                      {systemStats.diskUsage?.percentage || 0}%
                    </span>
                  </div>
                  <Progress value={systemStats.diskUsage?.percentage || 0} className="h-2" />
                </div>
                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span>Used</span>
                    <span className="font-medium">{systemStats.diskUsage?.used || '0'}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Free</span>
                    <span className="font-medium">{systemStats.diskUsage?.free || '0'}</span>
                  </div>
                  <div className="flex justify-between">
                    <span>Total</span>
                    <span className="font-medium">{systemStats.diskUsage?.total || '0'}</span>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Database Status */}
      {dbStatus && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <IconDatabase className="h-5 w-5" />
              <span>Database Status</span>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <div className="flex items-center justify-between">
              <div className="flex items-center space-x-3">
                <div className={cn(
                  "w-3 h-3 rounded-full",
                  dbStatus.status === 'online' ? 'bg-green-500' : 'bg-red-500'
                )} />
                <div>
                  <div className="font-medium">
                    Database is {dbStatus.status}
                  </div>
                  <div className="text-sm text-muted-foreground">
                    Response time: {dbStatus.responseTime}ms
                  </div>
                </div>
              </div>
              <Badge variant={dbStatus.status === 'online' ? 'default' : 'destructive'}>
                {dbStatus.status?.toUpperCase() || 'UNKNOWN'}
              </Badge>
            </div>
          </CardContent>
        </Card>
      )}

      {/* System Statistics Summary */}
      {systemStats && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center space-x-2">
              <IconServer className="h-5 w-5" />
              <span>System Statistics</span>
            </CardTitle>
            <CardDescription>
              Current system usage and activity metrics
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
              <div className="text-center">
                <div className="text-2xl font-bold text-green-600">{systemStats.activeUsers}</div>
                <div className="text-sm text-muted-foreground">Active Users</div>
                <div className="text-xs text-muted-foreground">
                  of {systemStats.totalUsers} total
                </div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold">{systemStats.totalEmployees}</div>
                <div className="text-sm text-muted-foreground">Employees</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold">{systemStats.totalCompanies}</div>
                <div className="text-sm text-muted-foreground">Companies</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold">{systemStats.totalDepartments}</div>
                <div className="text-sm text-muted-foreground">Departments</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold">{systemStats.attendanceRecords}</div>
                <div className="text-sm text-muted-foreground">Attendance Records</div>
              </div>
              <div className="text-center">
                <div className="text-2xl font-bold">{systemStats.backupFiles}</div>
                <div className="text-sm text-muted-foreground">Backup Files</div>
              </div>
            </div>
          </CardContent>
        </Card>
      )}

      {loading && (
        <div className="flex items-center justify-center py-8">
          <IconRefresh className="w-6 h-6 animate-spin mr-2" />
          <span>Loading health data...</span>
        </div>
      )}
    </div>
  )
}
