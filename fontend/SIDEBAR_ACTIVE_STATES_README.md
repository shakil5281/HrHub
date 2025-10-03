# Sidebar Active States Implementation

A comprehensive solution for implementing URL-based active states in the sidebar navigation with automatic menu expansion and visual highlighting.

## Features

### 🎯 **URL-Based Active States**
- **Automatic Detection**: Navigation items automatically become active based on current URL
- **Partial Matching**: Supports nested routes (e.g., `/admin/users/add` activates `/admin/users`)
- **Dynamic Updates**: Active states update automatically when navigating between pages

### 📂 **Smart Menu Expansion**
- **Auto-Expand Groups**: Navigation groups with active sub-items automatically expand
- **Collapsible Groups**: Inactive groups remain collapsed to save space
- **Persistent State**: Expanded state persists during navigation within the group

### 🎨 **Visual Highlighting**
- **Active Backgrounds**: Active items get highlighted with distinct background colors
- **Sub-Menu Highlighting**: Active sub-items are visually distinguished
- **Hover Estados**: Proper hover states for better user experience

## Implementation

### **Custom Hooks**

#### `useActiveLink(href, exact)`
Determines if a link is active based on current pathname.

```typescript
const isActive = useActiveLink('/admin/users', false) // Partial match
const isExactActive = useActiveLink('/admin/users', true) // Exact match
```

#### `useActiveGroup(items)`
Determines if a navigation group should be expanded based on active sub-items.

```typescript
const isGroupActive = useActiveGroup([
  { url: '/admin/users' },
  { url: '/admin/settings' }
])
```

### **Navigation Components**

#### NavGroup Component
- Auto-expands groups with active sub-items
- Highlights active sub-menu items
- Uses `useActiveGroup` for group state
- Uses `useActiveLink` for individual sub-items

#### NavMain Component
- Highlights main navigation items when active
- Uses `useActiveLink` for each main navigation item

### **Navigation Data Structure**

```typescript
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
  navGroup: [
    {
      title: "Settings",
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
  ],
}
```

## URL Matching Logic

### **Partial Matching (Default)**
- `/admin/users` matches `/admin/users`, `/admin/users/add`, `/admin/users/123/edit`
- `/company` matches `/company`, `/company/add`, `/company/123/edit`
- `/dashboard` matches `/dashboard`, `/dashboard/analytics`, `/dashboard/reports`

### **Exact Matching**
- `/admin/users` only matches exactly `/admin/users`
- Useful for specific pages that shouldn't activate parent routes

### **Group Active Logic**
A navigation group is considered active if ANY of its sub-items match the current URL:

```typescript
// Group is active if current URL matches any of:
// /company, /department, /section, /designation
const settingsGroup = {
  items: [
    { url: '/company' },
    { url: '/department' },
    { url: '/section' },
    { url: '/designation' }
  ]
}

// Group is active if current URL matches any of:
// /admin/users, /admin/users/add, /admin/users/123/edit
const adminGroup = {
  items: [
    { url: '/admin/users' }
  ]
}
```

## Usage Examples

### **Adding New Navigation Items**

1. **Add to Main Navigation:**
```typescript
// In components/layout/index.ts
export const data = {
  navMain: [
    // ... existing items
    {
      title: "Reports",
      url: "/reports",
      icon: IconChartBar,
    }
  ]
}
```

2. **Add to Group Navigation:**
```typescript
export const data = {
  navGroup: [
    {
      title: "Administrator",
        items: [
          { title: "User Management", url: "/admin/users" },
          { title: "System Settings", url: "/admin/settings" }, // New item
        ]
      }
  ]
}
```

### **Custom Active State Logic**

For complex navigation requirements, you can extend the hooks:

```typescript
// Custom hook for complex active logic
function useComplexActive(paths: string[], requireAll: boolean = false) {
  const pathname = usePathname()
  
  if (requireAll) {
    return paths.every(path => pathname.includes(path))
  }
  
  return paths.some(path => pathname.includes(path))
}

// Usage
const isAdminActive = useComplexActive(['/admin', '/users'], false)
const isSecureArea = useComplexActive(['/admin', '/secure'], true) // Both required
```

## File Structure

```
hooks/
├── use-active-links.ts       # Active link detection hooks

components/layout/
├── index.ts                  # Navigation data structure
├── nav-group.tsx            # Group navigation with active states
├── nav-main.tsx             # Main navigation with active states
└── app-sidebar.tsx          # Main sidebar component
```

## Browser Behavior

### **Automatic Updates**
- Active states update automatically when using Next.js router
- Works with both client-side navigation and direct URL access
- Handles browser back/forward buttons correctly

### **Performance**
- Uses Next.js `usePathname()` hook for efficient URL monitoring
- Minimal re-renders through optimized state management
- Lightweight implementation with no external dependencies

### **Accessibility**
- Maintains proper ARIA attributes for screen readers
- Keyboard navigation works seamlessly
- Focus management preserved during state changes

## Styling

The active states automatically apply appropriate CSS classes:

- **Active Items**: `data-active` attribute for styling
- **Expanded Groups**: `data-state="open"` for collapsible groups
- **Visual Feedback**: Consistent active/hover state styling

## Future Enhancements

### **Advanced Features**
- **Role-Based Navigation**: Hide/show navigation items based on user roles
- **Breadcrumb Integration**: Generate breadcrumbs from active navigation path
- **Dynamic Menu Items**: Load navigation items from API
- **Menu Search**: Search through navigation items
- **Recent Pages**: Track and highlight recently visited pages

### **Customization**
- **Theme Integration**: Support for different color themes
- **Animation Options**: Configurable transition animations  
- **Icon Management**: Dynamic icon loading and management
- **Custom Active Logic**: Support for complex business rules

This implementation provides a robust, maintainable solution for navigation active states that automatically adapts to your application's routing structure while providing excellent user experience and developer ergonomics.
