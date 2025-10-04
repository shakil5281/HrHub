"use client"

import { usePathname } from "next/navigation"
import Link from "next/link"
import { cn } from "@/lib/utils"
import { 
  IconDashboard, 
  IconDeviceDesktop, 
  IconFileText, 
  IconRefresh, 
  IconDownload, 
  IconActivity,
  IconSettings
} from "@tabler/icons-react"

const navigation = [
  {
    name: "Dashboard",
    href: "/attendance",
    icon: IconDashboard,
  },
  {
    name: "Devices",
    href: "/attendance/devices",
    icon: IconDeviceDesktop,
  },
  {
    name: "Logs",
    href: "/attendance/logs",
    icon: IconFileText,
  },
  {
    name: "Download Logs",
    href: "/attendance/sync",
    icon: IconDownload,
  },
  {
    name: "Statistics",
    href: "/attendance/statistics",
    icon: IconActivity,
  },
]

export function AttendanceNav() {
  const pathname = usePathname()

  return (
    <nav className="flex space-x-1">
      {navigation.map((item) => {
        const isActive = pathname === item.href || 
          (item.href !== "/attendance" && pathname.startsWith(item.href))
        
        return (
          <Link
            key={item.name}
            href={item.href}
            className={cn(
              "flex items-center space-x-2 px-3 py-2 rounded-md text-sm font-medium transition-colors",
              isActive
                ? "bg-primary text-primary-foreground"
                : "text-muted-foreground hover:text-foreground hover:bg-muted"
            )}
          >
            <item.icon className="h-4 w-4" />
            <span>{item.name}</span>
          </Link>
        )
      })}
    </nav>
  )
}
