"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { IconUsers, IconShield, IconSettings, IconPlus, IconRefresh, IconChartBar, IconServer, IconKey } from "@tabler/icons-react"
import Link from "next/link"
import { AdminNav } from "@/components/admin/admin-nav"

export default function AdminPage() {
  const adminFeatures = [
    {
      title: "User Management",
      description: "Manage system users, roles, and access controls",
      icon: IconUsers,
      href: "/admin/users",
      features: [
        "Create and edit users",
        "Assign user roles",
        "Manage user permissions",
        "User activity tracking"
      ]
    },
    {
      title: "Permission Management",
      description: "Configure and manage system permissions and access rights",
      icon: IconShield,
      href: "/admin/permissions",
      features: [
        "Assign permissions to users",
        "Bulk permission operations",
        "Permission inheritance",
        "Access control auditing"
      ]
    },
    {
      title: "Role Management",
      description: "Manage user roles and role-based permissions",
      icon: IconKey,
      href: "/admin/roles",
      features: [
        "Role creation and management",
        "Role-permission assignments",
        "User-role assignments",
        "Permission inheritance"
      ]
    },
  ]

  const quickActions = [
    {
      title: "Create User",
      description: "Add a new user to the system",
      icon: IconPlus,
      href: "/admin/users/add",
      color: "bg-blue-50 text-blue-700 border-blue-200 hover:bg-blue-100"
    },
    {
      title: "Manage Permissions",
      description: "Configure user permissions",
      icon: IconShield,
      href: "/admin/permissions",
      color: "bg-green-50 text-green-700 border-green-200 hover:bg-green-100"
    },
    {
      title: "Manage Roles",
      description: "Assign users to roles and manage permissions",
      icon: IconKey,
      href: "/admin/roles",
      color: "bg-purple-50 text-purple-700 border-purple-200 hover:bg-purple-100"
    },
    {
      title: "System Status",
      description: "View system health and statistics",
      icon: IconChartBar,
      href: "/admin/system",
      color: "bg-indigo-50 text-indigo-700 border-indigo-200 hover:bg-indigo-100"
    }
  ]

  return (
    <div className="space-y-6">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Admin Dashboard</h1>
          <p className="text-muted-foreground">
            Manage users, permissions, and system settings
          </p>
        </div>
      </div>

      {/* Navigation */}
      <div className="border-b">
        <AdminNav />
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

      {/* Admin Features */}
      <div className="space-y-4">
        <h2 className="text-2xl font-semibold">Administration Features</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {adminFeatures.map((feature) => (
            <Card key={feature.title} className="hover:shadow-lg transition-shadow">
              <CardHeader>
                <div className="flex items-center space-x-3">
                  <div className="p-2 bg-primary/10 rounded-lg">
                    <feature.icon className="h-6 w-6 text-primary" />
                  </div>
                  <div>
                    <CardTitle className="text-lg">{feature.title}</CardTitle>
                    <CardDescription>{feature.description}</CardDescription>
                  </div>
                </div>
              </CardHeader>
              <CardContent className="space-y-4">
                <ul className="space-y-2">
                  {feature.features.map((item, index) => (
                    <li key={index} className="flex items-center space-x-2 text-sm">
                      <IconRefresh className="h-4 w-4 text-green-500 flex-shrink-0" />
                      <span>{item}</span>
                    </li>
                  ))}
                </ul>
                <Link href={feature.href}>
                  <Button className="w-full">
                    <feature.icon className="h-4 w-4 mr-2" />
                    Manage {feature.title}
                  </Button>
                </Link>
              </CardContent>
            </Card>
          ))}
        </div>
      </div>

      {/* System Information */}
      <Card>
        <CardHeader>
          <CardTitle>System Information</CardTitle>
          <CardDescription>Current system status and configuration</CardDescription>
        </CardHeader>
        <CardContent>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
            <div className="text-center">
              <div className="p-3 bg-blue-100 rounded-full w-fit mx-auto mb-3">
                <IconUsers className="h-6 w-6 text-blue-600" />
              </div>
              <h3 className="font-semibold mb-2">User Management</h3>
              <p className="text-sm text-muted-foreground">
                Complete user lifecycle management
              </p>
            </div>
            <div className="text-center">
              <div className="p-3 bg-green-100 rounded-full w-fit mx-auto mb-3">
                <IconShield className="h-6 w-6 text-green-600" />
              </div>
              <h3 className="font-semibold mb-2">Security</h3>
              <p className="text-sm text-muted-foreground">
                Advanced permission and access control
              </p>
            </div>
            <div className="text-center">
              <div className="p-3 bg-purple-100 rounded-full w-fit mx-auto mb-3">
                <IconSettings className="h-6 w-6 text-purple-600" />
              </div>
              <h3 className="font-semibold mb-2">Configuration</h3>
              <p className="text-sm text-muted-foreground">
                Flexible system configuration options
              </p>
            </div>
            <div className="text-center">
              <div className="p-3 bg-orange-100 rounded-full w-fit mx-auto mb-3">
                <IconChartBar className="h-6 w-6 text-orange-600" />
              </div>
              <h3 className="font-semibold mb-2">Monitoring</h3>
              <p className="text-sm text-muted-foreground">
                Real-time system monitoring and analytics
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}
