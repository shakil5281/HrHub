"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { IconPlus, IconUser, IconUsers, IconBuilding, IconRefresh } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { UserTable } from "@/components/user/user-table"
import { getAllUsers, getUserStatistics, type User, type UserStatistics } from "@/lib/api/user"
import { formatNumber } from "@/lib/utils"

export default function UsersPage() {
  const [users, setUsers] = useState<User[]>([])
  const [statistics, setStatistics] = useState<UserStatistics | null>(null)
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const router = useRouter()

  const fetchUsers = async () => {
    try {
      setLoading(true)
      const [usersResponse, statsResponse] = await Promise.all([
        getAllUsers(),
        getUserStatistics()
      ])
      
      if (usersResponse.success) {
        setUsers(usersResponse.data.users)
      }
      
      if (statsResponse.success) {
        setStatistics(statsResponse.data)
      }
    } catch (error) {
      console.error('Error fetching users:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchUsers()
  }, [])

  const handleRefresh = () => {
    fetchUsers()
  }

  const handleEdit = (user: User) => {
    router.push(`/admin/users/${user.id}/edit`)
  }

  const filteredUsers = users.filter(user =>
    (user.firstName?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (user.lastName?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (user.email?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (user.department?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (user.position?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (user.companyName?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (user.roles?.some(role => role.toLowerCase().includes(searchTerm.toLowerCase())) || false)
  )

  // Use API statistics if available, fallback to local calculation
  const totalUsers = statistics?.totalUsers || users.length
  const activeUsers = statistics?.activeUsers || users.filter(u => u.isActive).length
  const uniqueCompanies = statistics?.usersByCompany ? 
    Object.keys(statistics.usersByCompany).length : 
    Object.keys(users.reduce((acc, user) => {
      if (user.companyName) {
        acc[user.companyName] = (acc[user.companyName] || 0) + 1
      }
      return acc
    }, {} as Record<string, number>)).length
  const uniqueDepartments = statistics?.usersByDepartment ?
    Object.keys(statistics.usersByDepartment).length :
    Object.keys(users.reduce((acc, user) => {
      if (user.department) {
        acc[user.department] = (acc[user.department] || 0) + 1
      }
      return acc
    }, {} as Record<string, number>)).length

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">User Management</h1>
          <p className="text-muted-foreground">
            Manage user accounts, roles, and permissions across your organization.
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button variant="outline" onClick={handleRefresh} disabled={loading}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => router.push('/admin/users/add')}>
            <IconPlus className="mr-2 h-4 w-4" />
            Add User
          </Button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Users</CardTitle>
            <IconUsers className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : formatNumber(totalUsers)}
            </div>
            <p className="text-xs text-muted-foreground">
              {activeUsers} active, {totalUsers - activeUsers} inactive
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Companies</CardTitle>
            <IconBuilding className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : formatNumber(uniqueCompanies)}
            </div>
            <p className="text-xs text-muted-foreground">
              Companies with users
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Departments</CardTitle>
            <IconUser className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : formatNumber(uniqueDepartments)}
            </div>
            <p className="text-xs text-muted-foreground">
              Active departments
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Avg per Company</CardTitle>
            <IconUsers className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : statistics?.averageUsersPerCompany || (uniqueCompanies > 0 ? Math.round(totalUsers / uniqueCompanies) : 0)}
            </div>
            <p className="text-xs text-muted-foreground">
              Users per company
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <div className="flex items-center space-x-4">
        <div className="flex-1">
          <Input
            placeholder="Search users by name, email, department, position, or company..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="max-w-sm"
          />
        </div>
        <div className="text-sm text-muted-foreground">
          {filteredUsers.length} of {users.length} users
        </div>
      </div>

      {/* Users Table */}
      <UserTable
        users={filteredUsers}
        loading={loading}
        onEdit={handleEdit}
        onRefresh={handleRefresh}
      />
    </div>
  )
}
