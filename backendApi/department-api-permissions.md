# Department API - Role-Based Access Control

## Updated Permissions for IT Role Users

### 📋 **Read Operations** (IT role can access)
- **GET /api/department** - Get all departments
  - **Roles**: Admin, Manager, HR, HR Manager, **IT**
  - **Description**: View list of all departments

- **GET /api/department/{id}** - Get department by ID  
  - **Roles**: Admin, Manager, HR, HR Manager, **IT**
  - **Description**: View specific department details

- **GET /api/department/statistics** - Get department statistics
  - **Roles**: Admin, HR Manager, **IT**
  - **Description**: View department analytics and statistics

### ✏️ **Write Operations** (IT role can access)
- **POST /api/department** - Create new department
  - **Roles**: Admin, HR Manager, **IT**
  - **Description**: Create new departments

- **PUT /api/department/{id}** - Update department
  - **Roles**: Admin, HR Manager, **IT**
  - **Description**: Update existing department information

### 🗑️ **Delete Operations** (Admin/IT only)
- **DELETE /api/department/{id}** - Delete department
  - **Roles**: Admin, **IT**
  - **Description**: Delete departments (restricted to Admin and IT for data safety)

## Summary of Changes

✅ **IT role users can now:**
- View all departments and their details
- Create new departments
- Update existing departments  
- View department statistics
- **Delete departments** (full CRUD access)

✅ **Complete CRUD Access:**
- IT users now have full Create, Read, Update, and Delete access to departments

## Use Cases for IT Role

- **System Administration**: IT personnel can manage department structure
- **User Management**: IT can create departments when setting up new users
- **Data Maintenance**: IT can update department information as needed
- **Reporting**: IT can access department statistics for system monitoring

## Security Notes

- All operations still require valid JWT authentication
- Role validation is enforced at the API level
- Audit trails are maintained for all department operations
- Delete operations are available to Admin and IT roles with proper validation
