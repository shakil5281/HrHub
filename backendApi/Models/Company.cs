using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.Models
{
    public class Company
    {
        [Key]
        public int Id { get; set; }

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

        [MaxLength(100)]
        public string? Email { get; set; }

        // Address Information
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

        // Company Details
        [MaxLength(50)]
        public string? RegistrationNumber { get; set; }

        [MaxLength(50)]
        public string? TaxId { get; set; }

        public DateTime? EstablishedDate { get; set; }

        public int? EmployeeCount { get; set; }

        // Logo/Image
        [MaxLength(500)]
        public string? LogoUrl { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<ApplicationUser> Employees { get; set; } = new List<ApplicationUser>();
    }
}
