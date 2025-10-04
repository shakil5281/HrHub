using HrHubAPI.DTOs;

namespace HrHubAPI.Services
{
    public interface IImportExportService
    {
        // Export Operations
        Task<ExportDataResponse> ExportDataAsync(ExportDataRequest request, string userId);
        Task<ExportDataResponse> ExportTableAsync(string tableName, string format, string userId);
        Task<ExportDataResponse> ExportQueryResultsAsync(string query, string format, string userId);
        Task<ExportDataResponse> ExportFullDatabaseAsync(string userId);
        Task<byte[]> DownloadExportAsync(string exportId);
        Task<List<ExportDataResponse>> GetExportHistoryAsync(string userId);

        // Import Operations
        Task<ImportDataResponse> ImportDataAsync(ImportDataRequest request, string userId);
        Task<ImportDataResponse> ImportFromFileAsync(string fileName, string tableName, string format, string userId);
        Task<ImportDataResponse> ImportFromJsonAsync(string jsonData, string tableName, string userId);
        Task<ImportDataResponse> ImportFromCsvAsync(string csvData, string tableName, string userId);
        Task<ImportDataResponse> ImportFromXmlAsync(string xmlData, string tableName, string userId);
        Task<List<ImportDataResponse>> GetImportHistoryAsync(string userId);

        // File Management
        Task<List<string>> GetAvailableFilesAsync(string directory);
        Task<bool> ValidateImportFileAsync(string fileName, string format);
        Task<Dictionary<string, object>> GetFileInfoAsync(string fileName);
        Task<bool> DeleteExportFileAsync(string exportId, string userId);
        Task<bool> CleanupOldExportsAsync(int daysOld, string userId);

        // Data Validation
        Task<Dictionary<string, object>> ValidateImportDataAsync(string data, string format, string tableName);
        Task<List<string>> GetValidationErrorsAsync(string data, string format, string tableName);
        Task<Dictionary<string, object>> PreviewImportDataAsync(string data, string format, int maxRows = 10);

        // Format Support
        Task<List<string>> GetSupportedExportFormatsAsync();
        Task<List<string>> GetSupportedImportFormatsAsync();
        Task<Dictionary<string, object>> GetFormatSpecificationsAsync(string format);

        // Bulk Operations
        Task<ImportDataResponse> BulkImportAsync(List<ImportDataRequest> requests, string userId);
        Task<ExportDataResponse> BulkExportAsync(List<ExportDataRequest> requests, string userId);
        Task<Dictionary<string, object>> GetBulkOperationStatusAsync(string operationId);

        // Data Transformation
        Task<string> TransformDataAsync(string data, string sourceFormat, string targetFormat);
        Task<Dictionary<string, object>> GetDataMappingAsync(string sourceFormat, string targetFormat);
        Task<string> ApplyDataMappingAsync(string data, Dictionary<string, string> mappings);

        // Template Management
        Task<string> CreateImportTemplateAsync(string tableName, string format);
        Task<string> CreateExportTemplateAsync(string tableName, string format);
        Task<List<string>> GetAvailableTemplatesAsync();
        Task<bool> DeleteTemplateAsync(string templateName, string userId);

        // Error Handling and Recovery
        Task<ImportDataResponse> RetryImportAsync(string importId, string userId);
        Task<List<string>> GetImportErrorsAsync(string importId);
        Task<Dictionary<string, object>> GetImportStatisticsAsync(string importId);
        Task<bool> RollbackImportAsync(string importId, string userId);

        // Data Quality
        Task<Dictionary<string, object>> AnalyzeDataQualityAsync(string tableName);
        Task<List<string>> FindDuplicateRecordsAsync(string tableName, List<string> columns);
        Task<Dictionary<string, object>> GetDataStatisticsAsync(string tableName);
        Task<List<string>> ValidateDataIntegrityAsync(string tableName);

        // Scheduling
        Task<string> ScheduleExportAsync(ExportDataRequest request, string cronExpression, string userId);
        Task<string> ScheduleImportAsync(ImportDataRequest request, string cronExpression, string userId);
        Task<List<string>> GetScheduledOperationsAsync();
        Task<bool> CancelScheduledOperationAsync(string operationId, string userId);

        // Integration
        Task<ExportDataResponse> ExportToExternalSystemAsync(string exportId, string systemName, string userId);
        Task<ImportDataResponse> ImportFromExternalSystemAsync(string systemName, string tableName, string userId);
        Task<List<string>> GetAvailableExternalSystemsAsync();
        Task<Dictionary<string, object>> TestExternalSystemConnectionAsync(string systemName);
    }
}
