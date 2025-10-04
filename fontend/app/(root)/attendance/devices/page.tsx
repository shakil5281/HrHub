"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from "@/components/ui/table"
import { 
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { 
  getAllAttendanceDevices, 
  deleteAttendanceDevice, 
  testDeviceConnection,
  testAllDeviceConnections,
  type AttendanceDevice 
} from "@/lib/api/attendance"
import { 
  IconPlus, 
  IconDots, 
  IconEdit, 
  IconTrash, 
  IconWifi, 
  IconWifiOff,
  IconRefresh,
  IconHeartbeat,
  IconAlertCircle
} from "@tabler/icons-react"
import { toast } from "sonner"

export default function AttendanceDevicesPage() {
  const [devices, setDevices] = useState<AttendanceDevice[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [testingConnection, setTestingConnection] = useState<string | null>(null)
  const [healthChecking, setHealthChecking] = useState(false)
  const router = useRouter()

  const loadDevices = async () => {
    try {
      setLoading(true)
      const response = await getAllAttendanceDevices()
      if (response.success) {
        setDevices(response.data)
      } else {
        setError(response.message || 'Failed to load devices')
      }
    } catch (err) {
      console.error('Error loading devices:', err)
      setError('Failed to load devices')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    loadDevices()
  }, [])

  const handleDeleteDevice = async (deviceId: string, deviceName: string) => {
    if (!confirm(`Are you sure you want to delete device "${deviceName}"?`)) {
      return
    }

    try {
      const response = await deleteAttendanceDevice(deviceId)
      if (response.success) {
        toast.success('Device deleted successfully')
        loadDevices()
      } else {
        toast.error(response.message || 'Failed to delete device')
      }
    } catch (err) {
      console.error('Error deleting device:', err)
      toast.error('Failed to delete device')
    }
  }

  const handleTestConnection = async (deviceId: string) => {
    try {
      setTestingConnection(deviceId)
      const response = await testDeviceConnection(deviceId)
      if (response.success && response.isConnected) {
        toast.success('Connection test successful - Device is reachable')
        // Refresh devices to get updated status
        loadDevices()
      } else {
        toast.error(response.message || 'Connection test failed - Device is not reachable')
      }
    } catch (err) {
      console.error('Error testing connection:', err)
      toast.error('Connection test failed')
    } finally {
      setTestingConnection(null)
    }
  }

  const handleHealthCheck = async () => {
    try {
      setHealthChecking(true)
      const response = await testAllDeviceConnections()
      if (response.success) {
        toast.success('Health check completed')
        loadDevices()
      } else {
        toast.error(response.message || 'Health check failed')
      }
    } catch (err) {
      console.error('Error performing health check:', err)
      toast.error('Health check failed')
    } finally {
      setHealthChecking(false)
    }
  }

  const getStatusBadge = (device: AttendanceDevice) => {
    if (!device.isConnected) {
      return <Badge variant="secondary">Offline</Badge>
    }
    
    // You could add more sophisticated status checking here
    return <Badge variant="default">Online</Badge>
  }

  const getConnectionIcon = (device: AttendanceDevice) => {
    if (!device.isConnected) {
      return <IconWifiOff className="h-4 w-4 text-gray-400" />
    }
    
    // You could add more sophisticated connection status checking here
    return <IconWifi className="h-4 w-4 text-green-500" />
  }

  if (loading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Attendance Devices</h1>
            <p className="text-muted-foreground">Manage attendance devices and their connections</p>
          </div>
        </div>
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto mb-4"></div>
              <p className="text-muted-foreground">Loading devices...</p>
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
          <h1 className="text-3xl font-bold tracking-tight">Attendance Devices</h1>
          <p className="text-muted-foreground">Manage attendance devices and their connections</p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            onClick={handleHealthCheck}
            disabled={healthChecking}
          >
            <IconHeartbeat className="mr-2 h-4 w-4" />
            {healthChecking ? "Checking..." : "Health Check"}
          </Button>
          <Button
            variant="outline"
            onClick={loadDevices}
          >
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => router.push('/attendance/devices/new')}>
            <IconPlus className="mr-2 h-4 w-4" />
            Add Device
          </Button>
        </div>
      </div>

      {error && (
        <div className="text-sm text-red-500 bg-red-50 p-4 rounded-md border border-red-200">
          <div className="flex items-center">
            <IconAlertCircle className="mr-2 h-4 w-4" />
            {error}
          </div>
        </div>
      )}

      {/* Devices Table */}
      <Card>
        <CardHeader>
          <CardTitle>Devices ({devices.length})</CardTitle>
        </CardHeader>
        <CardContent>
          {devices.length === 0 ? (
            <div className="text-center py-12">
              <IconWifiOff className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">No devices found</h3>
              <p className="text-muted-foreground mb-4">
                Get started by adding your first attendance device
              </p>
              <Button onClick={() => router.push('/attendance/devices/new')}>
                <IconPlus className="mr-2 h-4 w-4" />
                Add Device
              </Button>
            </div>
          ) : (
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Device</TableHead>
                  <TableHead>Type</TableHead>
                  <TableHead>IP Address</TableHead>
                  <TableHead>Port</TableHead>
                  <TableHead>Status</TableHead>
                  <TableHead>Last Sync</TableHead>
                  <TableHead className="w-[50px]"></TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {devices.map((device) => (
                  <TableRow key={device.id}>
                    <TableCell>
                      <div className="flex items-center space-x-2">
                        {getConnectionIcon(device)}
                        <div>
                          <div className="font-medium">{device.deviceName}</div>
                          <div className="text-sm text-muted-foreground">
                            ID: {device.id}
                          </div>
                        </div>
                      </div>
                    </TableCell>
                    <TableCell>
                      <Badge variant="outline">{device.productName}</Badge>
                    </TableCell>
                    <TableCell className="font-mono">{device.ipAddress}</TableCell>
                    <TableCell>{device.port}</TableCell>
                    <TableCell>{getStatusBadge(device)}</TableCell>
                    <TableCell>
                      {device.lastLogDownloadTime ? (
                        new Date(device.lastLogDownloadTime).toLocaleString()
                      ) : (
                        <span className="text-muted-foreground">Never</span>
                      )}
                    </TableCell>
                    <TableCell>
                      <DropdownMenu>
                        <DropdownMenuTrigger asChild>
                          <Button variant="ghost" size="sm">
                            <IconDots className="h-4 w-4" />
                          </Button>
                        </DropdownMenuTrigger>
                        <DropdownMenuContent align="end">
                          <DropdownMenuItem
                            onClick={() => router.push(`/attendance/devices/${device.id}/edit`)}
                          >
                            <IconEdit className="mr-2 h-4 w-4" />
                            Edit
                          </DropdownMenuItem>
                          <DropdownMenuItem
                            onClick={() => handleTestConnection(device.id.toString())}
                            disabled={testingConnection === device.id.toString()}
                          >
                            <IconWifi className="mr-2 h-4 w-4" />
                            {testingConnection === device.id.toString() ? "Testing..." : "Test Connection"}
                          </DropdownMenuItem>
                          <DropdownMenuItem
                            onClick={() => handleDeleteDevice(device.id.toString(), device.deviceName)}
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
    </div>
  )
}
