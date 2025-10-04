using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class ZkDeviceDto
    {
        public int Id { get; set; }
        public string DeviceName { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public int Port { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string MachineNumber { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int AdminCount { get; set; }
        public int FpCount { get; set; }
        public int FcCount { get; set; }
        public int PasswordCount { get; set; }
        public int LogCount { get; set; }
        public bool IsConnected { get; set; }
        public DateTime? LastConnectionTime { get; set; }
        public DateTime? LastLogDownloadTime { get; set; }
        public string Location { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateZkDeviceDto
    {
        [Required]
        [MaxLength(50)]
        public string DeviceName { get; set; } = string.Empty;

        [Required]
        [MaxLength(15)]
        public string IpAddress { get; set; } = string.Empty;

        [Required]
        [Range(1, 65535)]
        public int Port { get; set; } = 4370;

        [MaxLength(50)]
        public string SerialNumber { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ProductName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string MachineNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;
    }

    public class UpdateZkDeviceDto
    {
        [MaxLength(50)]
        public string? DeviceName { get; set; }

        [MaxLength(15)]
        public string? IpAddress { get; set; }

        [Range(1, 65535)]
        public int? Port { get; set; }

        [MaxLength(50)]
        public string? SerialNumber { get; set; }

        [MaxLength(50)]
        public string? ProductName { get; set; }

        [MaxLength(50)]
        public string? MachineNumber { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        public bool? IsActive { get; set; }
    }

    public class DeviceConnectionTestDto
    {
        public bool IsConnected { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime TestTime { get; set; } = DateTime.UtcNow;
        public int ResponseTimeMs { get; set; }
    }

    public class DownloadLogsRequestDto
    {
        public int? DeviceId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ForceDownload { get; set; } = false;
    }

    public class DownloadLogsResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int TotalLogsDownloaded { get; set; }
        public int NewLogs { get; set; }
        public int UpdatedLogs { get; set; }
        public int SkippedLogs { get; set; }
        public DateTime DownloadTime { get; set; } = DateTime.UtcNow;
        public List<string> Errors { get; set; } = new List<string>();
    }
}
