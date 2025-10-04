"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { IconFileText, IconUsers, IconChartBar, IconCalendar, IconDownload, IconTrendingUp, IconClock, IconCircleCheck } from "@tabler/icons-react"
import Link from "next/link"

export default function AttendanceReportsPage() {
  const reportTypes = [
    {
      title: "Daily Report",
      description: "View daily attendance reports for all employees on specific dates",
      icon: IconCalendar,
      href: "/attendance/reports/daily",
      features: [
        "Daily attendance overview",
        "Employee check-in/out times",
        "Attendance status tracking",
        "CSV export functionality"
      ]
    },
    {
      title: "Employee Report",
      description: "Generate detailed attendance reports for specific employees or all employees over date ranges",
      icon: IconUsers,
      href: "/attendance/reports/employee",
      features: [
        "Employee-specific reports",
        "Date range filtering",
        "Attendance rate calculations",
        "Daily detail breakdown"
      ]
    },
    {
      title: "Summary Report",
      description: "Comprehensive attendance analytics and department-wise performance summaries",
      icon: IconChartBar,
      href: "/attendance/reports/summary",
      features: [
        "Overall attendance statistics",
        "Department performance",
        "Device log summaries",
        "Trend analysis"
      ]
    }
  ]

  const quickActions = [
    {
      title: "Today's Report",
      description: "Quick access to today's attendance report",
      icon: IconCircleCheck,
      href: "/attendance/reports/daily",
      color: "bg-green-50 text-green-700 border-green-200 hover:bg-green-100"
    },
    {
      title: "This Month",
      description: "View attendance summary for current month",
      icon: IconTrendingUp,
      href: "/attendance/reports/summary",
      color: "bg-blue-50 text-blue-700 border-blue-200 hover:bg-blue-100"
    },
    {
      title: "Export Data",
      description: "Download attendance data in CSV format",
      icon: IconDownload,
      href: "/attendance/reports/employee",
      color: "bg-purple-50 text-purple-700 border-purple-200 hover:bg-purple-100"
    }
  ]

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Attendance Reports</h1>
        <p className="text-muted-foreground">
          Generate and analyze comprehensive attendance reports for your organization
        </p>
      </div>

      {/* Quick Actions */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {quickActions.map((action) => (
          <Link key={action.title} href={action.href}>
            <Card className={`transition-colors cursor-pointer ${action.color}`}>
              <CardContent className="p-4">
                <div className="flex items-center space-x-3">
                  <action.icon className="h-8 w-8" />
                  <div>
                    <h3 className="font-semibold">{action.title}</h3>
                    <p className="text-sm opacity-80">{action.description}</p>
                  </div>
                </div>
              </CardContent>
            </Card>
          </Link>
        ))}
      </div>

      {/* Report Types */}
      <div className="space-y-4">
        <h2 className="text-2xl font-semibold">Report Types</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {reportTypes.map((report) => (
            <Card key={report.title} className="hover:shadow-lg transition-shadow">
              <CardHeader>
                <div className="flex items-center space-x-3">
                  <div className="p-2 bg-primary/10 rounded-lg">
                    <report.icon className="h-6 w-6 text-primary" />
                  </div>
                  <div>
                    <CardTitle className="text-lg">{report.title}</CardTitle>
                    <CardDescription>{report.description}</CardDescription>
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-4">
                <ul className="space-y-2">
                  {report.features.map((feature, index) => (
                    <li key={index} className="flex items-center space-x-2 text-sm">
                      <IconCircleCheck className="h-4 w-4 text-green-500 flex-shrink-0" />
                      <span>{feature}</span>
                    </li>
                  ))}
                </ul>
                <Link href={report.href}>
                  <Button className="w-full">
                    <IconFileText className="h-4 w-4 mr-2" />
                    Generate Report
                  </Button>
                </Link>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>

      {/* Features Overview */}
      <Card>
        <CardHeader>
          <CardTitle>Report Features</CardTitle>
          <CardDescription>Comprehensive attendance reporting capabilities</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div className="text-center">
              <div className="p-3 bg-blue-100 rounded-full w-fit mx-auto mb-3">
                <IconCalendar className="h-6 w-6 text-blue-600" />
              </div>
              <h3 className="font-semibold mb-2">Date Filtering</h3>
              <p className="text-sm text-muted-foreground">
                Filter reports by specific dates or date ranges
              </p>
            </div>
            <div className="text-center">
              <div className="p-3 bg-green-100 rounded-full w-fit mx-auto mb-3">
                <IconDownload className="h-6 w-6 text-green-600" />
              </div>
              <h3 className="font-semibold mb-2">Export Options</h3>
              <p className="text-sm text-muted-foreground">
                Export reports to CSV format for external analysis
              </p>
            </div>
            <div className="text-center">
              <div className="p-3 bg-purple-100 rounded-full w-fit mx-auto mb-3">
                <IconChartBar className="h-6 w-6 text-purple-600" />
              </div>
              <h3 className="font-semibold mb-2">Analytics</h3>
              <p className="text-sm text-muted-foreground">
                Detailed analytics and performance metrics
              </p>
            </div>
            <div className="text-center">
              <div className="p-3 bg-orange-100 rounded-full w-fit mx-auto mb-3">
                <IconUsers className="h-6 w-6 text-orange-600" />
              </div>
              <h3 className="font-semibold mb-2">Employee Focus</h3>
              <p className="text-sm text-muted-foreground">
                Individual and department-wise reporting
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
