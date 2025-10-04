using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.DTOs
{
    // System Information DTOs
    public class SystemInfoResponse
    {
        public string ApplicationName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string Environment { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public string ServerName { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string FrameworkVersion { get; set; } = string.Empty;
        public long MemoryUsage { get; set; }
        public int ProcessorCount { get; set; }
        public string DatabaseProvider { get; set; } = string.Empty;
        public string DatabaseVersion { get; set; } = string.Empty;
    }

    public class DatabaseInfoResponse
    {
        public string DatabaseName { get; set; } = string.Empty;
        public string ServerName { get; set; } = string.Empty;
        public string Provider { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public long DatabaseSizeBytes { get; set; }
        public string DatabaseSizeFormatted { get; set; } = string.Empty;
        public int TableCount { get; set; }
        public int IndexCount { get; set; }
        public int StoredProcedureCount { get; set; }
        public int FunctionCount { get; set; }
        public DateTime LastBackupDate { get; set; }
        public bool IsOnline { get; set; }
        public string Collation { get; set; } = string.Empty;
        public List<TableInfo> Tables { get; set; } = new();
    }

    public class TableInfo
    {
        public string TableName { get; set; } = string.Empty;
        public string Schema { get; set; } = string.Empty;
        public int RowCount { get; set; }
        public long SizeBytes { get; set; }
        public string SizeFormatted { get; set; } = string.Empty;
        public int ColumnCount { get; set; }
        public int IndexCount { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }

    // Database Management DTOs
    public class DatabaseMaintenanceRequest
    {
        [Required]
        public string Operation { get; set; } = string.Empty; // Rebuild, Reorganize, UpdateStats, CheckIntegrity
        
        public List<string>? TableNames { get; set; }
        
        public bool IncludeIndexes { get; set; } = true;
        
        public bool IncludeStatistics { get; set; } = true;
        
        public string? Description { get; set; }
    }

    public class DatabaseMaintenanceResponse
    {
        public string OperationId { get; set; } = string.Empty;
        public string Operation { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> AffectedTables { get; set; } = new();
        public Dictionary<string, object> Results { get; set; } = new();
    }

    public class DatabaseQueryRequest
    {
        [Required]
        public string Query { get; set; } = string.Empty;
        
        public int? TimeoutSeconds { get; set; } = 30;
        
        public bool IsReadOnly { get; set; } = true;
        
        public string? Description { get; set; }
    }

    public class DatabaseQueryResponse
    {
        public string QueryId { get; set; } = string.Empty;
        public string Query { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ExecutedAt { get; set; }
        public TimeSpan Duration { get; set; }
        public int RowsAffected { get; set; }
        public List<Dictionary<string, object>> Results { get; set; } = new();
        public string? ErrorMessage { get; set; }
        public List<string> Columns { get; set; } = new();
    }

    // Import/Export DTOs
    public class ExportDataRequest
    {
        [Required]
        public string ExportType { get; set; } = string.Empty; // Table, Query, Full
        
        public string? TableName { get; set; }
        
        public string? Query { get; set; }
        
        public string? FileName { get; set; }
        
        public string ExportFormat { get; set; } = "JSON"; // JSON, CSV, XML, SQL
        
        public bool IncludeSchema { get; set; } = false;
        
        public bool CompressFile { get; set; } = true;
        
        public string? Description { get; set; }
        
        public Dictionary<string, object>? Filters { get; set; }
    }

    public class ExportDataResponse
    {
        public string ExportId { get; set; } = string.Empty;
        public string ExportType { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string FileSizeFormatted { get; set; } = string.Empty;
        public string ExportFormat { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int RecordCount { get; set; }
        public TimeSpan Duration { get; set; }
        public string? ErrorMessage { get; set; }
        public bool IsCompressed { get; set; }
    }

    public class ImportDataRequest
    {
        [Required]
        public string ImportType { get; set; } = string.Empty; // Table, File
        
        public string? TableName { get; set; }
        
        public string? FileName { get; set; }
        
        public string ImportFormat { get; set; } = "JSON"; // JSON, CSV, XML, SQL
        
        public bool TruncateTable { get; set; } = false;
        
        public bool SkipErrors { get; set; } = false;
        
        public int? BatchSize { get; set; } = 1000;
        
        public string? Description { get; set; }
        
        public Dictionary<string, string>? ColumnMappings { get; set; }
    }

    public class ImportDataResponse
    {
        public string ImportId { get; set; } = string.Empty;
        public string ImportType { get; set; } = string.Empty;
        public string TableName { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ImportFormat { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalRecords { get; set; }
        public int ProcessedRecords { get; set; }
        public int SuccessfulRecords { get; set; }
        public int FailedRecords { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? ErrorMessage { get; set; }
        public List<string> Errors { get; set; } = new();
        public Dictionary<string, object> Statistics { get; set; } = new();
    }

    // System Health DTOs
    public class SystemHealthResponse
    {
        public string OverallStatus { get; set; } = string.Empty;
        public DateTime CheckedAt { get; set; }
        public List<HealthCheck> HealthChecks { get; set; } = new();
        public SystemMetrics Metrics { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    public class HealthCheck
    {
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public string? ErrorMessage { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }

    public class SystemMetrics
    {
        public long MemoryUsageBytes { get; set; }
        public string MemoryUsageFormatted { get; set; } = string.Empty;
        public double CpuUsagePercent { get; set; }
        public long DiskUsageBytes { get; set; }
        public string DiskUsageFormatted { get; set; } = string.Empty;
        public int ActiveConnections { get; set; }
        public int TotalRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public int ErrorCount { get; set; }
    }

    // System Configuration DTOs
    public class SystemConfigurationRequest
    {
        [Required]
        public string ConfigurationKey { get; set; } = string.Empty;
        
        [Required]
        public string ConfigurationValue { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        public string? Category { get; set; }
        
        public bool IsEncrypted { get; set; } = false;
    }

    public class SystemConfigurationResponse
    {
        public string Id { get; set; } = string.Empty;
        public string ConfigurationKey { get; set; } = string.Empty;
        public string ConfigurationValue { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool IsEncrypted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }

    // System Log DTOs
    public class SystemLogQueryRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string? Level { get; set; } // Info, Warning, Error, Debug
        public string? Source { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? SearchTerm { get; set; }
        public string? SortBy { get; set; } = "Timestamp";
        public string? SortDirection { get; set; } = "desc";
    }

    public class SystemLogResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; }
        public DateTime Timestamp { get; set; }
        public string? UserId { get; set; }
        public string? RequestId { get; set; }
        public Dictionary<string, object> Properties { get; set; } = new();
    }

    public class SystemLogListResponse
    {
        public List<SystemLogResponse> Logs { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    // System Statistics DTOs
    public class SystemStatisticsResponse
    {
        public DateTime GeneratedAt { get; set; }
        public DatabaseStatistics Database { get; set; } = new();
        public ApplicationStatistics Application { get; set; } = new();
        public UserStatistics Users { get; set; } = new();
        public SystemPerformanceStatistics Performance { get; set; } = new();
    }

    public class DatabaseStatistics
    {
        public int TotalTables { get; set; }
        public int TotalRecords { get; set; }
        public long DatabaseSizeBytes { get; set; }
        public string DatabaseSizeFormatted { get; set; } = string.Empty;
        public int ActiveConnections { get; set; }
        public int TotalBackups { get; set; }
        public DateTime LastBackupDate { get; set; }
        public Dictionary<string, int> RecordsByTable { get; set; } = new();
    }

    public class ApplicationStatistics
    {
        public DateTime StartTime { get; set; }
        public TimeSpan Uptime { get; set; }
        public int TotalRequests { get; set; }
        public int SuccessfulRequests { get; set; }
        public int FailedRequests { get; set; }
        public double AverageResponseTime { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalUsers { get; set; }
    }

    public class UserStatistics
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public Dictionary<string, int> UsersByCompany { get; set; } = new();
        public int UsersWithDirectPermissions { get; set; }
    }

    public class SystemPerformanceStatistics
    {
        public long MemoryUsageBytes { get; set; }
        public string MemoryUsageFormatted { get; set; } = string.Empty;
        public double CpuUsagePercent { get; set; }
        public long DiskUsageBytes { get; set; }
        public string DiskUsageFormatted { get; set; } = string.Empty;
        public int GcCollections { get; set; }
        public long GcMemory { get; set; }
        public string GcMemoryFormatted { get; set; } = string.Empty;
    }
}
