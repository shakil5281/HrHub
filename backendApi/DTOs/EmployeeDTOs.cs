using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateEmployeeDto
    {
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

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int DesignationId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Gross salary must be a positive value")]
        public decimal GrossSalary { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Basic salary must be a positive value")]
        public decimal BasicSalary { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankAccountNo { get; set; } = string.Empty;
    }

    public class UpdateEmployeeDto
    {
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

        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public int DesignationId { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Gross salary must be a positive value")]
        public decimal GrossSalary { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Basic salary must be a positive value")]
        public decimal BasicSalary { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankAccountNo { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }

    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla version of employee name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public string NIDNo { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string MotherName { get; set; } = string.Empty;
        // Bangla version of father name - should use SutonnyMJ font for display
        public string? FatherNameBangla { get; set; }
        // Bangla version of mother name - should use SutonnyMJ font for display
        public string? MotherNameBangla { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        
        // Relational Information
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public string DesignationGrade { get; set; } = string.Empty;
        
        // Salary Information
        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }
        public string BankAccountNo { get; set; } = string.Empty;
        
        // Audit Information
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        // Bangla version of employee name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public string NIDNo { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        
        // Relational Information
        public string DepartmentName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string DesignationGrade { get; set; } = string.Empty;
        
        // Salary Information
        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmployeeSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public decimal GrossSalary { get; set; }
        public bool IsActive { get; set; }
    }
}
