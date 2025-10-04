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
    public class ImportExportController : ControllerBase
    {
        private readonly IImportExportService _importExportService;
        private readonly ILogger<ImportExportController> _logger;

        public ImportExportController(
            IImportExportService importExportService,
            ILogger<ImportExportController> logger)
        {
            _importExportService = importExportService;
            _logger = logger;
        }

        /// <summary>
        /// Export data from database
        /// </summary>
        /// <param name="request">Export request</param>
        /// <returns>Export operation result</returns>
        [HttpPost("export")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<ExportDataResponse>> ExportData([FromBody] ExportDataRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ExportDataAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export specific table
        /// </summary>
        /// <param name="tableName">Table name</param>
        /// <param name="format">Export format (JSON, CSV, XML, SQL)</param>
        /// <returns>Export operation result</returns>
        [HttpPost("export/table/{tableName}")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<ExportDataResponse>> ExportTable(string tableName, [FromQuery] string format = "JSON")
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ExportTableAsync(tableName, format, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exporting table: {tableName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export query results
        /// </summary>
        /// <param name="query">SQL query</param>
        /// <param name="format">Export format (JSON, CSV, XML, SQL)</param>
        /// <returns>Export operation result</returns>
        [HttpPost("export/query")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<ExportDataResponse>> ExportQueryResults([FromBody] string query, [FromQuery] string format = "JSON")
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ExportQueryResultsAsync(query, format, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting query results");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Export full database
        /// </summary>
        /// <returns>Export operation result</returns>
        [HttpPost("export/database")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<ExportDataResponse>> ExportFullDatabase()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ExportFullDatabaseAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting full database");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Download exported file
        /// </summary>
        /// <param name="exportId">Export ID</param>
        /// <returns>File download</returns>
        [HttpGet("export/{exportId}/download")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult> DownloadExport(string exportId)
        {
            try
            {
                var fileBytes = await _importExportService.DownloadExportAsync(exportId);
                return File(fileBytes, "application/octet-stream", $"export_{exportId}.zip");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading export: {exportId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get export history
        /// </summary>
        /// <returns>List of export operations</returns>
        [HttpGet("export/history")]
        [Authorize(Roles = "Admin,IT,HR Manager")]
        public async Task<ActionResult<List<ExportDataResponse>>> GetExportHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.GetExportHistoryAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving export history");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import data to database
        /// </summary>
        /// <param name="request">Import request</param>
        /// <returns>Import operation result</returns>
        [HttpPost("import")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<ImportDataResponse>> ImportData([FromBody] ImportDataRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ImportDataAsync(request, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing data");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import data from file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="tableName">Target table name</param>
        /// <param name="format">Import format (JSON, CSV, XML, SQL)</param>
        /// <returns>Import operation result</returns>
        [HttpPost("import/file")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<ImportDataResponse>> ImportFromFile([FromQuery] string fileName, [FromQuery] string tableName, [FromQuery] string format = "JSON")
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ImportFromFileAsync(fileName, tableName, format, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error importing from file: {fileName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import data from JSON
        /// </summary>
        /// <param name="jsonData">JSON data</param>
        /// <param name="tableName">Target table name</param>
        /// <returns>Import operation result</returns>
        [HttpPost("import/json")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<ImportDataResponse>> ImportFromJson([FromBody] string jsonData, [FromQuery] string tableName)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ImportFromJsonAsync(jsonData, tableName, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing from JSON");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import data from CSV
        /// </summary>
        /// <param name="csvData">CSV data</param>
        /// <param name="tableName">Target table name</param>
        /// <returns>Import operation result</returns>
        [HttpPost("import/csv")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<ImportDataResponse>> ImportFromCsv([FromBody] string csvData, [FromQuery] string tableName)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ImportFromCsvAsync(csvData, tableName, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing from CSV");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Import data from XML
        /// </summary>
        /// <param name="xmlData">XML data</param>
        /// <param name="tableName">Target table name</param>
        /// <returns>Import operation result</returns>
        [HttpPost("import/xml")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<ImportDataResponse>> ImportFromXml([FromBody] string xmlData, [FromQuery] string tableName)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.ImportFromXmlAsync(xmlData, tableName, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing from XML");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get import history
        /// </summary>
        /// <returns>List of import operations</returns>
        [HttpGet("import/history")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<ImportDataResponse>>> GetImportHistory()
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.GetImportHistoryAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving import history");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get available files for import
        /// </summary>
        /// <param name="directory">Directory to search</param>
        /// <returns>List of available files</returns>
        [HttpGet("files")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<string>>> GetAvailableFiles([FromQuery] string directory = "Imports")
        {
            try
            {
                var result = await _importExportService.GetAvailableFilesAsync(directory);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving available files from directory: {directory}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate import file
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="format">File format</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate/file")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> ValidateImportFile([FromQuery] string fileName, [FromQuery] string format)
        {
            try
            {
                var isValid = await _importExportService.ValidateImportFileAsync(fileName, format);
                return Ok(new { fileName, format, isValid, message = isValid ? "File is valid" : "File is invalid" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating import file: {fileName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get file information
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <returns>File information</returns>
        [HttpGet("files/{fileName}/info")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<Dictionary<string, object>>> GetFileInfo(string fileName)
        {
            try
            {
                var result = await _importExportService.GetFileInfoAsync(fileName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving file info: {fileName}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete export file
        /// </summary>
        /// <param name="exportId">Export ID</param>
        /// <returns>Deletion result</returns>
        [HttpDelete("export/{exportId}")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> DeleteExportFile(string exportId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.DeleteExportFileAsync(exportId, userId);
                
                if (!result)
                {
                    return NotFound(new { message = "Export file not found" });
                }

                return Ok(new { message = "Export file deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting export file: {exportId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Cleanup old exports
        /// </summary>
        /// <param name="daysOld">Number of days old</param>
        /// <returns>Cleanup result</returns>
        [HttpPost("cleanup/exports")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> CleanupOldExports([FromQuery] int daysOld = 30)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.CleanupOldExportsAsync(daysOld, userId);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to cleanup old exports" });
                }

                return Ok(new { message = "Old exports cleaned up successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning up old exports");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get supported export formats
        /// </summary>
        /// <returns>List of supported export formats</returns>
        [HttpGet("formats/export")]
        [Authorize]
        public async Task<ActionResult<List<string>>> GetSupportedExportFormats()
        {
            try
            {
                var result = await _importExportService.GetSupportedExportFormatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving supported export formats");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get supported import formats
        /// </summary>
        /// <returns>List of supported import formats</returns>
        [HttpGet("formats/import")]
        [Authorize]
        public async Task<ActionResult<List<string>>> GetSupportedImportFormats()
        {
            try
            {
                var result = await _importExportService.GetSupportedImportFormatsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving supported import formats");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get format specifications
        /// </summary>
        /// <param name="format">Format name</param>
        /// <returns>Format specifications</returns>
        [HttpGet("formats/{format}/specifications")]
        [Authorize]
        public async Task<ActionResult<Dictionary<string, object>>> GetFormatSpecifications(string format)
        {
            try
            {
                var result = await _importExportService.GetFormatSpecificationsAsync(format);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving format specifications: {format}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Preview import data
        /// </summary>
        /// <param name="data">Data to preview</param>
        /// <param name="format">Data format</param>
        /// <param name="maxRows">Maximum rows to preview</param>
        /// <returns>Preview data</returns>
        [HttpPost("preview")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<Dictionary<string, object>>> PreviewImportData([FromBody] string data, [FromQuery] string format, [FromQuery] int maxRows = 10)
        {
            try
            {
                var result = await _importExportService.PreviewImportDataAsync(data, format, maxRows);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error previewing import data");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Validate import data
        /// </summary>
        /// <param name="data">Data to validate</param>
        /// <param name="format">Data format</param>
        /// <param name="tableName">Target table name</param>
        /// <returns>Validation result</returns>
        [HttpPost("validate/data")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<Dictionary<string, object>>> ValidateImportData([FromBody] string data, [FromQuery] string format, [FromQuery] string tableName)
        {
            try
            {
                var result = await _importExportService.ValidateImportDataAsync(data, format, tableName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating import data");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get import errors
        /// </summary>
        /// <param name="importId">Import ID</param>
        /// <returns>List of import errors</returns>
        [HttpGet("import/{importId}/errors")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<List<string>>> GetImportErrors(string importId)
        {
            try
            {
                var result = await _importExportService.GetImportErrorsAsync(importId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving import errors: {importId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get import statistics
        /// </summary>
        /// <param name="importId">Import ID</param>
        /// <returns>Import statistics</returns>
        [HttpGet("import/{importId}/statistics")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<Dictionary<string, object>>> GetImportStatistics(string importId)
        {
            try
            {
                var result = await _importExportService.GetImportStatisticsAsync(importId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving import statistics: {importId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Retry failed import
        /// </summary>
        /// <param name="importId">Import ID</param>
        /// <returns>Retry result</returns>
        [HttpPost("import/{importId}/retry")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult<ImportDataResponse>> RetryImport(string importId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.RetryImportAsync(importId, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrying import: {importId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Rollback import
        /// </summary>
        /// <param name="importId">Import ID</param>
        /// <returns>Rollback result</returns>
        [HttpPost("import/{importId}/rollback")]
        [Authorize(Roles = "Admin,IT")]
        public async Task<ActionResult> RollbackImport(string importId)
        {
            try
            {
                var userId = GetCurrentUserId();
                var result = await _importExportService.RollbackImportAsync(importId, userId);
                
                if (!result)
                {
                    return BadRequest(new { message = "Failed to rollback import" });
                }

                return Ok(new { message = "Import rolled back successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rolling back import: {importId}");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
        }
    }
}
