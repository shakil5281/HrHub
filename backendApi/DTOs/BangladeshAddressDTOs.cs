using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateBangladeshAddressDto
    {
        [Required]
        [MaxLength(100)]
        public string Division { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DivisionBangla { get; set; }

        [Required]
        [MaxLength(100)]
        public string District { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DistrictBangla { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Upazila { get; set; }

        [MaxLength(100)]
        public string? UpazilaBangla { get; set; }

        [MaxLength(100)]
        public string? Union { get; set; }

        [MaxLength(100)]
        public string? UnionBangla { get; set; }

        [MaxLength(200)]
        public string? Area { get; set; }

        [MaxLength(200)]
        public string? AreaBangla { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class UpdateBangladeshAddressDto
    {
        [Required]
        [MaxLength(100)]
        public string Division { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DivisionBangla { get; set; }

        [Required]
        [MaxLength(100)]
        public string District { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? DistrictBangla { get; set; }

        [Required]
        [MaxLength(10)]
        public string PostalCode { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Upazila { get; set; }

        [MaxLength(100)]
        public string? UpazilaBangla { get; set; }

        [MaxLength(100)]
        public string? Union { get; set; }

        [MaxLength(100)]
        public string? UnionBangla { get; set; }

        [MaxLength(200)]
        public string? Area { get; set; }

        [MaxLength(200)]
        public string? AreaBangla { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class BangladeshAddressDto
    {
        public int Id { get; set; }
        public string Division { get; set; } = string.Empty;
        public string? DivisionBangla { get; set; }
        public string District { get; set; } = string.Empty;
        public string? DistrictBangla { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string? Upazila { get; set; }
        public string? UpazilaBangla { get; set; }
        public string? Union { get; set; }
        public string? UnionBangla { get; set; }
        public string? Area { get; set; }
        public string? AreaBangla { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        
        // Computed properties
        public string FullAddress => $"{Area}, {Union}, {Upazila}, {District}, {Division} - {PostalCode}";
        public string FullAddressBangla => $"{AreaBangla}, {UnionBangla}, {UpazilaBangla}, {DistrictBangla}, {DivisionBangla} - {PostalCode}";
    }

    public class BangladeshAddressListDto
    {
        public int Id { get; set; }
        public string Division { get; set; } = string.Empty;
        public string? DivisionBangla { get; set; }
        public string District { get; set; } = string.Empty;
        public string? DistrictBangla { get; set; }
        public string PostalCode { get; set; } = string.Empty;
        public string? Upazila { get; set; }
        public string? UpazilaBangla { get; set; }
        public string? Union { get; set; }
        public string? UnionBangla { get; set; }
        public string? Area { get; set; }
        public string? AreaBangla { get; set; }
        public bool IsActive { get; set; }
        
        // Computed properties
        public string FullAddress => $"{Area}, {Union}, {Upazila}, {District}, {Division} - {PostalCode}";
        public string FullAddressBangla => $"{AreaBangla}, {UnionBangla}, {UpazilaBangla}, {DistrictBangla}, {DivisionBangla} - {PostalCode}";
    }

    public class BangladeshAddressFilterDto
    {
        public string? Division { get; set; }
        public string? District { get; set; }
        public string? Upazila { get; set; }
        public string? Union { get; set; }
        public string? PostalCode { get; set; }
        public string? Area { get; set; }
        public bool? IsActive { get; set; }
        public bool? HasCoordinates { get; set; }
    }

    public class DivisionDto
    {
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public int DistrictCount { get; set; }
        public int AddressCount { get; set; }
    }

    public class DistrictDto
    {
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public string Division { get; set; } = string.Empty;
        public string? DivisionBangla { get; set; }
        public int UpazilaCount { get; set; }
        public int AddressCount { get; set; }
    }

    public class UpazilaDto
    {
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public string District { get; set; } = string.Empty;
        public string? DistrictBangla { get; set; }
        public string Division { get; set; } = string.Empty;
        public string? DivisionBangla { get; set; }
        public int UnionCount { get; set; }
        public int AddressCount { get; set; }
    }

    public class PostalCodeSearchDto
    {
        public string PostalCode { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string FullAddressBangla { get; set; } = string.Empty;
        public string Division { get; set; } = string.Empty;
        public string? DivisionBangla { get; set; }
        public string District { get; set; } = string.Empty;
        public string? DistrictBangla { get; set; }
        public string? Upazila { get; set; }
        public string? UpazilaBangla { get; set; }
        public string? Union { get; set; }
        public string? UnionBangla { get; set; }
        public string? Area { get; set; }
        public string? AreaBangla { get; set; }
    }

    public class AddressStatisticsDto
    {
        public int TotalAddresses { get; set; }
        public int TotalDivisions { get; set; }
        public int TotalDistricts { get; set; }
        public int TotalUpazilas { get; set; }
        public int TotalUnions { get; set; }
        public int TotalPostalCodes { get; set; }
        public int AddressesWithCoordinates { get; set; }
        public Dictionary<string, int> DivisionCounts { get; set; } = new();
        public Dictionary<string, int> DistrictCounts { get; set; } = new();
    }
}
