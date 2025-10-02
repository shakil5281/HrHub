using HrHubAPI.Data;
using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all employees with optional filtering
        /// </summary>
        /// <param name="includeInactive">Include inactive employees in the result</param>
        /// <param name="departmentId">Filter by department ID</param>
        /// <param name="sectionId">Filter by section ID</param>
        /// <param name="designationId">Filter by designation ID</param>
        /// <returns>List of employees</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)] // Forbidden - Admin/Manager/HR/IT role required
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllEmployees(
            [FromQuery] bool includeInactive = false,
            [FromQuery] int? departmentId = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] int? designationId = null)
        {
            try
            {
                var query = _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(e => e.IsActive);
                }

                if (departmentId.HasValue)
                {
                    query = query.Where(e => e.DepartmentId == departmentId.Value);
                }

                if (sectionId.HasValue)
                {
                    query = query.Where(e => e.SectionId == sectionId.Value);
                }

                if (designationId.HasValue)
                {
                    query = query.Where(e => e.DesignationId == designationId.Value);
                }

                var employees = await query
                    .OrderBy(e => e.Name)
                    .Select(e => new EmployeeListDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        JoiningDate = e.JoiningDate,
                        DepartmentName = e.Department.Name,
                        SectionName = e.Section.Name,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        IsActive = e.IsActive,
                        CreatedAt = e.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<EmployeeListDto>>
                {
                    Success = true,
                    Message = $"Retrieved {employees.Count} employees successfully",
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving employees",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get employee by ID
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Employee details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)] // Forbidden - Admin/Manager/HR/IT role required
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Where(e => e.Id == id)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        FatherName = e.FatherName,
                        MotherName = e.MotherName,
                        FatherNameBangla = e.FatherNameBangla,
                        MotherNameBangla = e.MotherNameBangla,
                        DateOfBirth = e.DateOfBirth,
                        Address = e.Address,
                        JoiningDate = e.JoiningDate,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.Name,
                        SectionId = e.SectionId,
                        SectionName = e.Section.Name,
                        DesignationId = e.DesignationId,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        BankAccountNo = e.BankAccountNo,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt,
                        IsActive = e.IsActive,
                        CreatedBy = e.CreatedBy,
                        UpdatedBy = e.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                if (employee == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee not found",
                        Errors = new List<string> { $"No employee found with ID: {id}" }
                    });
                }

                return Ok(new ApiResponse<EmployeeDto>
                {
                    Success = true,
                    Message = "Employee retrieved successfully",
                    Data = employee
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the employee",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new employee
        /// </summary>
        /// <param name="model">Employee creation data</param>
        /// <returns>Created employee details</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)] // Forbidden - Admin/HR/IT role required
        [ProducesResponseType(typeof(ApiResponse<object>), 409)] // Conflict - NID already exists
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeDto model)
        {
            try
            {
                // Check if NID already exists
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.NIDNo == model.NIDNo);

                if (existingEmployee != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee with this NID already exists",
                        Errors = new List<string> { $"An employee with NID '{model.NIDNo}' already exists" }
                    });
                }

                // Validate department, section, and designation exist and are active
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == model.DepartmentId && d.IsActive);
                if (department == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid department",
                        Errors = new List<string> { $"Department with ID {model.DepartmentId} not found or inactive" }
                    });
                }

                var section = await _context.Sections
                    .FirstOrDefaultAsync(s => s.Id == model.SectionId && s.IsActive && s.DepartmentId == model.DepartmentId);
                if (section == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid section",
                        Errors = new List<string> { $"Section with ID {model.SectionId} not found, inactive, or doesn't belong to the specified department" }
                    });
                }

                var designation = await _context.Designations
                    .FirstOrDefaultAsync(d => d.Id == model.DesignationId && d.IsActive && d.SectionId == model.SectionId);
                if (designation == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid designation",
                        Errors = new List<string> { $"Designation with ID {model.DesignationId} not found, inactive, or doesn't belong to the specified section" }
                    });
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var employee = new Employee
                {
                    Name = model.Name,
                    NameBangla = model.NameBangla,
                    NIDNo = model.NIDNo,
                    FatherName = model.FatherName,
                    MotherName = model.MotherName,
                    FatherNameBangla = model.FatherNameBangla,
                    MotherNameBangla = model.MotherNameBangla,
                    DateOfBirth = model.DateOfBirth,
                    Address = model.Address,
                    JoiningDate = model.JoiningDate,
                    DepartmentId = model.DepartmentId,
                    SectionId = model.SectionId,
                    DesignationId = model.DesignationId,
                    GrossSalary = model.GrossSalary,
                    BasicSalary = model.BasicSalary,
                    BankAccountNo = model.BankAccountNo,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Fetch the created employee with related data
                var createdEmployee = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Where(e => e.Id == employee.Id)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        FatherName = e.FatherName,
                        MotherName = e.MotherName,
                        FatherNameBangla = e.FatherNameBangla,
                        MotherNameBangla = e.MotherNameBangla,
                        DateOfBirth = e.DateOfBirth,
                        Address = e.Address,
                        JoiningDate = e.JoiningDate,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.Name,
                        SectionId = e.SectionId,
                        SectionName = e.Section.Name,
                        DesignationId = e.DesignationId,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        BankAccountNo = e.BankAccountNo,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt,
                        IsActive = e.IsActive,
                        CreatedBy = e.CreatedBy,
                        UpdatedBy = e.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, new ApiResponse<EmployeeDto>
                {
                    Success = true,
                    Message = "Employee created successfully",
                    Data = createdEmployee!
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the employee",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing employee
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <param name="model">Employee update data</param>
        /// <returns>Updated employee details</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<EmployeeDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)] // Forbidden - Admin/HR/IT role required
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)] // Conflict - NID already exists
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeDto model)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee not found",
                        Errors = new List<string> { $"No employee found with ID: {id}" }
                    });
                }

                // Check if NID already exists for another employee
                var existingEmployee = await _context.Employees
                    .FirstOrDefaultAsync(e => e.NIDNo == model.NIDNo && e.Id != id);

                if (existingEmployee != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee with this NID already exists",
                        Errors = new List<string> { $"Another employee with NID '{model.NIDNo}' already exists" }
                    });
                }

                // Validate department, section, and designation exist and are active
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == model.DepartmentId && d.IsActive);
                if (department == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid department",
                        Errors = new List<string> { $"Department with ID {model.DepartmentId} not found or inactive" }
                    });
                }

                var section = await _context.Sections
                    .FirstOrDefaultAsync(s => s.Id == model.SectionId && s.IsActive && s.DepartmentId == model.DepartmentId);
                if (section == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid section",
                        Errors = new List<string> { $"Section with ID {model.SectionId} not found, inactive, or doesn't belong to the specified department" }
                    });
                }

                var designation = await _context.Designations
                    .FirstOrDefaultAsync(d => d.Id == model.DesignationId && d.IsActive && d.SectionId == model.SectionId);
                if (designation == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid designation",
                        Errors = new List<string> { $"Designation with ID {model.DesignationId} not found, inactive, or doesn't belong to the specified section" }
                    });
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Update employee properties
                employee.Name = model.Name;
                employee.NameBangla = model.NameBangla;
                employee.NIDNo = model.NIDNo;
                employee.FatherName = model.FatherName;
                employee.MotherName = model.MotherName;
                employee.FatherNameBangla = model.FatherNameBangla;
                employee.MotherNameBangla = model.MotherNameBangla;
                employee.DateOfBirth = model.DateOfBirth;
                employee.Address = model.Address;
                employee.JoiningDate = model.JoiningDate;
                employee.DepartmentId = model.DepartmentId;
                employee.SectionId = model.SectionId;
                employee.DesignationId = model.DesignationId;
                employee.GrossSalary = model.GrossSalary;
                employee.BasicSalary = model.BasicSalary;
                employee.BankAccountNo = model.BankAccountNo;
                employee.IsActive = model.IsActive;
                employee.UpdatedBy = currentUserId;
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Fetch the updated employee with related data
                var updatedEmployee = await _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Where(e => e.Id == id)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        FatherName = e.FatherName,
                        MotherName = e.MotherName,
                        FatherNameBangla = e.FatherNameBangla,
                        MotherNameBangla = e.MotherNameBangla,
                        DateOfBirth = e.DateOfBirth,
                        Address = e.Address,
                        JoiningDate = e.JoiningDate,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.Name,
                        SectionId = e.SectionId,
                        SectionName = e.Section.Name,
                        DesignationId = e.DesignationId,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        BankAccountNo = e.BankAccountNo,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt,
                        IsActive = e.IsActive,
                        CreatedBy = e.CreatedBy,
                        UpdatedBy = e.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                return Ok(new ApiResponse<EmployeeDto>
                {
                    Success = true,
                    Message = "Employee updated successfully",
                    Data = updatedEmployee!
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the employee",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Soft delete an employee (Admin and IT only)
        /// </summary>
        /// <param name="id">Employee ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,IT")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)] // Forbidden - Admin/IT role required
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee not found",
                        Errors = new List<string> { $"No employee found with ID: {id}" }
                    });
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Soft delete
                employee.IsActive = false;
                employee.UpdatedBy = currentUserId;
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Employee deleted successfully",
                    Data = new { Id = id, Name = employee.Name, DeletedAt = employee.UpdatedAt }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the employee",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get employee summary (minimal info for dropdowns, etc.)
        /// </summary>
        /// <param name="departmentId">Filter by department ID</param>
        /// <param name="sectionId">Filter by section ID</param>
        /// <param name="designationId">Filter by designation ID</param>
        /// <returns>List of employee summaries</returns>
        [HttpGet("summary")]
        [Authorize(Roles = "Admin,Manager,HR,HR Manager,IT")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<EmployeeSummaryDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)] // Forbidden - Admin/Manager/HR/IT role required
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetEmployeeSummary(
            [FromQuery] int? departmentId = null,
            [FromQuery] int? sectionId = null,
            [FromQuery] int? designationId = null)
        {
            try
            {
                var query = _context.Employees
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Where(e => e.IsActive)
                    .AsQueryable();

                if (departmentId.HasValue)
                {
                    query = query.Where(e => e.DepartmentId == departmentId.Value);
                }

                if (sectionId.HasValue)
                {
                    query = query.Where(e => e.SectionId == sectionId.Value);
                }

                if (designationId.HasValue)
                {
                    query = query.Where(e => e.DesignationId == designationId.Value);
                }

                var employees = await query
                    .OrderBy(e => e.Name)
                    .Select(e => new EmployeeSummaryDto
                    {
                        Id = e.Id,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        DepartmentName = e.Department.Name,
                        SectionName = e.Section.Name,
                        DesignationName = e.Designation.Name,
                        GrossSalary = e.GrossSalary,
                        IsActive = e.IsActive
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<EmployeeSummaryDto>>
                {
                    Success = true,
                    Message = $"Retrieved {employees.Count} employee summaries successfully",
                    Data = employees
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving employee summaries",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
