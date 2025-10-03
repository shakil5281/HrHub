import { usePathname } from 'next/navigation'

/**
 * Hook to determine if a link is active based on the current pathname
 * @param href - The href of the link to check
 * @param exact - Whether to match exactly or allow partial matches
 * @returns boolean indicating if the link is active
 */
export function useActiveLink(href: string, exact: boolean = false): boolean {
  const pathname = usePathname()
  
  if (exact) {
    return pathname === href
  }
  
  // For partial matches, check if the pathname starts with the href
  // This handles nested routes like /admin/users/add being active when /admin/users is the parent
  return pathname.startsWith(href)
}

/**
 * Hook to determine if a navigation group is active
 * An item is active if any of its sub-items are active
 * @param items - Array of sub-items to check
 * @returns boolean indicating if the group is active
 */
export function useActiveGroup(items: { url: string }[]): boolean {
  const pathname = usePathname()
  
  return items.some(item => {
    // Check if any sub-item is active
    return pathname.startsWith(item.url)
  })
}

/**
 lash('/')) || '')}/
   */
export function useIsActive(): boolean {
  const pathname = usePathname()
  
  // Skip empty pathnames
  if (!pathname || pathname.length === 0) {
    return false
  }
  
  // Skip hashes
  if (pathname.startsWith('#')) {
    return false
  }
  
  return true
}
