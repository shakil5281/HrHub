"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { IconClock, IconShield, IconSettings, IconLoader } from "@tabler/icons-react"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import { scheduleBackup, type CreateSchedulePayload } from "@/lib/api/backup"
import { toast } from "sonner"

const scheduleFormSchema = z.object({
  name: z.string().min(1, "Schedule name is required").max(100, "Name must be less than 100 characters"),
  description: z.string().max(500, "Description must be less than 500 characters").optional(),
  scheduleType: z.enum(["DAILY", "WEEKLY", "MONTHLY", "CUSTOM"]),
  scheduleExpression: z.string().min(1, "Schedule expression is required"),
  backupType: z.enum(["FULL", "INCREMENTAL", "DIFFERENTIAL"]),
  retentionDays: z.number().min(1, "Retention must be at least 1 day").max(365, "Retention cannot exceed 365 days"),
  compressionType: z.enum(["NONE", "GZIP", "ZIP"]),
  encryptionEnabled: z.boolean(),
})

type ScheduleFormValues = z.infer<typeof scheduleFormSchema>

interface BackupScheduleFormProps {
  onSuccess?: () => void
  onCancel?: () => void
}

export function BackupScheduleForm({ onSuccess, onCancel }: BackupScheduleFormProps) {
  const [isCreating, setIsCreating] = useState(false)

  const form = useForm<ScheduleFormValues>({
    resolver: zodResolver(scheduleFormSchema),
    defaultValues: {
      name: "",
      description: "",
      scheduleType: "DAILY",
      scheduleExpression: "0 2 * * *", // Daily at 2 AM
      backupType: "FULL",
      retentionDays: 30,
      compressionType: "GZIP",
      encryptionEnabled: false,
    },
  })

  const scheduleType = form.watch("scheduleType")

  const getScheduleExpressionPlaceholder = (type: string) => {
    switch (type) {
      case "DAILY":
        return "0 2 * * * (Daily at 2 AM)"
      case "WEEKLY":
        return "0 2 * * 0 (Weekly on Sunday at 2 AM)"
      case "MONTHLY":
        return "0 2 1 * * (Monthly on 1st at 2 AM)"
      case "CUSTOM":
        return "0 2 * * * (Custom cron expression)"
      default:
        return "0 2 * * *"
    }
  }

  const getScheduleExpressionDescription = (type: string) => {
    switch (type) {
      case "DAILY":
        return "Backup will run daily at the specified time"
      case "WEEKLY":
        return "Backup will run weekly on the specified day and time"
      case "MONTHLY":
        return "Backup will run monthly on the specified date and time"
      case "CUSTOM":
        return "Use a custom cron expression for advanced scheduling"
      default:
        return ""
    }
  }

  const onSubmit = async (values: ScheduleFormValues) => {
    setIsCreating(true)
    try {
      const payload: CreateSchedulePayload = {
        name: values.name,
        description: values.description || undefined,
        scheduleType: values.scheduleType,
        scheduleExpression: values.scheduleExpression,
        backupType: values.backupType,
        retentionDays: values.retentionDays,
        compressionType: values.compressionType,
        encryptionEnabled: values.encryptionEnabled,
      }

      const response = await scheduleBackup(payload)
      
      if (response.success) {
        toast.success("Backup schedule created successfully")
        onSuccess?.()
      } else {
        toast.error(response.message || "Failed to create backup schedule")
      }
    } catch (error) {
      console.error('Error creating backup schedule:', error)
      toast.error("Failed to create backup schedule")
    } finally {
      setIsCreating(false)
    }
  }

  return (
    <Card className="max-w-2xl mx-auto">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <IconClock className="h-5 w-5" />
          Schedule Automatic Backup
        </CardTitle>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            {/* Basic Information */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold flex items-center gap-2">
                <IconSettings className="h-4 w-4" />
                Schedule Information
              </h3>
              
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Schedule Name *</FormLabel>
                    <FormControl>
                      <Input placeholder="Enter schedule name" {...field} />
                    </FormControl>
                    <FormDescription>
                      A descriptive name for this backup schedule
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="description"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Description</FormLabel>
                    <FormControl>
                      <Textarea 
                        placeholder="Enter schedule description (optional)"
                        className="resize-none"
                        rows={3}
                        {...field} 
                      />
                    </FormControl>
                    <FormDescription>
                      Optional description for this backup schedule
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Schedule Configuration */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold flex items-center gap-2">
                <IconClock className="h-4 w-4" />
                Schedule Configuration
              </h3>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <FormField
                  control={form.control}
                  name="scheduleType"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Schedule Type</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Select schedule type" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="DAILY">Daily</SelectItem>
                          <SelectItem value="WEEKLY">Weekly</SelectItem>
                          <SelectItem value="MONTHLY">Monthly</SelectItem>
                          <SelectItem value="CUSTOM">Custom</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormDescription>
                        {getScheduleExpressionDescription(scheduleType)}
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />

                <FormField
                  control={form.control}
                  name="backupType"
                  render={({ field }) => (
                    <FormItem>
                      <FormLabel>Backup Type</FormLabel>
                      <Select onValueChange={field.onChange} defaultValue={field.value}>
                        <FormControl>
                          <SelectTrigger>
                            <SelectValue placeholder="Select backup type" />
                          </SelectTrigger>
                        </FormControl>
                        <SelectContent>
                          <SelectItem value="FULL">Full Backup</SelectItem>
                          <SelectItem value="INCREMENTAL">Incremental</SelectItem>
                          <SelectItem value="DIFFERENTIAL">Differential</SelectItem>
                        </SelectContent>
                      </Select>
                      <FormDescription>
                        Type of backup to perform
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />
              </div>

              <FormField
                control={form.control}
                name="scheduleExpression"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Cron Expression *</FormLabel>
                    <FormControl>
                      <Input 
                        placeholder={getScheduleExpressionPlaceholder(scheduleType)}
                        {...field} 
                      />
                    </FormControl>
                    <FormDescription>
                      Cron expression for scheduling. Format: minute hour day month day-of-week
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="retentionDays"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Retention Period (Days)</FormLabel>
                    <FormControl>
                      <Input 
                        type="number" 
                        min="1" 
                        max="365"
                        placeholder="30"
                        {...field}
                        onChange={(e) => field.onChange(parseInt(e.target.value) || 30)}
                      />
                    </FormControl>
                    <FormDescription>
                      How long to keep backups from this schedule (1-365 days)
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Backup Settings */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold">Backup Settings</h3>
              
              <FormField
                control={form.control}
                name="compressionType"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Compression</FormLabel>
                    <Select onValueChange={field.onChange} defaultValue={field.value}>
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue placeholder="Select compression" />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        <SelectItem value="NONE">No Compression</SelectItem>
                        <SelectItem value="GZIP">GZIP</SelectItem>
                        <SelectItem value="ZIP">ZIP</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormDescription>
                      Compression reduces file size but may take longer
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="encryptionEnabled"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Enable Encryption</FormLabel>
                      <FormDescription>
                        Encrypt the backup files for additional security
                      </FormDescription>
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

            {/* Action Buttons */}
            <div className="flex justify-end space-x-2 pt-4">
              {onCancel && (
                <Button type="button" variant="outline" onClick={onCancel}>
                  Cancel
                </Button>
              )}
              <Button type="submit" disabled={isCreating}>
                {isCreating ? (
                  <>
                    <IconLoader className="mr-2 h-4 w-4 animate-spin" />
                    Creating Schedule...
                  </>
                ) : (
                  <>
                    <IconClock className="mr-2 h-4 w-4" />
                    Create Schedule
                  </>
                )}
              </Button>
            </div>
          </form>
        </Form>
      </CardContent>
    </Card>
  )
}
