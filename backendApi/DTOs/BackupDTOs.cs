using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    public class CreateBackupRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string BackupName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IncludeData { get; set; } = true;
        public bool IncludeSchema { get; set; } = true;
        public bool CompressBackup { get; set; } = true;
    }

    public class BackupResponse
    {
        public string Id { get; set; } = string.Empty;
        public string BackupName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string FileSizeFormatted { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public bool IncludeData { get; set; }
        public bool IncludeSchema { get; set; }
        public bool IsCompressed { get; set; }
        public TimeSpan Duration { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class BackupListResponse
    {
        public List<BackupResponse> Backups { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    public class RestoreBackupRequest
    {
        [Required]
        public string BackupId { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public bool CreateBackupBeforeRestore { get; set; } = true;
        public string? PreRestoreBackupName { get; set; }
    }

    public class RestoreBackupResponse
    {
        public string Id { get; set; } = string.Empty;
        public string BackupId { get; set; } = string.Empty;
        public string BackupName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan? Duration { get; set; }
        public string? ErrorMessage { get; set; }
        public string? PreRestoreBackupId { get; set; }
    }

    public class BackupStatusResponse
    {
        public string Status { get; set; } = string.Empty;
        public DateTime? LastBackupDate { get; set; }
        public int TotalBackups { get; set; }
        public long TotalBackupSizeBytes { get; set; }
        public string TotalBackupSizeFormatted { get; set; } = string.Empty;
        public List<string> RecentBackups { get; set; } = new();
        public string? ErrorMessage { get; set; }
    }

    public class BackupQueryRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? Status { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortDirection { get; set; } = "desc";
    }

    public class DeleteBackupRequest
    {
        [Required]
        public string BackupId { get; set; } = string.Empty;
        public bool ConfirmDelete { get; set; } = false;
    }

    public class BackupValidationResponse
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Warnings { get; set; } = new();
        public BackupResponse? BackupInfo { get; set; }
    }
}
