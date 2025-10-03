"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { IconPlus, IconBuilding, IconUsers, IconTrendingUp, IconRefresh } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { SectionTable } from "@/components/section/section-table"
import { getAllSections, type Section } from "@/lib/api/section"

export default function SectionPage() {
  const [sections, setSections] = useState<Section[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const router = useRouter()

  const fetchSections = async () => {
    try {
      setLoading(true)
      const response = await getAllSections()
      if (response.success) {
        setSections(response.data)
      }
    } catch (error) {
      console.error('Error fetching sections:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchSections()
  }, [])

  const handleRefresh = () => {
    fetchSections()
  }

  const handleEdit = (section: Section) => {
    const id = section.id
    if (id) {
      router.push(`/section/${id}/edit`)
    }
  }

  const filteredSections = sections.filter(section =>
    section.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (section.nameBangla && section.nameBangla.toLowerCase().includes(searchTerm.toLowerCase())) ||
    (section.departmentName && section.departmentName.toLowerCase().includes(searchTerm.toLowerCase()))
  )

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Section Management</h1>
          <p className="text-muted-foreground">
            Manage your organization&apos;s sections within departments.
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button variant="outline" onClick={handleRefresh} disabled={loading}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => router.push('/section/add')}>
            <IconPlus className="mr-2 h-4 w-4" />
            Add Section
          </Button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Sections</CardTitle>
            <IconBuilding className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : sections.length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Active sections in system
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
              {loading ? "..." : sections.filter(s => s.nameBangla).length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Sections with Bangla names
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Sections</CardTitle>
            <IconTrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : sections.filter(s => s.isActive).length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Currently active sections
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
              {loading ? "..." : new Set(sections.map(s => s.departmentId)).size.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Departments with sections
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <div className="flex items-center space-x-4">
        <div className="flex-1">
          <Input
            placeholder="Search sections by name or department..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="max-w-sm"
          />
        </div>
        <div className="text-sm text-muted-foreground">
          {filteredSections.length} of {sections.length} sections
        </div>
      </div>

      {/* Sections Table */}
      <SectionTable
        sections={filteredSections}
        loading={loading}
        onEdit={handleEdit}
        onRefresh={handleRefresh}
      />
    </div>
  )
}
