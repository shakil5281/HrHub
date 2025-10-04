using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class BangladeshAddress
    {
        [Key]
        public int Id { get; set; }

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

        // Additional fields for comprehensive address
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

        // Geographic coordinates (optional)
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        // Status and metadata
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }

        // Computed property for full address
        [NotMapped]
        public string FullAddress => $"{Area}, {Union}, {Upazila}, {District}, {Division} - {PostalCode}";

        [NotMapped]
        public string FullAddressBangla => $"{AreaBangla}, {UnionBangla}, {UpazilaBangla}, {DistrictBangla}, {DivisionBangla} - {PostalCode}";
    }
}
