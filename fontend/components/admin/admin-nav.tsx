"use client"

import { usePathname } from "next/navigation"
import Link from "next/link"
import { cn } from "@/lib/utils"
import { 
  IconUsers, 
  IconShield,
  IconSettings,
  IconServer,
  IconKey
} from "@tabler/icons-react"

const navigation = [
  {
    name: "Users",
    href: "/admin/users",
    icon: IconUsers,
  },
  {
    name: "Permissions",
    href: "/admin/permissions",
    icon: IconShield,
  },
  {
    name: "Roles",
    href: "/admin/roles",
    icon: IconKey,
  },
  {
    name: "System",
    href: "/admin/system",
    icon: IconServer,
  },
  {
    name: "Settings",
    href: "/admin/settings",
    icon: IconSettings,
  },
]

export function AdminNav() {
  const pathname = usePathname()

  return (
    <nav className="flex space-x-1">
      {navigation.map((item) => {
        const isActive = pathname === item.href || 
          (item.href !== "/admin" && pathname.startsWith(item.href))
        
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
