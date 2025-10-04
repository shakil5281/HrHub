"use client"

import { useState, useEffect } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Label } from "@/components/ui/label"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Calendar } from "@/components/ui/calendar"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { IconCalendar, IconRefresh, IconChartBar, IconUsers, IconClock, IconAlertCircle, IconCircleCheck, IconCircleX, IconTrendingUp, IconBuilding } from "@tabler/icons-react"
import { format } from "date-fns"
import { cn } from "@/lib/utils"
import { getAttendanceSummary, getAttendanceLogSummary, type AttendanceSummary, type AttendanceLogSummary, type DepartmentSummary, type DeviceLogSummary } from "@/lib/api/attendance-report"
import { getAllDepartments, type Department, type DepartmentsResponse } from "@/lib/api/department"
import { toast } from "sonner"

export default function AttendanceSummaryReportPage() {
  const [startDate, setStartDate] = useState<Date>(new Date(new Date().setDate(new Date().getDate() - 30)))
  const [endDate, setEndDate] = useState<Date>(new Date())
  const [selectedDepartment, setSelectedDepartment] = useState<number | undefined>(undefined)
  const [summaryData, setSummaryData] = useState<AttendanceSummary | null>(null)
  const [logSummaryData, setLogSummaryData] = useState<AttendanceLogSummary | null>(null)
  const [departments, setDepartments] = useState<Department[]>([])
  const [loading, setLoading] = useState(false)
  const [startCalendarOpen, setStartCalendarOpen] = useState(false)
  const [endCalendarOpen, setEndCalendarOpen] = useState(false)

  const loadDepartments = async () => {
    try {
      const response = await getAllDepartments()
      setDepartments(response.data)
    } catch (error) {
      console.error("Error loading departments:", error)
      toast.error("Failed to load departments")
    }
  }

  const loadSummaryReport = async () => {
    if (!startDate || !endDate) return

    setLoading(true)
    try {
      const params = {
        startDate: format(startDate, "yyyy-MM-dd"),
        endDate: format(endDate, "yyyy-MM-dd"),
        departmentId: selectedDepartment
      }
      
      const [summary, logSummary] = await Promise.all([
        getAttendanceSummary(params),
        getAttendanceLogSummary({ startDate: params.startDate, endDate: params.endDate })
      ])
      
      setSummaryData(summary)
      setLogSummaryData(logSummary)
    } catch (error) {
      console.error("Error loading summary report:", error)
      toast.error("Failed to load attendance summary report")
    } finally {
      setLoading(false)
    }
  }

  const getAttendanceRateColor = (rate: number) => {
    if (rate >= 90) return "text-green-600"
    if (rate >= 70) return "text-orange-600"
    return "text-red-600"
  }

  const getAttendanceRateBadge = (rate?: number) => {
    const safeRate = rate || 0
    if (safeRate >= 90) return <Badge variant="default" className="bg-green-100 text-green-800">{safeRate.toFixed(1)}%</Badge>
    if (safeRate >= 70) return <Badge variant="secondary" className="bg-orange-100 text-orange-800">{safeRate.toFixed(1)}%</Badge>
    return <Badge variant="destructive">{safeRate.toFixed(1)}%</Badge>
  }

  const formatHours = (hours?: number) => {
    if (!hours) return "-"
    return `${hours.toFixed(2)}h`
  }

  useEffect(() => {
    loadDepartments()
  }, [])

  useEffect(() => {
    loadSummaryReport()
  }, [startDate, endDate, selectedDepartment])

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Attendance Summary Report</h1>
          <p className="text-muted-foreground">
            View comprehensive attendance summaries and analytics
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button
            variant="outline"
            onClick={loadSummaryReport}
            disabled={loading}
          >
            <IconRefresh className={cn("w-4 h-4 mr-2", loading && "animate-spin")} />
            Refresh
          </Button>
        </div>
      </div>

      {/* Filters */}
      <Card>
        <CardHeader>
          <CardTitle>Report Filters</CardTitle>
          <CardDescription>Select the date range and department for the summary report</CardDescription>
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
              <Label htmlFor="department">Department (Optional)</Label>
              <Select value={selectedDepartment?.toString() || "all"} onValueChange={(value) => setSelectedDepartment(value === "all" ? undefined : parseInt(value))}>
                <SelectTrigger>
                  <SelectValue placeholder="Select department" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="all">All Departments</SelectItem>
                  {(departments || []).map((department) => (
                    <SelectItem key={department.id || 0} value={(department.id || 0).toString()}>
                      {department.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Overall Summary Cards */}
      {summaryData && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Total Employees</CardTitle>
              <IconUsers className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{summaryData.totalEmployees}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Working Days</CardTitle>
              <IconCalendar className="h-4 w-4 text-muted-foreground" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold">{summaryData.totalWorkingDays}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Present Days</CardTitle>
              <IconCircleCheck className="h-4 w-4 text-green-600" />
            </CardHeader>
            <CardContent>
              <div className="text-2xl font-bold text-green-600">{summaryData.totalPresentDays}</div>
            </CardContent>
          </Card>
          <Card>
            <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
              <CardTitle className="text-sm font-medium">Overall Rate</CardTitle>
              <IconTrendingUp className="h-4 w-4 text-blue-600" />
            </CardHeader>
            <CardContent>
              <div className={cn("text-2xl font-bold", getAttendanceRateColor(summaryData.overallAttendanceRate))}>
                {(summaryData.overallAttendanceRate || 0).toFixed(1)}%
              </div>
            </CardContent>
          </Card>
        </div>
      )}

      {/* Detailed Reports */}
      {summaryData && logSummaryData && (
        <Tabs defaultValue="attendance" className="space-y-4">
          <TabsList>
            <TabsTrigger value="attendance">Attendance Summary</TabsTrigger>
            <TabsTrigger value="departments">Department Breakdown</TabsTrigger>
            <TabsTrigger value="logs">Log Summary</TabsTrigger>
          </TabsList>
          
          <TabsContent value="attendance" className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Card>
                <CardHeader>
                  <CardTitle>Attendance Statistics</CardTitle>
                  <CardDescription>Overall attendance metrics for the selected period</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Absent Days</span>
                    <span className="text-red-600 font-bold">{summaryData.totalAbsentDays}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Late Days</span>
                    <span className="text-orange-600 font-bold">{summaryData.totalLateDays}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Early Departures</span>
                    <span className="text-yellow-600 font-bold">{summaryData.totalEarlyDepartureDays}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Average Hours/Day</span>
                    <span className="text-blue-600 font-bold">{formatHours(summaryData.averageHoursPerDay)}</span>
                  </div>
                </CardContent>
              </Card>
              
              <Card>
                <CardHeader>
                  <CardTitle>Attendance Rate</CardTitle>
                  <CardDescription>Overall attendance performance</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="text-center">
                    <div className={cn("text-4xl font-bold mb-2", getAttendanceRateColor(summaryData.overallAttendanceRate))}>
                      {(summaryData.overallAttendanceRate || 0).toFixed(1)}%
                    </div>
                    <div className="text-sm text-muted-foreground">
                      {summaryData.overallAttendanceRate >= 90 ? "Excellent" : 
                       summaryData.overallAttendanceRate >= 70 ? "Good" : "Needs Improvement"}
                    </div>
                  </div>
                </CardContent>
              </Card>
            </div>
          </TabsContent>
          
          <TabsContent value="departments" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Department Breakdown</CardTitle>
                <CardDescription>
                  Attendance performance by department from {format(startDate, "PPP")} to {format(endDate, "PPP")}
                </CardDescription>
              </CardHeader>
              <CardContent>
                <div className="rounded-md border">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Department</TableHead>
                        <TableHead>Employees</TableHead>
                        <TableHead>Present Days</TableHead>
                        <TableHead>Absent Days</TableHead>
                        <TableHead>Late Days</TableHead>
                        <TableHead>Early Departures</TableHead>
                        <TableHead>Attendance Rate</TableHead>
                        <TableHead>Avg Hours/Day</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      {(summaryData.departmentSummary || []).map((dept) => (
                        <TableRow key={dept.departmentId}>
                          <TableCell className="font-medium">{dept.departmentName}</TableCell>
                          <TableCell>{dept.totalEmployees}</TableCell>
                          <TableCell className="text-green-600 font-medium">{dept.presentDays}</TableCell>
                          <TableCell className="text-red-600 font-medium">{dept.absentDays}</TableCell>
                          <TableCell className="text-orange-600 font-medium">{dept.lateDays}</TableCell>
                          <TableCell className="text-yellow-600 font-medium">{dept.earlyDepartureDays}</TableCell>
                          <TableCell>{getAttendanceRateBadge(dept.attendanceRate)}</TableCell>
                          <TableCell>{formatHours(dept.averageHoursPerDay)}</TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
          
          <TabsContent value="logs" className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <Card>
                <CardHeader>
                  <CardTitle>Log Statistics</CardTitle>
                  <CardDescription>Overall log metrics for the selected period</CardDescription>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Total Logs</span>
                    <span className="font-bold">{logSummaryData.totalLogs}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Check Ins</span>
                    <span className="text-green-600 font-bold">{logSummaryData.totalCheckIns}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Check Outs</span>
                    <span className="text-blue-600 font-bold">{logSummaryData.totalCheckOuts}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Total Hours</span>
                    <span className="text-purple-600 font-bold">{formatHours(logSummaryData.totalHours)}</span>
                  </div>
                  <div className="flex items-center justify-between">
                    <span className="text-sm font-medium">Avg Hours/Day</span>
                    <span className="text-orange-600 font-bold">{formatHours(logSummaryData.averageHoursPerDay)}</span>
                  </div>
                </CardContent>
              </Card>
              
              <Card>
                <CardHeader>
                  <CardTitle>Device Summary</CardTitle>
                  <CardDescription>Log activity by device</CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="space-y-3">
                    {(logSummaryData.deviceSummary || []).map((device) => (
                      <div key={device.deviceId} className="flex items-center justify-between p-3 border rounded-lg">
                        <div>
                          <div className="font-medium">{device.deviceName}</div>
                          <div className="text-sm text-muted-foreground">ID: {device.deviceId}</div>
                        </div>
                        <div className="text-right">
                          <div className="text-sm font-medium">{device.totalLogs} logs</div>
                          <div className="text-xs text-muted-foreground">
                            {device.checkIns} in, {device.checkOuts} out
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>
          </TabsContent>
        </Tabs>
      )}

      {loading && (
        <div className="flex items-center justify-center py-8">
          <IconRefresh className="w-6 h-6 animate-spin mr-2" />
          <span>Loading summary report...</span>
        </div>
      )}

      {!loading && !summaryData && (
        <Card>
          <CardContent className="flex items-center justify-center py-8">
            <div className="text-center">
              <IconChartBar className="w-12 h-12 text-muted-foreground mx-auto mb-4" />
              <h3 className="text-lg font-medium mb-2">No Summary Data</h3>
              <p className="text-muted-foreground">
                Select a date range and click refresh to load the attendance summary report.
              </p>
            </div>
          </CardContent>
        </Card>
      )}
    </div>
  )
}
