"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Progress } from "@/components/ui/progress"
import { 
  IconServer, 
  IconDatabase, 
  IconActivity, 
  IconRefresh, 
  IconAlertCircle, 
  IconCircleCheck, 
  IconClock, 
  IconCpu, 
  IconSettings, 
  IconInfoCircle,
  IconTrendingUp,
  IconCode,
  IconGitBranch
} from "@tabler/icons-react"
import { cn } from "@/lib/utils"
import { formatDate } from "@/lib/utils"
import { 
  getSystemInfo,
  getDatabaseInfo,
  getSystemHealth,
  getSystemStatistics,
  getDatabasePerformance,
  getDatabaseStatus,
  getVersionInfo,
  getEnvironmentInfo,
  type SystemInfo,
  type DatabaseInfo,
  type SystemHealth,
  type SystemStatistics,
  type DatabasePerformance,
  type VersionInfo,
  type EnvironmentInfo
} from "@/lib/api/system-management"
import { toast } from "sonner"

export default function SystemManagementPage() {
  const [systemInfo, setSystemInfo] = useState<SystemInfo | null>(null)
  const [databaseInfo, setDatabaseInfo] = useState<DatabaseInfo | null>(null)
  const [systemHealth, setSystemHealth] = useState<SystemHealth | null>(null)
  const [systemStats, setSystemStats] = useState<SystemStatistics | null>(null)
  const [dbPerformance, setDbPerformance] = useState<DatabasePerformance | null>(null)
  const [dbStatus, setDbStatus] = useState<{ status: 'online' | 'offline'; responseTime: number } | null>(null)
  const [versionInfo, setVersionInfo] = useState<VersionInfo | null>(null)
  const [environmentInfo, setEnvironmentInfo] = useState<EnvironmentInfo | null>(null)
  const [loading, setLoading] = useState(false)
  const [lastRefresh, setLastRefresh] = useState<Date | null>(null)

  const loadSystemData = async () => {
    setLoading(true)
    try {
      const [
        systemData,
        databaseData,
        healthData,
        statsData,
        performanceData,
        statusData,
        versionData,
        environmentData
      ] = await Promise.all([
        getSystemInfo(),
        getDatabaseInfo(),
        getSystemHealth(),
        getSystemStatistics(),
        getDatabasePerformance(),
        getDatabaseStatus(),
        getVersionInfo(),
        getEnvironmentInfo()
      ])

      setSystemInfo(systemData)
      setDatabaseInfo(databaseData)
      setSystemHealth(healthData)
      setSystemStats(statsData)
      setDbPerformance(performanceData)
      setDbStatus(statusData)
      setVersionInfo(versionData)
      setEnvironmentInfo(environmentData)
      setLastRefresh(new Date())
    } catch (error) {
      console.error("Error loading system data:", error)
      toast.error("Failed to load system data")
    } finally {
      setLoading(false)
    }
  }

  const getHealthStatusColor = (status: string) => {
    switch (status) {
      case 'healthy': return 'text-green-600 bg-green-100'
      case 'warning': return 'text-yellow-600 bg-yellow-100'
      case 'critical': return 'text-red-600 bg-red-100'
      default: return 'text-gray-600 bg-gray-100'
    }
  }

  const getHealthStatusIcon = (status: string) => {
    switch (status) {
      case 'healthy': return IconCircleCheck
      case 'warning': return IconAlertCircle
      case 'critical': return IconAlertCircle
      default: return IconInfoCircle
    }
  }

  useEffect(() => {
    loadSystemData()
  }, [])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">System Management</h1>
          <p className="text-muted-foreground">
            Monitor system health, database performance, and configuration
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <div className="text-sm text-muted-foreground">
            Last updated: {lastRefresh ? formatDate(lastRefresh) : 'Never'}
          </div>
          <Button
            variant="outline"
            onClick={loadSystemData}
            disabled={loading}
          >
            <IconRefresh className={cn("w-4 h-4 mr-2", loading && "animate-spin")} />
            Refresh
          </Button>
        </div>
      </div>

      {/* System Health Overview */}
      {systemHealth && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">System Health</CardTitle>
              {(() => {
                const Icon = getHealthStatusIcon(systemHealth.status)
                return <Icon className="h-4 w-4 text-muted-foreground" />
              })()}
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold mb-2">{systemHealth.overallScore}%</div>
              <Badge className={getHealthStatusColor(systemHealth.status)}>
                {systemHealth.status?.toUpperCase() || 'UNKNOWN'}
              </Badge>
            </CardContent>
          </Card>
          
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Database Status</CardTitle>
              <IconDatabase className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold mb-2">
                {dbStatus?.status === 'online' ? 'Online' : 'Offline'}
              </div>
              <div className="text-sm text-muted-foreground">
                {dbStatus?.responseTime}ms response time
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Active Users</CardTitle>
              <IconActivity className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{systemStats?.activeUsers || 0}</div>
              <div className="text-sm text-muted-foreground">
                of {systemStats?.totalUsers || 0} total users
              </div>
            </CardContent>
          </Card>

          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">System Uptime</CardTitle>
              <IconClock className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{systemInfo?.uptime || 'N/A'}</div>
              <div className="text-sm text-muted-foreground">
                Since {systemInfo?.serverTime || 'N/A'}
              </div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Main Content Tabs */}
      <Tabs defaultValue="overview" className="space-y-4">
        <TabsList>
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="database">Database</TabsTrigger>
          <TabsTrigger value="performance">Performance</TabsTrigger>
          <TabsTrigger value="configuration">Configuration</TabsTrigger>
          <TabsTrigger value="version">Version Info</TabsTrigger>
        </TabsList>

        {/* Overview Tab */}
        <TabsContent value="overview" className="space-y-4">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* System Information */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center space-x-2">
                  <IconServer className="h-5 w-5" />
                  <span>System Information</span>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {systemInfo ? (
                  <>
                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">System Name</div>
                        <div className="text-lg font-semibold">{systemInfo.systemName}</div>
                      </div>
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">Version</div>
                        <div className="text-lg font-semibold">{systemInfo.version}</div>
                      </div>
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">Environment</div>
                        <div className="text-lg font-semibold">{systemInfo.environment}</div>
                      </div>
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">OS</div>
                        <div className="text-lg font-semibold">{systemInfo.os}</div>
                      </div>
                    </div>
                    <div className="pt-4 border-t">
                      <div className="text-sm font-medium text-muted-foreground mb-2">Network Interfaces</div>
                      <div className="space-y-2">
                        {(systemInfo.networkInterfaces || []).map((iface, index) => (
                          <div key={index} className="flex items-center justify-between">
                            <span className="font-medium">{iface.name}</span>
                            <div className="flex items-center space-x-2">
                              <span className="text-sm text-muted-foreground">{iface.ipAddress}</span>
                              <Badge variant={iface.status === 'up' ? 'default' : 'secondary'}>
                                {iface.status}
                              </Badge>
                            </div>
                          </div>
                        ))}
                      </div>
                    </div>
                  </>
                ) : (
                  <div className="text-center py-8 text-muted-foreground">
                    Loading system information...
                  </div>
                )}
              </CardContent>
            </Card>

            {/* Database Information */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center space-x-2">
                  <IconDatabase className="h-5 w-5" />
                  <span>Database Information</span>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                {databaseInfo ? (
                  <>
                    <div className="grid grid-cols-2 gap-4">
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">Database Name</div>
                        <div className="text-lg font-semibold">{databaseInfo.name}</div>
                      </div>
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">Version</div>
                        <div className="text-lg font-semibold">{databaseInfo.version}</div>
                      </div>
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">Host</div>
                        <div className="text-lg font-semibold">{databaseInfo.host}:{databaseInfo.port}</div>
                      </div>
                      <div>
                        <div className="text-sm font-medium text-muted-foreground">Status</div>
                        <Badge variant={databaseInfo.status === 'online' ? 'default' : 'destructive'}>
                          {databaseInfo.status}
                        </Badge>
                      </div>
                    </div>
                    <div className="pt-4 border-t">
                      <div className="grid grid-cols-2 gap-4">
                        <div>
                          <div className="text-sm font-medium text-muted-foreground">Connections</div>
                          <div className="text-lg font-semibold">
                            {databaseInfo.connections}/{databaseInfo.maxConnections}
                          </div>
                        </div>
                        <div>
                          <div className="text-sm font-medium text-muted-foreground">Database Size</div>
                          <div className="text-lg font-semibold">{databaseInfo.databaseSize}</div>
                        </div>
                      </div>
                    </div>
                  </>
                ) : (
                  <div className="text-center py-8 text-muted-foreground">
                    Loading database information...
                  </div>
                )}
              </CardContent>
            </Card>
          </div>

          {/* System Statistics */}
          {systemStats && (
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center space-x-2">
                  <IconTrendingUp className="h-5 w-5" />
                  <span>System Statistics</span>
                </CardTitle>
              </CardHeader>
              <CardContent>
                <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-6 gap-4">
                  <div className="text-center">
                    <div className="text-2xl font-bold">{systemStats.totalUsers}</div>
                    <div className="text-sm text-muted-foreground">Total Users</div>
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
        </TabsContent>

        {/* Database Tab */}
        <TabsContent value="database" className="space-y-4">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Database Performance */}
            {dbPerformance && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center space-x-2">
                    <IconActivity className="h-5 w-5" />
                    <span>Database Performance</span>
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Queries/Second</div>
                      <div className="text-lg font-semibold">{dbPerformance.queriesPerSecond}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Avg Query Time</div>
                      <div className="text-lg font-semibold">{dbPerformance.averageQueryTime}ms</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Slow Queries</div>
                      <div className="text-lg font-semibold">{dbPerformance.slowQueries}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Active Connections</div>
                      <div className="text-lg font-semibold">{dbPerformance.connectionsActive}</div>
                    </div>
                  </div>
                  <div className="pt-4 border-t">
                    <div className="space-y-3">
                      <div>
                        <div className="flex justify-between text-sm mb-1">
                          <span>Buffer Pool Hit Rate</span>
                          <span>{dbPerformance.bufferPoolHitRate}%</span>
                        </div>
                        <Progress value={dbPerformance.bufferPoolHitRate} className="h-2" />
                      </div>
                      <div>
                        <div className="flex justify-between text-sm mb-1">
                          <span>Cache Hit Rate</span>
                          <span>{dbPerformance.cacheHitRate}%</span>
                        </div>
                        <Progress value={dbPerformance.cacheHitRate} className="h-2" />
                      </div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* System Resources */}
            {systemStats && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center space-x-2">
                    <IconCpu className="h-5 w-5" />
                    <span>System Resources</span>
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <div className="flex justify-between text-sm mb-1">
                      <span>CPU Usage</span>
                      <span>{systemStats.systemLoad?.cpuUsage || 0}%</span>
                    </div>
                    <Progress value={systemStats.systemLoad?.cpuUsage || 0} className="h-2" />
                  </div>
                  <div>
                    <div className="flex justify-between text-sm mb-1">
                      <span>Memory Usage</span>
                      <span>{systemStats.memoryUsage?.percentage || 0}%</span>
                    </div>
                    <Progress value={systemStats.memoryUsage?.percentage || 0} className="h-2" />
                    <div className="text-xs text-muted-foreground mt-1">
                      {systemStats.memoryUsage?.used || '0'} / {systemStats.memoryUsage?.total || '0'}
                    </div>
                  </div>
                  <div>
                    <div className="flex justify-between text-sm mb-1">
                      <span>Disk Usage</span>
                      <span>{systemStats.diskUsage?.percentage || 0}%</span>
                    </div>
                    <Progress value={systemStats.diskUsage?.percentage || 0} className="h-2" />
                    <div className="text-xs text-muted-foreground mt-1">
                      {systemStats.diskUsage?.used || '0'} / {systemStats.diskUsage?.total || '0'}
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
          </div>
        </TabsContent>

        {/* Performance Tab */}
        <TabsContent value="performance" className="space-y-4">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Load Averages */}
            {systemStats && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center space-x-2">
                    <IconTrendingUp className="h-5 w-5" />
                    <span>Load Averages</span>
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid grid-cols-3 gap-4">
                    <div className="text-center">
                      <div className="text-2xl font-bold">{systemStats.systemLoad?.loadAverage1m || 0}</div>
                      <div className="text-sm text-muted-foreground">1 min</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold">{systemStats.systemLoad?.loadAverage5m || 0}</div>
                      <div className="text-sm text-muted-foreground">5 min</div>
                    </div>
                    <div className="text-center">
                      <div className="text-2xl font-bold">{systemStats.systemLoad?.loadAverage15m || 0}</div>
                      <div className="text-sm text-muted-foreground">15 min</div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Database Metrics */}
            {dbPerformance && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center space-x-2">
                    <IconDatabase className="h-5 w-5" />
                    <span>Database Metrics</span>
                  </CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Lock Waits</div>
                      <div className="text-lg font-semibold">{dbPerformance.lockWaits}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Deadlocks</div>
                      <div className="text-lg font-semibold">{dbPerformance.deadlocks}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Idle Connections</div>
                      <div className="text-lg font-semibold">{dbPerformance.connectionsIdle}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Response Time</div>
                      <div className="text-lg font-semibold">{dbStatus?.responseTime}ms</div>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}
          </div>
        </TabsContent>

        {/* Configuration Tab */}
        <TabsContent value="configuration" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconSettings className="h-5 w-5" />
                <span>Environment Configuration</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              {environmentInfo ? (
                <div className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Environment</div>
                      <div className="text-lg font-semibold">{environmentInfo.nodeEnv}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Port</div>
                      <div className="text-lg font-semibold">{environmentInfo.port}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Log Level</div>
                      <div className="text-lg font-semibold">{environmentInfo.logLevel}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Features</div>
                      <div className="flex flex-wrap gap-1">
                        {(environmentInfo.features || []).map((feature, index) => (
                          <Badge key={index} variant="outline" className="text-xs">
                            {feature}
                          </Badge>
                        ))}
                      </div>
                    </div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-8 text-muted-foreground">
                  Loading environment configuration...
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        {/* Version Tab */}
        <TabsContent value="version" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconCode className="h-5 w-5" />
                <span>Version Information</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              {versionInfo ? (
                <div className="space-y-4">
                  <div className="grid grid-cols-2 gap-4">
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Application Version</div>
                      <div className="text-lg font-semibold">{versionInfo.applicationVersion}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Build Number</div>
                      <div className="text-lg font-semibold">{versionInfo.buildNumber}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Build Date</div>
                      <div className="text-lg font-semibold">{versionInfo.buildDate}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Git Branch</div>
                      <div className="text-lg font-semibold flex items-center">
                        <IconGitBranch className="h-4 w-4 mr-1" />
                        {versionInfo.gitBranch}
                      </div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Git Commit</div>
                      <div className="text-lg font-semibold font-mono text-sm">{versionInfo.gitCommit}</div>
                    </div>
                    <div>
                      <div className="text-sm font-medium text-muted-foreground">Node Version</div>
                      <div className="text-lg font-semibold">{versionInfo.nodeVersion}</div>
                    </div>
                  </div>
                  <div className="pt-4 border-t">
                    <div className="text-sm font-medium text-muted-foreground mb-2">Dependencies</div>
                    <div className="space-y-2">
                      {(versionInfo.dependencies || []).map((dep, index) => (
                        <div key={index} className="flex items-center justify-between">
                          <span className="font-medium">{dep.name}</span>
                          <div className="flex items-center space-x-2">
                            <span className="text-sm text-muted-foreground">{dep.version}</span>
                            {dep.outdated && (
                              <Badge variant="outline" className="text-xs">
                                Outdated
                              </Badge>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              ) : (
                <div className="text-center py-8 text-muted-foreground">
                  Loading version information...
                </div>
              )}
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>

      {loading && (
        <div className="flex items-center justify-center py-8">
          <IconRefresh className="w-6 h-6 animate-spin mr-2" />
          <span>Loading system data...</span>
        </div>
      )}
    </div>
  )
}
