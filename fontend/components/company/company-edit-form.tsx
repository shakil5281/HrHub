"use client"

import { useState, useEffect } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import { getCompanyById, updateCompany, type Company } from "@/lib/api/company"
import { IconArrowLeft, IconLoader } from "@tabler/icons-react"

const companySchema = z.object({
  name: z.string().min(1, "Company name is required"),
  companyNameBangla: z.string().min(1, "Company name in Bangla is required"),
  description: z.string().min(1, "Description is required"),
  phone: z.string().min(1, "Phone number is required"),
  email: z.string().email("Please enter a valid email address"),
  address: z.string().min(1, "Address is required"),
  addressBangla: z.string().min(1, "Address in Bangla is required"),
  city: z.string().min(1, "City is required"),
  state: z.string().min(1, "State is required"),
  postalCode: z.string().min(1, "Postal code is required"),
  country: z.string().min(1, "Country is required"),
  logoUrl: z.string().url("Please enter a valid URL").optional().or(z.literal("")),
  authorizedSignature: z.string().min(1, "Authorized signature is required"),
})

type CompanyFormData = z.infer<typeof companySchema>

interface CompanyEditFormProps {
  companyId: string
}

export function CompanyEditForm({ companyId }: CompanyEditFormProps) {
  const [loading, setLoading] = useState(false)
  const [initialLoading, setInitialLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [company, setCompany] = useState<Company | null>(null)
  const router = useRouter()

  const form = useForm<CompanyFormData>({
    resolver: zodResolver(companySchema),
    defaultValues: {
      name: "",
      companyNameBangla: "",
      description: "",
      phone: "",
      email: "",
      address: "",
      addressBangla: "",
      city: "",
      state: "",
      postalCode: "",
      country: "",
      logoUrl: "",
      authorizedSignature: "",
    },
  })

  // Fetch company data on component mount
  useEffect(() => {
    const fetchCompany = async () => {
      try {
        setInitialLoading(true)
        const response = await getCompanyById(companyId)
        if (response.success) {
          const companyData = response.data
          setCompany(companyData)
          
          // Reset form with fetched data
          form.reset({
            name: companyData.name || "",
            companyNameBangla: companyData.companyNameBangla || "",
            description: companyData.description || "",
            phone: companyData.phone || "",
            email: companyData.email || "",
            address: companyData.address || "",
            addressBangla: companyData.addressBangla || "",
            city: companyData.city || "",
            state: companyData.state || "",
            postalCode: companyData.postalCode || "",
            country: companyData.country || "",
            logoUrl: companyData.logoUrl || "",
            authorizedSignature: companyData.authorizedSignature || "",
          })
        } else {
          setError("Failed to load company data")
        }
      } catch (err: unknown) {
        const error = err as { response?: { data?: { message?: string } }; message?: string }
        setError(error?.response?.data?.message || error?.message || "Failed to load company data")
      } finally {
        setInitialLoading(false)
      }
    }

    if (companyId) {
      fetchCompany()
    }
  }, [companyId, form])

  const onSubmit = async (values: CompanyFormData) => {
    if (!company) return

    setLoading(true)
    setError(null)

    try {
      const companyData = {
        ...values,
        logoUrl: values.logoUrl || "",
      }

      const id = company.id || company.companyId
      if (!id) {
        throw new Error("Company ID not found")
      }

      await updateCompany(id, companyData)
      router.push("/company")
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while updating the company."
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  if (initialLoading) {
    return (
      <div className="space-y-6">
        <div className="flex items-center justify-center py-12">
          <div className="flex items-center space-x-2">
            <IconLoader className="h-6 w-6 animate-spin" />
            <span>Loading company data...</span>
          </div>
        </div>
      </div>
    )
  }

  if (error && !company) {
    return (
      <div className="space-y-6">
        <div className="flex items-center space-x-4">
          <Button
            variant="outline"
            size="sm"
            onClick={() => router.back()}
          >
            <IconArrowLeft className="mr-2 h-4 w-4" />
            Back
          </Button>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Edit Company</h1>
            <p className="text-muted-foreground">
              Update company information and details.
            </p>
          </div>
        </div>
        
        <Card>
          <CardContent className="flex flex-col items-center justify-center py-12">
            <div className="text-red-500 text-center">
              <h3 className="text-lg font-semibold mb-2">Error Loading Company</h3>
              <p className="text-sm">{error}</p>
              <Button 
                variant="outline" 
                className="mt-4"
                onClick={() => router.back()}
              >
                Go Back
              </Button>
            </div>
          </CardContent>
        </Card>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div className="space-y-4">
          <Button
            variant="outline"
            size="sm"
            onClick={() => router.back()}
          >
            <IconArrowLeft className="mr-2 h-4 w-4" />
            Back
          </Button>
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Edit Company</h1>
            <p className="text-muted-foreground">
              Update the company information and details below.
            </p>
            {company && (
              <div className="flex items-center space-x-2 mt-2">
                <span className="text-sm text-gray-500">Editing:</span>
                <span className="text-sm font-medium">{company.name}</span>
                {company.companyNameBangla && (
                  <span className="text-sm text-blue-600 font-sutonnymj">({company.companyNameBangla})</span>
                )}
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Form */}
      <Card>
        <CardHeader>
          <CardTitle>Company Information</CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
              <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                {/* Basic Information */}
                <div className="space-y-6">
                  <h3 className="text-lg font-semibold border-b pb-2">Basic Information</h3>
                  
                  <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Company Name *</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter company name" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="companyNameBangla"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Company Name (Bangla) *</FormLabel>
                        <FormControl>
                          <Input 
                            placeholder="‡Kv¤úvbx" 
                            className="font-sutonnymj"
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="description"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Description *</FormLabel>
                        <FormControl>
                          <Textarea 
                            placeholder="Enter company description" 
                            className="min-h-[120px]"
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="logoUrl"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Logo URL</FormLabel>
                        <FormControl>
                          <Input placeholder="https://example.com/logo.png" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="authorizedSignature"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Authorized Signature *</FormLabel>
                        <FormControl>
                          <Input placeholder="Enter authorized signature" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>

                {/* Contact & Address Information */}
                <div className="space-y-6">
                  <h3 className="text-lg font-semibold border-b pb-2">Contact & Address</h3>
                  
                  <FormField
                    control={form.control}
                    name="email"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Email *</FormLabel>
                        <FormControl>
                          <Input type="email" placeholder="contact@company.com" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="phone"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Phone *</FormLabel>
                        <FormControl>
                          <Input placeholder="+1 (555) 123-4567" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="address"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Address *</FormLabel>
                        <FormControl>
                          <Input placeholder="123 Main Street" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="addressBangla"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Address (Bangla) *</FormLabel>
                        <FormControl>
                          <Input 
                            placeholder="ঠিকানা বাংলায়" 
                            className="font-sutonnymj"
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <div className="grid grid-cols-2 gap-4">
                    <FormField
                      control={form.control}
                      name="city"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>City *</FormLabel>
                          <FormControl>
                            <Input placeholder="New York" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={form.control}
                      name="state"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>State *</FormLabel>
                          <FormControl>
                            <Input placeholder="NY" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>

                  <div className="grid grid-cols-2 gap-4">
                    <FormField
                      control={form.control}
                      name="postalCode"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Postal Code *</FormLabel>
                          <FormControl>
                            <Input placeholder="10001" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />

                    <FormField
                      control={form.control}
                      name="country"
                      render={({ field }) => (
                        <FormItem>
                          <FormLabel>Country *</FormLabel>
                          <FormControl>
                            <Input placeholder="United States" {...field} />
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </div>
                </div>
              </div>

              {error && (
                <div className="text-sm text-red-500 bg-red-50 p-4 rounded-md border border-red-200">
                  {error}
                </div>
              )}

              <div className="flex items-center justify-end space-x-4 pt-6 border-t">
                <Button
                  type="button"
                  variant="outline"
                  onClick={() => router.back()}
                  disabled={loading}
                >
                  Cancel
                </Button>
                <Button type="submit" disabled={loading}>
                  {loading ? "Updating Company..." : "Update Company"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
