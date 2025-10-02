# Employee API Summary

## Overview
The Employee API provides comprehensive CRUD operations for managing employee records with full organizational hierarchy integration. The API uses relational IDs to connect employees with their Department, Section, and Designation.

## Schema Fields
- **id**: Auto-generated primary key
- **name**: Employee full name (required)
- **nameBangla**: Employee name in Bangla (SutonnyMJ font)
- **nidNo**: National ID Number (required, unique)
- **fatherName**: Father's name (required)
- **motherName**: Mother's name (required)
- **fatherNameBangla**: Father's name in Bangla (SutonnyMJ font)
- **motherNameBangla**: Mother's name in Bangla (SutonnyMJ font)
- **dateOfBirth**: Date of birth (required)
- **address**: Full address (required)
- **joiningDate**: Employment start date (required)
- **departmentId**: Reference to Department (required)
- **sectionId**: Reference to Section (required)
- **designationId**: Reference to Designation (required)
- **grossSalary**: Total salary amount (required)
- **basicSalary**: Base salary amount (required)
- **bankAccountNo**: Bank account number (required)

## API Endpoints

### GET /api/employee
**Access**: Admin, Manager, HR, HR Manager, IT
**Description**: Get all employees with optional filtering
**Query Parameters**:
- `includeInactive`: Include inactive employees (default: false)
- `departmentId`: Filter by department ID
- `sectionId`: Filter by section ID
- `designationId`: Filter by designation ID

### GET /api/employee/{id}
**Access**: Admin, Manager, HR, HR Manager, IT
**Description**: Get employee by ID with full details

### POST /api/employee
**Access**: Admin, HR, HR Manager, IT
**Description**: Create new employee
**Validation**:
- NID number uniqueness check
- Department/Section/Designation existence and hierarchy validation
- Salary validation (positive values)

### PUT /api/employee/{id}
**Access**: Admin, HR, HR Manager, IT
**Description**: Update existing employee
**Validation**:
- Same as create, plus existing employee check
- NID uniqueness for other employees

### DELETE /api/employee/{id}
**Access**: Admin, IT (Restricted access)
**Description**: Soft delete employee (sets IsActive = false)

### GET /api/employee/summary
**Access**: Admin, Manager, HR, HR Manager, IT
**Description**: Get employee summaries for dropdowns/lists
**Query Parameters**: Same filtering as main GET endpoint

## Role-Based Access Control

### IT Role Access
The IT role has **FULL ACCESS** to all Employee API endpoints, including:
- ✅ View all employees (GET /api/employee)
- ✅ View employee details (GET /api/employee/{id})
- ✅ Create employees (POST /api/employee)
- ✅ Update employees (PUT /api/employee/{id})
- ✅ Delete employees (DELETE /api/employee/{id})
- ✅ View employee summaries (GET /api/employee/summary)

### Other Roles
- **Admin**: Full access to all operations
- **HR/HR Manager**: Full access except delete operations
- **Manager**: Read-only access to employee data
- **Employee**: No access to Employee API

## Data Relationships
- **Department → Section → Designation**: Hierarchical validation ensures data integrity
- **Employee**: References all three organizational levels
- **Cascade Protection**: Employee deletion is restricted to prevent data loss

## Key Features
1. **Unique NID Validation**: Prevents duplicate National ID numbers
2. **Hierarchical Validation**: Ensures Section belongs to Department and Designation belongs to Section
3. **Soft Delete**: Maintains data integrity while allowing logical deletion
4. **Comprehensive Filtering**: Multiple filter options for efficient data retrieval
5. **Bangla Support**: Full support for Bangla names using SutonnyMJ font
6. **Audit Trail**: Tracks creation and modification with user information
7. **Salary Management**: Separate tracking of gross and basic salary components
8. **Banking Integration**: Bank account number storage for payroll processing

## Usage Examples
See `swagger-create-employee-example.json` and `swagger-update-employee-example.json` for detailed request examples with Bangla text using SutonnyMJ font.

## Database Notes
- Use `create-employee-table.sql` for manual table creation if EF migrations conflict
- NID field has unique index for performance and data integrity
- Foreign key constraints use NO ACTION to prevent accidental cascade deletions
- Multiple indexes optimize query performance for common search patterns
