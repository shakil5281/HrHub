"use client"

import { useState, useEffect } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
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
import { 
  getBackupList,
  restoreBackup,
  type Backup,
  type BackupRestoreRequest
} from "@/lib/api/backup"
import { toast } from "sonner"
import { IconDatabase, IconShield, IconAlertTriangle } from "@tabler/icons-react"

const restoreSchema = z.object({
  backupId: z.number(),
  restoreToDatabase: z.string().optional(),
  dropExistingTables: z.boolean(),
  createBackupBeforeRestore: z.boolean(),
})

type RestoreFormData = z.infer<typeof restoreSchema>

interface BackupRestoreDialogProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess: () => void
}

export function BackupRestoreDialog({ open, onOpenChange, onSuccess }: BackupRestoreDialogProps) {
  const [loading, setLoading] = useState(false)
  const [backups, setBackups] = useState<Backup[]>([])

  const form = useForm<RestoreFormData>({
    resolver: zodResolver(restoreSchema),
    defaultValues: {
      backupId: 0,
      restoreToDatabase: "",
      dropExistingTables: false,
      createBackupBeforeRestore: true,
    },
  })

  const loadBackups = async () => {
    try {
      const response = await getBackupList({ status: "COMPLETED" })
      if (response.success) {
        setBackups(response.data.backups)
      }
    } catch (error) {
      console.error('Failed to load backups:', error)
    }
  }

  useEffect(() => {
    if (open) {
      loadBackups()
    }
  }, [open])

  const onSubmit = async (values: RestoreFormData) => {
    if (!confirm('Are you sure you want to restore from this backup? This action will overwrite the current database.')) {
      return
    }

    setLoading(true)
    try {
      const restoreData: BackupRestoreRequest = {
        backupId: values.backupId,
        restoreToDatabase: values.restoreToDatabase || undefined,
        dropExistingTables: values.dropExistingTables,
        createBackupBeforeRestore: values.createBackupBeforeRestore,
        includeTables: undefined,
        excludeTables: undefined,
      }

      const response = await restoreBackup(restoreData)
      if (response.success) {
        toast.success('Backup restore started successfully')
        onSuccess()
      } else {
        toast.error(response.message || 'Failed to start backup restore')
      }
    } catch (error) {
      console.error('Failed to restore backup:', error)
      toast.error('Failed to restore backup')
    } finally {
      setLoading(false)
    }
  }

  const selectedBackup = backups.find(backup => backup.id === form.watch('backupId'))

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <IconDatabase className="h-5 w-5" />
            Restore from Backup
          </DialogTitle>
          <DialogDescription>
            Restore your database from a previous backup. This action will overwrite the current database.
          </DialogDescription>
        </DialogHeader>

        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-4">
          <div className="flex items-start gap-2">
            <IconAlertTriangle className="h-5 w-5 text-yellow-600 mt-0.5" />
            <div className="text-sm text-yellow-800">
              <p className="font-medium">Warning: This action cannot be undone</p>
              <p>Restoring from a backup will completely replace your current database. Make sure you have a recent backup before proceeding.</p>
            </div>
          </div>
        </div>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name="backupId"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Select Backup</FormLabel>
                  <Select onValueChange={(value) => field.onChange(parseInt(value))}>
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a backup to restore from" />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {(backups || []).map((backup) => (
                        <SelectItem key={backup.id} value={backup.id.toString()}>
                          <div className="flex flex-col">
                            <span className="font-medium">{backup.name}</span>
                            <span className="text-sm text-gray-500">
                              {new Date(backup.createdAt).toLocaleString()} • {backup.backupType}
                            </span>
                          </div>
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            {selectedBackup && (
              <div className="bg-gray-50 rounded-lg p-4 space-y-2">
                <h4 className="font-medium">Backup Details</h4>
                <div className="grid grid-cols-2 gap-2 text-sm">
                  <div>
                    <span className="text-gray-500">Type:</span> {selectedBackup.backupType}
                  </div>
                  <div>
                    <span className="text-gray-500">Size:</span> {((selectedBackup.fileSize || 0) / 1024 / 1024).toFixed(2)} MB
                  </div>
                  <div>
                    <span className="text-gray-500">Records:</span> {selectedBackup.recordsCount.toLocaleString()}
                  </div>
                  <div>
                    <span className="text-gray-500">Created:</span> {new Date(selectedBackup.createdAt).toLocaleDateString()}
                  </div>
                </div>
                {selectedBackup.description && (
                  <div className="text-sm">
                    <span className="text-gray-500">Description:</span> {selectedBackup.description}
                  </div>
                )}
              </div>
            )}

            <FormField
              control={form.control}
              name="restoreToDatabase"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Target Database (Optional)</FormLabel>
                  <FormControl>
                    <Input 
                      placeholder="Leave empty to restore to current database" 
                      {...field} 
                    />
                  </FormControl>
                  <FormDescription>
                    Specify a different database name to restore to, or leave empty to restore to the current database
                  </FormDescription>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="space-y-4">
              <FormField
                control={form.control}
                name="createBackupBeforeRestore"
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
                        Create Backup Before Restore
                      </FormLabel>
                      <FormDescription>
                        Create a backup of the current database before restoring (recommended)
                      </FormDescription>
                    </div>
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="dropExistingTables"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-start space-x-3 space-y-0">
                    <FormControl>
                      <Checkbox
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                    <div className="space-y-1 leading-none">
                      <FormLabel>Drop Existing Tables</FormLabel>
                      <FormDescription>
                        Remove existing tables before restoring (use with caution)
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
              <Button type="submit" disabled={loading || !form.watch('backupId')}>
                {loading ? "Restoring..." : "Restore Database"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}