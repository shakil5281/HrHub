using HrHubAPI.DTOs;

namespace HrHubAPI.Services
{
    public interface IDatabaseManagementService
    {
        // Database Information
        Task<DatabaseInfoResponse> GetDatabaseInfoAsync();
        Task<List<TableInfo>> GetTableInfoAsync();
        Task<TableInfo?> GetTableInfoAsync(string tableName);
        Task<SystemInfoResponse> GetSystemInfoAsync();

        // Database Maintenance
        Task<DatabaseMaintenanceResponse> PerformMaintenanceAsync(DatabaseMaintenanceRequest request, string userId);
        Task<DatabaseMaintenanceResponse> RebuildIndexesAsync(List<string>? tableNames, string userId);
        Task<DatabaseMaintenanceResponse> ReorganizeIndexesAsync(List<string>? tableNames, string userId);
        Task<DatabaseMaintenanceResponse> UpdateStatisticsAsync(List<string>? tableNames, string userId);
        Task<DatabaseMaintenanceResponse> CheckIntegrityAsync(List<string>? tableNames, string userId);
        Task<DatabaseMaintenanceResponse> ShrinkDatabaseAsync(string userId);

        // Database Queries
        Task<DatabaseQueryResponse> ExecuteQueryAsync(DatabaseQueryRequest request, string userId);
        Task<DatabaseQueryResponse> ExecuteReadOnlyQueryAsync(string query, string userId);
        Task<bool> ValidateQueryAsync(string query);

        // Database Schema Operations
        Task<List<string>> GetTableNamesAsync();
        Task<List<string>> GetColumnNamesAsync(string tableName);
        Task<Dictionary<string, string>> GetTableSchemaAsync(string tableName);
        Task<bool> TableExistsAsync(string tableName);
        Task<long> GetTableRowCountAsync(string tableName);

        // Database Performance
        Task<List<object>> GetSlowQueriesAsync(int limit = 10);
        Task<List<object>> GetDatabaseConnectionsAsync();
        Task<Dictionary<string, object>> GetDatabaseMetricsAsync();
        Task<List<object>> GetIndexUsageStatsAsync();

        // Database Backup Integration
        Task<bool> CreateFullBackupAsync(string backupName, string userId);
        Task<bool> CreateDifferentialBackupAsync(string backupName, string userId);
        Task<bool> CreateTransactionLogBackupAsync(string backupName, string userId);
        Task<List<string>> GetBackupHistoryAsync();

        // Database Security
        Task<List<object>> GetDatabaseUsersAsync();
        Task<List<object>> GetDatabaseRolesAsync();
        Task<bool> CheckDatabasePermissionsAsync(string userId);
        Task<Dictionary<string, bool>> GetUserPermissionsAsync(string userId);

        // Database Monitoring
        Task<SystemHealthResponse> CheckDatabaseHealthAsync();
        Task<List<object>> GetDatabaseAlertsAsync();
        Task<Dictionary<string, object>> GetDatabasePerformanceCountersAsync();
        Task<bool> IsDatabaseOnlineAsync();

        // Database Configuration
        Task<Dictionary<string, object>> GetDatabaseConfigurationAsync();
        Task<bool> UpdateDatabaseConfigurationAsync(string key, string value, string userId);
        Task<List<object>> GetDatabaseSettingsAsync();

        // Database Cleanup
        Task<DatabaseMaintenanceResponse> CleanupOrphanedDataAsync(string userId);
        Task<DatabaseMaintenanceResponse> ArchiveOldDataAsync(int daysOld, string userId);
        Task<DatabaseMaintenanceResponse> OptimizeDatabaseAsync(string userId);
    }
}
