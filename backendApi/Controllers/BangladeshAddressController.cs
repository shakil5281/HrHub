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
    public class BangladeshAddressController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<BangladeshAddressController> _logger;

        public BangladeshAddressController(
            ApplicationDbContext context,
            ILogger<BangladeshAddressController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all Bangladesh addresses with optional filtering
        /// </summary>
        /// <param name="division">Filter by division name</param>
        /// <param name="district">Filter by district name</param>
        /// <param name="upazila">Filter by upazila name</param>
        /// <param name="union">Filter by union name</param>
        /// <param name="postalCode">Filter by postal code</param>
        /// <param name="area">Filter by area name</param>
        /// <param name="isActive">Filter by active status</param>
        /// <param name="page">Page number for pagination</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>List of Bangladesh addresses</returns>
        /// <response code="200">Addresses retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<BangladeshAddressListDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAllAddresses(
            [FromQuery] string? division = null,
            [FromQuery] string? district = null,
            [FromQuery] string? upazila = null,
            [FromQuery] string? union = null,
            [FromQuery] string? postalCode = null,
            [FromQuery] string? area = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var query = _context.BangladeshAddresses.AsQueryable();

                // Apply filters
                if (!string.IsNullOrEmpty(division))
                    query = query.Where(a => a.Division.Contains(division) || 
                                           (a.DivisionBangla != null && a.DivisionBangla.Contains(division)));

                if (!string.IsNullOrEmpty(district))
                    query = query.Where(a => a.District.Contains(district) || 
                                           (a.DistrictBangla != null && a.DistrictBangla.Contains(district)));

                if (!string.IsNullOrEmpty(upazila))
                    query = query.Where(a => a.Upazila != null && (a.Upazila.Contains(upazila) || 
                                           (a.UpazilaBangla != null && a.UpazilaBangla.Contains(upazila))));

                if (!string.IsNullOrEmpty(union))
                    query = query.Where(a => a.Union != null && (a.Union.Contains(union) || 
                                           (a.UnionBangla != null && a.UnionBangla.Contains(union))));

                if (!string.IsNullOrEmpty(postalCode))
                    query = query.Where(a => a.PostalCode.Contains(postalCode));

                if (!string.IsNullOrEmpty(area))
                    query = query.Where(a => a.Area != null && (a.Area.Contains(area) || 
                                           (a.AreaBangla != null && a.AreaBangla.Contains(area))));

                if (isActive.HasValue)
                    query = query.Where(a => a.IsActive == isActive.Value);

                // Apply pagination
                var totalCount = await query.CountAsync();
                var addresses = await query
                    .OrderBy(a => a.Division)
                    .ThenBy(a => a.District)
                    .ThenBy(a => a.Upazila)
                    .ThenBy(a => a.PostalCode)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(a => new BangladeshAddressListDto
                    {
                        Id = a.Id,
                        Division = a.Division,
                        DivisionBangla = a.DivisionBangla,
                        District = a.District,
                        DistrictBangla = a.DistrictBangla,
                        PostalCode = a.PostalCode,
                        Upazila = a.Upazila,
                        UpazilaBangla = a.UpazilaBangla,
                        Union = a.Union,
                        UnionBangla = a.UnionBangla,
                        Area = a.Area,
                        AreaBangla = a.AreaBangla,
                        IsActive = a.IsActive
                    })
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} Bangladesh addresses with filters", addresses.Count);

                return Ok(new ApiResponse<IEnumerable<BangladeshAddressListDto>>
                {
                    Success = true,
                    Message = $"Retrieved {addresses.Count} addresses successfully",
                    Data = addresses,
                    Meta = new { TotalCount = totalCount, Page = page, PageSize = pageSize, TotalPages = (int)Math.Ceiling((double)totalCount / pageSize) }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving Bangladesh addresses");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving addresses",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get Bangladesh address by ID
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <returns>Address details</returns>
        /// <response code="200">Address retrieved successfully</response>
        /// <response code="404">Address not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<BangladeshAddressDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAddressById(int id)
        {
            try
            {
                var address = await _context.BangladeshAddresses
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (address == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Address not found",
                        Errors = new List<string> { $"Address with ID {id} does not exist" }
                    });
                }

                var addressDto = new BangladeshAddressDto
                {
                    Id = address.Id,
                    Division = address.Division,
                    DivisionBangla = address.DivisionBangla,
                    District = address.District,
                    DistrictBangla = address.DistrictBangla,
                    PostalCode = address.PostalCode,
                    Upazila = address.Upazila,
                    UpazilaBangla = address.UpazilaBangla,
                    Union = address.Union,
                    UnionBangla = address.UnionBangla,
                    Area = address.Area,
                    AreaBangla = address.AreaBangla,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                    IsActive = address.IsActive,
                    CreatedAt = address.CreatedAt,
                    UpdatedAt = address.UpdatedAt,
                    CreatedBy = address.CreatedBy,
                    UpdatedBy = address.UpdatedBy
                };

                return Ok(new ApiResponse<BangladeshAddressDto>
                {
                    Success = true,
                    Message = "Address retrieved successfully",
                    Data = addressDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving address with ID {AddressId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving the address",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Search addresses by postal code
        /// </summary>
        /// <param name="postalCode">Postal code to search</param>
        /// <returns>Address details for the postal code</returns>
        /// <response code="200">Address found successfully</response>
        /// <response code="404">Address not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("postal-code/{postalCode}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<PostalCodeSearchDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetAddressByPostalCode(string postalCode)
        {
            try
            {
                var address = await _context.BangladeshAddresses
                    .FirstOrDefaultAsync(a => a.PostalCode == postalCode && a.IsActive);

                if (address == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Address not found",
                        Errors = new List<string> { $"No address found for postal code {postalCode}" }
                    });
                }

                var addressDto = new PostalCodeSearchDto
                {
                    PostalCode = address.PostalCode,
                    FullAddress = address.FullAddress,
                    FullAddressBangla = address.FullAddressBangla,
                    Division = address.Division,
                    DivisionBangla = address.DivisionBangla,
                    District = address.District,
                    DistrictBangla = address.DistrictBangla,
                    Upazila = address.Upazila,
                    UpazilaBangla = address.UpazilaBangla,
                    Union = address.Union,
                    UnionBangla = address.UnionBangla,
                    Area = address.Area,
                    AreaBangla = address.AreaBangla
                };

                return Ok(new ApiResponse<PostalCodeSearchDto>
                {
                    Success = true,
                    Message = "Address found successfully",
                    Data = addressDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching address by postal code {PostalCode}", postalCode);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while searching the address",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get all divisions
        /// </summary>
        /// <returns>List of divisions with statistics</returns>
        /// <response code="200">Divisions retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("divisions")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DivisionDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDivisions()
        {
            try
            {
                var divisions = await _context.BangladeshAddresses
                    .Where(a => a.IsActive)
                    .GroupBy(a => new { a.Division, a.DivisionBangla })
                    .Select(g => new DivisionDto
                    {
                        Name = g.Key.Division,
                        NameBangla = g.Key.DivisionBangla,
                        DistrictCount = g.Select(a => a.District).Distinct().Count(),
                        AddressCount = g.Count()
                    })
                    .OrderBy(d => d.Name)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<DivisionDto>>
                {
                    Success = true,
                    Message = $"Retrieved {divisions.Count} divisions successfully",
                    Data = divisions
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving divisions");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving divisions",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get districts by division
        /// </summary>
        /// <param name="division">Division name</param>
        /// <returns>List of districts in the division</returns>
        /// <response code="200">Districts retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("districts")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DistrictDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetDistricts([FromQuery] string? division = null)
        {
            try
            {
                var query = _context.BangladeshAddresses
                    .Where(a => a.IsActive);

                if (!string.IsNullOrEmpty(division))
                {
                    query = query.Where(a => a.Division == division || 
                                           (a.DivisionBangla != null && a.DivisionBangla == division));
                }

                var districts = await query
                    .GroupBy(a => new { a.District, a.DistrictBangla, a.Division, a.DivisionBangla })
                    .Select(g => new DistrictDto
                    {
                        Name = g.Key.District,
                        NameBangla = g.Key.DistrictBangla,
                        Division = g.Key.Division,
                        DivisionBangla = g.Key.DivisionBangla,
                        UpazilaCount = g.Select(a => a.Upazila).Where(u => u != null).Distinct().Count(),
                        AddressCount = g.Count()
                    })
                    .OrderBy(d => d.Division)
                    .ThenBy(d => d.Name)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<DistrictDto>>
                {
                    Success = true,
                    Message = $"Retrieved {districts.Count} districts successfully",
                    Data = districts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving districts");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving districts",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get upazilas by district
        /// </summary>
        /// <param name="district">District name</param>
        /// <returns>List of upazilas in the district</returns>
        /// <response code="200">Upazilas retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("upazilas")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UpazilaDto>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetUpazilas([FromQuery] string? district = null)
        {
            try
            {
                var query = _context.BangladeshAddresses
                    .Where(a => a.IsActive && a.Upazila != null);

                if (!string.IsNullOrEmpty(district))
                {
                    query = query.Where(a => a.District == district || 
                                           (a.DistrictBangla != null && a.DistrictBangla == district));
                }

                var upazilas = await query
                    .GroupBy(a => new { a.Upazila, a.UpazilaBangla, a.District, a.DistrictBangla, a.Division, a.DivisionBangla })
                    .Select(g => new UpazilaDto
                    {
                        Name = g.Key.Upazila!,
                        NameBangla = g.Key.UpazilaBangla,
                        District = g.Key.District,
                        DistrictBangla = g.Key.DistrictBangla,
                        Division = g.Key.Division,
                        DivisionBangla = g.Key.DivisionBangla,
                        UnionCount = g.Select(a => a.Union).Where(u => u != null).Distinct().Count(),
                        AddressCount = g.Count()
                    })
                    .OrderBy(u => u.Division)
                    .ThenBy(u => u.District)
                    .ThenBy(u => u.Name)
                    .ToListAsync();

                return Ok(new ApiResponse<IEnumerable<UpazilaDto>>
                {
                    Success = true,
                    Message = $"Retrieved {upazilas.Count} upazilas successfully",
                    Data = upazilas
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving upazilas");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving upazilas",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Get address statistics
        /// </summary>
        /// <returns>Address statistics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("statistics")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AddressStatisticsDto>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var totalAddresses = await _context.BangladeshAddresses.CountAsync();
                var totalDivisions = await _context.BangladeshAddresses.Select(a => a.Division).Distinct().CountAsync();
                var totalDistricts = await _context.BangladeshAddresses.Select(a => a.District).Distinct().CountAsync();
                var totalUpazilas = await _context.BangladeshAddresses.Where(a => a.Upazila != null).Select(a => a.Upazila).Distinct().CountAsync();
                var totalUnions = await _context.BangladeshAddresses.Where(a => a.Union != null).Select(a => a.Union).Distinct().CountAsync();
                var totalPostalCodes = await _context.BangladeshAddresses.Select(a => a.PostalCode).Distinct().CountAsync();
                var addressesWithCoordinates = await _context.BangladeshAddresses.Where(a => a.Latitude.HasValue && a.Longitude.HasValue).CountAsync();

                var divisionCounts = await _context.BangladeshAddresses
                    .GroupBy(a => a.Division)
                    .Select(g => new { Division = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Division, x => x.Count);

                var districtCounts = await _context.BangladeshAddresses
                    .GroupBy(a => a.District)
                    .Select(g => new { District = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.District, x => x.Count);

                var statistics = new AddressStatisticsDto
                {
                    TotalAddresses = totalAddresses,
                    TotalDivisions = totalDivisions,
                    TotalDistricts = totalDistricts,
                    TotalUpazilas = totalUpazilas,
                    TotalUnions = totalUnions,
                    TotalPostalCodes = totalPostalCodes,
                    AddressesWithCoordinates = addressesWithCoordinates,
                    DivisionCounts = divisionCounts,
                    DistrictCounts = districtCounts
                };

                return Ok(new ApiResponse<AddressStatisticsDto>
                {
                    Success = true,
                    Message = "Statistics retrieved successfully",
                    Data = statistics
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving statistics");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while retrieving statistics",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Create a new Bangladesh address (Admin only)
        /// </summary>
        /// <param name="createDto">Address creation data</param>
        /// <returns>Created address details</returns>
        /// <response code="201">Address created successfully</response>
        /// <response code="400">Invalid input data</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="409">Conflict - Postal code already exists</response>
        /// <response code="500">Internal server error</response>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<BangladeshAddressDto>), 201)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 409)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> CreateAddress([FromBody] CreateBangladeshAddressDto createDto)
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

                // Check if postal code already exists
                var existingAddress = await _context.BangladeshAddresses
                    .FirstOrDefaultAsync(a => a.PostalCode == createDto.PostalCode);

                if (existingAddress != null)
                {
                    return Conflict(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Postal code already exists",
                        Errors = new List<string> { $"An address with postal code {createDto.PostalCode} already exists" }
                    });
                }

                var address = new BangladeshAddress
                {
                    Division = createDto.Division,
                    DivisionBangla = createDto.DivisionBangla,
                    District = createDto.District,
                    DistrictBangla = createDto.DistrictBangla,
                    PostalCode = createDto.PostalCode,
                    Upazila = createDto.Upazila,
                    UpazilaBangla = createDto.UpazilaBangla,
                    Union = createDto.Union,
                    UnionBangla = createDto.UnionBangla,
                    Area = createDto.Area,
                    AreaBangla = createDto.AreaBangla,
                    Latitude = createDto.Latitude,
                    Longitude = createDto.Longitude,
                    CreatedBy = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.BangladeshAddresses.Add(address);
                await _context.SaveChangesAsync();

                var addressDto = new BangladeshAddressDto
                {
                    Id = address.Id,
                    Division = address.Division,
                    DivisionBangla = address.DivisionBangla,
                    District = address.District,
                    DistrictBangla = address.DistrictBangla,
                    PostalCode = address.PostalCode,
                    Upazila = address.Upazila,
                    UpazilaBangla = address.UpazilaBangla,
                    Union = address.Union,
                    UnionBangla = address.UnionBangla,
                    Area = address.Area,
                    AreaBangla = address.AreaBangla,
                    Latitude = address.Latitude,
                    Longitude = address.Longitude,
                    IsActive = address.IsActive,
                    CreatedAt = address.CreatedAt,
                    UpdatedAt = address.UpdatedAt,
                    CreatedBy = address.CreatedBy,
                    UpdatedBy = address.UpdatedBy
                };

                _logger.LogInformation("Bangladesh address created successfully by user {UserId} with postal code {PostalCode}", 
                    userId, createDto.PostalCode);

                return CreatedAtAction(nameof(GetAddressById), new { id = address.Id }, new ApiResponse<BangladeshAddressDto>
                {
                    Success = true,
                    Message = "Address created successfully",
                    Data = addressDto
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating Bangladesh address");
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while creating the address",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        /// <summary>
        /// Delete a Bangladesh address (Admin only)
        /// </summary>
        /// <param name="id">Address ID</param>
        /// <returns>Success message</returns>
        /// <response code="200">Address deleted successfully</response>
        /// <response code="401">Unauthorized - Valid JWT token required</response>
        /// <response code="403">Forbidden - Admin role required</response>
        /// <response code="404">Address not found</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        [ProducesResponseType(typeof(ApiResponse<object>), 403)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        [ProducesResponseType(typeof(ApiResponse<object>), 500)]
        public async Task<IActionResult> DeleteAddress(int id)
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

                var address = await _context.BangladeshAddresses.FindAsync(id);
                if (address == null)
                {
                    return NotFound(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Address not found",
                        Errors = new List<string> { $"Address with ID {id} does not exist" }
                    });
                }

                _context.BangladeshAddresses.Remove(address);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Bangladesh address {AddressId} deleted successfully by user {UserId}", id, userId);

                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Address deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting Bangladesh address with ID {AddressId}", id);
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "An error occurred while deleting the address",
                    Errors = new List<string> { ex.Message }
                });
            }
        }
    }
}
