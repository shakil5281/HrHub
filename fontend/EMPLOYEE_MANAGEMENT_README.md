# Employee Management System

A comprehensive employee management system built with Next.js, TypeScript, and integrated with REST API endpoints.

## Overview

The Employee Management System provides full CRUD (Create, Read, Update, Delete) operations for managing employee records, including personal information, work details, and salary information.

## Features

### Employee Data Management
- **Create New Employees**: Add employees with comprehensive personal and work information
- **View Employee List**: Table view with search, filtering, and detailed employee information
- **Edit Employee Records**: Update existing employee information
- **Delete Employees**: Soft delete functionality (Admin and IT only)
- **Employee Statistics**: Dashboard with key metrics and overview

### Employee Information Fields
- **Personal Information**:
  - Full Name (English & Bangla)
  - NID Number
  - Father's Name
  - Mother's Name
  - Date of Birth
  - Address

- **Work Information**:
  - Joining Date
  - Department
  - Section
  - Designation
  - Designation Grade

- **Salary Information**:
  - Gross Salary
  - Basic Salary
  - Bank Account Number

## API Integration

### Employee Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/Employee` | Get all employees with optional filtering |
| POST | `/api/Employee` | Create a new employee |
| GET | `/api/Employee/{id}` | Get employee by ID |
| PUT | `/api/Employee/{id}` | Update an existing employee |
| DELETE | `/api/Employee/{id}` | Soft delete an employee (Admin and IT only) |
| GET | `/api/Employee/summary` | Get employee summary (minimal info for dropdowns, etc.) |

### Request/Response Formats

#### Create Employee Request Body
```json
{
  "name": "string",
  "nameBangla": "string",
  "nidNo": "string",
  "fatherName": "string",
  "motherName": "string",
  "fatherNameBangla": "string",
  "motherNameBangla": "string",
  "dateOfBirth": "2025-10-03T16:01:03.844Z",
  "address": "string",
  "joiningDate": "2025-10-03T16:01:03.844Z",
  "departmentId": 0,
  "sectionId": 0,
  "designationId": 0,
  "grossSalary": 0,
  "basicSalary": 0,
  "bankAccountNo": "string"
}
```

#### Employee List Response
```json
{
  "success": true,
  "message": "string",
  "data": [
    {
      "id": 0,
      "name": "string",
      "nameBangla": "string",
      "nidNo": "string",
      "joiningDate": "2025-10-03T16:00:15.412Z",
      "departmentName": "string",
      "sectionName": "string",
      "designationName": "string",
      "designationGrade": "string",
      "grossSalary": 0,
      "basicSalary": 0,
      "isActive": true,
      "createdAt": "2025-10-03T16:00:15.412Z"
    }
  ],
  "errors": ["string"]
}
```

## Components

### 1. EmployeeTable (`components/employee/employee-table.tsx`)
- Displays employee data in a structured table format
- Shows employee details, job information, salary, and status
- Provides view, edit, and delete actions
- Includes detailed employee profile modal
- Loading states and error handling

### 2. EmployeeAddForm (`components/employee/employee-add-form.tsx`)
- Comprehensive form for creating new employees
- Organized sections for personal, work, and salary information
- Integration with department, section, and designation APIs
- Form validation with Zod schema
- Error handling and loading states

### 3. EmployeeEditForm (`components/employee/employee-edit-form.tsx`)
- Pre-populated form for editing existing employees
- Same structure as add form with existing data
- Update functionality with API integration
- Validation and error handling

## Pages

### 1. Employee List Page (`app/(root)/employee/page.tsx`)
- Dashboard view with employee statistics cards
- Search and filter functionality
- Statistics: Total employees, Department coverage, Average salary, Active rate
- Employee table integration
- Refresh and add employee navigation

### 2. Add Employee Page (`app/(root)/employee/add/page.tsx`)
- Simple wrapper for EmployeeAddForm component
- Clean page structure

### 3. Edit Employee Page (`app/(root)/employee/[id]/edit/page.tsx`)
- Dynamic routing for employee ID
- EmployeeEditForm wrapper with employee ID parsing

## Navigation Integration

Employee Management is integrated into the sidebar navigation (`components/layout/index.ts`) with:
- **Employee Management** group with Users icon
- **Employee List** link (`/employee`)
- **Add Employee** link (`/employee/add`)

## Usage Examples

### Creating a New Employee
1. Navigate to `/employee/add`
2. Fill in required fields (marked avec *)
3. Select department, section, and designation from dropdowns
4. Enter salary information
5. Submit to create employee

### Viewing Employee Details
1. Go to `/employee` (Employee List)
2. Click the view icon in the Actions column
3. Review comprehensive employee information in modal

### Editing Employee Information
1. From Employee List, click edit icon in Actions column
2. Navigate to edit page with pre-populated data
3. Modify fields as needed
4. Save changes

### Searching Employees
- Use the search input on Employee List page
- Search by name, NID, department, designation, or family information
- Real-time filtering results

## Data Dependencies

The employee system integrates with:
- **Department API** (`lib/api/department.ts`) - For department selection
- **Section API** (`lib/api/section.ts`) - For section selection  
- **Designation API** (`lib/api/designation.ts`) - For designation selection

## Features Implemented

✅ **Complete CRUD Operations**
- Create new employees with comprehensive form
- Read employee data with advanced table view
- Update existing employee records
- Delete employees (soft delete)

✅ **Advanced Search & Filtering**
- Multi-field search across name, NID, department, designation, family info
- Real-time filtering with result counts

✅ **Employee Dashboard**
- Statistics cards with key metrics
- Visual overview of employee data
- Active/inactive employee tracking

✅ **Form Validation**
- Zod schema validation for all forms
- Required field validation
- Numeric validation for salaries
- Date format validation

✅ **User Experience**
- Loading states throughout
- Error handling with user-friendly messages
- Responsive design
- Modal dialogs for detailed views
- Confirmation dialogs for delete operations

✅ **API Integration**
- Full integration with Employee API endpoints
- Proper error handling and response processing
- Type-safe interfaces and request/response handling

✅ **Navigation Integration**
- Sidebar navigation with active states
- Breadcrumb navigation
- Contextual actions and routing

## Active Link Background Color

The employee navigation items integrate with the active link system:
- Primary background color (`bg-primary`) for active link highlighting
- Automatic detection based on current URL
- Consistent styling across all navigation items

Employees can be managed comprehensively through the intuitive interface with proper API integration and user experience features.
