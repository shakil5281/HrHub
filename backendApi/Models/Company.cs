using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

        // Company ID (could be a custom company identifier)
        [MaxLength(50)]
        public string? CompanyId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of company name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? CompanyNameBangla { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(20)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        // Address Information
        [MaxLength(500)]
        public string? Address { get; set; }

        // Bangla version of address - should use SutonnyMJ font for display
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

        // Logo/Image
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        // Authorized Signature
        [MaxLength(500)]
        public string? AuthorizedSignature { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
