"use client"

import { useState, useEffect, useCallback } from "react"
import { useRouter } from "next/navigation"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Input } from "@/components/ui/input"
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
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { 
  getAttendanceLogs, 
  deleteAttendanceLog,
  getAllAttendanceDevices,
  type AttendanceLog,
  type AttendanceDevice 
} from "@/lib/api/attendance"
import { 
  IconDots, 
  IconTrash, 
  IconRefresh,
  IconFilter,
  IconDownload,
  IconClock,
  IconUser,
  IconDeviceDesktop,
  IconAlertCircle,
  IconArrowUp,
  IconArrowDown
} from "@tabler/icons-react"
import { toast } from "sonner"

export default function AttendanceLogsPage() {
  const [logs, setLogs] = useState<AttendanceLog[]>([])
  const [devices, setDevices] = useState<AttendanceDevice[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [totalCount, setTotalCount] = useState(0)
  const [currentPage, setCurrentPage] = useState(1)
  const [pageSize] = useState(20)
  
  // Filters
  const [filters, setFilters] = useState({
    employeeId: "",
    deviceId: "all",
    startDate: "",
    endDate: "",
    logType: "all",
  })
  
  const router = useRouter()

  const loadLogs = useCallback(async (page = 1) => {
    try {
      setLoading(true)
      const params: {
        page: number;
        pageSize: number;
        employeeId?: number;
        deviceId?: string;
        startDate?: string;
        endDate?: string;
        logType?: "IN" | "OUT";
      } = {
        page,
        pageSize,
      }
      
      if (filters.employeeId) params.employeeId = parseInt(filters.employeeId)
      if (filters.deviceId && filters.deviceId !== 'all') params.deviceId = filters.deviceId
      if (filters.startDate) params.startDate = filters.startDate
      if (filters.endDate) params.endDate = filters.endDate
      if (filters.logType && filters.logType !== 'all') params.logType = filters.logType as "IN" | "OUT"
      
      const response = await getAttendanceLogs(params)
      if (response.success) {
        setLogs(response.data)
        setTotalCount(response.totalCount)
        setCurrentPage(response.page)
      } else {
        setError(response.message || 'Failed to load logs')
      }
    } catch (err) {
      console.error('Error loading logs:', err)
      setError('Failed to load logs')
    } finally {
      setLoading(false)
    }
  }, [filters, pageSize])

  const loadDevices = useCallback(async () => {
    try {
      const response = await getAllAttendanceDevices()
      if (response.success) {
        setDevices(response.data)
      }
    } catch (err) {
      console.error('Error loading devices:', err)
    }
  }, [])

  useEffect(() => {
    loadLogs()
    loadDevices()
  }, [loadLogs, loadDevices])

  useEffect(() => {
    loadLogs(1)
  }, [filters, loadLogs])

  const handleDeleteLog = async (logId: number, employeeName: string) => {
    if (!confirm(`Are you sure you want to delete this attendance log for ${employeeName}?`)) {
      return
    }

    try {
      const response = await deleteAttendanceLog(logId)
      if (response.success) {
        toast.success('Attendance log deleted successfully')
        loadLogs(currentPage)
      } else {
        toast.error(response.message || 'Failed to delete log')
      }
    } catch (err) {
      console.error('Error deleting log:', err)
      toast.error('Failed to delete log')
    }
  }

  const handleFilterChange = (key: string, value: string) => {
    setFilters(prev => ({ ...prev, [key]: value }))
  }

  const clearFilters = () => {
    setFilters({
      employeeId: "",
      deviceId: "",
      startDate: "",
      endDate: "",
      logType: "",
    })
  }

  const getLogTypeBadge = (logType: string) => {
    if (logType === 'IN') {
      return <Badge variant="default" className="bg-green-100 text-green-800"><IconArrowDown className="mr-1 h-3 w-3" />Check In</Badge>
    } else {
      return <Badge variant="default" className="bg-red-100 text-red-800"><IconArrowUp className="mr-1 h-3 w-3" />Check Out</Badge>
    }
  }

  const getProcessedBadge = (isProcessed: boolean) => {
    return isProcessed ? 
      <Badge variant="default" className="bg-blue-100 text-blue-800">Processed</Badge> :
      <Badge variant="secondary">Pending</Badge>
  }

  const totalPages = Math.ceil(totalCount / pageSize)

  if (loading && logs.length === 0) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Attendance Logs</h1>
            <p className="text-muted-foreground">View and manage attendance records</p>
          </div>
        </div>
        <Card>
          <CardContent className="flex items-center justify-center py-12">
            <div className="text-center">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto mb-4"></div>
              <p className="text-muted-foreground">Loading logs...</p>
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
          <h1 className="text-3xl font-bold tracking-tight">Attendance Logs</h1>
          <p className="text-muted-foreground">View and manage attendance records</p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            onClick={() => loadLogs(currentPage)}
          >
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button
            variant="outline"
            onClick={() => router.push('/attendance/export')}
          >
            <IconDownload className="mr-2 h-4 w-4" />
            Export
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

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center">
            <IconFilter className="mr-2 h-5 w-5" />
            Filters
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-5 gap-4">
            <div>
              <label className="text-sm font-medium mb-2 block">Employee ID</label>
              <Input
                placeholder="Employee ID"
                value={filters.employeeId}
                onChange={(e) => handleFilterChange('employeeId', e.target.value)}
              />
            </div>
            
            <div>
              <label className="text-sm font-medium mb-2 block">Device</label>
              <Select value={filters.deviceId} onValueChange={(value) => handleFilterChange('deviceId', value)}>
                <SelectTrigger>
                  <SelectValue placeholder="All devices" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All devices</SelectItem>
                  {devices.map((device) => (
                    <SelectItem key={device.id} value={device.id.toString()}>
                      {device.deviceName}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
            
            <div>
              <label className="text-sm font-medium mb-2 block">Start Date</label>
              <Input
                type="date"
                value={filters.startDate}
                onChange={(e) => handleFilterChange('startDate', e.target.value)}
              />
            </div>
            
            <div>
              <label className="text-sm font-medium mb-2 block">End Date</label>
              <Input
                type="date"
                value={filters.endDate}
                onChange={(e) => handleFilterChange('endDate', e.target.value)}
              />
            </div>
            
            <div>
              <label className="text-sm font-medium mb-2 block">Log Type</label>
              <Select value={filters.logType} onValueChange={(value) => handleFilterChange('logType', value)}>
                <SelectTrigger>
                  <SelectValue placeholder="All types" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All types</SelectItem>
                  <SelectItem value="IN">Check In</SelectItem>
                  <SelectItem value="OUT">Check Out</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
          
          <div className="flex items-center justify-between mt-4">
            <Button variant="outline" onClick={clearFilters}>
              Clear Filters
            </Button>
            <div className="text-sm text-muted-foreground">
              {totalCount} total records
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Logs Table */}
      <Card>
        <CardHeader>
          <CardTitle>Attendance Logs ({totalCount})</CardTitle>
        </CardHeader>
        <CardContent>
          {logs.length === 0 ? (
            <div className="text-center py-12">
              <IconClock className="mx-auto h-12 w-12 text-gray-400 mb-4" />
              <h3 className="text-lg font-semibold mb-2">No logs found</h3>
              <p className="text-muted-foreground">
                No attendance logs match your current filters
              </p>
            </div>
          ) : (
            <>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Employee</TableHead>
                    <TableHead>Device</TableHead>
                    <TableHead>Log Time</TableHead>
                    <TableHead>Type</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Created</TableHead>
                    <TableHead className="w-[50px]"></TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {logs.map((log) => (
                    <TableRow key={log.id}>
                      <TableCell>
                        <div className="flex items-center space-x-2">
                          <IconUser className="h-4 w-4 text-gray-400" />
                          <div>
                            <div className="font-medium">{log.employeeName}</div>
                            <div className="text-sm text-muted-foreground">
                              ID: {log.employeeId}
                            </div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center space-x-2">
                          <IconDeviceDesktop className="h-4 w-4 text-gray-400" />
                          <div>
                            <div className="font-medium">{log.deviceName}</div>
                            <div className="text-sm text-muted-foreground">
                              ID: {log.deviceId}
                            </div>
                          </div>
                        </div>
                      </TableCell>
                      <TableCell>
                        <div className="flex items-center space-x-2">
                          <IconClock className="h-4 w-4 text-gray-400" />
                          {new Date(log.logTime).toLocaleString()}
                        </div>
                      </TableCell>
                      <TableCell>{getLogTypeBadge(log.logType)}</TableCell>
                      <TableCell>{getProcessedBadge(log.isProcessed)}</TableCell>
                      <TableCell>
                        {new Date(log.createdAt).toLocaleDateString()}
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
                              onClick={() => handleDeleteLog(log.id, log.employeeName)}
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

              {/* Pagination */}
              {totalPages > 1 && (
                <div className="flex items-center justify-between mt-4">
                  <div className="text-sm text-muted-foreground">
                    Showing {((currentPage - 1) * pageSize) + 1} to {Math.min(currentPage * pageSize, totalCount)} of {totalCount} results
                  </div>
                  <div className="flex items-center space-x-2">
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => loadLogs(currentPage - 1)}
                      disabled={currentPage === 1}
                    >
                      Previous
                    </Button>
                    <span className="text-sm">
                      Page {currentPage} of {totalPages}
                    </span>
                    <Button
                      variant="outline"
                      size="sm"
                      onClick={() => loadLogs(currentPage + 1)}
                      disabled={currentPage === totalPages}
                    >
                      Next
                    </Button>
                  </div>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </div>
  )
}
