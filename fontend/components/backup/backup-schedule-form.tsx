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
import { scheduleBackup, type BackupScheduleRequest } from "@/lib/api/backup"
import { toast } from "sonner"
import { IconCalendar, IconClock } from "@tabler/icons-react"

const scheduleSchema = z.object({
  name: z.string().min(1, "Schedule name is required"),
  description: z.string().optional(),
  scheduleType: z.enum(["DAILY", "WEEKLY", "MONTHLY"]),
  scheduleTime: z.string().min(1, "Schedule time is required"),
  backupType: z.enum(["FULL", "INCREMENTAL", "DIFFERENTIAL"]),
  retentionDays: z.number().min(1, "Retention must be at least 1 day").max(365, "Retention cannot exceed 365 days"),
  maxBackups: z.number().min(1, "Max backups must be at least 1").max(100, "Max backups cannot exceed 100"),
  isActive: z.boolean(),
})

type ScheduleFormData = z.infer<typeof scheduleSchema>

interface BackupScheduleFormProps {
  open: boolean
  onOpenChange: (open: boolean) => void
  onSuccess: () => void
}

export function BackupScheduleForm({ open, onOpenChange, onSuccess }: BackupScheduleFormProps) {
  const [loading, setLoading] = useState(false)

  const form = useForm<ScheduleFormData>({
    resolver: zodResolver(scheduleSchema),
    defaultValues: {
      name: "",
      description: "",
      scheduleType: "DAILY",
      scheduleTime: "02:00",
      backupType: "FULL",
      retentionDays: 30,
      maxBackups: 10,
      isActive: true,
    },
  })

  const onSubmit = async (values: ScheduleFormData) => {
    setLoading(true)
    try {
      const scheduleData: BackupScheduleRequest = {
        name: values.name,
        description: values.description,
        scheduleType: values.scheduleType,
        scheduleTime: values.scheduleTime,
        backupType: values.backupType,
        retentionDays: values.retentionDays,
        maxBackups: values.maxBackups,
        isActive: values.isActive,
      }

      const response = await scheduleBackup(scheduleData)
      if (response.success) {
        toast.success('Backup schedule created successfully')
        onSuccess()
      } else {
        toast.error(response.message || 'Failed to create backup schedule')
      }
    } catch (error) {
      console.error('Failed to create backup schedule:', error)
      toast.error('Failed to create backup schedule')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="sm:max-w-[600px]">
        <DialogHeader>
          <DialogTitle className="flex items-center gap-2">
            <IconCalendar className="h-5 w-5" />
            Schedule Automatic Backup
          </DialogTitle>
          <DialogDescription>
            Create a schedule for automatic database backups. The system will create backups according to your specified schedule.
          </DialogDescription>
        </DialogHeader>

        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="name"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Schedule Name</FormLabel>
                    <FormControl>
                      <Input placeholder="Enter schedule name" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

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
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="description"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Description (Optional)</FormLabel>
                  <FormControl>
                    <Textarea 
                      placeholder="Enter schedule description" 
                      className="resize-none"
                      rows={2}
                      {...field} 
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <FormField
                control={form.control}
                name="scheduleTime"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel className="flex items-center gap-2">
                      <IconClock className="h-4 w-4" />
                      Schedule Time
                    </FormLabel>
                    <FormControl>
                      <Input 
                        type="time" 
                        {...field} 
                      />
                    </FormControl>
                    <FormDescription>
                      Time when the backup should run
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
                        <SelectItem value="INCREMENTAL">Incremental Backup</SelectItem>
                        <SelectItem value="DIFFERENTIAL">Differential Backup</SelectItem>
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
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
                      How long to keep backups (1-365 days)
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="maxBackups"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Maximum Backups</FormLabel>
                    <FormControl>
                      <Input 
                        type="number" 
                        min="1" 
                        max="100"
                        placeholder="10"
                        {...field}
                        onChange={(e) => field.onChange(parseInt(e.target.value) || 10)}
                      />
                    </FormControl>
                    <FormDescription>
                      Maximum number of backups to keep (1-100)
                    </FormDescription>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <FormField
              control={form.control}
              name="isActive"
              render={({ field }) => (
                <FormItem className="flex flex-row items-start space-x-3 space-y-0">
                  <FormControl>
                    <Checkbox
                      checked={field.value}
                      onCheckedChange={field.onChange}
                    />
                  </FormControl>
                  <div className="space-y-1 leading-none">
                    <FormLabel>Enable Schedule</FormLabel>
                    <FormDescription>
                      Activate this backup schedule immediately
                    </FormDescription>
                  </div>
                </FormItem>
              )}
            />

            <DialogFooter>
              <Button type="button" variant="outline" onClick={() => onOpenChange(false)}>
                Cancel
              </Button>
              <Button type="submit" disabled={loading}>
                {loading ? "Creating..." : "Create Schedule"}
              </Button>
            </DialogFooter>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  )
}