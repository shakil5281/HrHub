import {
  IconCamera,
  IconChartBar,
  IconDashboard,
  IconDatabase,
  IconFileAi,
  IconFileDescription,
  IconFileWord,
  IconFolder,
  IconHelp,
  IconInnerShadowTop,
  IconListDetails,
  IconReport,
  IconSearch,
  IconSettings,
  IconUsers,
  IconBuilding,
  IconShield,
} from "@tabler/icons-react"

export const data = {
  navMain: [
    {
      title: "Dashboard",
      url: "/dashboard",
      icon: IconDashboard,
    },
    {
      title: "Companies",
      url: "/company",
      icon: IconBuilding,
    }
  ],
  navSecondary: [
    {
      title: "Settings",
      url: "#",
      icon: IconSettings,
    },
    {
      title: "Get Help",
      url: "#",
      icon: IconHelp,
    },
    {
      title: "Search",
      url: "#",
      icon: IconSearch,
    },
  ],
  navGroup: [

    {
      title: "Company Management",
      url: "#",
      icon: IconDatabase,
      items: [
        {
          title: "Company list",
          url: "/company",
        },
        {
          title: "Department",
          url: "/department",
        },
        {
          title: "Section",
          url: "/section",
        },
        {
          title: "Designation",
          url: "/designation",
        },

      ],
    },
    {
      title: "Data collection",
      url: "#",
      icon: IconBuilding,
      items: [
        {
          title: "Collect data",
          url: "/data-collection",
        },
        {
          title: "Daily Progress",
          url: "/daily-progress",
        },
      ],
    },
    {
      title: "Employee Management",
      url: "#",
      icon: IconUsers,
      items: [
        {
          title: "Employee List",
          url: "/employee",
        },
      ],
    },
    {
      title: "Administrator",
      url: "#",
      icon: IconReport,
      items: [
        {
          title: "User Management",
          url: "/admin/users",
        },
      ],
    },
    {
      title: "System Management",
      url: "#",
      icon: IconShield,
      items: [
        {
          title: "Backup Management",
          url: "/backup",
        },
      ],
    },
  ],
}