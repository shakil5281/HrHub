"use client"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { IconDatabase, IconShield, IconSettings, IconLoader } from "@tabler/icons-react"
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
import { createBackup, type CreateBackupPayload } from "@/lib/api/backup"
import { toast } from "sonner"

const backupFormSchema = z.object({
  name: z.string().min(1, "Backup name is required").max(100, "Name must be less than 100 characters"),
  description: z.string().max(500, "Description must be less than 500 characters").optional(),
  backupType: z.enum(["FULL", "INCREMENTAL", "DIFFERENTIAL"]),
  compressionType: z.enum(["NONE", "GZIP", "ZIP"]),
  encryptionEnabled: z.boolean(),
  retentionDays: z.number().min(1, "Retention must be at least 1 day").max(365, "Retention cannot exceed 365 days").optional(),
})

type BackupFormValues = z.infer<typeof backupFormSchema>

interface BackupCreateFormProps {
  onSuccess?: () => void
  onCancel?: () => void
}

export function BackupCreateForm({ onSuccess, onCancel }: BackupCreateFormProps) {
  const [isCreating, setIsCreating] = useState(false)
  const router = useRouter()

  const form = useForm<BackupFormValues>({
    resolver: zodResolver(backupFormSchema),
    defaultValues: {
      name: "",
      description: "",
      backupType: "FULL",
      compressionType: "GZIP",
      encryptionEnabled: false,
      retentionDays: 30,
    },
  })

  const onSubmit = async (values: BackupFormValues) => {
    setIsCreating(true)
    try {
      const payload: CreateBackupPayload = {
        name: values.name,
        description: values.description || undefined,
        backupType: values.backupType,
        compressionType: values.compressionType,
        encryptionEnabled: values.encryptionEnabled,
        retentionDays: values.retentionDays,
      }

      const response = await createBackup(payload)
      
      if (response.success) {
        toast.success("Backup creation started successfully")
        onSuccess?.()
        if (!onSuccess) {
          router.push("/backup")
        }
      } else {
        toast.error(response.message || "Failed to create backup")
      }
    } catch (error) {
      console.error('Error creating backup:', error)
      toast.error("Failed to create backup")
    } finally {
      setIsCreating(false)
    }
  }

  return (
    <Card className="max-w-2xl mx-auto">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <IconDatabase className="h-5 w-5" />
          Create New Backup
        </CardTitle>
      </CardHeader>
      <CardContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            {/* Basic Information */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold flex items-center gap-2">
                <IconSettings className="h-4 w-4" />
                Basic Information
              </h3>
              
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Backup Name *</FormLabel>
                    <FormControl>
                      <Input placeholder="Enter backup name" {...field} />
                    </FormControl>
                    <FormDescription>
                      A descriptive name for this backup
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
                        placeholder="Enter backup description (optional)"
                        className="resize-none"
                        rows={3}
                        {...field} 
                      />
                    </FormControl>
                    <FormDescription>
                      Optional description for this backup
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Backup Configuration */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold flex items-center gap-2">
                <IconDatabase className="h-4 w-4" />
                Backup Configuration
              </h3>
              
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
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
                        Full: Complete backup. Incremental: Only changed data since last backup. Differential: All changes since last full backup.
                      </FormDescription>
                      <FormMessage />
                    </FormItem>
                  )}
                />

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
              </div>

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
                        onChange={(e) => field.onChange(parseInt(e.target.value) || undefined)}
                      />
                    </FormControl>
                    <FormDescription>
                      How long to keep this backup (1-365 days)
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Security Settings */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold flex items-center gap-2">
                <IconShield className="h-4 w-4" />
                Security Settings
              </h3>
              
              <FormField
                control={form.control}
                name="encryptionEnabled"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Enable Encryption</FormLabel>
                      <FormDescription>
                        Encrypt the backup file for additional security
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
                    Creating Backup...
                  </>
                ) : (
                  <>
                    <IconDatabase className="mr-2 h-4 w-4" />
                    Create Backup
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
