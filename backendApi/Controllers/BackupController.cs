using HrHubAPI.DTOs;
using HrHubAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BackupController : ControllerBase
    {
        private readonly IBackupService _backupService;
        private readonly ILogger<BackupController> _logger;

        public BackupController(IBackupService backupService, ILogger<BackupController> logger)
        {
            _backupService = backupService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new database backup
        /// </summary>
        /// <param name="request">Backup creation request</param>
        /// <returns>Backup response with details</returns>
        [HttpPost("create")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<BackupResponse>> CreateBackup([FromBody] CreateBackupRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _backupService.CreateBackupAsync(request, userId);
                
                if (result.Status == "Failed")
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating backup");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get list of backups with filtering and pagination
        /// </summary>
        /// <param name="request">Query parameters for filtering and pagination</param>
        /// <returns>Paginated list of backups</returns>
        [HttpGet("list")]
        [Authorize(Roles = "Admin,IT,HR Manager,HR")]
        public async Task<ActionResult<BackupListResponse>> GetBackups([FromQuery] BackupQueryRequest request)
        {
            try
            {
                var result = await _backupService.GetBackupsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving backups");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get backup details by ID
        /// </summary>
        /// <param name="id">Backup ID</param>
        /// <returns>Backup details</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,IT,HR Manager,HR")]
        public async Task<ActionResult<BackupResponse>> GetBackup(string id)
        {
            try
            {
                var result = await _backupService.GetBackupByIdAsync(id);
                if (result == null)
                {
                    return NotFound(new { message = "Backup not found" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving backup: {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate backup integrity
        /// </summary>
        /// <param name="id">Backup ID</param>
        /// <returns>Backup validation result</returns>
        [HttpGet("{id}/validate")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<BackupValidationResponse>> ValidateBackup(string id)
        {
            try
            {
                var result = await _backupService.ValidateBackupAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating backup: {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Restore database from backup
        /// </summary>
        /// <param name="request">Restore request</param>
        /// <returns>Restore operation result</returns>
        [HttpPost("restore")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<RestoreBackupResponse>> RestoreBackup([FromBody] RestoreBackupRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _backupService.RestoreBackupAsync(request, userId);
                
                if (result.Status == "Failed")
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error restoring backup");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete a backup
        /// </summary>
        /// <param name="id">Backup ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> DeleteBackup(string id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _backupService.DeleteBackupAsync(id, userId);
                
                if (!result)
                {
                    return NotFound(new { message = "Backup not found or could not be deleted" });
                }

                return Ok(new { message = "Backup deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting backup: {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get backup system status
        /// </summary>
        /// <returns>Backup system status</returns>
        [HttpGet("status")]
        [Authorize(Roles = "Admin,IT,HR Manager,HR")]
        public async Task<ActionResult<BackupStatusResponse>> GetBackupStatus()
        {
            try
            {
                var result = await _backupService.GetBackupStatusAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting backup status");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Download backup file
        /// </summary>
        /// <param name="id">Backup ID</param>
        /// <returns>Backup file download</returns>
        [HttpGet("{id}/download")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult> DownloadBackup(string id)
        {
            try
            {
                var backup = await _backupService.GetBackupByIdAsync(id);
                if (backup == null)
                {
                    return NotFound(new { message = "Backup not found" });
                }

                var fileBytes = await _backupService.DownloadBackupAsync(id);
                var contentType = backup.IsCompressed ? "application/zip" : "application/octet-stream";
                
                return File(fileBytes, contentType, backup.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading backup: {id}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Schedule automatic backup
        /// </summary>
        /// <param name="request">Backup schedule request</param>
        /// <param name="cronExpression">Cron expression for scheduling</param>
        /// <returns>Scheduling result</returns>
        [HttpPost("schedule")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> ScheduleBackup([FromBody] CreateBackupRequest request, [FromQuery] string cronExpression)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _backupService.ScheduleBackupAsync(request, userId, cronExpression);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to schedule backup" });
                }

                return Ok(new { message = "Backup scheduled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling backup");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get list of scheduled backups
        /// </summary>
        /// <returns>List of scheduled backups</returns>
        [HttpGet("scheduled")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<string>>> GetScheduledBackups()
        {
            try
            {
                var result = await _backupService.GetScheduledBackupsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scheduled backups");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cancel scheduled backup
        /// </summary>
        /// <param name="scheduleId">Schedule ID</param>
        /// <returns>Cancellation result</returns>
        [HttpDelete("schedule/{scheduleId}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CancelScheduledBackup(string scheduleId)
        {
            try
            {
                var result = await _backupService.CancelScheduledBackupAsync(scheduleId);
                
                if (!result)
                {
                    return NotFound(new { message = "Scheduled backup not found" });
                }

                return Ok(new { message = "Scheduled backup cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling scheduled backup: {scheduleId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Clean up old backups
        /// </summary>
        /// <param name="daysToKeep">Number of days to keep backups (default: 30)</param>
        /// <returns>Cleanup result</returns>
        [HttpPost("cleanup")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CleanupOldBackups([FromQuery] int daysToKeep = 30)
        {
            try
            {
                var result = await _backupService.CleanupOldBackupsAsync(daysToKeep);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to cleanup old backups" });
                }

                return Ok(new { message = "Old backups cleaned up successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old backups");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get backup statistics and health check
        /// </summary>
        /// <returns>Backup health information</returns>
        [HttpGet("health")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult> GetBackupHealth()
        {
            try
            {
                var status = await _backupService.GetBackupStatusAsync();
                var health = new
                {
                    Status = status.Status,
                    LastBackup = status.LastBackupDate,
                    TotalBackups = status.TotalBackups,
                    TotalSize = status.TotalBackupSizeFormatted,
                    RecentBackups = status.RecentBackups,
                    HealthScore = CalculateHealthScore(status),
                    Recommendations = GetHealthRecommendations(status)
                };

                return Ok(health);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting backup health");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #region Private Methods

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        }

        private int CalculateHealthScore(BackupStatusResponse status)
        {
            int score = 100;

            // Deduct points for no recent backup
            if (!status.LastBackupDate.HasValue)
            {
                score -= 50;
            }
            else if (status.LastBackupDate.Value < DateTime.UtcNow.AddDays(-7))
            {
                score -= 30;
            }
            else if (status.LastBackupDate.Value < DateTime.UtcNow.AddDays(-3))
            {
                score -= 15;
            }

            // Deduct points for too few backups
            if (status.TotalBackups < 3)
            {
                score -= 20;
            }

            // Deduct points for very large total size (potential storage issues)
            if (status.TotalBackupSizeBytes > 10L * 1024 * 1024 * 1024) // 10GB
            {
                score -= 10;
            }

            return Math.Max(0, score);
        }

        private List<string> GetHealthRecommendations(BackupStatusResponse status)
        {
            var recommendations = new List<string>();

            if (!status.LastBackupDate.HasValue)
            {
                recommendations.Add("Create your first backup immediately");
            }
            else if (status.LastBackupDate.Value < DateTime.UtcNow.AddDays(-7))
            {
                recommendations.Add("Create a new backup - last backup is over a week old");
            }

            if (status.TotalBackups < 3)
            {
                recommendations.Add("Consider creating more frequent backups for better data protection");
            }

            if (status.TotalBackupSizeBytes > 5L * 1024 * 1024 * 1024) // 5GB
            {
                recommendations.Add("Consider cleaning up old backups to save storage space");
            }

            if (recommendations.Count == 0)
            {
                recommendations.Add("Backup system is healthy - no immediate action required");
            }

            return recommendations;
        }

        #endregion
    }
}
