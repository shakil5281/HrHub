# User Management System

A comprehensive user management system with full CRUD operations, built for the HR Hub application.

## Features

### 🔐 User Authentication & Management
- **Create Users**: Add new users with email, password, personal info, and work details
- **View Users**: Browse all users with detailed information and statistics
- **Edit Users**: Update user information (excluding password for security)
- **Delete Users**: Remove users with confirmation dialogs
- **User Details**: View comprehensive user profiles with roles and permissions

### 📊 Dashboard & Analytics
- **Statistics Cards**: Total users, companies, departments, and averages
- **Search & Filter**: Find users by name, email, department, position, or company
- **Real-time Updates**: Refresh data and see changes immediately

### 🏢 Company Integration
- **Company Selection**: Users are assigned to specific companies
- **Department Management**: Users belong to departments within companies
- **Position Tracking**: Track user roles and positions

## API Endpoints

### User Management
- `POST /Auth/register` - Create a new user (registration)
- `GET /User` - Get all users with pagination
- `GET /User/{id}` - Get user by ID
- `PUT /User/{id}` - Update user
- `DELETE /User/{id}` - Delete user
- `PUT /User/{id}/status` - Update user status (active/inactive)
- `PUT /User/{id}/roles` - Update user roles
- `GET /User/roles` - Get available roles list
- `DELETE /User/{id}/permanent` - Permanently delete a user
- `GET /User/statistics` - Get user statistics
- `GET /User/company/{companyId}` - Get users by company
- `GET /User/department/{department}` - Get users by department

### Request/Response Format

#### Create User Request
```json
{
  "email": "user@example.com",
  "password": "string",
  "confirmPassword": "string",
  "firstName": "string",
  "lastName": "string",
  "department": "string",
  "position": "string",
  "companyId": 0
}
```

#### Users List Response
```json
{
  "success": true,
  "message": "string",
  "data": {
    "users": [
      {
        "id": "string",
        "email": "string",
        "firstName": "string",
        "lastName": "string",
        "department": "string",
        "position": "string",
        "companyId": 0,
        "companyName": "string",
        "isActive": true,
        "createdAt": "2025-10-03T13:12:28.499Z",
        "updatedAt": "2025-10-03T13:12:28.499Z",
        "roles": ["string"]
      }
    ],
    "pagination": {
      "currentPage": 0,
      "pageSize": 0,
      "totalItems": 0,
      "totalPages": 0,
      "hasNextPage": true,
      "hasPreviousPage": true
    }
  },
  "errors": ["string"]
}
```

#### User Statistics Response
```json
{
  "success": true,
  "message": "string",
  "data": {
    "totalUsers": 0,
    "activeUsers": 0,
    "inactiveUsers": 0,
    "usersByCompany": {},
    "usersByDepartment": {},
    "averageUsersPerCompany": 0,
    "recentRegistrations": 0
  },
  "errors": ["string"]
}
```

#### Available Roles Response
```json
{
  "success": true,
  "message": "Roles retrieved successfully",
  "data": [
    {
      "id": "b90f6334-c54a-40b1-bdd0-5f3ca8addb33",
      "name": "Admin",
      "description": "System Administrator with full access",
      "userCount": 1,
      "activeUserCount": 1
    },
    {
      "id": "a91587f2-ba82-4290-b6f9-d3ab4842b396",
      "name": "Employee",
      "description": "Regular employee with basic access",
      "userCount": 2,
      "activeUserCount": 2
    }
  ],
  "errors": []
}
```

## File Structure

```
lib/api/
├── user.ts                 # User API service with CRUD operations

components/user/
├── user-add-form.tsx       # Form for creating new users
├── user-edit-form.tsx      # Form for editing existing users
├── user-table.tsx          # Data table for displaying users
└── index.ts               # Export file

app/(root)/admin/users/
├── page.tsx               # Main user management page
├── add/
│   └── page.tsx          # Add new user page
└── [id]/edit/
    └── page.tsx          # Edit user page
```

## Components

### UserAddForm
- **Purpose**: Create new users
- **Features**: 
  - Form validation with Zod schema
  - Company and department selection
  - Password confirmation
  - Error handling and loading states

### UserEditForm
- **Purpose**: Edit existing users
- **Features**:
  - Pre-populated form with current user data
  - Company and department selection
  - Role display (read-only)
  - Update user information

### UserTable
- **Purpose**: Display and manage users
- **Features**:
  - Sortable columns
  - Action dropdowns (view, edit, manage roles, delete)
  - User details modal
  - Delete confirmation dialog
  - Status toggle functionality
  - Loading states and empty states

### UserRolesDialog
- **Purpose**: Manage user roles and permissions
- **Features**:
  - View current user roles
  - Select from predefined system roles
  - Add new roles from available roles list
  - Remove existing roles
  - Role descriptions and user counts
  - Real-time role management
  - Form validation and error handling

## Navigation

The user management system is accessible through:
- **Main Navigation**: Administrator → User Management
- **Direct Links**: 
  - `/admin/users` - User list
  - `/admin/users/add` - Add user
  - `/admin/users/{id}/edit` - Edit user

## Security Features

- **Password Validation**: Minimum 6 characters with confirmation
- **Email Validation**: Proper email format validation
- **Role-based Access**: Users have assigned roles and permissions
- **Confirmation Dialogs**: Delete operations require confirmation
- **Error Handling**: Comprehensive error messages and validation

## Integration

### Dependencies
- **Companies**: Users must be assigned to a company
- **Departments**: Users belong to departments within companies
- **Authentication**: Integrated with existing auth system

### Data Flow
1. **Create User**: Form → Auth Register API → Database → Response
2. **View Users**: API → Database → Table Display
3. **Edit User**: Load Data → Form → API → Database → Response
4. **Delete User**: Confirmation → API → Database → Refresh

## Usage Examples

### Creating a User
1. Navigate to `/admin/users/add`
2. Fill in personal information (name, email)
3. Select company and department
4. Enter position and password
5. Submit form

### Managing Users
1. Navigate to `/admin/users`
2. View user statistics and search
3. Use action dropdowns for user operations
4. View detailed user information in modals
5. Toggle user status (active/inactive)
6. Manage user roles and permissions

### Editing Users
1. Click "Edit" from user table or details modal
2. Update user information
3. Save changes

### Managing User Roles
1. Click "Manage Roles" from user table dropdown
2. View current user roles
3. Select from predefined system roles in the dropdown
4. Add selected roles by clicking the plus button
5. Remove roles by clicking the X on role badges
6. View role descriptions and user counts
7. Save changes to update user permissions

## Error Handling

- **Form Validation**: Real-time validation with error messages
- **API Errors**: Displayed in user-friendly format
- **Loading States**: Visual feedback during operations
- **Empty States**: Helpful messages when no data exists

## Future Enhancements

- **Bulk Operations**: Select multiple users for batch operations
- **Advanced Filtering**: Filter by roles, creation date, etc.
- **User Import/Export**: CSV import/export functionality
- **Password Reset**: Admin-initiated password resets
- **User Activity Logs**: Track user actions and changes
- **Advanced Permissions**: Granular role-based permissions
