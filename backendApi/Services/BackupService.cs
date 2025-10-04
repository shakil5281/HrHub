using HrHubAPI.Data;
using HrHubAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.IO.Compression;
using System.Text.Json;

namespace HrHubAPI.Services
{
    public class BackupService : IBackupService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BackupService> _logger;
        private readonly string _backupDirectory;
        private readonly string _connectionString;

        public BackupService(
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<BackupService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")!;
            _backupDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Backups");
            
            // Ensure backup directory exists
            if (!Directory.Exists(_backupDirectory))
            {
                Directory.CreateDirectory(_backupDirectory);
            }
        }

        public async Task<BackupResponse> CreateBackupAsync(CreateBackupRequest request, string userId)
        {
            var startTime = DateTime.UtcNow;
            var backupId = Guid.NewGuid().ToString();
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{request.BackupName}_{timestamp}.bak";
            var filePath = Path.Combine(_backupDirectory, fileName);

            try
            {
                _logger.LogInformation($"Starting backup creation: {request.BackupName} by user {userId}");

                // Create SQL Server backup
                await CreateSqlServerBackupAsync(filePath, request.BackupName);

                var fileInfo = new FileInfo(filePath);
                var backupResponse = new BackupResponse
                {
                    Id = backupId,
                    BackupName = request.BackupName,
                    Description = request.Description,
                    FileName = fileName,
                    FilePath = filePath,
                    FileSizeBytes = fileInfo.Length,
                    FileSizeFormatted = FormatFileSize(fileInfo.Length),
                    CreatedAt = startTime,
                    CreatedBy = userId,
                    Status = "Completed",
                    IncludeData = request.IncludeData,
                    IncludeSchema = request.IncludeSchema,
                    IsCompressed = request.CompressBackup,
                    Duration = DateTime.UtcNow - startTime
                };

                // Save backup metadata to database
                await SaveBackupMetadataAsync(backupResponse);

                // Compress if requested
                if (request.CompressBackup)
                {
                    await CompressBackupFileAsync(filePath);
                    backupResponse.FileName = $"{fileName}.zip";
                    backupResponse.FilePath = $"{filePath}.zip";
                    backupResponse.IsCompressed = true;
                    
                    // Update file size after compression
                    var compressedFileInfo = new FileInfo(backupResponse.FilePath);
                    backupResponse.FileSizeBytes = compressedFileInfo.Length;
                    backupResponse.FileSizeFormatted = FormatFileSize(compressedFileInfo.Length);
                }

                _logger.LogInformation($"Backup created successfully: {backupResponse.FileName}");
                return backupResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating backup: {request.BackupName}");
                
                // Clean up failed backup file
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                return new BackupResponse
                {
                    Id = backupId,
                    BackupName = request.BackupName,
                    Status = "Failed",
                    CreatedAt = startTime,
                    CreatedBy = userId,
                    Duration = DateTime.UtcNow - startTime,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<BackupListResponse> GetBackupsAsync(BackupQueryRequest request)
        {
            try
            {
                var backups = await GetBackupMetadataAsync();
                
                // Apply filters
                if (!string.IsNullOrEmpty(request.SearchTerm))
                {
                    backups = backups.Where(b => 
                        b.BackupName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (b.Description != null && b.Description.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                if (request.FromDate.HasValue)
                {
                    backups = backups.Where(b => b.CreatedAt >= request.FromDate.Value).ToList();
                }

                if (request.ToDate.HasValue)
                {
                    backups = backups.Where(b => b.CreatedAt <= request.ToDate.Value).ToList();
                }

                if (!string.IsNullOrEmpty(request.Status))
                {
                    backups = backups.Where(b => b.Status.Equals(request.Status, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Apply sorting
                backups = request.SortBy?.ToLower() switch
                {
                    "backupname" => request.SortDirection == "asc" 
                        ? backups.OrderBy(b => b.BackupName).ToList()
                        : backups.OrderByDescending(b => b.BackupName).ToList(),
                    "createdat" => request.SortDirection == "asc"
                        ? backups.OrderBy(b => b.CreatedAt).ToList()
                        : backups.OrderByDescending(b => b.CreatedAt).ToList(),
                    "filesize" => request.SortDirection == "asc"
                        ? backups.OrderBy(b => b.FileSizeBytes).ToList()
                        : backups.OrderByDescending(b => b.FileSizeBytes).ToList(),
                    _ => backups.OrderByDescending(b => b.CreatedAt).ToList()
                };

                // Apply pagination
                var totalCount = backups.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var pagedBackups = backups
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new BackupListResponse
                {
                    Backups = pagedBackups,
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving backups");
                throw;
            }
        }

        public async Task<BackupResponse?> GetBackupByIdAsync(string backupId)
        {
            try
            {
                var backups = await GetBackupMetadataAsync();
                return backups.FirstOrDefault(b => b.Id == backupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving backup: {backupId}");
                throw;
            }
        }

        public async Task<BackupValidationResponse> ValidateBackupAsync(string backupId)
        {
            try
            {
                var backup = await GetBackupByIdAsync(backupId);
                if (backup == null)
                {
                    return new BackupValidationResponse
                    {
                        IsValid = false,
                        ErrorMessage = "Backup not found"
                    };
                }

                var warnings = new List<string>();

                // Check if file exists
                if (!File.Exists(backup.FilePath))
                {
                    return new BackupValidationResponse
                    {
                        IsValid = false,
                        ErrorMessage = "Backup file not found on disk"
                    };
                }

                // Check file size
                var fileInfo = new FileInfo(backup.FilePath);
                if (fileInfo.Length == 0)
                {
                    return new BackupValidationResponse
                    {
                        IsValid = false,
                        ErrorMessage = "Backup file is empty"
                    };
                }

                // Check if file is too old (optional warning)
                if (backup.CreatedAt < DateTime.UtcNow.AddDays(-30))
                {
                    warnings.Add("Backup is older than 30 days");
                }

                // Check if file is too large (optional warning)
                if (fileInfo.Length > 1024 * 1024 * 1024) // 1GB
                {
                    warnings.Add("Backup file is larger than 1GB");
                }

                return new BackupValidationResponse
                {
                    IsValid = true,
                    Warnings = warnings,
                    BackupInfo = backup
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating backup: {backupId}");
                return new BackupValidationResponse
                {
                    IsValid = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<RestoreBackupResponse> RestoreBackupAsync(RestoreBackupRequest request, string userId)
        {
            var startTime = DateTime.UtcNow;
            var restoreId = Guid.NewGuid().ToString();

            try
            {
                _logger.LogInformation($"Starting backup restore: {request.BackupId} by user {userId}");

                var backup = await GetBackupByIdAsync(request.BackupId);
                if (backup == null)
                {
                    throw new FileNotFoundException("Backup not found");
                }

                // Create pre-restore backup if requested
                string? preRestoreBackupId = null;
                if (request.CreateBackupBeforeRestore)
                {
                    var preBackupRequest = new CreateBackupRequest
                    {
                        BackupName = request.PreRestoreBackupName ?? $"PreRestore_{DateTime.UtcNow:yyyyMMdd_HHmmss}",
                        Description = "Automatic backup before restore operation",
                        IncludeData = true,
                        IncludeSchema = true,
                        CompressBackup = true
                    };

                    var preBackup = await CreateBackupAsync(preBackupRequest, userId);
                    preRestoreBackupId = preBackup.Id;
                }

                // Perform restore
                await RestoreSqlServerBackupAsync(backup.FilePath);

                var response = new RestoreBackupResponse
                {
                    Id = restoreId,
                    BackupId = request.BackupId,
                    BackupName = backup.BackupName,
                    StartedAt = startTime,
                    CompletedAt = DateTime.UtcNow,
                    Status = "Completed",
                    Duration = DateTime.UtcNow - startTime,
                    PreRestoreBackupId = preRestoreBackupId
                };

                _logger.LogInformation($"Backup restored successfully: {backup.BackupName}");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring backup: {request.BackupId}");
                return new RestoreBackupResponse
                {
                    Id = restoreId,
                    BackupId = request.BackupId,
                    StartedAt = startTime,
                    Status = "Failed",
                    Duration = DateTime.UtcNow - startTime,
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<bool> DeleteBackupAsync(string backupId, string userId)
        {
            try
            {
                var backup = await GetBackupByIdAsync(backupId);
                if (backup == null)
                {
                    return false;
                }

                // Delete file from disk
                if (File.Exists(backup.FilePath))
                {
                    File.Delete(backup.FilePath);
                }

                // Remove metadata
                await RemoveBackupMetadataAsync(backupId);

                _logger.LogInformation($"Backup deleted: {backup.BackupName} by user {userId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting backup: {backupId}");
                return false;
            }
        }

        public async Task<BackupStatusResponse> GetBackupStatusAsync()
        {
            try
            {
                var backups = await GetBackupMetadataAsync();
                var completedBackups = backups.Where(b => b.Status == "Completed").ToList();

                return new BackupStatusResponse
                {
                    Status = "Healthy",
                    LastBackupDate = completedBackups.Any() ? completedBackups.Max(b => b.CreatedAt) : null,
                    TotalBackups = backups.Count,
                    TotalBackupSizeBytes = completedBackups.Sum(b => b.FileSizeBytes),
                    TotalBackupSizeFormatted = FormatFileSize(completedBackups.Sum(b => b.FileSizeBytes)),
                    RecentBackups = completedBackups
                        .OrderByDescending(b => b.CreatedAt)
                        .Take(5)
                        .Select(b => b.BackupName)
                        .ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting backup status");
                return new BackupStatusResponse
                {
                    Status = "Error",
                    ErrorMessage = ex.Message
                };
            }
        }

        public async Task<byte[]> DownloadBackupAsync(string backupId)
        {
            try
            {
                var backup = await GetBackupByIdAsync(backupId);
                if (backup == null || !File.Exists(backup.FilePath))
                {
                    throw new FileNotFoundException("Backup file not found");
                }

                return await File.ReadAllBytesAsync(backup.FilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading backup: {backupId}");
                throw;
            }
        }

        public async Task<bool> ScheduleBackupAsync(CreateBackupRequest request, string userId, string cronExpression)
        {
            // This is a placeholder for scheduled backup functionality
            // In a real implementation, you would integrate with a job scheduler like Hangfire or Quartz.NET
            _logger.LogInformation($"Scheduled backup requested: {request.BackupName} with cron: {cronExpression}");
            return await Task.FromResult(true);
        }

        public async Task<List<string>> GetScheduledBackupsAsync()
        {
            // This is a placeholder for scheduled backup functionality
            return await Task.FromResult(new List<string>());
        }

        public async Task<bool> CancelScheduledBackupAsync(string scheduleId)
        {
            // This is a placeholder for scheduled backup functionality
            _logger.LogInformation($"Cancel scheduled backup requested: {scheduleId}");
            return await Task.FromResult(true);
        }

        public async Task<bool> CleanupOldBackupsAsync(int daysToKeep = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
                var backups = await GetBackupMetadataAsync();
                var oldBackups = backups.Where(b => b.CreatedAt < cutoffDate).ToList();

                foreach (var backup in oldBackups)
                {
                    await DeleteBackupAsync(backup.Id, "System");
                }

                _logger.LogInformation($"Cleaned up {oldBackups.Count} old backups");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old backups");
                return false;
            }
        }

        #region Private Methods

        private async Task CreateSqlServerBackupAsync(string filePath, string backupName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var backupCommand = $@"
                BACKUP DATABASE [{GetDatabaseName()}] 
                TO DISK = '{filePath}'
                WITH FORMAT, INIT, NAME = '{backupName}', 
                     SKIP, NOREWIND, NOUNLOAD, STATS = 10";

            using var command = new SqlCommand(backupCommand, connection);
            await command.ExecuteNonQueryAsync();
        }

        private async Task RestoreSqlServerBackupAsync(string filePath)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            // Set database to single user mode
            var singleUserCommand = $"ALTER DATABASE [{GetDatabaseName()}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE";
            using var singleUserCmd = new SqlCommand(singleUserCommand, connection);
            await singleUserCmd.ExecuteNonQueryAsync();

            try
            {
                // Restore database
                var restoreCommand = $@"
                    RESTORE DATABASE [{GetDatabaseName()}] 
                    FROM DISK = '{filePath}'
                    WITH REPLACE, STATS = 10";

                using var restoreCmd = new SqlCommand(restoreCommand, connection);
                await restoreCmd.ExecuteNonQueryAsync();
            }
            finally
            {
                // Set database back to multi-user mode
                var multiUserCommand = $"ALTER DATABASE [{GetDatabaseName()}] SET MULTI_USER";
                using var multiUserCmd = new SqlCommand(multiUserCommand, connection);
                await multiUserCmd.ExecuteNonQueryAsync();
            }
        }

        private string GetDatabaseName()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            return builder.InitialCatalog;
        }

        private Task CompressBackupFileAsync(string filePath)
        {
            var zipPath = $"{filePath}.zip";
            using var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create);
            archive.CreateEntryFromFile(filePath, Path.GetFileName(filePath));
            
            // Delete original file after compression
            File.Delete(filePath);
            return Task.CompletedTask;
        }

        private async Task SaveBackupMetadataAsync(BackupResponse backup)
        {
            var metadataFile = Path.Combine(_backupDirectory, "backup_metadata.json");
            var backups = await GetBackupMetadataAsync();
            
            backups.Add(backup);
            
            var json = JsonSerializer.Serialize(backups, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metadataFile, json);
        }

        private async Task<List<BackupResponse>> GetBackupMetadataAsync()
        {
            var metadataFile = Path.Combine(_backupDirectory, "backup_metadata.json");
            
            if (!File.Exists(metadataFile))
            {
                return new List<BackupResponse>();
            }

            var json = await File.ReadAllTextAsync(metadataFile);
            return JsonSerializer.Deserialize<List<BackupResponse>>(json) ?? new List<BackupResponse>();
        }

        private async Task RemoveBackupMetadataAsync(string backupId)
        {
            var metadataFile = Path.Combine(_backupDirectory, "backup_metadata.json");
            var backups = await GetBackupMetadataAsync();
            
            backups.RemoveAll(b => b.Id == backupId);
            
            var json = JsonSerializer.Serialize(backups, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(metadataFile, json);
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        #endregion
    }
}
