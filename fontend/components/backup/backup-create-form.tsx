"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Checkbox } from "@/components/ui/checkbox"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import {
  Form,
  FormControl,
  FormDescription,
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
import { createBackup, type BackupCreateRequest } from "@/lib/api/backup"
import { toast } from "sonner"
import { IconDatabase, IconShield } from "@tabler/icons-react"

const backupSchema = z.object({
  name: z.string().min(1, "Backup name is required"),
  description: z.string().optional(),
  backupType: z.enum(["FULL", "INCREMENTAL", "DIFFERENTIAL"]),
  compress: z.boolean(),
  encrypt: z.boolean(),
})

type BackupFormData = z.infer<typeof backupSchema>

interface BackupCreateFormProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess: () => void
}

export function BackupCreateForm({ open, onOpenChange, onSuccess }: BackupCreateFormProps) {
  const [loading, setLoading] = useState(false)

  const form = useForm<BackupFormData>({
    resolver: zodResolver(backupSchema),
    defaultValues: {
      name: "",
      description: "",
      backupType: "FULL",
      compress: true,
      encrypt: false,
    },
  })

  const onSubmit = async (values: BackupFormData) => {
    setLoading(true)
    try {
      const backupData: BackupCreateRequest = {
        name: values.name,
        description: values.description,
        backupType: values.backupType,
        compress: values.compress,
        encrypt: values.encrypt,
      }

      const response = await createBackup(backupData)
      if (response.success) {
        toast.success('Backup creation started successfully')
        onSuccess()
      } else {
        toast.error(response.message || 'Failed to create backup')
      }
    } catch (error) {
      console.error('Failed to create backup:', error)
      toast.error('Failed to create backup')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[500px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <IconDatabase className="h-5 w-5" />
            Create New Backup
          </DialogTitle>
          <DialogDescription>
            Create a new database backup. This process may take several minutes depending on the database size.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="name"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Backup Name</FormLabel>
                  <FormControl>
                    <Input placeholder="Enter backup name" {...field} />
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
                  <FormLabel>Description (Optional)</FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder="Enter backup description" 
                      className="resize-none"
                      rows={3}
                      {...field} 
                    />
                  </FormControl>
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
                      <SelectItem value="INCREMENTAL">Incremental Backup</SelectItem>
                      <SelectItem value="DIFFERENTIAL">Differential Backup</SelectItem>
                    </SelectContent>
                  </Select>
                  <FormDescription>
                    Full: Complete database backup. Incremental: Only changed data since last backup. Differential: All changes since last full backup.
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="space-y-4">
              <FormField
                control={form.control}
                name="compress"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-start space-x-3 space-y-0">
                    <FormControl>
                      <Checkbox
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                    <div className="space-y-1 leading-none">
                      <FormLabel>Compress Backup</FormLabel>
                      <FormDescription>
                        Reduce backup file size by compressing the data
                      </FormDescription>
                    </div>
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="encrypt"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-start space-x-3 space-y-0">
                    <FormControl>
                      <Checkbox
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                    <div className="space-y-1 leading-none">
                      <FormLabel className="flex items-center gap-2">
                        <IconShield className="h-4 w-4" />
                        Encrypt Backup
                      </FormLabel>
                      <FormDescription>
                        Encrypt the backup file for additional security
                      </FormDescription>
                    </div>
                  </FormItem>
                )}
              />
            </div>

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={loading}>
                {loading ? "Creating..." : "Create Backup"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}