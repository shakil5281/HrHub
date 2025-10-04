"use client"

import { ChevronRight } from "lucide-react"
import { type Icon } from "@tabler/icons-react"

import {
  Collapsible,
  CollapsibleContent,
  CollapsibleTrigger,
} from "@/components/ui/collapsible"
import {
  SidebarGroup,
  SidebarGroupLabel,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarMenuSub,
  SidebarMenuSubButton,
  SidebarMenuSubItem,
} from "@/components/ui/sidebar"
import { useActiveLink, useActiveGroup } from "@/hooks/use-active-links"
import Link from "next/link"

interface NavItem {
  title: string
  url: string
  icon?: React.ComponentType<{ className?: string }>
  isActive?: boolean
  items?: {
    title: string
    url: string
  }[]
}

function NavGroupItem({ item }: { item: NavItem }) {
  const isGroupActive = useActiveGroup(item.items || [])

  return (
    <Collapsible
      asChild
      defaultOpen={isGroupActive}
      className="group/collapsible"
    >
      <SidebarMenuItem>
        <CollapsibleTrigger asChild>
          <SidebarMenuButton
            tooltip={item.title}
            className={isGroupActive ? "" : ""}
            isActive={isGroupActive}
          >
            {item.icon && <item.icon />}
            <span>{item.title}</span>
            <ChevronRight className="ml-auto transition-transform duration-200 group-data-[state=open]/collapsible:rotate-90" />
          </SidebarMenuButton>
        </CollapsibleTrigger>
        <CollapsibleContent>
          <SidebarMenuSub>
            {item.items?.map((subItem) => {
              return (
                <NavSubItem key={subItem.title} subItem={subItem} />
              )
            })}
          </SidebarMenuSub>
        </CollapsibleContent>
      </SidebarMenuItem>
    </Collapsible>
  )
}

function NavSubItem({ subItem }: { subItem: { title: string; url: string } }) {
  const isSubItemActive = useActiveLink(subItem.url)

  return (
    <SidebarMenuSubItem>
      <SidebarMenuSubButton
        asChild
        className={isSubItemActive ? "" : ""}
        isActive={isSubItemActive}
      >
        <Link href={subItem.url}>
          <span>{subItem.title}</span>
        </Link>
      </SidebarMenuSubButton>
    </SidebarMenuSubItem>
  )
}

export function NavGroup({
  items,
}: {
  items: NavItem[]
}) {
  return (
    <SidebarGroup>
      <SidebarGroupLabel>Platform</SidebarGroupLabel>
      <SidebarMenu>
        {items.map((item) => {
          return (
            <NavGroupItem key={item.title} item={item} />
          )
        })}
      </SidebarMenu>
    </SidebarGroup>
  )
}