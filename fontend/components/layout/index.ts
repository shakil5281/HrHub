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
  IconDeviceDesktop,
  IconFileText,
  IconDownload,
  IconActivity,
  IconReportAnalytics,
  IconKey,
  IconServer,
  IconClock,
  IconCalendar,
  IconTrendingUp,
  IconCircleCheck,
  IconCircleX,
  IconAlertCircle,
  IconUser,
  IconChartBar as IconBarChart,
  IconWifi,
  IconWifiOff,
  IconEdit,
  IconTrash,
  IconEye,
  IconStar,
  IconBriefcase
} from "@tabler/icons-react"

export const data = {
  navMain: [
    {
      title: "Dashboard",
      url: "/dashboard",
      icon: IconDashboard,
    },
  ],
  navSecondary: [
    {
      title: "Settings",
      url: "/admin/system",
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
      icon: IconBuilding,
      items: [
        {
          title: "Companies",
          url: "/company",
        },
        {
          title: "Departments",
          url: "/department",
        },
        {
          title: "Sections",
          url: "/section",
        },
        {
          title: "Designations",
          url: "/designation",
        },
      ],
    },
    {
      title: "Employee Management",
      url: "#",
      icon: IconUsers,
      items: [
        {
          title: "Employees",
          url: "/employee",
        },
      ],
    },
    {
      title: "Attendance Management",
      url: "#",
      icon: IconClock,
      items: [
        {
          title: "Dashboard",
          url: "/attendance",
        },
        {
          title: "Devices",
          url: "/attendance/devices",
        },
        {
          title: "Logs",
          url: "/attendance/logs",
        },
        {
          title: "Sync & Download",
          url: "/attendance/sync",
        },
        {
          title: "Statistics",
          url: "/attendance/statistics",
        },
        {
          title: "Reports",
          url: "/attendance/reports",
        },
        {
          title: "Daily Reports",
          url: "/attendance/reports/daily",
        },
        {
          title: "Employee Reports",
          url: "/attendance/reports/employee",
        },
        {
          title: "Summary Reports",
          url: "/attendance/reports/summary",
        },
      ],
    },
    {
      title: "Administration",
      url: "#",
      icon: IconShield,
      items: [
        {
          title: "Admin Dashboard",
          url: "/admin",
        },
        {
          title: "User Management",
          url: "/admin/users",
        },
        {
          title: "Permissions",
          url: "/admin/permissions",
        },
        {
          title: "Roles",
          url: "/admin/roles",
        },
        {
          title: "System Management",
          url: "/admin/system",
        },
      ],
    },
    {
      title: "System Tools",
      url: "#",
      icon: IconDatabase,
      items: [
        {
          title: "Backup Management",
          url: "/backup",
        },
      ],
    },
  ],
}