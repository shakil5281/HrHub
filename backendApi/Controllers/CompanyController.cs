using HrHubAPI.Data;
using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            ILogger<CompanyController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Get companies based on user role (Admin gets all companies, others get their assigned companies)
        /// </summary>
        /// <param name="includeInactive">Include inactive companies (Admin only)</param>
        /// <returns>List of companies based on user role</returns>
        /// <response code="200">Companies retrieved successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize] // Allow all authenticated users
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CompanyListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllCompanies([FromQuery] bool includeInactive = false)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Invalid token",
                        Errors = new List<string> { "User ID not found in token" }
                    });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Unauthorized(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "User not found",
                        Errors = new List<string> { "User does not exist" }
                    });
                }

                var userRoles = await _userManager.GetRolesAsync(user);
                var isAdmin = userRoles.Contains("Admin");

                List<CompanyListDto> companies;

                if (isAdmin)
                {
                    // Admin users can see all companies
                    var query = _context.Companies.AsQueryable();

                    if (!includeInactive)
                    {
                        query = query.Where(c => c.IsActive);
                    }

                    companies = await query
                        .OrderBy(c => c.Name)
                        .Select(c => new CompanyListDto
                        {
                            Id = c.Id,
                            CompanyId = c.CompanyId,
                            Name = c.Name,
                            CompanyNameBangla = c.CompanyNameBangla,
                            City = c.City,
                            Country = c.Country,
                            IsActive = c.IsActive,
                            CreatedAt = c.CreatedAt
                        })
                        .ToListAsync();

                    _logger.LogInformation("Admin user {UserId} retrieved all companies", userId);
                }
                else
                {
                    // Non-admin users can only see their assigned companies
                    var userCompanyIds = new List<int>();

                    // Get primary company (from CompanyId field)
                    if (user.CompanyId.HasValue)
                    {
                        userCompanyIds.Add(user.CompanyId.Value);
                    }

                    // Get additional companies from UserCompany relationship
                    var additionalCompanies = await _context.UserCompanies
                        .Where(uc => uc.UserId == userId && uc.IsActive)
                        .Select(uc => uc.CompanyId)
                        .ToListAsync();

                    userCompanyIds.AddRange(additionalCompanies);
                    userCompanyIds = userCompanyIds.Distinct().ToList();

                    if (!userCompanyIds.Any())
                    {
                        // User has no company assigned
                        companies = new List<CompanyListDto>();
                    }
                    else
                    {
                        var query = _context.Companies
                            .Where(c => userCompanyIds.Contains(c.Id) && c.IsActive);

                        companies = await query
                            .OrderBy(c => c.Name)
                            .Select(c => new CompanyListDto
                            {
                                Id = c.Id,
                                CompanyId = c.CompanyId,
                                Name = c.Name,
                                CompanyNameBangla = c.CompanyNameBangla,
                                City = c.City,
                                Country = c.Country,
                                IsActive = c.IsActive,
                                CreatedAt = c.CreatedAt
                            })
                            .ToListAsync();
                    }

                    _logger.LogInformation("User {UserId} retrieved {Count} assigned companies", userId, companies.Count);
                }

                return Ok(new ApiResponse<IEnumerable<CompanyListDto>>
                {
                    Success = true,
                    Message = isAdmin ? "All companies retrieved successfully" : "Assigned companies retrieved successfully",
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
                    CompanyId = company.CompanyId,
                    Name = company.Name,
                    CompanyNameBangla = company.CompanyNameBangla,
                    Description = company.Description,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    AddressBangla = company.AddressBangla,
                    City = company.City,
                    State = company.State,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    LogoUrl = company.LogoUrl,
                    AuthorizedSignature = company.AuthorizedSignature,
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
                    CompanyId = createCompanyDto.CompanyId,
                    Name = createCompanyDto.Name,
                    CompanyNameBangla = createCompanyDto.CompanyNameBangla,
                    Description = createCompanyDto.Description,
                    Phone = createCompanyDto.Phone,
                    Email = createCompanyDto.Email,
                    Address = createCompanyDto.Address,
                    AddressBangla = createCompanyDto.AddressBangla,
                    City = createCompanyDto.City,
                    State = createCompanyDto.State,
                    PostalCode = createCompanyDto.PostalCode,
                    Country = createCompanyDto.Country,
                    LogoUrl = createCompanyDto.LogoUrl,
                    AuthorizedSignature = createCompanyDto.AuthorizedSignature,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    CreatedBy = userId
                };

                _context.Companies.Add(company);
                await _context.SaveChangesAsync();

                var companyDto = new CompanyDto
                {
                    Id = company.Id,
                    CompanyId = company.CompanyId,
                    Name = company.Name,
                    CompanyNameBangla = company.CompanyNameBangla,
                    Description = company.Description,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    AddressBangla = company.AddressBangla,
                    City = company.City,
                    State = company.State,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    LogoUrl = company.LogoUrl,
                    AuthorizedSignature = company.AuthorizedSignature,
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
                company.CompanyId = updateCompanyDto.CompanyId;
                company.Name = updateCompanyDto.Name;
                company.CompanyNameBangla = updateCompanyDto.CompanyNameBangla;
                company.Description = updateCompanyDto.Description;
                company.Phone = updateCompanyDto.Phone;
                company.Email = updateCompanyDto.Email;
                company.Address = updateCompanyDto.Address;
                company.AddressBangla = updateCompanyDto.AddressBangla;
                company.City = updateCompanyDto.City;
                company.State = updateCompanyDto.State;
                company.PostalCode = updateCompanyDto.PostalCode;
                company.Country = updateCompanyDto.Country;
                company.LogoUrl = updateCompanyDto.LogoUrl;
                company.AuthorizedSignature = updateCompanyDto.AuthorizedSignature;
                company.IsActive = updateCompanyDto.IsActive;
                company.UpdatedAt = DateTime.UtcNow;
                company.UpdatedBy = userId;

                await _context.SaveChangesAsync();

                var companyDto = new CompanyDto
                {
                    Id = company.Id,
                    CompanyId = company.CompanyId,
                    Name = company.Name,
                    CompanyNameBangla = company.CompanyNameBangla,
                    Description = company.Description,
                    Phone = company.Phone,
                    Email = company.Email,
                    Address = company.Address,
                    AddressBangla = company.AddressBangla,
                    City = company.City,
                    State = company.State,
                    PostalCode = company.PostalCode,
                    Country = company.Country,
                    LogoUrl = company.LogoUrl,
                    AuthorizedSignature = company.AuthorizedSignature,
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

                // Industry stats removed as Industry field was removed from Company model

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
                        c.City,
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
