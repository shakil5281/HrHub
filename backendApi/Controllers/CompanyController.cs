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
    [Authorize]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ApplicationDbContext context, ILogger<CompanyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all companies (Admin only)
        /// </summary>
        /// <returns>List of companies</returns>
        /// <response code="200">Companies retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllCompanies([FromQuery] bool includeInactive = false)
        {
            try
            {
                var query = _context.Companies.AsQueryable();

                if (!includeInactive)
                {
                    query = query.Where(c => c.IsActive);
                }

                var companies = await query
                    .OrderBy(c => c.Name)
                    .Select(c => new CompanyListDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Industry = c.Industry,
                        City = c.City,
                        Country = c.Country,
                        EmployeeCount = c.EmployeeCount,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<CompanyListDto>>
                {
                    Success = true,
                    Message = "Companies retrieved successfully",
                    Data = companies
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving companies");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving companies",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get company by ID (Admin only)
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>Company details</returns>
        /// <response code="200">Company retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Company not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetCompanyById(int id)
        {
            try
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {id} does not exist" }
                    });
                }

                var companyDto = new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    Industry = company.Industry,
                    Website = company.Website,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    City = company.City,
                    State = company.State,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    RegistrationNumber = company.RegistrationNumber,
                    TaxId = company.TaxId,
                    EstablishedDate = company.EstablishedDate,
                    EmployeeCount = company.EmployeeCount,
                    LogoUrl = company.LogoUrl,
                    CreatedAt = company.CreatedAt,
                    UpdatedAt = company.UpdatedAt,
                    IsActive = company.IsActive,
                    CreatedBy = company.CreatedBy,
                    UpdatedBy = company.UpdatedBy
                };

                return Ok(new ApiResponse<CompanyDto>
                {
                    Success = true,
                    Message = "Company retrieved successfully",
                    Data = companyDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving company {CompanyId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the company",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new company (Admin only)
        /// </summary>
        /// <param name="createCompanyDto">Company creation data</param>
        /// <returns>Created company details</returns>
        /// <response code="201">Company created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateCompany([FromBody] CreateCompanyDto createCompanyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid model state",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                // Check if company with same name already exists
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == createCompanyDto.Name.ToLower());

                if (existingCompany != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company with this name already exists",
                        Errors = new List<string> { "A company with the same name is already registered" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                var company = new Company
                {
                    Name = createCompanyDto.Name,
                    Description = createCompanyDto.Description,
                    Industry = createCompanyDto.Industry,
                    Website = createCompanyDto.Website,
                    Phone = createCompanyDto.Phone,
                    Email = createCompanyDto.Email,
                    Address = createCompanyDto.Address,
                    City = createCompanyDto.City,
                    State = createCompanyDto.State,
                    PostalCode = createCompanyDto.PostalCode,
                    Country = createCompanyDto.Country,
                    RegistrationNumber = createCompanyDto.RegistrationNumber,
                    TaxId = createCompanyDto.TaxId,
                    EstablishedDate = createCompanyDto.EstablishedDate,
                    EmployeeCount = createCompanyDto.EmployeeCount,
                    LogoUrl = createCompanyDto.LogoUrl,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = userId
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                var companyDto = new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    Industry = company.Industry,
                    Website = company.Website,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    City = company.City,
                    State = company.State,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    RegistrationNumber = company.RegistrationNumber,
                    TaxId = company.TaxId,
                    EstablishedDate = company.EstablishedDate,
                    EmployeeCount = company.EmployeeCount,
                    LogoUrl = company.LogoUrl,
                    CreatedAt = company.CreatedAt,
                    UpdatedAt = company.UpdatedAt,
                    IsActive = company.IsActive,
                    CreatedBy = company.CreatedBy,
                    UpdatedBy = company.UpdatedBy
                };

                _logger.LogInformation("Company {CompanyName} created successfully by user {UserId}", company.Name, userId);

                return CreatedAtAction(nameof(GetCompanyById), new { id = company.Id }, new ApiResponse<CompanyDto>
                {
                    Success = true,
                    Message = "Company created successfully",
                    Data = companyDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating company");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the company",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Update an existing company (Admin only)
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <param name="updateCompanyDto">Company update data</param>
        /// <returns>Updated company details</returns>
        /// <response code="200">Company updated successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Company not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<CompanyDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] UpdateCompanyDto updateCompanyDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid model state",
                        Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList()
                    });
                }

                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {id} does not exist" }
                    });
                }

                // Check if another company with same name already exists (excluding current company)
                var existingCompany = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Name.ToLower() == updateCompanyDto.Name.ToLower() && c.Id != id);

                if (existingCompany != null)
                {
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company with this name already exists",
                        Errors = new List<string> { "Another company with the same name is already registered" }
                    });
                }

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                // Update company properties
                company.Name = updateCompanyDto.Name;
                company.Description = updateCompanyDto.Description;
                company.Industry = updateCompanyDto.Industry;
                company.Website = updateCompanyDto.Website;
                company.Phone = updateCompanyDto.Phone;
                company.Email = updateCompanyDto.Email;
                company.Address = updateCompanyDto.Address;
                company.City = updateCompanyDto.City;
                company.State = updateCompanyDto.State;
                company.PostalCode = updateCompanyDto.PostalCode;
                company.Country = updateCompanyDto.Country;
                company.RegistrationNumber = updateCompanyDto.RegistrationNumber;
                company.TaxId = updateCompanyDto.TaxId;
                company.EstablishedDate = updateCompanyDto.EstablishedDate;
                company.EmployeeCount = updateCompanyDto.EmployeeCount;
                company.LogoUrl = updateCompanyDto.LogoUrl;
                company.IsActive = updateCompanyDto.IsActive;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                var companyDto = new CompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    Description = company.Description,
                    Industry = company.Industry,
                    Website = company.Website,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    City = company.City,
                    State = company.State,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    RegistrationNumber = company.RegistrationNumber,
                    TaxId = company.TaxId,
                    EstablishedDate = company.EstablishedDate,
                    EmployeeCount = company.EmployeeCount,
                    LogoUrl = company.LogoUrl,
                    CreatedAt = company.CreatedAt,
                    UpdatedAt = company.UpdatedAt,
                    IsActive = company.IsActive,
                    CreatedBy = company.CreatedBy,
                    UpdatedBy = company.UpdatedBy
                };

                _logger.LogInformation("Company {CompanyId} updated successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<CompanyDto>
                {
                    Success = true,
                    Message = "Company updated successfully",
                    Data = companyDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating company {CompanyId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while updating the company",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a company (Admin only)
        /// </summary>
        /// <param name="id">Company ID</param>
        /// <returns>Success or error response</returns>
        /// <response code="200">Company deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Company not found</response>
        /// <response code="409">Conflict - Company has associated employees</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteCompany(int id)
        {
            try
            {
                var company = await _context.Companies
                    .Include(c => c.Employees)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (company == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Company not found",
                        Errors = new List<string> { $"Company with ID {id} does not exist" }
                    });
                }

                // Check if company has associated employees
                if (company.Employees.Any())
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Cannot delete company with associated employees",
                        Errors = new List<string> { $"Company has {company.Employees.Count} associated employees. Please reassign or remove employees before deleting the company." }
                    });
                }

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation("Company {CompanyId} deleted successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Company deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting company {CompanyId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the company",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get company statistics (Admin only)
        /// </summary>
        /// <returns>Company statistics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetCompanyStatistics()
        {
            try
            {
                var totalCompanies = await _context.Companies.CountAsync();
                var activeCompanies = await _context.Companies.CountAsync(c => c.IsActive);
                var inactiveCompanies = totalCompanies - activeCompanies;

                var industriesStats = await _context.Companies
                    .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Industry))
                    .GroupBy(c => c.Industry)
                    .Select(g => new
                    {
                        Industry = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync();

                var countriesStats = await _context.Companies
                    .Where(c => c.IsActive && !string.IsNullOrEmpty(c.Country))
                    .GroupBy(c => c.Country)
                    .Select(g => new
                    {
                        Country = g.Key,
                        Count = g.Count()
                    })
                    .OrderByDescending(x => x.Count)
                    .Take(10)
                    .ToListAsync();

                var recentCompanies = await _context.Companies
                    .Where(c => c.IsActive)
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(5)
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.Industry,
                        c.CreatedAt
                    })
                    .ToListAsync();

                var statistics = new
                {
                    Overview = new
                    {
                        TotalCompanies = totalCompanies,
                        ActiveCompanies = activeCompanies,
                        InactiveCompanies = inactiveCompanies
                    },
                    IndustryBreakdown = industriesStats,
                    CountryBreakdown = countriesStats,
                    RecentCompanies = recentCompanies
                };

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Company statistics retrieved successfully",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving company statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving company statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
