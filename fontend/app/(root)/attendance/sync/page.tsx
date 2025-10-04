"use client"

import { useState, useEffect } from "react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { 
  getAllAttendanceDevices,
  downloadDeviceLogs,
  downloadAllDeviceLogs,
  type AttendanceDevice 
} from "@/lib/api/attendance"
import { 
  IconDownload, 
  IconRefresh, 
  IconDeviceDesktop,
  IconCheck,
  IconX
} from "@tabler/icons-react"
import { toast } from "sonner"

export default function AttendanceSyncPage() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [devices, setDevices] = useState<AttendanceDevice[]>([])
  const [downloading, setDownloading] = useState<string | null>(null)
  const [bulkDownloading, setBulkDownloading] = useState(false)

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

  const handleDownloadDeviceLogs = async (deviceId: string, deviceName: string) => {
    try {
      setDownloading(deviceId)
      const response = await downloadDeviceLogs(deviceId)
      if (response.success) {
        toast.success(`Logs downloaded successfully from ${deviceName}`)
      } else {
        toast.error(response.message || 'Failed to download logs')
      }
    } catch (err) {
      console.error('Error downloading logs:', err)
      toast.error('Failed to download logs')
    } finally {
      setDownloading(null)
    }
  }

  const handleDownloadAllLogs = async () => {
    try {
      setBulkDownloading(true)
      const response = await downloadAllDeviceLogs()
      if (response.success) {
        toast.success('All device logs downloaded successfully')
      } else {
        toast.error(response.message || 'Failed to download all logs')
      }
    } catch (err) {
      console.error('Error downloading all logs:', err)
      toast.error('Failed to download all logs')
    } finally {
      setBulkDownloading(false)
    }
  }

  const getConnectionStatus = (device: AttendanceDevice) => {
    if (device.isConnected) {
      return <Badge variant="default" className="bg-green-100 text-green-800">Online</Badge>
    }
    return <Badge variant="destructive">Offline</Badge>
  }

  if (loading) {
    return (
      <div className="space-y-6">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Download Device Logs</h1>
          <p className="text-muted-foreground">
            Download attendance logs from ZK devices
          </p>
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
          <h1 className="text-3xl font-bold tracking-tight">Download Device Logs</h1>
          <p className="text-muted-foreground">
            Download attendance logs from ZK devices
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            size="sm"
            onClick={loadDevices}
            disabled={loading}
          >
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button
            onClick={handleDownloadAllLogs}
            disabled={bulkDownloading || devices.length === 0}
          >
            <IconDownload className="mr-2 h-4 w-4" />
            {bulkDownloading ? "Downloading All..." : "Download All Logs"}
          </Button>
        </div>
      </div>

      {error && (
        <div className="text-sm text-red-500 bg-red-50 p-4 rounded-md border border-red-200">
          {error}
        </div>
      )}

      {/* Devices List */}
      <div className="grid gap-4">
        {devices.map((device) => (
          <Card key={device.id}>
            <CardHeader>
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-3">
                  <IconDeviceDesktop className="h-5 w-5 text-muted-foreground" />
                  <div>
                    <CardTitle className="text-lg">{device.deviceName}</CardTitle>
                    <p className="text-sm text-muted-foreground">
                      {device.ipAddress}:{device.port} • {device.location}
                    </p>
                  </div>
                </div>
                <div className="flex items-center space-x-2">
                  {getConnectionStatus(device)}
                  <Button
                    size="sm"
                    onClick={() => handleDownloadDeviceLogs(device.id.toString(), device.deviceName)}
                    disabled={downloading === device.id.toString() || !device.isConnected}
                  >
                    {downloading === device.id.toString() ? (
                      <>
                        <IconRefresh className="mr-2 h-4 w-4 animate-spin" />
                        Downloading...
                      </>
                    ) : (
                      <>
                        <IconDownload className="mr-2 h-4 w-4" />
                        Download Logs
                      </>
                    )}
                  </Button>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-2 md:grid-cols-4 gap-4 text-sm">
                <div>
                  <p className="text-muted-foreground">Serial Number</p>
                  <p className="font-medium">{device.serialNumber}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Product</p>
                  <p className="font-medium">{device.productName}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Log Count</p>
                  <p className="font-medium">{device.logCount}</p>
                </div>
                <div>
                  <p className="text-muted-foreground">Last Download</p>
                  <p className="font-medium">
                    {device.lastLogDownloadTime 
                      ? new Date(device.lastLogDownloadTime).toLocaleDateString()
                      : 'Never'
                    }
                  </p>
                </div>
              </div>
            </CardContent>
          </Card>
        ))}
      </div>

      {devices.length === 0 && !loading && (
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="text-center">
              <IconDeviceDesktop className="h-12 w-12 text-muted-foreground mx-auto mb-4" />
              <h3 className="text-lg font-semibold mb-2">No devices found</h3>
              <p className="text-muted-foreground">
                No ZK devices are configured. Add devices to start downloading logs.
              </p>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}