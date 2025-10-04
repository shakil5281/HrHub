using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HrHubAPI.Models
{
    public class ZkDevice
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string DeviceName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        public int Port { get; set; } = 4370;

        [MaxLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string MachineNumber { get; set; } = string.Empty;

        public int UserCount { get; set; } = 0;
        public int AdminCount { get; set; } = 0;
        public int FpCount { get; set; } = 0;
        public int FcCount { get; set; } = 0;
        public int PasswordCount { get; set; } = 0;
        public int LogCount { get; set; } = 0;

        public bool IsConnected { get; set; } = false;
        public DateTime? LastConnectionTime { get; set; }
        public DateTime? LastLogDownloadTime { get; set; }

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(50)]
        public string CreatedBy { get; set; } = string.Empty;

        [MaxLength(50)]
        public string UpdatedBy { get; set; } = string.Empty;

        // Navigation property
        public virtual ICollection<AttendanceLog> AttendanceLogs { get; set; } = new List<AttendanceLog>();
    }
}
