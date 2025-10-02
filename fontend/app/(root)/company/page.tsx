"use client"

import { useState, useEffect } from "react"
import { IconPlus, IconBuilding, IconUsers, IconTrendingUp, IconRefresh } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { CompanyForm } from "@/components/company/company-form"
import { CompanyTable } from "@/components/company/company-table"
import { getAllCompanies, type Company } from "@/lib/api/company"

export default function CompanyPage() {
  const [companies, setCompanies] = useState<Company[]>([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState("")
  const [formOpen, setFormOpen] = useState(false)
  const [editingCompany, setEditingCompany] = useState<Company | undefined>(undefined)

  const fetchCompanies = async () => {
    try {
      setLoading(true)
      const response = await getAllCompanies()
      if (response.success) {
        setCompanies(response.data)
      }
    } catch (error) {
      console.error('Error fetching companies:', error)
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    fetchCompanies()
  }, [])

  const handleRefresh = () => {
    fetchCompanies()
  }

  const handleEdit = (company: Company) => {
    setEditingCompany(company)
    setFormOpen(true)
  }

  const handleFormSuccess = () => {
    fetchCompanies()
    setEditingCompany(undefined)
  }

  const handleCloseForm = (open: boolean) => {
    setFormOpen(open)
    if (!open) {
      setEditingCompany(undefined)
    }
  }

  const filteredCompanies = companies.filter(company =>
    company.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (company.companyNameBangla && company.companyNameBangla.toLowerCase().includes(searchTerm.toLowerCase())) ||
    company.city.toLowerCase().includes(searchTerm.toLowerCase()) ||
    company.email.toLowerCase().includes(searchTerm.toLowerCase()) ||
    (company.authorizedSignature && company.authorizedSignature.toLowerCase().includes(searchTerm.toLowerCase()))
  )

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Company Management</h1>
          <p className="text-muted-foreground">
            Manage your organization&apos;s company information and details.
          </p>
        </div>
        <div className="flex items-center space-x-2">
          <Button variant="outline" onClick={handleRefresh} disabled={loading}>
            <IconRefresh className="mr-2 h-4 w-4" />
            Refresh
          </Button>
          <Button onClick={() => setFormOpen(true)}>
            <IconPlus className="mr-2 h-4 w-4" />
            Add Company
          </Button>
        </div>
      </div>

      {/* Statistics Cards */}
      <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Total Companies</CardTitle>
            <IconBuilding className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : companies.length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Active companies in system
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">Active Companies</CardTitle>
            <IconUsers className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : companies.length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Currently registered
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">With Bangla Names</CardTitle>
            <IconTrendingUp className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : companies.filter(c => c.companyNameBangla).length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Companies with Bangla names
            </p>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
            <CardTitle className="text-sm font-medium">With Signatures</CardTitle>
            <IconBuilding className="h-4 w-4 text-muted-foreground" />
          </CardHeader>
          <CardContent>
            <div className="text-2xl font-bold">
              {loading ? "..." : companies.filter(c => c.authorizedSignature).length.toLocaleString()}
            </div>
            <p className="text-xs text-muted-foreground">
              Companies with authorized signatures
            </p>
          </CardContent>
        </Card>
      </div>

      {/* Search and Filters */}
      <div className="flex items-center space-x-4">
        <div className="flex-1">
          <Input
            placeholder="Search companies by name, location, email, or signature..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="max-w-sm"
          />
        </div>
        <div className="text-sm text-muted-foreground">
          {filteredCompanies.length} of {companies.length} companies
        </div>
      </div>

      {/* Companies Table */}
      <CompanyTable
        companies={filteredCompanies}
        loading={loading}
        onEdit={handleEdit}
        onRefresh={handleRefresh}
      />

      {/* Company Form Modal */}
      <CompanyForm
        open={formOpen}
        onOpenChange={handleCloseForm}
        company={editingCompany}
        onSuccess={handleFormSuccess}
      />
    </div>
  )
}
