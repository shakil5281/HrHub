"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Calendar } from "@/components/ui/calendar"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { IconCalendar, IconDownload, IconRefresh, IconFileText, IconUsers, IconClock, IconAlertCircle, IconCircleCheck, IconCircleX, IconUser } from "@tabler/icons-react"
import { format } from "date-fns"
import { cn } from "@/lib/utils"
import { getEmployeeAttendanceReport, getEmployeeAttendanceReportById, exportEmployeeAttendanceReport, type EmployeeAttendanceReport, type DailyAttendanceRecord } from "@/lib/api/attendance-report"
import { getAllEmployees, type Employee, type EmployeesResponse } from "@/lib/api/employee"
import { toast } from "sonner"

export default function EmployeeAttendanceReportPage() {
  const [startDate, setStartDate] = useState<Date>(new Date(new Date().setDate(new Date().getDate() - 30)))
  const [endDate, setEndDate] = useState<Date>(new Date())
  const [selectedEmployee, setSelectedEmployee] = useState<number | undefined>(undefined)
  const [reportData, setReportData] = useState<EmployeeAttendanceReport[]>([])
  const [employees, setEmployees] = useState<Employee[]>([])
  const [loading, setLoading] = useState(false)
  const [exporting, setExporting] = useState(false)
  const [startCalendarOpen, setStartCalendarOpen] = useState(false)
  const [endCalendarOpen, setEndCalendarOpen] = useState(false)

  const loadEmployees = async () => {
    try {
      const response = await getAllEmployees()
      setEmployees(response.data)
    } catch (error) {
      console.error("Error loading employees:", error)
      toast.error("Failed to load employees")
    }
  }

  const loadReport = async () => {
    if (!startDate || !endDate) return

    setLoading(true)
    try {
      const params = {
        startDate: format(startDate, "yyyy-MM-dd"),
        endDate: format(endDate, "yyyy-MM-dd"),
        employeeId: selectedEmployee
      }
      
      const data = selectedEmployee 
        ? [await getEmployeeAttendanceReportById(selectedEmployee, { startDate: params.startDate, endDate: params.endDate })]
        : await getEmployeeAttendanceReport(params)
      
      setReportData(data)
    } catch (error) {
      console.error("Error loading employee report:", error)
      toast.error("Failed to load employee attendance report")
    } finally {
      setLoading(false)
    }
  }

  const handleExport = async () => {
    if (!startDate || !endDate) return

    setExporting(true)
    try {
      const params = {
        startDate: format(startDate, "yyyy-MM-dd"),
        endDate: format(endDate, "yyyy-MM-dd"),
        employeeId: selectedEmployee
      }
      
      const blob = await exportEmployeeAttendanceReport(params)
      
      // Create download link
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement("a")
      link.href = url
      link.download = `employee-attendance-report-${params.startDate}-to-${params.endDate}.csv`
      document.body.appendChild(link)
      link.click()
      document.body.removeChild(link)
      window.URL.revokeObjectURL(url)
      
      toast.success("Report exported successfully")
    } catch (error) {
      console.error("Error exporting report:", error)
      toast.error("Failed to export report")
    } finally {
      setExporting(false)
    }
  }

  const getStatusBadge = (status: string) => {
    switch (status) {
      case "PRESENT":
        return <Badge variant="default" className="bg-green-100 text-green-800"><IconCircleCheck className="w-3 h-3 mr-1" />Present</Badge>
      case "ABSENT":
        return <Badge variant="destructive"><IconCircleX className="w-3 h-3 mr-1" />Absent</Badge>
      case "LATE":
        return <Badge variant="secondary" className="bg-orange-100 text-orange-800"><IconClock className="w-3 h-3 mr-1" />Late</Badge>
      case "EARLY_DEPARTURE":
        return <Badge variant="outline" className="bg-yellow-100 text-yellow-800"><IconAlertCircle className="w-3 h-3 mr-1" />Early Departure</Badge>
      case "PARTIAL":
        return <Badge variant="outline" className="bg-blue-100 text-blue-800"><IconFileText className="w-3 h-3 mr-1" />Partial</Badge>
      default:
        return <Badge variant="outline">{status}</Badge>
    }
  }

  const formatTime = (time?: string) => {
    if (!time) return "-"
    return new Date(time).toLocaleTimeString()
  }

  const formatHours = (hours?: number) => {
    if (!hours) return "-"
    return `${hours.toFixed(2)}h`
  }

  useEffect(() => {
    loadEmployees()
  }, [])

  useEffect(() => {
    loadReport()
  }, [startDate, endDate, selectedEmployee])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Employee Attendance Report</h1>
          <p className="text-muted-foreground">
            View and export employee attendance reports for date ranges
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            onClick={loadReport}
            disabled={loading}
          >
            <IconRefresh className={cn("w-4 h-4 mr-2", loading && "animate-spin")} />
            Refresh
          </Button>
          <Button
            onClick={handleExport}
            disabled={exporting || reportData.length === 0}
          >
            <IconDownload className="w-4 h-4 mr-2" />
            {exporting ? "Exporting..." : "Export CSV"}
          </Button>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle>Report Filters</CardTitle>
          <CardDescription>Select the date range and employee for the attendance report</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            <div className="space-y-2">
              <Label htmlFor="startDate">Start Date</Label>
              <Popover open={startCalendarOpen} onOpenChange={setStartCalendarOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !startDate && "text-muted-foreground"
                    )}
                  >
                    <IconCalendar className="mr-2 h-4 w-4" />
                    {startDate ? format(startDate, "PPP") : "Pick start date"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={startDate}
                    onSelect={(date) => {
                      setStartDate(date || new Date())
                      setStartCalendarOpen(false)
                    }}
                    initialFocus
                  />
                </PopoverContent>
              </Popover>
            </div>
            <div className="space-y-2">
              <Label htmlFor="endDate">End Date</Label>
              <Popover open={endCalendarOpen} onOpenChange={setEndCalendarOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !endDate && "text-muted-foreground"
                    )}
                  >
                    <IconCalendar className="mr-2 h-4 w-4" />
                    {endDate ? format(endDate, "PPP") : "Pick end date"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={endDate}
                    onSelect={(date) => {
                      setEndDate(date || new Date())
                      setEndCalendarOpen(false)
                    }}
                    initialFocus
                  />
                </PopoverContent>
              </Popover>
            </div>
            <div className="space-y-2">
              <Label htmlFor="employee">Employee (Optional)</Label>
              <Select value={selectedEmployee?.toString() || "all"} onValueChange={(value) => setSelectedEmployee(value === "all" ? undefined : parseInt(value))}>
                <SelectTrigger>
                  <SelectValue placeholder="Select employee" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Employees</SelectItem>
                  {(employees || []).map((employee) => (
                    <SelectItem key={employee.id} value={employee.id.toString()}>
                      {employee.name} ({employee.id})
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Summary Cards */}
      {reportData.length > 0 && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Days</CardTitle>
              <IconCalendar className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{reportData[0]?.totalDays || 0}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Present Days</CardTitle>
              <IconCircleCheck className="h-4 w-4 text-green-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-green-600">{reportData[0]?.presentDays || 0}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Absent Days</CardTitle>
              <IconCircleX className="h-4 w-4 text-red-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-red-600">{reportData[0]?.absentDays || 0}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Attendance Rate</CardTitle>
              <IconClock className="h-4 w-4 text-blue-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-blue-600">{(reportData[0]?.attendanceRate || 0).toFixed(1)}%</div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Report Tabs */}
      {reportData.length > 0 && (
        <Tabs defaultValue="summary" className="space-y-4">
          <TabsList>
            <TabsTrigger value="summary">Summary</TabsTrigger>
            <TabsTrigger value="details">Daily Details</TabsTrigger>
          </TabsList>
          
          <TabsContent value="summary" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Employee Summary</CardTitle>
                <CardDescription>
                  Attendance summary for {selectedEmployee ? "selected employee" : "all employees"} from {format(startDate, "PPP")} to {format(endDate, "PPP")}
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="rounded-md border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Employee</TableHead>
                        <TableHead>Department</TableHead>
                        <TableHead>Total Days</TableHead>
                        <TableHead>Present</TableHead>
                        <TableHead>Absent</TableHead>
                        <TableHead>Late</TableHead>
                        <TableHead>Early Departure</TableHead>
                        <TableHead>Attendance Rate</TableHead>
                        <TableHead>Total Hours</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {(reportData || []).map((report) => (
                        <TableRow key={report.employeeId}>
                          <TableCell>
                            <div>
                              <div className="font-medium">{report.employeeName}</div>
                              <div className="text-sm text-muted-foreground">{report.employeeCode}</div>
                            </div>
                          </TableCell>
                          <TableCell>{report.department}</TableCell>
                          <TableCell>{report.totalDays}</TableCell>
                          <TableCell className="text-green-600 font-medium">{report.presentDays}</TableCell>
                          <TableCell className="text-red-600 font-medium">{report.absentDays}</TableCell>
                          <TableCell className="text-orange-600 font-medium">{report.lateDays}</TableCell>
                          <TableCell className="text-yellow-600 font-medium">{report.earlyDepartureDays}</TableCell>
                          <TableCell>
                            <Badge variant={(report.attendanceRate || 0) >= 90 ? "default" : (report.attendanceRate || 0) >= 70 ? "secondary" : "destructive"}>
                              {(report.attendanceRate || 0).toFixed(1)}%
                            </Badge>
                          </TableCell>
                          <TableCell>{formatHours(report.totalHours)}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
          
          <TabsContent value="details" className="space-y-4">
            {(reportData || []).map((report) => (
              <Card key={report.employeeId}>
                <CardHeader>
                  <CardTitle>{report.employeeName} - Daily Details</CardTitle>
                  <CardDescription>
                    Daily attendance records for {report.employeeCode} from {format(startDate, "PPP")} to {format(endDate, "PPP")}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="rounded-md border">
                    <Table>
                      <TableHeader>
                        <TableRow>
                          <TableHead>Date</TableHead>
                          <TableHead>Check In</TableHead>
                          <TableHead>Check Out</TableHead>
                          <TableHead>Total Hours</TableHead>
                          <TableHead>Status</TableHead>
                          <TableHead>Remarks</TableHead>
                        </TableRow>
                      </TableHeader>
                      <TableBody>
                        {(report.reports || []).map((record) => (
                          <TableRow key={record.date}>
                            <TableCell>{format(new Date(record.date), "PPP")}</TableCell>
                            <TableCell>{formatTime(record.checkInTime)}</TableCell>
                            <TableCell>{formatTime(record.checkOutTime)}</TableCell>
                            <TableCell>{formatHours(record.totalHours)}</TableCell>
                            <TableCell>{getStatusBadge(record.status)}</TableCell>
                            <TableCell>{record.remarks || "-"}</TableCell>
                          </TableRow>
                        ))}
                      </TableBody>
                    </Table>
                  </div>
                </CardContent>
              </Card>
            ))}
          </TabsContent>
        </Tabs>
      )}

      {loading && (
        <div className="flex items-center justify-center py-8">
          <IconRefresh className="w-6 h-6 animate-spin mr-2" />
          <span>Loading report...</span>
        </div>
      )}

      {!loading && reportData.length === 0 && (
        <Card>
          <CardContent className="flex items-center justify-center py-8">
            <div className="text-center">
              <IconUser className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
              <h3 className="text-lg font-medium mb-2">No Report Data</h3>
              <p className="text-muted-foreground">
                Select a date range and click refresh to load the employee attendance report.
              </p>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
