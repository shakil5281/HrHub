using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateEmployeeDto
    {
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
        [Range(0, 999999999.99, ErrorMessage = "House allowance must be a positive value")]
        public decimal? House { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Rent/Medical allowance must be a positive value")]
        public decimal? RentMedical { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Food allowance must be a positive value")]
        public decimal? Food { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Conveyance allowance must be a positive value")]
        public decimal? Conveyance { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Transport allowance must be a positive value")]
        public decimal? Transport { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Night bill must be a positive value")]
        public decimal? NightBill { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Mobile bill must be a positive value")]
        public decimal? MobileBill { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Other allowance must be a positive value")]
        public decimal? OtherAllowance { get; set; }

        // Salary Information
        [Required]
        [Range(0, 999999999.99, ErrorMessage = "Gross salary must be a positive value")]
        public decimal GrossSalary { get; set; }

        [Required]
        [Range(0, 999999999.99, ErrorMessage = "Basic salary must be a positive value")]
        public decimal BasicSalary { get; set; }

        [MaxLength(50)]
        public string? SalaryType { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankAccountNo { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Bank { get; set; }
    }

    public class UpdateEmployeeDto
    {
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
        [Range(0, 999999999.99, ErrorMessage = "House allowance must be a positive value")]
        public decimal? House { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Rent/Medical allowance must be a positive value")]
        public decimal? RentMedical { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Food allowance must be a positive value")]
        public decimal? Food { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Conveyance allowance must be a positive value")]
        public decimal? Conveyance { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Transport allowance must be a positive value")]
        public decimal? Transport { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Night bill must be a positive value")]
        public decimal? NightBill { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Mobile bill must be a positive value")]
        public decimal? MobileBill { get; set; }

        [Range(0, 999999999.99, ErrorMessage = "Other allowance must be a positive value")]
        public decimal? OtherAllowance { get; set; }

        // Salary Information
        [Required]
        [Range(0, 999999999.99, ErrorMessage = "Gross salary must be a positive value")]
        public decimal GrossSalary { get; set; }

        [Required]
        [Range(0, 999999999.99, ErrorMessage = "Basic salary must be a positive value")]
        public decimal BasicSalary { get; set; }

        [MaxLength(50)]
        public string? SalaryType { get; set; }

        [Required]
        [MaxLength(50)]
        public string BankAccountNo { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Bank { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class EmployeeDto
    {
        public int Id { get; set; }
        public string EmpId { get; set; } = string.Empty;
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
        
        // Permanent Address
        public string PermanentAddress { get; set; } = string.Empty;
        public string PermanentDivision { get; set; } = string.Empty;
        public string PermanentDistrict { get; set; } = string.Empty;
        public string PermanentUpazila { get; set; } = string.Empty;
        public string PermanentPostalCode { get; set; } = string.Empty;
        
        // Present Address
        public string PresentAddress { get; set; } = string.Empty;
        public string PresentDivision { get; set; } = string.Empty;
        public string PresentDistrict { get; set; } = string.Empty;
        public string PresentUpazila { get; set; } = string.Empty;
        public string PresentPostalCode { get; set; } = string.Empty;
        
        public DateTime JoiningDate { get; set; }
        
        // Personal Information
        public string? BloodGroup { get; set; }
        public string? Gender { get; set; }
        public string? Religion { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Education { get; set; }
        
        // Relational Information
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string SectionName { get; set; } = string.Empty;
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public string DesignationGrade { get; set; } = string.Empty;
        public int? LineId { get; set; }
        public string? LineName { get; set; }
        public int? ShiftId { get; set; }
        public string? ShiftName { get; set; }
        public int? DegreeId { get; set; }
        public string? DegreeName { get; set; }
        
        // Work Information
        public string? Floor { get; set; }
        public string? EmpType { get; set; }
        public string? Group { get; set; }
        
        // Allowances and Benefits
        public decimal? House { get; set; }
        public decimal? RentMedical { get; set; }
        public decimal? Food { get; set; }
        public decimal? Conveyance { get; set; }
        public decimal? Transport { get; set; }
        public decimal? NightBill { get; set; }
        public decimal? MobileBill { get; set; }
        public decimal? OtherAllowance { get; set; }
        
        // Salary Information
        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }
        public string? SalaryType { get; set; }
        public string BankAccountNo { get; set; } = string.Empty;
        public string? Bank { get; set; }
        
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
        public string EmpId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        // Bangla version of employee name - should use SutonnyMJ font for display
        public string? NameBangla { get; set; }
        public string NIDNo { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        
        // Personal Information
        public string? BloodGroup { get; set; }
        public string? Gender { get; set; }
        public string? Religion { get; set; }
        public string? MaritalStatus { get; set; }
        
        // Relational Information
        public string CompanyName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string DesignationGrade { get; set; } = string.Empty;
        public string? LineName { get; set; }
        public string? ShiftName { get; set; }
        
        // Work Information
        public string? Floor { get; set; }
        public string? EmpType { get; set; }
        public string? Group { get; set; }
        
        // Salary Information
        public decimal GrossSalary { get; set; }
        public decimal BasicSalary { get; set; }
        public string? SalaryType { get; set; }
        public string? Bank { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmployeeSummaryDto
    {
        public int Id { get; set; }
        public string EmpId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? NameBangla { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string SectionName { get; set; } = string.Empty;
        public string DesignationName { get; set; } = string.Empty;
        public string? LineName { get; set; }
        public string? ShiftName { get; set; }
        public decimal GrossSalary { get; set; }
        public bool IsActive { get; set; }
    }
}
