"use client"

import { useState } from "react"
import { useForm } from "react-hook-form"
import { zodResolver } from "@hookform/resolvers/zod"
import * as z from "zod"
import { IconRestore, IconShield, IconAlertTriangle, IconLoader } from "@tabler/icons-react"
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Switch } from "@/components/ui/switch"
import {
  Form,
  FormControl,
  FormDescription,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form"
import { Alert, AlertDescription } from "@/components/ui/alert"
import { restoreBackup, type Backup, type RestoreBackupPayload } from "@/lib/api/backup"
import { toast } from "sonner"

const restoreFormSchema = z.object({
  restoreToDatabase: z.string().optional(),
  confirmRestore: z.boolean().refine(val => val === true, "You must confirm the restore operation"),
  backupBeforeRestore: z.boolean(),
})

type RestoreFormValues = z.infer<typeof restoreFormSchema>

interface BackupRestoreDialogProps {
  backup: Backup | null
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess?: () => void
}

export function BackupRestoreDialog({ backup, open, onOpenChange, onSuccess }: BackupRestoreDialogProps) {
  const [isRestoring, setIsRestoring] = useState(false)

  const form = useForm<RestoreFormValues>({
    resolver: zodResolver(restoreFormSchema),
    defaultValues: {
      restoreToDatabase: "",
      confirmRestore: false,
      backupBeforeRestore: true,
    },
  })

  const onSubmit = async (values: RestoreFormValues) => {
    if (!backup) return

    setIsRestoring(true)
    try {
      const payload: RestoreBackupPayload = {
        backupId: backup.id,
        restoreToDatabase: values.restoreToDatabase || undefined,
        confirmRestore: values.confirmRestore,
        backupBeforeRestore: values.backupBeforeRestore,
      }

      const response = await restoreBackup(payload)
      
      if (response.success) {
        toast.success("Database restore started successfully")
        onSuccess?.()
        onOpenChange(false)
        form.reset()
      } else {
        toast.error(response.message || "Failed to start restore")
      }
    } catch (error) {
      console.error('Error restoring backup:', error)
      toast.error("Failed to restore backup")
    } finally {
      setIsRestoring(false)
    }
  }

  const handleOpenChange = (newOpen: boolean) => {
    if (!isRestoring) {
      onOpenChange(newOpen)
      if (!newOpen) {
        form.reset()
      }
    }
  }

  if (!backup) return null

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <IconRestore className="h-5 w-5" />
            Restore Database from Backup
          </DialogTitle>
          <DialogDescription>
            Restore your database from the backup: <strong>{backup.name}</strong>
          </DialogDescription>
        </DialogHeader>

        <Alert className="border-amber-200 bg-amber-50">
          <IconAlertTriangle className="h-4 w-4 text-amber-600" />
          <AlertDescription className="text-amber-800">
            <strong>Warning:</strong> This operation will replace your current database with the backup data. 
            This action cannot be undone. Make sure you have a recent backup before proceeding.
          </AlertDescription>
        </Alert>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-6">
            {/* Backup Information */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold">Backup Information</h3>
              <div className="grid grid-cols-2 gap-4 p-4 bg-gray-50 rounded-lg">
                <div>
                  <Label className="text-sm font-medium text-gray-600">Backup Name</Label>
                  <p className="text-sm">{backup.name}</p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-gray-600">Backup Type</Label>
                  <p className="text-sm">{backup.backupType}</p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-gray-600">Created</Label>
                  <p className="text-sm">{new Date(backup.createdAt).toLocaleString()}</p>
                </div>
                <div>
                  <Label className="text-sm font-medium text-gray-600">Size</Label>
                  <p className="text-sm">
                    {backup.fileSize > 0 ? `${(backup.fileSize / (1024 * 1024)).toFixed(2)} MB` : 'Unknown'}
                  </p>
                </div>
              </div>
            </div>

            {/* Restore Configuration */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold">Restore Configuration</h3>
              
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
                      Specify a different database name to restore to. Leave empty to restore to the current database.
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="backupBeforeRestore"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-center justify-between rounded-lg border p-4">
                    <div className="space-y-0.5">
                      <FormLabel className="text-base">Create Backup Before Restore</FormLabel>
                      <FormDescription>
                        Create a backup of the current database before restoring
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

            {/* Confirmation */}
            <div className="space-y-4">
              <h3 className="text-lg font-semibold flex items-center gap-2">
                <IconShield className="h-4 w-4" />
                Confirmation
              </h3>
              
              <FormField
                control={form.control}
                name="confirmRestore"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-start space-x-3 space-y-0 rounded-lg border p-4">
                    <FormControl>
                      <Switch
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                    <div className="space-y-1 leading-none">
                      <FormLabel className="text-base">
                        I understand that this will replace my current database
                      </FormLabel>
                      <FormDescription>
                        By checking this box, you confirm that you understand the restore operation will 
                        completely replace your current database with the backup data. This action cannot be undone.
                      </FormDescription>
                    </div>
                  </FormItem>
                )}
              />
            </div>

            <DialogFooter>
              <Button 
                type="button" 
                variant="outline" 
                onClick={() => handleOpenChange(false)}
                disabled={isRestoring}
              >
                Cancel
              </Button>
              <Button 
                type="submit" 
                disabled={isRestoring || !form.watch('confirmRestore')}
                className="bg-amber-600 hover:bg-amber-700"
              >
                {isRestoring ? (
                  <>
                    <IconLoader className="mr-2 h-4 w-4 animate-spin" />
                    Restoring...
                  </>
                ) : (
                  <>
                    <IconRestore className="mr-2 h-4 w-4" />
                    Restore Database
                  </>
                )}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}
