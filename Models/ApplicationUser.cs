using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Department { get; set; }

        [MaxLength(200)]
        public string? Position { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Refresh Token properties
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }

        // Company relationship
        public int? CompanyId { get; set; }
        public virtual Company? Company { get; set; }

        // Navigation property for user roles
        public virtual ICollection<IdentityUserRole<string>> UserRoles { get; set; } = new List<IdentityUserRole<string>>();
    }
}
