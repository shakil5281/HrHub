"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { IconPlus, IconBuilding, IconUsers, IconTrendingUp, IconRefresh, IconStar } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { DesignationTable } from "@/components/designation/designation-table"
import { getAllDesignations, type Designation } from "@/lib/api/designation"

export default function DesignationPage() {
  const [designations, setDesignations] = useState<Designation[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const router = useRouter()

  const fetchDesignations = async () => {
    try {
      setLoading(true)
      const response = await getAllDesignations()
      if (response.success) {
        setDesignations(response.data)
      }
    } catch (error) {
      console.error('Error fetching designations:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchDesignations()
  }, [])

  const handleRefresh = () => {
    fetchDesignations()
  }

  const handleEdit = (designation: Designation) => {
    const id = designation.id
    if (id) {
      router.push(`/designation/${id}/edit`)
    }
  }

  const filteredDesignations = designations.filter(designation =>
    designation.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (designation.nameBangla && designation.nameBangla.toLowerCase().includes(searchTerm.toLowerCase())) ||
    designation.grade.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (designation.sectionName && designation.sectionName.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (designation.departmentName && designation.departmentName.toLowerCase().includes(searchTerm.toLowerCase()))
  )

  const averageBonus = designations.length > 0 
    ? designations.reduce((sum, d) => sum + d.attendanceBonus, 0) / designations.length 
    : 0

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Designation Management</h1>
          <p className="text-muted-foreground">
            Manage your organization&apos;s designations and hierarchy levels.
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button variant="outline" onClick={handleRefresh} disabled={loading}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => router.push('/designation/add')}>
            <IconPlus className="mr-2 h-4 w-4" />
            Add Designation
          </Button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Designations</CardTitle>
            <IconBuilding className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : designations.length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Active designations in system
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Average Bonus</CardTitle>
            <IconStar className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : averageBonus.toFixed(0)}
            </div>
            <p className="text-xs text-muted-foreground">
              Average attendance bonus
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Designations</CardTitle>
            <IconTrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : designations.filter(d => d.isActive).length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Currently active designations
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Sections</CardTitle>
            <IconUsers className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : new Set(designations.map(d => d.sectionId)).size.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Sections with designations
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <div className="flex items-center space-x-4">
        <div className="flex-1">
          <Input
            placeholder="Search designations by name, grade, section, or department..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="max-w-sm"
          />
        </div>
        <div className="text-sm text-muted-foreground">
          {filteredDesignations.length} of {designations.length} designations
        </div>
      </div>

      {/* Designations Table */}
      <DesignationTable
        designations={filteredDesignations}
        loading={loading}
        onEdit={handleEdit}
        onRefresh={handleRefresh}
      />
    </div>
  )
}
