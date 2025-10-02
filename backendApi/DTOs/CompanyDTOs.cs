using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateCompanyDto
    {
        [MaxLength(50)]
        public string? CompanyId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? CompanyNameBangla { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        // Bangla address - should use SutonnyMJ font for display
        [MaxLength(500)]
        public string? AddressBangla { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        // Authorized signature image/document URL
        [MaxLength(500)]
        public string? AuthorizedSignature { get; set; }
    }

    public class UpdateCompanyDto
    {
        [MaxLength(50)]
        public string? CompanyId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla company name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? CompanyNameBangla { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        // Bangla address - should use SutonnyMJ font for display
        [MaxLength(500)]
        public string? AddressBangla { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        // Authorized signature image/document URL
        [MaxLength(500)]
        public string? AuthorizedSignature { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string? CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla company name - should use SutonnyMJ font for display
        public string? CompanyNameBangla { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        // Bangla address - should use SutonnyMJ font for display
        public string? AddressBangla { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? LogoUrl { get; set; }
        public string? AuthorizedSignature { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CompanyListDto
    {
        public int Id { get; set; }
        public string? CompanyId { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla company name - should use SutonnyMJ font for display
        public string? CompanyNameBangla { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
