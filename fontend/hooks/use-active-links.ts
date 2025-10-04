import { usePathname } from 'next/navigation'

/**
 * Hook to determine if a link is active based on the current pathname
 * This ensures only the most specific (deepest) match is active
 * @param href - The href of the link to check
 * @param exact - Whether to match exactly or allow partial matches
 * @returns boolean indicating if the link is active
 */
export function useActiveLink(href: string, exact: boolean = false): boolean {
  const pathname = usePathname()
  
  if (exact) {
    return pathname === href
  }
  
  // Only exact matches are considered active
  // This prevents parent paths from being active when child paths are active
  return pathname === href
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
    // Only exact matches are considered active
    return pathname === item.url
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
