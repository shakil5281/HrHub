"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { IconPlus, IconUser, IconUsers, IconBuilding, IconRefresh, IconCash } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { EmployeeTable } from "@/components/employee/employee-table"
import { getAllEmployees, getEmployeeSummary, type Employee } from "@/lib/api/employee"

export default function EmployeePage() {
  const [employees, setEmployees] = useState<Employee[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const router = useRouter()

  const fetchEmployees = async () => {
    try {
      setLoading(true)
      const response = await getAllEmployees()
      if (response.success) {
        setEmployees(response.data)
      }
    } catch (error) {
      console.error('Error fetching employees:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchEmployees()
  }, [])

  const handleRefresh = () => {
    fetchEmployees()
  }

  const handleEdit = (employee: Employee) => {
    router.push(`/employee/${employee.id}/edit`)
  }

  const filteredEmployees = employees.filter(employee =>
    (employee.name?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (employee.nameBangla?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (employee.nidNo?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (employee.departmentName?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (employee.sectionName?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (employee.designationName?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (employee.fatherName?.toLowerCase().includes(searchTerm.toLowerCase()) || false) ||
    (employee.motherName?.toLowerCase().includes(searchTerm.toLowerCase()) || false)
  )

  // Calculate statistics
  const totalEmployees = employees.length
  const activeEmployees = employees.filter(e => e.isActive).length
  const avgSalary = employees.length > 0 ? employees.reduce((sum, e) => sum + (e.grossSalary || 0), 0) / employees.length : 0
  const departmentsCovered = new Set(employees.map(e => e.departmentName).filter(Boolean)).size

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Employee Management</h1>
          <p className="text-muted-foreground">
            Manage employee records, personal information, and work details.
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button variant="outline" onClick={handleRefresh} disabled={loading}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => router.push('/employee/add')}>
            <IconPlus className="mr-2 h-4 w-4" />
            Add Employee
          </Button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Employees</CardTitle>
            <IconUsers className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : totalEmployees.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              {activeEmployees} active, {totalEmployees - activeEmployees} inactive
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Departments</CardTitle>
            <IconBuilding className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : departmentsCovered.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Department coverage
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Avg Salary</CardTitle>
            <IconCash className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : `৳${Math.round(avgSalary).toLocaleString()}`}
            </div>
            <p className="text-xs text-muted-foreground">
              Average gross salary
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Rate</CardTitle>
            <IconUser className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : totalEmployees > 0 ? `${Math.round((activeEmployees / totalEmployees) * 100)}%` : "0%"}
            </div>
            <p className="text-xs text-muted-foreground">
              Active employees
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <div className="flex items-center space-x-4">
        <div className="flex-1">
          <Input
            placeholder="Search employees by name, NID, department, designation, or family info..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="max-w-sm"
          />
        </div>
        <div className="text-sm text-muted-foreground">
          {filteredEmployees.length} of {employees.length} employees
        </div>
      </div>

      {/* Employees Table */}
      <EmployeeTable
        employees={filteredEmployees}
        loading={loading}
        onEdit={handleEdit}
        onRefresh={handleRefresh}
      />
    </div>
  )
}
