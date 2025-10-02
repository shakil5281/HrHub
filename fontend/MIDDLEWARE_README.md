# Authentication Middleware

This project includes a comprehensive authentication middleware system for Next.js that provides route protection, role-based access control, and automatic token management.

## Files Created

### Core Middleware Files
- `middleware.ts` - Main Next.js middleware for route protection
- `lib/middleware.ts` - Utility functions for token handling and route configuration
- `hooks/use-auth.ts` - React hook for client-side authentication state management
- `components/with-auth.tsx` - Higher-order component for protecting React components

### Example Files
- `app/admin/page.tsx` - Example of a role-protected admin page

## Features

### 1. Route Protection
- Automatically redirects unauthenticated users to login page
- Preserves intended destination with redirect parameter
- Prevents authenticated users from accessing login page

### 2. Role-Based Access Control
- Supports multiple user roles (Admin, HR, HRManager, etc.)
- Configurable route-role mappings
- Automatic access denial for insufficient permissions

### 3. Token Management
- JWT token validation and expiration checking
- Automatic token cleanup on expiration
- Support for both localStorage and cookie-based storage

### 4. Client-Side Authentication
- React hook for authentication state management
- Higher-order component for component-level protection
- Automatic logout on token expiration

## Configuration

### Route Configuration
Edit `lib/middleware.ts` to configure protected routes:

```typescript
export const defaultRouteConfigs: RouteConfig[] = [
  { path: '/dashboard', requireAuth: true },
  { path: '/admin', requireAuth: true, roles: ['Admin'] },
  { path: '/hr', requireAuth: true, roles: ['HR', 'HRManager', 'Admin'] },
  { path: '/login', requireAuth: false },
  { path: '/register', requireAuth: false },
  { path: '/', requireAuth: false },
]
```

### Middleware Matcher
The middleware runs on all routes except:
- API routes (`/api/*`)
- Static files (`/_next/static/*`, `/_next/image/*`)
- Favicon and image files

## Usage Examples

### 1. Using the useAuth Hook

```tsx
'use client'
import { useAuth } from '@/hooks/use-auth'

export default function MyComponent() {
  const { user, isLoading, isAuthenticated, logout } = useAuth()

  if (isLoading) return <div>Loading...</div>
  if (!isAuthenticated) return <div>Please login</div>

  return (
    <div>
      <h1>Welcome, {user?.firstName}!</h1>
      <p>Roles: {user?.roles.join(', ')}</p>
      <button onClick={logout}>Logout</button>
    </div>
  )
}
```

### 2. Using the withAuth HOC

```tsx
import { withAuth } from '@/components/with-auth'

function AdminPanel() {
  return <div>Admin only content</div>
}

// Protect component with Admin role requirement
export default withAuth(AdminPanel, ['Admin'])
```

### 3. Creating Protected Pages

Simply create pages in protected directories. The middleware will automatically handle authentication:

```tsx
// app/dashboard/page.tsx
export default function Dashboard() {
  return <div>Protected dashboard content</div>
}
```

## Token Storage

The system uses dual storage for maximum compatibility:

1. **localStorage** - For client-side JavaScript access
2. **Cookies** - For server-side middleware access

Both are automatically managed by the login/logout functions.

## Error Handling

### Automatic Redirects
- Unauthenticated users → `/login?redirect=/intended-page`
- Insufficient permissions → `/dashboard?error=unauthorized`
- Expired tokens → `/login?expired=true&redirect=/intended-page`

### Error States
The `useAuth` hook provides loading and error states for proper UX handling.

## Security Features

1. **Token Expiration Checking** - Automatic validation of JWT expiration
2. **Role Validation** - Server-side role checking in middleware
3. **Automatic Cleanup** - Expired tokens are automatically removed
4. **Secure Redirects** - Prevents open redirect vulnerabilities

## Customization

### Adding New Roles
1. Update your backend to include new roles in JWT tokens
2. Add role configurations in `lib/middleware.ts`
3. Use the new roles in route configurations

### Custom Route Protection
You can create custom route configurations by modifying the `defaultRouteConfigs` array or passing custom configs to `getRouteConfig()`.

### Styling Loading States
Customize the loading spinners and error messages in `components/with-auth.tsx` to match your design system.

## Integration with Existing Code

The middleware integrates seamlessly with your existing authentication system:

1. **Login Form** - Already updated to set both localStorage and cookies
2. **API Client** - Axios interceptors handle token attachment and cleanup
3. **Logout Function** - Clears all authentication data

## Troubleshooting

### Common Issues

1. **Middleware not running** - Check that files are in the correct locations and `next.config.ts` doesn't exclude middleware
2. **Token not found** - Ensure login form sets both localStorage and cookies
3. **Role access denied** - Verify JWT token contains correct role claims
4. **Infinite redirects** - Check route configurations for conflicts

### Debug Mode
Add console logs to middleware for debugging:

```typescript
console.log('Middleware running for:', pathname)
console.log('Token found:', !!token)
console.log('Route config:', routeConfig)
```

## Performance Considerations

- Middleware runs on every request matching the matcher pattern
- Token decoding is lightweight (no cryptographic verification in middleware)
- Route configurations are cached for performance
- Client-side hooks use React's built-in optimization

This middleware system provides enterprise-grade authentication and authorization while remaining simple to use and customize.
