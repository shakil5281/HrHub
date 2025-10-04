"use client"

import { usePathname } from "next/navigation"
import { Separator } from "@/components/ui/separator"
import { SidebarTrigger } from "@/components/ui/sidebar"
import { LogoutButton } from "@/components/logout-button"

export function SiteHeader() {
  const pathname = usePathname()
  
  // Get page title based on current path
  const getPageTitle = () => {
    const pathSegments = pathname.split('/').filter(Boolean)
    
    if (pathSegments.length === 0) return "Dashboard"
    
    const lastSegment = pathSegments[pathSegments.length - 1]
    
    // Convert kebab-case to Title Case
    return lastSegment
      .split('-')
      .map(word => word.charAt(0).toUpperCase() + word.slice(1))
      .join(' ')
  }

  return (
    <header className="flex h-(--header-height) shrink-0 items-center gap-2 border-b transition-[width,height] ease-linear group-has-data-[collapsible=icon]/sidebar-wrapper:h-(--header-height)">
      <div className="flex w-full items-center gap-1 px-4 lg:gap-2 lg:px-6 py-6">
        <SidebarTrigger className="-ml-1" />
        <Separator
          orientation="vertical"
          className="mx-2 data-[orientation=vertical]:h-4"
        />
        <h1 className="text-base font-medium">{getPageTitle()}</h1>
        <div className="ml-auto flex items-center gap-2">
          <LogoutButton showConfirmation={true} />
        </div>
      </div>
    </header>
  )
}
