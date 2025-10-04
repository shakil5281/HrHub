import { Metadata } from "next"

export const metadata: Metadata = {
  title: "Backup Management | HR Hub",
  description: "Manage database backups, schedules, and system health",
}

export default function BackupLayout({
  children,
}: {
  children: React.ReactNode
}) {
  return <>{children}</>
}
