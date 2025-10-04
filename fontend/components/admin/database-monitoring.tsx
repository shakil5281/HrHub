"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog"
import { 
  IconDatabase, 
  IconTable, 
  IconActivity, 
  IconAlertCircle, 
  IconClock, 
  IconRefresh, 
  IconEye,
  IconTrendingUp,
  IconTrendingDown,
  IconUsers,
  IconCode,
  IconSettings
} from "@tabler/icons-react"
import { cn } from "@/lib/utils"
import { 
  getDatabaseTables,
  getTableInfo,
  getDatabaseConnections,
  getSlowQueries,
  getIndexUsage,
  getDatabaseConfiguration,
  getDatabaseAlerts,
  type TableInfo,
  type DatabaseConnection,
  type SlowQuery,
  type IndexUsage,
  type DatabaseConfiguration,
  type DatabaseAlert
} from "@/lib/api/system-management"
import { toast } from "sonner"

export function DatabaseMonitoringComponents() {
  const [tables, setTables] = useState<TableInfo[]>([])
  const [connections, setConnections] = useState<DatabaseConnection[]>([])
  const [slowQueries, setSlowQueries] = useState<SlowQuery[]>([])
  const [indexUsage, setIndexUsage] = useState<IndexUsage[]>([])
  const [configuration, setConfiguration] = useState<DatabaseConfiguration[]>([])
  const [alerts, setAlerts] = useState<DatabaseAlert[]>([])
  const [selectedTable, setSelectedTable] = useState<TableInfo | null>(null)
  const [loading, setLoading] = useState(false)

  const loadDatabaseData = async () => {
    setLoading(true)
    try {
      const [
        tablesData,
        connectionsData,
        slowQueriesData,
        indexUsageData,
        configurationData,
        alertsData
      ] = await Promise.all([
        getDatabaseTables(),
        getDatabaseConnections(),
        getSlowQueries(),
        getIndexUsage(),
        getDatabaseConfiguration(),
        getDatabaseAlerts()
      ])

      setTables(tablesData)
      setConnections(connectionsData)
      setSlowQueries(slowQueriesData)
      setIndexUsage(indexUsageData)
      setConfiguration(configurationData)
      setAlerts(alertsData)
    } catch (error) {
      console.error("Error loading database data:", error)
      toast.error("Failed to load database data")
    } finally {
      setLoading(false)
    }
  }

  const getAlertSeverityColor = (severity: string) => {
    switch (severity) {
      case 'critical': return 'text-red-600 bg-red-100'
      case 'high': return 'text-orange-600 bg-orange-100'
      case 'medium': return 'text-yellow-600 bg-yellow-100'
      case 'low': return 'text-blue-600 bg-blue-100'
      default: return 'text-gray-600 bg-gray-100'
    }
  }

  useEffect(() => {
    loadDatabaseData()
  }, [])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="text-2xl font-bold tracking-tight">Database Monitoring</h2>
          <p className="text-muted-foreground">
            Monitor database tables, connections, performance, and alerts
          </p>
        </div>
        <Button
          variant="outline"
          onClick={loadDatabaseData}
          disabled={loading}
        >
          <IconRefresh className={cn("w-4 h-4 mr-2", loading && "animate-spin")} />
          Refresh
        </Button>
      </div>

      <Tabs defaultValue="tables" className="space-y-4">
        <TabsList>
          <TabsTrigger value="tables">Tables</TabsTrigger>
          <TabsTrigger value="connections">Connections</TabsTrigger>
          <TabsTrigger value="performance">Performance</TabsTrigger>
          <TabsTrigger value="alerts">Alerts</TabsTrigger>
          <TabsTrigger value="configuration">Configuration</TabsTrigger>
        </TabsList>

        {/* Tables Tab */}
        <TabsContent value="tables" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconTable className="h-5 w-5" />
                <span>Database Tables</span>
              </CardTitle>
              <CardDescription>
                Overview of all database tables and their statistics
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="rounded-md border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Table Name</TableHead>
                      <TableHead>Rows</TableHead>
                      <TableHead>Data Size</TableHead>
                      <TableHead>Index Size</TableHead>
                      <TableHead>Total Size</TableHead>
                      <TableHead>Engine</TableHead>
                      <TableHead>Actions</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {tables.map((table) => (
                      <TableRow key={table.tableName}>
                        <TableCell className="font-medium">{table.tableName}</TableCell>
                        <TableCell>{table.rowCount.toLocaleString()}</TableCell>
                        <TableCell>{table.dataSize}</TableCell>
                        <TableCell>{table.indexSize}</TableCell>
                        <TableCell>{table.totalSize}</TableCell>
                        <TableCell>
                          <Badge variant="outline">{table.engine}</Badge>
                        </TableCell>
                        <TableCell>
                          <Dialog>
                            <DialogTrigger asChild>
                              <Button variant="ghost" size="sm">
                                <IconEye className="h-4 w-4" />
                              </Button>
                            </DialogTrigger>
                            <DialogContent className="max-w-2xl">
                              <DialogHeader>
                                <DialogTitle>Table Details: {table.tableName}</DialogTitle>
                                <DialogDescription>
                                  Detailed information about this table
                                </DialogDescription>
                              </DialogHeader>
                              <div className="space-y-4">
                                <div className="grid grid-cols-2 gap-4">
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Table Name</div>
                                    <div className="text-lg font-semibold">{table.tableName}</div>
                                  </div>
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Row Count</div>
                                    <div className="text-lg font-semibold">{table.rowCount.toLocaleString()}</div>
                                  </div>
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Data Size</div>
                                    <div className="text-lg font-semibold">{table.dataSize}</div>
                                  </div>
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Index Size</div>
                                    <div className="text-lg font-semibold">{table.indexSize}</div>
                                  </div>
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Total Size</div>
                                    <div className="text-lg font-semibold">{table.totalSize}</div>
                                  </div>
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Engine</div>
                                    <div className="text-lg font-semibold">{table.engine}</div>
                                  </div>
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Collation</div>
                                    <div className="text-lg font-semibold">{table.collation}</div>
                                  </div>
                                  <div>
                                    <div className="text-sm font-medium text-muted-foreground">Created At</div>
                                    <div className="text-lg font-semibold">{table.createdAt}</div>
                                  </div>
                                </div>
                              </div>
                            </DialogContent>
                          </Dialog>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Connections Tab */}
        <TabsContent value="connections" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconUsers className="h-5 w-5" />
                <span>Active Connections</span>
              </CardTitle>
              <CardDescription>
                Current database connections and their status
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="rounded-md border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>ID</TableHead>
                      <TableHead>User</TableHead>
                      <TableHead>Host</TableHead>
                      <TableHead>Database</TableHead>
                      <TableHead>Command</TableHead>
                      <TableHead>Time</TableHead>
                      <TableHead>State</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {connections.map((connection) => (
                      <TableRow key={connection.id}>
                        <TableCell className="font-medium">{connection.id}</TableCell>
                        <TableCell>{connection.user}</TableCell>
                        <TableCell>{connection.host}</TableCell>
                        <TableCell>{connection.database}</TableCell>
                        <TableCell>
                          <Badge variant="outline">{connection.command}</Badge>
                        </TableCell>
                        <TableCell>{connection.time}s</TableCell>
                        <TableCell>
                          <Badge variant={connection.state === 'Sleep' ? 'secondary' : 'default'}>
                            {connection.state}
                          </Badge>
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Performance Tab */}
        <TabsContent value="performance" className="space-y-4">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Slow Queries */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center space-x-2">
                  <IconClock className="h-5 w-5" />
                  <span>Slow Queries</span>
                </CardTitle>
                <CardDescription>
                  Queries taking longer than expected to execute
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {slowQueries.slice(0, 5).map((query, index) => (
                    <div key={index} className="p-3 border rounded-lg">
                      <div className="flex items-center justify-between mb-2">
                        <div className="text-sm font-medium">Execution Time: {query.executionTime}ms</div>
                        <Badge variant="outline">{query.user}@{query.host}</Badge>
                      </div>
                      <div className="text-xs text-muted-foreground font-mono bg-muted p-2 rounded">
                        {query.query.length > 100 ? `${query.query.substring(0, 100)}...` : query.query}
                      </div>
                      <div className="flex items-center justify-between mt-2 text-xs text-muted-foreground">
                        <span>Rows: {query.rowsExamined} examined, {query.rowsSent} sent</span>
                        <span>{query.timestamp}</span>
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>

            {/* Index Usage */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center space-x-2">
                  <IconTrendingUp className="h-5 w-5" />
                  <span>Index Usage Statistics</span>
                </CardTitle>
                <CardDescription>
                  Most frequently used database indexes
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="space-y-4">
                  {indexUsage.slice(0, 5).map((index, indexKey) => (
                    <div key={indexKey} className="p-3 border rounded-lg">
                      <div className="flex items-center justify-between mb-2">
                        <div className="font-medium">{index.tableName}.{index.indexName}</div>
                        <Badge variant="outline">{index.size}</Badge>
                      </div>
                      <div className="grid grid-cols-2 gap-4 text-sm">
                        <div>
                          <div className="text-muted-foreground">Usage Count</div>
                          <div className="font-semibold">{index.usageCount}</div>
                        </div>
                        <div>
                          <div className="text-muted-foreground">Cardinality</div>
                          <div className="font-semibold">{index.cardinality}</div>
                        </div>
                      </div>
                      <div className="text-xs text-muted-foreground mt-2">
                        Last used: {index.lastUsed}
                      </div>
                    </div>
                  ))}
                </div>
              </CardContent>
            </Card>
          </div>
        </TabsContent>

        {/* Alerts Tab */}
        <TabsContent value="alerts" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconAlertCircle className="h-5 w-5" />
                <span>Database Alerts</span>
              </CardTitle>
              <CardDescription>
                Current database alerts and warnings
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="space-y-4">
                {alerts.map((alert) => (
                  <div key={alert.id} className="p-4 border rounded-lg">
                    <div className="flex items-start justify-between">
                      <div className="flex items-start space-x-3">
                        <IconAlertCircle className={cn(
                          "h-5 w-5 mt-0.5",
                          alert.type === 'error' ? "text-red-500" :
                          alert.type === 'warning' ? "text-yellow-500" : "text-blue-500"
                        )} />
                        <div>
                          <div className="font-medium">{alert.message}</div>
                          <div className="text-sm text-muted-foreground mt-1">
                            {alert.timestamp}
                          </div>
                        </div>
                      </div>
                      <div className="flex items-center space-x-2">
                        <Badge className={getAlertSeverityColor(alert.severity)}>
                          {alert.severity}
                        </Badge>
                        {alert.resolved && (
                          <Badge variant="outline">Resolved</Badge>
                        )}
                      </div>
                    </div>
                  </div>
                ))}
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        {/* Configuration Tab */}
        <TabsContent value="configuration" className="space-y-4">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center space-x-2">
                <IconSettings className="h-5 w-5" />
                <span>Database Configuration</span>
              </CardTitle>
              <CardDescription>
                Current database configuration settings
              </CardDescription>
            </CardHeader>
            <CardContent>
              <div className="rounded-md border">
                <Table>
                  <TableHeader>
                    <TableRow>
                      <TableHead>Key</TableHead>
                      <TableHead>Value</TableHead>
                      <TableHead>Category</TableHead>
                      <TableHead>Description</TableHead>
                    </TableRow>
                  </TableHeader>
                  <TableBody>
                    {configuration.map((config) => (
                      <TableRow key={config.key}>
                        <TableCell className="font-medium">{config.key}</TableCell>
                        <TableCell>
                          <code className="text-sm bg-muted px-2 py-1 rounded">
                            {config.value}
                          </code>
                        </TableCell>
                        <TableCell>
                          <Badge variant="outline">{config.category}</Badge>
                        </TableCell>
                        <TableCell className="text-sm text-muted-foreground">
                          {config.description}
                        </TableCell>
                      </TableRow>
                    ))}
                  </TableBody>
                </Table>
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  )
}
