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
    public class DatabaseManagementController : ControllerBase
    {
        private readonly IDatabaseManagementService _databaseService;
        private readonly ILogger<DatabaseManagementController> _logger;

        public DatabaseManagementController(
            IDatabaseManagementService databaseService,
            ILogger<DatabaseManagementController> logger)
        {
            _databaseService = databaseService;
            _logger = logger;
        }

        /// <summary>
        /// Perform database maintenance operation
        /// </summary>
        /// <param name="request">Maintenance request</param>
        /// <returns>Maintenance operation result</returns>
        [HttpPost("maintenance")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> PerformMaintenance([FromBody] DatabaseMaintenanceRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.PerformMaintenanceAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing database maintenance");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Rebuild database indexes
        /// </summary>
        /// <param name="tableNames">List of table names (optional)</param>
        /// <returns>Index rebuild result</returns>
        [HttpPost("maintenance/rebuild-indexes")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> RebuildIndexes([FromBody] List<string>? tableNames = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.RebuildIndexesAsync(tableNames, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rebuilding indexes");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Reorganize database indexes
        /// </summary>
        /// <param name="tableNames">List of table names (optional)</param>
        /// <returns>Index reorganization result</returns>
        [HttpPost("maintenance/reorganize-indexes")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> ReorganizeIndexes([FromBody] List<string>? tableNames = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.ReorganizeIndexesAsync(tableNames, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reorganizing indexes");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update database statistics
        /// </summary>
        /// <param name="tableNames">List of table names (optional)</param>
        /// <returns>Statistics update result</returns>
        [HttpPost("maintenance/update-statistics")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> UpdateStatistics([FromBody] List<string>? tableNames = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.UpdateStatisticsAsync(tableNames, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating statistics");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check database integrity
        /// </summary>
        /// <param name="tableNames">List of table names (optional)</param>
        /// <returns>Integrity check result</returns>
        [HttpPost("maintenance/check-integrity")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> CheckIntegrity([FromBody] List<string>? tableNames = null)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.CheckIntegrityAsync(tableNames, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database integrity");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Shrink database
        /// </summary>
        /// <returns>Database shrink result</returns>
        [HttpPost("maintenance/shrink")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> ShrinkDatabase()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.ShrinkDatabaseAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error shrinking database");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Optimize database
        /// </summary>
        /// <returns>Database optimization result</returns>
        [HttpPost("maintenance/optimize")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> OptimizeDatabase()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.OptimizeDatabaseAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing database");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Execute database query
        /// </summary>
        /// <param name="request">Query request</param>
        /// <returns>Query execution result</returns>
        [HttpPost("query")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseQueryResponse>> ExecuteQuery([FromBody] DatabaseQueryRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.ExecuteQueryAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing database query");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Execute read-only database query
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <returns>Query execution result</returns>
        [HttpPost("query/readonly")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<DatabaseQueryResponse>> ExecuteReadOnlyQuery([FromBody] string query)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.ExecuteReadOnlyQueryAsync(query, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing read-only query");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate SQL query
        /// </summary>
        /// <param name="query">SQL query to validate</param>
        /// <returns>Validation result</returns>
        [HttpPost("query/validate")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> ValidateQuery([FromBody] string query)
        {
            try
            {
                var isValid = await _databaseService.ValidateQueryAsync(query);
                return Ok(new { isValid, message = isValid ? "Query is valid" : "Query is invalid" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating query");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get table names
        /// </summary>
        /// <returns>List of table names</returns>
        [HttpGet("tables")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<string>>> GetTableNames()
        {
            try
            {
                var result = await _databaseService.GetTableNamesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving table names");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get column names for a table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>List of column names</returns>
        [HttpGet("tables/{tableName}/columns")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<string>>> GetColumnNames(string tableName)
        {
            try
            {
                var result = await _databaseService.GetColumnNamesAsync(tableName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving column names for table: {tableName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get table schema
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Table schema</returns>
        [HttpGet("tables/{tableName}/schema")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<Dictionary<string, string>>> GetTableSchema(string tableName)
        {
            try
            {
                var result = await _databaseService.GetTableSchemaAsync(tableName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving table schema: {tableName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if table exists
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Table existence status</returns>
        [HttpGet("tables/{tableName}/exists")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> TableExists(string tableName)
        {
            try
            {
                var exists = await _databaseService.TableExistsAsync(tableName);
                return Ok(new { tableName, exists });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking table existence: {tableName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get table row count
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Row count</returns>
        [HttpGet("tables/{tableName}/rowcount")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> GetTableRowCount(string tableName)
        {
            try
            {
                var rowCount = await _databaseService.GetTableRowCountAsync(tableName);
                return Ok(new { tableName, rowCount });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving row count for table: {tableName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create full database backup
        /// </summary>
        /// <param name="backupName">Backup name</param>
        /// <returns>Backup creation result</returns>
        [HttpPost("backup/full")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CreateFullBackup([FromBody] string backupName)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.CreateFullBackupAsync(backupName, userId);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to create full backup" });
                }

                return Ok(new { message = "Full backup created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating full backup");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create differential database backup
        /// </summary>
        /// <param name="backupName">Backup name</param>
        /// <returns>Backup creation result</returns>
        [HttpPost("backup/differential")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CreateDifferentialBackup([FromBody] string backupName)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.CreateDifferentialBackupAsync(backupName, userId);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to create differential backup" });
                }

                return Ok(new { message = "Differential backup created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating differential backup");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create transaction log backup
        /// </summary>
        /// <param name="backupName">Backup name</param>
        /// <returns>Backup creation result</returns>
        [HttpPost("backup/transaction-log")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CreateTransactionLogBackup([FromBody] string backupName)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.CreateTransactionLogBackupAsync(backupName, userId);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to create transaction log backup" });
                }

                return Ok(new { message = "Transaction log backup created successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating transaction log backup");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get backup history
        /// </summary>
        /// <returns>List of backup history</returns>
        [HttpGet("backup/history")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<string>>> GetBackupHistory()
        {
            try
            {
                var result = await _databaseService.GetBackupHistoryAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving backup history");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cleanup orphaned data
        /// </summary>
        /// <returns>Cleanup result</returns>
        [HttpPost("cleanup/orphaned-data")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> CleanupOrphanedData()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.CleanupOrphanedDataAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up orphaned data");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Archive old data
        /// </summary>
        /// <param name="daysOld">Number of days old</param>
        /// <returns>Archive result</returns>
        [HttpPost("cleanup/archive-old-data")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseMaintenanceResponse>> ArchiveOldData([FromQuery] int daysOld = 365)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _databaseService.ArchiveOldDataAsync(daysOld, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error archiving old data");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        }
    }
}
