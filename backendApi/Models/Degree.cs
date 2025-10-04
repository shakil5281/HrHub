using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class Degree
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        // Bangla version of degree name - should use SutonnyMJ font for display
        [MaxLength(200)]
        public string? NameBangla { get; set; }

        // Degree level (e.g., SSC, HSC, Bachelor, Master, PhD)
        [Required]
        [MaxLength(50)]
        public string Level { get; set; } = string.Empty;

        // Bangla version of degree level
        [MaxLength(50)]
        public string? LevelBangla { get; set; }

        // Institution type (e.g., University, College, School, Board)
        [MaxLength(100)]
        public string? InstitutionType { get; set; }

        // Bangla version of institution type
        [MaxLength(100)]
        public string? InstitutionTypeBangla { get; set; }

        // Company relationship
        [Required]
        public int CompanyId { get; set; }

        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; } = null!;

        // Audit Fields
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(128)]
        public string? CreatedBy { get; set; }

        [MaxLength(128)]
        public string? UpdatedBy { get; set; }
    }
}
