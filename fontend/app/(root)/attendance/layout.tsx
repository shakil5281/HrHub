import { AttendanceNav } from "@/components/attendance/attendance-nav"

export default function AttendanceLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return (
    <div className="space-y-6">
      <div className="border-b">
        <AttendanceNav />
      </div>
      {children}
    </div>
  )
}
