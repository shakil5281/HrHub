using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateCompanyDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Industry { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(50)]
        public string? RegistrationNumber { get; set; }

        [MaxLength(50)]
        public string? TaxId { get; set; }

        public DateTime? EstablishedDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Employee count must be greater than 0")]
        public int? EmployeeCount { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }
    }

    public class UpdateCompanyDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Industry { get; set; }

        [MaxLength(100)]
        public string? Website { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [EmailAddress]
        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(100)]
        public string? State { get; set; }

        [MaxLength(20)]
        public string? PostalCode { get; set; }

        [MaxLength(100)]
        public string? Country { get; set; }

        [MaxLength(50)]
        public string? RegistrationNumber { get; set; }

        [MaxLength(50)]
        public string? TaxId { get; set; }

        public DateTime? EstablishedDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Employee count must be greater than 0")]
        public int? EmployeeCount { get; set; }

        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class CompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Industry { get; set; }
        public string? Website { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? RegistrationNumber { get; set; }
        public string? TaxId { get; set; }
        public DateTime? EstablishedDate { get; set; }
        public int? EmployeeCount { get; set; }
        public string? LogoUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class CompanyListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Industry { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public int? EmployeeCount { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
