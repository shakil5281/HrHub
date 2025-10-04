using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class AttendanceLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ZkDeviceId { get; set; }

        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [MaxLength(200)]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public DateTime LogTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string LogType { get; set; } = string.Empty; // IN, OUT

        [MaxLength(20)]
        public string VerificationMode { get; set; } = string.Empty; // FP, RF, CARD, PASSWORD

        [MaxLength(20)]
        public string WorkCode { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;

        public bool IsProcessed { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [MaxLength(50)]
        public string UpdatedBy { get; set; } = string.Empty;

        // Navigation property
        [ForeignKey("ZkDeviceId")]
        public virtual ZkDevice ZkDevice { get; set; } = null!;
    }
}
