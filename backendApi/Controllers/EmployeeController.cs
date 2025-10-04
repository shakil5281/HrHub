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
                    .Include(e => e.Company)
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Line)
                    .Include(e => e.Shift)
                    .Include(e => e.Degree)
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
                        EmpId = e.EmpId,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        JoiningDate = e.JoiningDate,
                        BloodGroup = e.BloodGroup,
                        Gender = e.Gender,
                        Religion = e.Religion,
                        MaritalStatus = e.MaritalStatus,
                        CompanyName = e.Company.Name,
                        DepartmentName = e.Department.Name,
                        SectionName = e.Section.Name,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        LineName = e.Line != null ? e.Line.Name : null,
                        ShiftName = e.Shift != null ? e.Shift.Name : null,
                        Floor = e.Floor,
                        EmpType = e.EmpType,
                        Group = e.Group,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        SalaryType = e.SalaryType,
                        Bank = e.Bank,
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
                    .Include(e => e.Company)
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Line)
                    .Include(e => e.Shift)
                    .Include(e => e.Degree)
                    .Where(e => e.Id == id)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        EmpId = e.EmpId,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        FatherName = e.FatherName,
                        MotherName = e.MotherName,
                        FatherNameBangla = e.FatherNameBangla,
                        MotherNameBangla = e.MotherNameBangla,
                        DateOfBirth = e.DateOfBirth,
                        PermanentAddress = e.PermanentAddress,
                        PermanentDivision = e.PermanentDivision,
                        PermanentDistrict = e.PermanentDistrict,
                        PermanentUpazila = e.PermanentUpazila,
                        PermanentPostalCode = e.PermanentPostalCode,
                        PresentAddress = e.PresentAddress,
                        PresentDivision = e.PresentDivision,
                        PresentDistrict = e.PresentDistrict,
                        PresentUpazila = e.PresentUpazila,
                        PresentPostalCode = e.PresentPostalCode,
                        JoiningDate = e.JoiningDate,
                        BloodGroup = e.BloodGroup,
                        Gender = e.Gender,
                        Religion = e.Religion,
                        MaritalStatus = e.MaritalStatus,
                        Education = e.Education,
                        CompanyId = e.CompanyId,
                        CompanyName = e.Company.Name,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.Name,
                        SectionId = e.SectionId,
                        SectionName = e.Section.Name,
                        DesignationId = e.DesignationId,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        LineId = e.LineId,
                        LineName = e.Line != null ? e.Line.Name : null,
                        ShiftId = e.ShiftId,
                        ShiftName = e.Shift != null ? e.Shift.Name : null,
                        DegreeId = e.DegreeId,
                        DegreeName = e.Degree != null ? e.Degree.Name : null,
                        Floor = e.Floor,
                        EmpType = e.EmpType,
                        Group = e.Group,
                        House = e.House,
                        RentMedical = e.RentMedical,
                        Food = e.Food,
                        Conveyance = e.Conveyance,
                        Transport = e.Transport,
                        NightBill = e.NightBill,
                        MobileBill = e.MobileBill,
                        OtherAllowance = e.OtherAllowance,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        SalaryType = e.SalaryType,
                        BankAccountNo = e.BankAccountNo,
                        Bank = e.Bank,
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
                // Check if EmpId already exists
                var existingEmployeeByEmpId = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmpId == model.EmpId);

                if (existingEmployeeByEmpId != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee with this EmpId already exists",
                        Errors = new List<string> { $"An employee with EmpId '{model.EmpId}' already exists" }
                    });
                }

                // Check if NID already exists
                var existingEmployeeByNID = await _context.Employees
                    .FirstOrDefaultAsync(e => e.NIDNo == model.NIDNo);

                if (existingEmployeeByNID != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee with this NID already exists",
                        Errors = new List<string> { $"An employee with NID '{model.NIDNo}' already exists" }
                    });
                }

                // Validate company exists and is active
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == model.CompanyId && c.IsActive);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {model.CompanyId} not found or inactive" }
                    });
                }

                // Validate department, section, and designation exist and are active
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == model.DepartmentId && d.IsActive && d.CompanyId == model.CompanyId);
                if (department == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid department",
                        Errors = new List<string> { $"Department with ID {model.DepartmentId} not found, inactive, or doesn't belong to the specified company" }
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

                // Validate optional foreign keys if provided
                if (model.LineId.HasValue)
                {
                    var line = await _context.Lines
                        .FirstOrDefaultAsync(l => l.Id == model.LineId.Value && l.IsActive && l.CompanyId == model.CompanyId);
                    if (line == null)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid line",
                            Errors = new List<string> { $"Line with ID {model.LineId.Value} not found, inactive, or doesn't belong to the specified company" }
                        });
                    }
                }

                if (model.ShiftId.HasValue)
                {
                    var shift = await _context.Shifts
                        .FirstOrDefaultAsync(s => s.Id == model.ShiftId.Value && s.IsActive && s.CompanyId == model.CompanyId);
                    if (shift == null)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid shift",
                            Errors = new List<string> { $"Shift with ID {model.ShiftId.Value} not found, inactive, or doesn't belong to the specified company" }
                        });
                    }
                }

                if (model.DegreeId.HasValue)
                {
                    var degree = await _context.Degrees
                        .FirstOrDefaultAsync(d => d.Id == model.DegreeId.Value && d.IsActive && d.CompanyId == model.CompanyId);
                    if (degree == null)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid degree",
                            Errors = new List<string> { $"Degree with ID {model.DegreeId.Value} not found, inactive, or doesn't belong to the specified company" }
                        });
                    }
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var employee = new Employee
                {
                    EmpId = model.EmpId,
                    Name = model.Name,
                    NameBangla = model.NameBangla,
                    NIDNo = model.NIDNo,
                    FatherName = model.FatherName,
                    MotherName = model.MotherName,
                    FatherNameBangla = model.FatherNameBangla,
                    MotherNameBangla = model.MotherNameBangla,
                    DateOfBirth = model.DateOfBirth,
                    PermanentAddress = model.PermanentAddress,
                    PermanentDivision = model.PermanentDivision,
                    PermanentDistrict = model.PermanentDistrict,
                    PermanentUpazila = model.PermanentUpazila,
                    PermanentPostalCode = model.PermanentPostalCode,
                    PresentAddress = model.PresentAddress,
                    PresentDivision = model.PresentDivision,
                    PresentDistrict = model.PresentDistrict,
                    PresentUpazila = model.PresentUpazila,
                    PresentPostalCode = model.PresentPostalCode,
                    JoiningDate = model.JoiningDate,
                    BloodGroup = model.BloodGroup,
                    Gender = model.Gender,
                    Religion = model.Religion,
                    MaritalStatus = model.MaritalStatus,
                    Education = model.Education,
                    CompanyId = model.CompanyId,
                    DepartmentId = model.DepartmentId,
                    SectionId = model.SectionId,
                    DesignationId = model.DesignationId,
                    LineId = model.LineId,
                    ShiftId = model.ShiftId,
                    DegreeId = model.DegreeId,
                    Floor = model.Floor,
                    EmpType = model.EmpType,
                    Group = model.Group,
                    House = model.House,
                    RentMedical = model.RentMedical,
                    Food = model.Food,
                    Conveyance = model.Conveyance,
                    Transport = model.Transport,
                    NightBill = model.NightBill,
                    MobileBill = model.MobileBill,
                    OtherAllowance = model.OtherAllowance,
                    GrossSalary = model.GrossSalary,
                    BasicSalary = model.BasicSalary,
                    SalaryType = model.SalaryType,
                    BankAccountNo = model.BankAccountNo,
                    Bank = model.Bank,
                    CreatedBy = currentUserId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                // Fetch the created employee with related data
                var createdEmployee = await _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Line)
                    .Include(e => e.Shift)
                    .Include(e => e.Degree)
                    .Where(e => e.Id == employee.Id)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        EmpId = e.EmpId,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        FatherName = e.FatherName,
                        MotherName = e.MotherName,
                        FatherNameBangla = e.FatherNameBangla,
                        MotherNameBangla = e.MotherNameBangla,
                        DateOfBirth = e.DateOfBirth,
                        PermanentAddress = e.PermanentAddress,
                        PermanentDivision = e.PermanentDivision,
                        PermanentDistrict = e.PermanentDistrict,
                        PermanentUpazila = e.PermanentUpazila,
                        PermanentPostalCode = e.PermanentPostalCode,
                        PresentAddress = e.PresentAddress,
                        PresentDivision = e.PresentDivision,
                        PresentDistrict = e.PresentDistrict,
                        PresentUpazila = e.PresentUpazila,
                        PresentPostalCode = e.PresentPostalCode,
                        JoiningDate = e.JoiningDate,
                        BloodGroup = e.BloodGroup,
                        Gender = e.Gender,
                        Religion = e.Religion,
                        MaritalStatus = e.MaritalStatus,
                        Education = e.Education,
                        CompanyId = e.CompanyId,
                        CompanyName = e.Company.Name,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.Name,
                        SectionId = e.SectionId,
                        SectionName = e.Section.Name,
                        DesignationId = e.DesignationId,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        LineId = e.LineId,
                        LineName = e.Line != null ? e.Line.Name : null,
                        ShiftId = e.ShiftId,
                        ShiftName = e.Shift != null ? e.Shift.Name : null,
                        DegreeId = e.DegreeId,
                        DegreeName = e.Degree != null ? e.Degree.Name : null,
                        Floor = e.Floor,
                        EmpType = e.EmpType,
                        Group = e.Group,
                        House = e.House,
                        RentMedical = e.RentMedical,
                        Food = e.Food,
                        Conveyance = e.Conveyance,
                        Transport = e.Transport,
                        NightBill = e.NightBill,
                        MobileBill = e.MobileBill,
                        OtherAllowance = e.OtherAllowance,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        SalaryType = e.SalaryType,
                        BankAccountNo = e.BankAccountNo,
                        Bank = e.Bank,
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

                // Check if EmpId already exists for another employee
                var existingEmployeeByEmpId = await _context.Employees
                    .FirstOrDefaultAsync(e => e.EmpId == model.EmpId && e.Id != id);

                if (existingEmployeeByEmpId != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee with this EmpId already exists",
                        Errors = new List<string> { $"Another employee with EmpId '{model.EmpId}' already exists" }
                    });
                }

                // Check if NID already exists for another employee
                var existingEmployeeByNID = await _context.Employees
                    .FirstOrDefaultAsync(e => e.NIDNo == model.NIDNo && e.Id != id);

                if (existingEmployeeByNID != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Employee with this NID already exists",
                        Errors = new List<string> { $"Another employee with NID '{model.NIDNo}' already exists" }
                    });
                }

                // Validate company exists and is active
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == model.CompanyId && c.IsActive);
                if (company == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid company",
                        Errors = new List<string> { $"Company with ID {model.CompanyId} not found or inactive" }
                    });
                }

                // Validate department, section, and designation exist and are active
                var department = await _context.Departments
                    .FirstOrDefaultAsync(d => d.Id == model.DepartmentId && d.IsActive && d.CompanyId == model.CompanyId);
                if (department == null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid department",
                        Errors = new List<string> { $"Department with ID {model.DepartmentId} not found, inactive, or doesn't belong to the specified company" }
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

                // Validate optional foreign keys if provided
                if (model.LineId.HasValue)
                {
                    var line = await _context.Lines
                        .FirstOrDefaultAsync(l => l.Id == model.LineId.Value && l.IsActive && l.CompanyId == model.CompanyId);
                    if (line == null)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid line",
                            Errors = new List<string> { $"Line with ID {model.LineId.Value} not found, inactive, or doesn't belong to the specified company" }
                        });
                    }
                }

                if (model.ShiftId.HasValue)
                {
                    var shift = await _context.Shifts
                        .FirstOrDefaultAsync(s => s.Id == model.ShiftId.Value && s.IsActive && s.CompanyId == model.CompanyId);
                    if (shift == null)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid shift",
                            Errors = new List<string> { $"Shift with ID {model.ShiftId.Value} not found, inactive, or doesn't belong to the specified company" }
                        });
                    }
                }

                if (model.DegreeId.HasValue)
                {
                    var degree = await _context.Degrees
                        .FirstOrDefaultAsync(d => d.Id == model.DegreeId.Value && d.IsActive && d.CompanyId == model.CompanyId);
                    if (degree == null)
                    {
                        return BadRequest(new ApiResponse<object>
                        {
                            Success = false,
                            Message = "Invalid degree",
                            Errors = new List<string> { $"Degree with ID {model.DegreeId.Value} not found, inactive, or doesn't belong to the specified company" }
                        });
                    }
                }

                var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Update employee properties
                employee.EmpId = model.EmpId;
                employee.Name = model.Name;
                employee.NameBangla = model.NameBangla;
                employee.NIDNo = model.NIDNo;
                employee.FatherName = model.FatherName;
                employee.MotherName = model.MotherName;
                employee.FatherNameBangla = model.FatherNameBangla;
                employee.MotherNameBangla = model.MotherNameBangla;
                employee.DateOfBirth = model.DateOfBirth;
                employee.PermanentAddress = model.PermanentAddress;
                employee.PermanentDivision = model.PermanentDivision;
                employee.PermanentDistrict = model.PermanentDistrict;
                employee.PermanentUpazila = model.PermanentUpazila;
                employee.PermanentPostalCode = model.PermanentPostalCode;
                employee.PresentAddress = model.PresentAddress;
                employee.PresentDivision = model.PresentDivision;
                employee.PresentDistrict = model.PresentDistrict;
                employee.PresentUpazila = model.PresentUpazila;
                employee.PresentPostalCode = model.PresentPostalCode;
                employee.JoiningDate = model.JoiningDate;
                employee.BloodGroup = model.BloodGroup;
                employee.Gender = model.Gender;
                employee.Religion = model.Religion;
                employee.MaritalStatus = model.MaritalStatus;
                employee.Education = model.Education;
                employee.CompanyId = model.CompanyId;
                employee.DepartmentId = model.DepartmentId;
                employee.SectionId = model.SectionId;
                employee.DesignationId = model.DesignationId;
                employee.LineId = model.LineId;
                employee.ShiftId = model.ShiftId;
                employee.DegreeId = model.DegreeId;
                employee.Floor = model.Floor;
                employee.EmpType = model.EmpType;
                employee.Group = model.Group;
                employee.House = model.House;
                employee.RentMedical = model.RentMedical;
                employee.Food = model.Food;
                employee.Conveyance = model.Conveyance;
                employee.Transport = model.Transport;
                employee.NightBill = model.NightBill;
                employee.MobileBill = model.MobileBill;
                employee.OtherAllowance = model.OtherAllowance;
                employee.GrossSalary = model.GrossSalary;
                employee.BasicSalary = model.BasicSalary;
                employee.SalaryType = model.SalaryType;
                employee.BankAccountNo = model.BankAccountNo;
                employee.Bank = model.Bank;
                employee.IsActive = model.IsActive;
                employee.UpdatedBy = currentUserId;
                employee.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                // Fetch the updated employee with related data
                var updatedEmployee = await _context.Employees
                    .Include(e => e.Company)
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Line)
                    .Include(e => e.Shift)
                    .Include(e => e.Degree)
                    .Where(e => e.Id == id)
                    .Select(e => new EmployeeDto
                    {
                        Id = e.Id,
                        EmpId = e.EmpId,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        NIDNo = e.NIDNo,
                        FatherName = e.FatherName,
                        MotherName = e.MotherName,
                        FatherNameBangla = e.FatherNameBangla,
                        MotherNameBangla = e.MotherNameBangla,
                        DateOfBirth = e.DateOfBirth,
                        PermanentAddress = e.PermanentAddress,
                        PermanentDivision = e.PermanentDivision,
                        PermanentDistrict = e.PermanentDistrict,
                        PermanentUpazila = e.PermanentUpazila,
                        PermanentPostalCode = e.PermanentPostalCode,
                        PresentAddress = e.PresentAddress,
                        PresentDivision = e.PresentDivision,
                        PresentDistrict = e.PresentDistrict,
                        PresentUpazila = e.PresentUpazila,
                        PresentPostalCode = e.PresentPostalCode,
                        JoiningDate = e.JoiningDate,
                        BloodGroup = e.BloodGroup,
                        Gender = e.Gender,
                        Religion = e.Religion,
                        MaritalStatus = e.MaritalStatus,
                        Education = e.Education,
                        CompanyId = e.CompanyId,
                        CompanyName = e.Company.Name,
                        DepartmentId = e.DepartmentId,
                        DepartmentName = e.Department.Name,
                        SectionId = e.SectionId,
                        SectionName = e.Section.Name,
                        DesignationId = e.DesignationId,
                        DesignationName = e.Designation.Name,
                        DesignationGrade = e.Designation.Grade,
                        LineId = e.LineId,
                        LineName = e.Line != null ? e.Line.Name : null,
                        ShiftId = e.ShiftId,
                        ShiftName = e.Shift != null ? e.Shift.Name : null,
                        DegreeId = e.DegreeId,
                        DegreeName = e.Degree != null ? e.Degree.Name : null,
                        Floor = e.Floor,
                        EmpType = e.EmpType,
                        Group = e.Group,
                        House = e.House,
                        RentMedical = e.RentMedical,
                        Food = e.Food,
                        Conveyance = e.Conveyance,
                        Transport = e.Transport,
                        NightBill = e.NightBill,
                        MobileBill = e.MobileBill,
                        OtherAllowance = e.OtherAllowance,
                        GrossSalary = e.GrossSalary,
                        BasicSalary = e.BasicSalary,
                        SalaryType = e.SalaryType,
                        BankAccountNo = e.BankAccountNo,
                        Bank = e.Bank,
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
                    .Include(e => e.Company)
                    .Include(e => e.Department)
                    .Include(e => e.Section)
                    .Include(e => e.Designation)
                    .Include(e => e.Line)
                    .Include(e => e.Shift)
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
                        EmpId = e.EmpId,
                        Name = e.Name,
                        NameBangla = e.NameBangla,
                        CompanyName = e.Company.Name,
                        DepartmentName = e.Department.Name,
                        SectionName = e.Section.Name,
                        DesignationName = e.Designation.Name,
                        LineName = e.Line != null ? e.Line.Name : null,
                        ShiftName = e.Shift != null ? e.Shift.Name : null,
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
