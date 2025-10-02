using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class Designation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of designation name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        [Required]
        [MaxLength(50)]
        public string Grade { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal AttendanceBonus { get; set; } = 0;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        public virtual Section Section { get; set; } = null!;
    }
}
