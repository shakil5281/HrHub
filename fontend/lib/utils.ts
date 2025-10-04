import { clsx, type ClassValue } from "clsx"
import { twMerge } from "tailwind-merge"

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs))
}

// Utility functions to prevent hydration mismatches
export function formatNumber(value: number): string {
  return value.toLocaleString('en-US')
}

export function formatCurrency(value: number, currency: string = '৳'): string {
  return `${currency}${value.toLocaleString('en-US')}`
}

export function formatDate(date: string | Date): string {
  const dateObj = typeof date === 'string' ? new Date(date) : date
  return dateObj.toLocaleString('en-US', {
    year: 'numeric',
    month: 'short',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}