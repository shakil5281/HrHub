using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        // Employee ID (custom employee identifier)
        [Required]
        [MaxLength(50)]
        public string EmpId { get; set; } = string.Empty;

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

        // Permanent Address
        [Required]
        [MaxLength(500)]
        public string PermanentAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PermanentDivision { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PermanentDistrict { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PermanentUpazila { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PermanentPostalCode { get; set; } = string.Empty;

        // Present Address
        [Required]
        [MaxLength(500)]
        public string PresentAddress { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PresentDivision { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PresentDistrict { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string PresentUpazila { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PresentPostalCode { get; set; } = string.Empty;

        [Required]
        public DateTime JoiningDate { get; set; }

        // Personal Information
        [MaxLength(10)]
        public string? BloodGroup { get; set; }

        [MaxLength(50)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? Religion { get; set; }

        [MaxLength(50)]
        public string? MaritalStatus { get; set; }

        // Education Information
        [MaxLength(200)]
        public string? Education { get; set; }

        // Relational IDs
        [Required]
        public int CompanyId { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int DesignationId { get; set; }

        public int? LineId { get; set; }

        public int? ShiftId { get; set; }

        public int? DegreeId { get; set; }

        // Work Information
        [MaxLength(50)]
        public string? Floor { get; set; }

        [MaxLength(50)]
        public string? EmpType { get; set; }

        [MaxLength(50)]
        public string? Group { get; set; }

        // Allowances and Benefits
        [Column(TypeName = "decimal(18,2)")]
        public decimal? House { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RentMedical { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Food { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Conveyance { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Transport { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? NightBill { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MobileBill { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? OtherAllowance { get; set; }

        // Salary Information
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal GrossSalary { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasicSalary { get; set; }

        [MaxLength(50)]
        public string? SalaryType { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankAccountNo { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Bank { get; set; }

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual Company Company { get; set; } = null!;
        public virtual Department Department { get; set; } = null!;
        public virtual Section Section { get; set; } = null!;
        public virtual Designation Designation { get; set; } = null!;
        public virtual Line? Line { get; set; }
        public virtual Shift? Shift { get; set; }
        public virtual Degree? Degree { get; set; }
    }
}
