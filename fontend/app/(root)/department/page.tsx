"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { IconPlus, IconBuilding, IconUsers, IconTrendingUp, IconRefresh } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { DepartmentTable } from "@/components/department/department-table"
import { getAllDepartments, type Department } from "@/lib/api/department"

export default function DepartmentPage() {
  const [departments, setDepartments] = useState<Department[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const router = useRouter()

  const fetchDepartments = async () => {
    try {
      setLoading(true)
      const response = await getAllDepartments()
      if (response.success) {
        setDepartments(response.data)
      }
    } catch (error) {
      console.error('Error fetching departments:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchDepartments()
  }, [])

  const handleRefresh = () => {
    fetchDepartments()
  }

  const handleEdit = (department: Department) => {
    const id = department.id
    if (id) {
      router.push(`/department/${id}/edit`)
    }
  }

  const filteredDepartments = departments.filter(department =>
    department.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (department.nameBangla && department.nameBangla.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (department.companyName && department.companyName.toLowerCase().includes(searchTerm.toLowerCase()))
  )

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Department Management</h1>
          <p className="text-muted-foreground">
            Manage your organization&apos;s departments and organizational structure.
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button variant="outline" onClick={handleRefresh} disabled={loading}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => router.push('/department/add')}>
            <IconPlus className="mr-2 h-4 w-4" />
            Add Department
          </Button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Departments</CardTitle>
            <IconBuilding className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : departments.length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Active departments in system
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">With Bangla Names</CardTitle>
            <IconUsers className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : departments.filter(d => d.nameBangla).length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Departments with Bangla names
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Departments</CardTitle>
            <IconTrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : departments.filter(d => d.isActive).length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Currently active departments
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
              {loading ? "..." : new Set(departments.map(d => d.companyId)).size.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Companies with departments
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <div className="flex items-center space-x-4">
        <div className="flex-1">
          <Input
            placeholder="Search departments by name or company..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="max-w-sm"
          />
        </div>
        <div className="text-sm text-muted-foreground">
          {filteredDepartments.length} of {departments.length} departments
        </div>
      </div>

      {/* Departments Table */}
      <DepartmentTable
        departments={filteredDepartments}
        loading={loading}
        onEdit={handleEdit}
        onRefresh={handleRefresh}
      />
    </div>
  )
}
