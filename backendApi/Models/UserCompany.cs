using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.Models
{
    /// <summary>
    /// Junction table for many-to-many relationship between Users and Companies
    /// </summary>
    public class UserCompany
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CompanyId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(128)]
        public string? AssignedBy { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Company Company { get; set; } = null!;
    }
}
