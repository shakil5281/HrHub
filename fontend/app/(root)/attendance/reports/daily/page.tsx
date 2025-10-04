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
import { IconCalendar, IconDownload, IconRefresh, IconFileText, IconUsers, IconClock, IconAlertCircle, IconCircleCheck, IconCircleX } from "@tabler/icons-react"
import { format } from "date-fns"
import { cn } from "@/lib/utils"
import { getDailyAttendanceReport, getAllEmployeesDailyReport, exportDailyAttendanceReport, type DailyAttendanceReport, type EmployeeDailyReport } from "@/lib/api/attendance-report"
import { toast } from "sonner"

export default function DailyAttendanceReportPage() {
  const [selectedDate, setSelectedDate] = useState<Date>(new Date())
  const [reportData, setReportData] = useState<DailyAttendanceReport | null>(null)
  const [loading, setLoading] = useState(false)
  const [exporting, setExporting] = useState(false)
  const [reportType, setReportType] = useState<"all" | "specific">("all")
  const [calendarOpen, setCalendarOpen] = useState(false)

  const loadReport = async () => {
    if (!selectedDate) return

    setLoading(true)
    try {
      const dateString = format(selectedDate, "yyyy-MM-dd")
      const data = reportType === "all" 
        ? await getAllEmployeesDailyReport(dateString)
        : await getDailyAttendanceReport(dateString)
      setReportData(data)
    } catch (error) {
      console.error("Error loading daily report:", error)
      toast.error("Failed to load daily attendance report")
    } finally {
      setLoading(false)
    }
  }

  const handleExport = async () => {
    if (!selectedDate) return

    setExporting(true)
    try {
      const dateString = format(selectedDate, "yyyy-MM-dd")
      const blob = await exportDailyAttendanceReport(dateString)
      
      // Create download link
      const url = window.URL.createObjectURL(blob)
      const link = document.createElement("a")
      link.href = url
      link.download = `daily-attendance-report-${dateString}.csv`
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
    loadReport()
  }, [selectedDate, reportType])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Daily Attendance Report</h1>
          <p className="text-muted-foreground">
            View and export daily attendance reports for all employees
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
            disabled={exporting || !reportData}
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
          <CardDescription>Select the date and report type for the daily attendance report</CardDescription>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <Label htmlFor="date">Select Date</Label>
              <Popover open={calendarOpen} onOpenChange={setCalendarOpen}>
                <PopoverTrigger asChild>
                  <Button
                    variant="outline"
                    className={cn(
                      "w-full justify-start text-left font-normal",
                      !selectedDate && "text-muted-foreground"
                    )}
                  >
                    <IconCalendar className="mr-2 h-4 w-4" />
                    {selectedDate ? format(selectedDate, "PPP") : "Pick a date"}
                  </Button>
                </PopoverTrigger>
                <PopoverContent className="w-auto p-0" align="start">
                  <Calendar
                    mode="single"
                    selected={selectedDate}
                    onSelect={(date) => {
                      setSelectedDate(date || new Date())
                      setCalendarOpen(false)
                    }}
                    initialFocus
                  />
                </PopoverContent>
              </Popover>
            </div>
            <div className="space-y-2">
              <Label htmlFor="reportType">Report Type</Label>
              <Select value={reportType} onValueChange={(value: "all" | "specific") => setReportType(value)}>
                <SelectTrigger>
                  <SelectValue placeholder="Select report type" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Employees</SelectItem>
                  <SelectItem value="specific">Specific Report</SelectItem>
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Summary Cards */}
      {reportData && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Employees</CardTitle>
              <IconUsers className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{reportData.totalEmployees}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Present</CardTitle>
              <IconCircleCheck className="h-4 w-4 text-green-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-green-600">{reportData.presentEmployees}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Absent</CardTitle>
              <IconCircleX className="h-4 w-4 text-red-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-red-600">{reportData.absentEmployees}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Attendance Rate</CardTitle>
              <IconClock className="h-4 w-4 text-blue-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-blue-600">{(reportData.attendanceRate || 0).toFixed(1)}%</div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Detailed Report Table */}
      {reportData && (
        <Card>
          <CardHeader>
            <CardTitle>Employee Details</CardTitle>
            <CardDescription>
              Detailed attendance information for {format(selectedDate, "PPP")}
            </CardDescription>
          </CardHeader>
          <CardContent>
            <div className="rounded-md border">
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Employee</TableHead>
                    <TableHead>Department</TableHead>
                    <TableHead>Designation</TableHead>
                    <TableHead>Check In</TableHead>
                    <TableHead>Check Out</TableHead>
                    <TableHead>Total Hours</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Remarks</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {(reportData.reports || []).map((report) => (
                    <TableRow key={report.employeeId}>
                      <TableCell>
                        <div>
                          <div className="font-medium">{report.employeeName}</div>
                          <div className="text-sm text-muted-foreground">{report.employeeCode}</div>
                        </div>
                      </TableCell>
                      <TableCell>{report.department}</TableCell>
                      <TableCell>{report.designation}</TableCell>
                      <TableCell>{formatTime(report.checkInTime)}</TableCell>
                      <TableCell>{formatTime(report.checkOutTime)}</TableCell>
                      <TableCell>{formatHours(report.totalHours)}</TableCell>
                      <TableCell>{getStatusBadge(report.status)}</TableCell>
                      <TableCell>{report.remarks || "-"}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </div>
          </CardContent>
        </Card>
      )}

      {loading && (
        <div className="flex items-center justify-center py-8">
          <IconRefresh className="w-6 h-6 animate-spin mr-2" />
          <span>Loading report...</span>
        </div>
      )}

      {!loading && !reportData && (
        <Card>
          <CardContent className="flex items-center justify-center py-8">
            <div className="text-center">
              <IconFileText className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
              <h3 className="text-lg font-medium mb-2">No Report Data</h3>
              <p className="text-muted-foreground">
                Select a date and click refresh to load the daily attendance report.
              </p>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
