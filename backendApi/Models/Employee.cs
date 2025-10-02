using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of employee name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        [MaxLength(50)]
        public string NIDNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FatherName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string MotherName { get; set; } = string.Empty;

        // Bangla version of father name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? FatherNameBangla { get; set; }

        // Bangla version of mother name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? MotherNameBangla { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [MaxLength(500)]
        public string Address { get; set; } = string.Empty;

        [Required]
        public DateTime JoiningDate { get; set; }

        // Relational IDs
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int DesignationId { get; set; }

        // Salary Information
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossSalary { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankAccountNo { get; set; } = string.Empty;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual Department Department { get; set; } = null!;
        public virtual Section Section { get; set; } = null!;
        public virtual Designation Designation { get; set; } = null!;
    }
}
