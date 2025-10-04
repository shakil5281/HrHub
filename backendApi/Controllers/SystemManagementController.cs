using HrHubAPI.DTOs;
using HrHubAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SystemManagementController : ControllerBase
    {
        private readonly IDatabaseManagementService _databaseService;
        private readonly IBackupService _backupService;
        private readonly ILogger<SystemManagementController> _logger;
        private readonly IConfiguration _configuration;

        public SystemManagementController(
            IDatabaseManagementService databaseService,
            IBackupService backupService,
            ILogger<SystemManagementController> logger,
            IConfiguration configuration)
        {
            _databaseService = databaseService;
            _backupService = backupService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Get system information
        /// </summary>
        /// <returns>System information</returns>
        [HttpGet("info")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<SystemInfoResponse>> GetSystemInfo()
        {
            try
            {
                var result = await _databaseService.GetSystemInfoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system information");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get database information
        /// </summary>
        /// <returns>Database information</returns>
        [HttpGet("database/info")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<DatabaseInfoResponse>> GetDatabaseInfo()
        {
            try
            {
                var result = await _databaseService.GetDatabaseInfoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database information");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get system health status
        /// </summary>
        /// <returns>System health information</returns>
        [HttpGet("health")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<SystemHealthResponse>> GetSystemHealth()
        {
            try
            {
                var result = await _databaseService.CheckDatabaseHealthAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking system health");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        /// <returns>System statistics</returns>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<SystemStatisticsResponse>> GetSystemStatistics()
        {
            try
            {
                var systemInfo = await _databaseService.GetSystemInfoAsync();
                var databaseInfo = await _databaseService.GetDatabaseInfoAsync();
                var backupStatus = await _backupService.GetBackupStatusAsync();

                var result = new SystemStatisticsResponse
                {
                    GeneratedAt = DateTime.UtcNow,
                    Database = new DatabaseStatistics
                    {
                        TotalTables = databaseInfo.TableCount,
                        TotalRecords = databaseInfo.Tables.Sum(t => t.RowCount),
                        DatabaseSizeBytes = databaseInfo.DatabaseSizeBytes,
                        DatabaseSizeFormatted = databaseInfo.DatabaseSizeFormatted,
                        ActiveConnections = 0, // Will be implemented in service
                        TotalBackups = backupStatus.TotalBackups,
                        LastBackupDate = backupStatus.LastBackupDate ?? DateTime.MinValue,
                        RecordsByTable = databaseInfo.Tables.ToDictionary(t => t.TableName, t => (int)t.RowCount)
                    },
                    Application = new ApplicationStatistics
                    {
                        StartTime = systemInfo.StartTime,
                        Uptime = systemInfo.Uptime,
                        TotalRequests = 0, // Will be implemented with request tracking
                        SuccessfulRequests = 0,
                        FailedRequests = 0,
                        AverageResponseTime = 0,
                        ActiveUsers = 0,
                        TotalUsers = 0
                    },
                    Users = new UserStatistics
                    {
                        TotalUsers = 0,
                        ActiveUsers = 0,
                        NewUsersThisMonth = 0,
                        UsersByRole = new Dictionary<string, int>(),
                        UsersByCompany = new Dictionary<string, int>(),
                        UsersWithDirectPermissions = 0
                    },
                    Performance = new SystemPerformanceStatistics
                    {
                        MemoryUsageBytes = systemInfo.MemoryUsage,
                        MemoryUsageFormatted = FormatBytes(systemInfo.MemoryUsage),
                        CpuUsagePercent = 0, // Will be implemented
                        DiskUsageBytes = 0,
                        DiskUsageFormatted = "0 B",
                        GcCollections = GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2),
                        GcMemory = GC.GetTotalMemory(false),
                        GcMemoryFormatted = FormatBytes(GC.GetTotalMemory(false))
                    }
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system statistics");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get table information
        /// </summary>
        /// <returns>List of tables with information</returns>
        [HttpGet("database/tables")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<TableInfo>>> GetTableInfo()
        {
            try
            {
                var result = await _databaseService.GetTableInfoAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving table information");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get specific table information
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <returns>Table information</returns>
        [HttpGet("database/tables/{tableName}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<TableInfo>> GetTableInfo(string tableName)
        {
            try
            {
                var result = await _databaseService.GetTableInfoAsync(tableName);
                if (result == null)
                {
                    return NotFound(new { message = "Table not found" });
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving table information: {tableName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get database performance metrics
        /// </summary>
        /// <returns>Database performance metrics</returns>
        [HttpGet("database/performance")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<Dictionary<string, object>>> GetDatabasePerformance()
        {
            try
            {
                var result = await _databaseService.GetDatabaseMetricsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database performance metrics");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get database connections
        /// </summary>
        /// <returns>List of database connections</returns>
        [HttpGet("database/connections")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<object>>> GetDatabaseConnections()
        {
            try
            {
                var result = await _databaseService.GetDatabaseConnectionsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database connections");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get slow queries
        /// </summary>
        /// <param name="limit">Number of queries to return</param>
        /// <returns>List of slow queries</returns>
        [HttpGet("database/slow-queries")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<object>>> GetSlowQueries([FromQuery] int limit = 10)
        {
            try
            {
                var result = await _databaseService.GetSlowQueriesAsync(limit);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving slow queries");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get index usage statistics
        /// </summary>
        /// <returns>Index usage statistics</returns>
        [HttpGet("database/index-usage")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<object>>> GetIndexUsageStats()
        {
            try
            {
                var result = await _databaseService.GetIndexUsageStatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving index usage statistics");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Check if database is online
        /// </summary>
        /// <returns>Database online status</returns>
        [HttpGet("database/status")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult> GetDatabaseStatus()
        {
            try
            {
                var isOnline = await _databaseService.IsDatabaseOnlineAsync();
                return Ok(new { isOnline, checkedAt = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking database status");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get database configuration
        /// </summary>
        /// <returns>Database configuration</returns>
        [HttpGet("database/configuration")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<Dictionary<string, object>>> GetDatabaseConfiguration()
        {
            try
            {
                var result = await _databaseService.GetDatabaseConfigurationAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database configuration");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get database alerts
        /// </summary>
        /// <returns>List of database alerts</returns>
        [HttpGet("database/alerts")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<object>>> GetDatabaseAlerts()
        {
            try
            {
                var result = await _databaseService.GetDatabaseAlertsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database alerts");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get system configuration
        /// </summary>
        /// <returns>System configuration</returns>
        [HttpGet("configuration")]
        [Authorize(Roles = "Admin,IT")]
        public Task<ActionResult<Dictionary<string, object>>> GetSystemConfiguration()
        {
            try
            {
                var config = new Dictionary<string, object>();

                // Get connection strings (without sensitive data)
                var connectionStrings = _configuration.GetSection("ConnectionStrings").GetChildren();
                foreach (var connectionString in connectionStrings)
                {
                    config[$"ConnectionStrings.{connectionString.Key}"] = "***HIDDEN***";
                }

                // Get other configuration sections
                var jwtSettings = _configuration.GetSection("JwtSettings").GetChildren();
                foreach (var setting in jwtSettings)
                {
                    if (setting.Key != "SecretKey")
                    {
                        config[$"JwtSettings.{setting.Key}"] = setting.Value ?? string.Empty;
                    }
                    else
                    {
                        config[$"JwtSettings.{setting.Key}"] = "***HIDDEN***";
                    }
                }

                var backupSettings = _configuration.GetSection("BackupSettings").GetChildren();
                foreach (var setting in backupSettings)
                {
                    config[$"BackupSettings.{setting.Key}"] = setting.Value ?? string.Empty;
                }

                return Task.FromResult<ActionResult<Dictionary<string, object>>>(Ok(config));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system configuration");
                return Task.FromResult<ActionResult<Dictionary<string, object>>>(StatusCode(500, new { message = "Internal server error", error = ex.Message }));
            }
        }

        /// <summary>
        /// Get application version information
        /// </summary>
        /// <returns>Version information</returns>
        [HttpGet("version")]
        [Authorize]
        public ActionResult GetVersion()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var version = assembly.GetName().Version;
                var fileVersion = FileVersionInfo.GetVersionInfo(assembly.Location);

                var result = new
                {
                    ApplicationName = assembly.GetName().Name,
                    Version = version?.ToString() ?? "Unknown",
                    FileVersion = fileVersion.FileVersion,
                    ProductVersion = fileVersion.ProductVersion,
                    BuildDate = System.IO.File.GetCreationTime(assembly.Location),
                    FrameworkVersion = Environment.Version.ToString(),
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving version information");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get system environment information
        /// </summary>
        /// <returns>Environment information</returns>
        [HttpGet("environment")]
        [Authorize(Roles = "Admin,IT")]
        public ActionResult GetEnvironment()
        {
            try
            {
                var result = new
                {
                    MachineName = Environment.MachineName,
                    UserName = Environment.UserName,
                    UserDomainName = Environment.UserDomainName,
                    OSVersion = Environment.OSVersion.ToString(),
                    ProcessorCount = Environment.ProcessorCount,
                    WorkingSet = Environment.WorkingSet,
                    WorkingSetFormatted = FormatBytes(Environment.WorkingSet),
                    SystemDirectory = Environment.SystemDirectory,
                    CurrentDirectory = Environment.CurrentDirectory,
                    CommandLine = Environment.CommandLine,
                    Is64BitOperatingSystem = Environment.Is64BitOperatingSystem,
                    Is64BitProcess = Environment.Is64BitProcess,
                    TickCount = Environment.TickCount,
                    EnvironmentVariables = Environment.GetEnvironmentVariables()
                        .Cast<System.Collections.DictionaryEntry>()
                        .Where(e => !e.Key.ToString()!.Contains("PASSWORD") && 
                                   !e.Key.ToString()!.Contains("SECRET") &&
                                   !e.Key.ToString()!.Contains("KEY"))
                        .ToDictionary(e => e.Key.ToString()!, e => e.Value?.ToString())
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving environment information");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        }

        private static string FormatBytes(long bytes)
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
    }
}
