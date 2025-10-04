using HrHubAPI.DTOs;

namespace HrHubAPI.Services
{
    public interface IBackupService
    {
        Task<BackupResponse> CreateBackupAsync(CreateBackupRequest request, string userId);
        Task<BackupListResponse> GetBackupsAsync(BackupQueryRequest request);
        Task<BackupResponse?> GetBackupByIdAsync(string backupId);
        Task<BackupValidationResponse> ValidateBackupAsync(string backupId);
        Task<RestoreBackupResponse> RestoreBackupAsync(RestoreBackupRequest request, string userId);
        Task<bool> DeleteBackupAsync(string backupId, string userId);
        Task<BackupStatusResponse> GetBackupStatusAsync();
        Task<byte[]> DownloadBackupAsync(string backupId);
        Task<bool> ScheduleBackupAsync(CreateBackupRequest request, string userId, string cronExpression);
        Task<List<string>> GetScheduledBackupsAsync();
        Task<bool> CancelScheduledBackupAsync(string scheduleId);
        Task<bool> CleanupOldBackupsAsync(int daysToKeep = 30);
    }
}
