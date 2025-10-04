"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Switch } from "@/components/ui/switch"
import { createAttendanceDevice, type AttendanceDeviceCreateRequest } from "@/lib/api/attendance"
import { IconArrowLeft, IconDeviceDesktop, IconWifi } from "@tabler/icons-react"
import { toast } from "sonner"

const deviceSchema = z.object({
  name: z.string().min(1, "Device name is required"),
  ipAddress: z.string().min(1, "IP address is required").regex(
    /^(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$/,
    "Invalid IP address format"
  ),
  port: z.number().min(1, "Port must be greater than 0").max(65535, "Port must be less than 65536"),
  deviceType: z.string().min(1, "Device type is required"),
  isActive: z.boolean(),
})

type DeviceFormData = z.infer<typeof deviceSchema>

const deviceTypes = [
  { value: "ZKTeco", label: "ZKTeco" },
  { value: "Hikvision", label: "Hikvision" },
  { value: "Suprema", label: "Suprema" },
  { value: "Mantra", label: "Mantra" },
  { value: "BioMax", label: "BioMax" },
  { value: "Other", label: "Other" },
]

export default function NewAttendanceDevicePage() {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const router = useRouter()

  const form = useForm<DeviceFormData>({
    resolver: zodResolver(deviceSchema),
    defaultValues: {
      name: "",
      ipAddress: "",
      port: 4370,
      deviceType: "",
      isActive: true,
    },
  })

  const onSubmit = async (values: DeviceFormData) => {
    setLoading(true)
    setError(null)

    try {
      const deviceData: AttendanceDeviceCreateRequest = {
        name: values.name,
        ipAddress: values.ipAddress,
        port: values.port,
        deviceType: values.deviceType,
        isActive: values.isActive,
      }

      const response = await createAttendanceDevice(deviceData)
      if (response.success) {
        toast.success('Device created successfully')
        router.push("/attendance/devices")
      } else {
        setError(response.message || 'Failed to create device')
      }
    } catch (err: unknown) {
      const error = err as { response?: { data?: { message?: string; errors?: string[] } }; message?: string }
      const errorMessage = error?.response?.data?.message ||
        error?.response?.data?.errors?.join(", ") ||
        error?.message ||
        "An error occurred while creating the device."
      setError(errorMessage)
    } finally {
      setLoading(false)
    }
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
            <h1 className="text-3xl font-bold tracking-tight">Add New Device</h1>
            <p className="text-muted-foreground">
              Configure a new attendance device for your organization.
            </p>
          </div>
        </div>
      </div>

      {/* Form */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center">
            <IconDeviceDesktop className="mr-2 h-5 w-5" />
            Device Configuration
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
              {/* Basic Information */}
              <div className="space-y-4">
                <h3 className="text-lg font-semibold border-b pb-2">Basic Information</h3>
                
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  <FormField
                    control={form.control}
                    name="name"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Device Name *</FormLabel>
                        <FormControl>
                          <Input placeholder="e.g., Main Office Device" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="deviceType"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Device Type *</FormLabel>
                        <Select onValueChange={field.onChange} value={field.value}>
                          <FormControl>
                            <SelectTrigger>
                              <SelectValue placeholder="Select device type" />
                            </SelectTrigger>
                          </FormControl>
                          <SelectContent>
                            {deviceTypes.map((type) => (
                              <SelectItem key={type.value} value={type.value}>
                                {type.label}
                              </SelectItem>
                            ))}
                          </SelectContent>
                        </Select>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>

              {/* Network Configuration */}
              <div className="space-y-4">
                <h3 className="text-lg font-semibold border-b pb-2 flex items-center">
                  <IconWifi className="mr-2 h-4 w-4" />
                  Network Configuration
                </h3>
                
                <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                  <FormField
                    control={form.control}
                    name="ipAddress"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>IP Address *</FormLabel>
                        <FormControl>
                          <Input 
                            placeholder="192.168.1.100" 
                            {...field} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />

                  <FormField
                    control={form.control}
                    name="port"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Port *</FormLabel>
                        <FormControl>
                          <Input 
                            type="number" 
                            placeholder="4370" 
                            {...field} 
                            onChange={(e) => field.onChange(parseInt(e.target.value) || 0)} 
                          />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </div>

              {/* Status Configuration */}
              <div className="space-y-4">
                <h3 className="text-lg font-semibold border-b pb-2">Status Configuration</h3>
                
                <FormField
                  control={form.control}
                  name="isActive"
                  render={({ field }) => (
                    <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                      <div className="space-y-0.5">
                        <FormLabel className="text-base">
                          Active Device
                        </FormLabel>
                        <div className="text-sm text-muted-foreground">
                          Enable this device to start collecting attendance data
                        </div>
                      </div>
                      <FormControl>
                        <Switch
                          checked={field.value}
                          onCheckedChange={field.onChange}
                        />
                      </FormControl>
                    </FormItem>
                  )}
                />
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
                  {loading ? "Creating Device..." : "Create Device"}
                </Button>
              </div>
            </form>
          </Form>
        </CardContent>
      </Card>
    </div>
  )
}
