"use client"

import { useState, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { createCompany, updateCompany, type Company } from "@/lib/api/company"

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

interface CompanyFormProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  company?: Company
  onSuccess: () => void
}

export function CompanyForm({ open, onOpenChange, company, onSuccess }: CompanyFormProps) {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

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

  // Reset form when company prop changes
  useEffect(() => {
    if (company) {
      form.reset({
        name: company.name || "",
        companyNameBangla: company.companyNameBangla || "",
        description: company.description || "",
        phone: company.phone || "",
        email: company.email || "",
        address: company.address || "",
        addressBangla: company.addressBangla || "",
        city: company.city || "",
        state: company.state || "",
        postalCode: company.postalCode || "",
        country: company.country || "",
        logoUrl: company.logoUrl || "",
        authorizedSignature: company.authorizedSignature || "",
      })
    } else {
      form.reset({
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
      })
    }
  }, [company, form])

  const onSubmit = async (values: CompanyFormData) => {
    setLoading(true)
    setError(null)

    try {
      const companyData = {
        ...values,
        logoUrl: values.logoUrl || "",
      }

      if (company?.id || company?.companyId) {
        await updateCompany(company.id || company.companyId!, companyData)
      } else {
        await createCompany(companyData)
      }

      onSuccess()
      onOpenChange(false)
      form.reset()
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while saving the company."
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-4xl max-h-[90vh] overflow-y-auto">
        <DialogHeader>
          <DialogTitle>
            {company ? "Edit Company" : "Create New Company"}
          </DialogTitle>
          <DialogDescription>
            {company 
              ? "Update the company information below." 
              : "Fill in the details to create a new company."
            }
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Basic Information */}
              <div className="space-y-4">
                <h3 className="text-lg font-semibold">Basic Information</h3>
                
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
                          className="min-h-[100px]"
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
              <div className="space-y-4">
                <h3 className="text-lg font-semibold">Contact & Address</h3>
                
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

                <div className="grid grid-cols-2 gap-2">
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

                <div className="grid grid-cols-2 gap-2">
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
              <div className="text-sm text-red-500 bg-red-50 p-3 rounded-md">
                {error}
              </div>
            )}

            <DialogFooter>
              <Button
                type="button"
                variant="outline"
                onClick={() => onOpenChange(false)}
                disabled={loading}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={loading}>
                {loading ? "Saving..." : company ? "Update Company" : "Create Company"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
