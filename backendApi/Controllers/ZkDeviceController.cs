using HrHubAPI.DTOs;
using HrHubAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace HrHubAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ZkDeviceController : ControllerBase
    {
        private readonly IZkDeviceService _zkDeviceService;
        private readonly ILogger<ZkDeviceController> _logger;

        public ZkDeviceController(IZkDeviceService zkDeviceService, ILogger<ZkDeviceController> logger)
        {
            _zkDeviceService = zkDeviceService;
            _logger = logger;
        }

        #region Device Management

        /// <summary>
        /// Get all ZK devices
        /// </summary>
        [HttpGet("devices")]
        public async Task<ActionResult<List<ZkDeviceDto>>> GetAllDevices()
        {
            try
            {
                var devices = await _zkDeviceService.GetAllDevicesAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all devices");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get device by ID
        /// </summary>
        [HttpGet("devices/{deviceId}")]
        public async Task<ActionResult<ZkDeviceDto>> GetDeviceById(int deviceId)
        {
            try
            {
                var device = await _zkDeviceService.GetDeviceByIdAsync(deviceId);
                if (device == null)
                    return NotFound(new { message = "Device not found" });

                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Create new ZK device
        /// </summary>
        [HttpPost("devices")]
        public async Task<ActionResult<ZkDeviceDto>> CreateDevice([FromBody] CreateZkDeviceDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var device = await _zkDeviceService.CreateDeviceAsync(createDto);
                return CreatedAtAction(nameof(GetDeviceById), new { deviceId = device.Id }, device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating device");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Update ZK device
        /// </summary>
        [HttpPut("devices/{deviceId}")]
        public async Task<ActionResult<ZkDeviceDto>> UpdateDevice(int deviceId, [FromBody] UpdateZkDeviceDto updateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var device = await _zkDeviceService.UpdateDeviceAsync(deviceId, updateDto);
                if (device == null)
                    return NotFound(new { message = "Device not found" });

                return Ok(device);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating device {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete ZK device
        /// </summary>
        [HttpDelete("devices/{deviceId}")]
        public async Task<ActionResult> DeleteDevice(int deviceId)
        {
            try
            {
                var result = await _zkDeviceService.DeleteDeviceAsync(deviceId);
                if (!result)
                    return NotFound(new { message = "Device not found" });

                return Ok(new { message = "Device deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting device {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Device Connection

        /// <summary>
        /// Test connection to a specific device
        /// </summary>
        [HttpPost("devices/{deviceId}/test-connection")]
        public async Task<ActionResult<DeviceConnectionTestDto>> TestDeviceConnection(int deviceId)
        {
            try
            {
                var result = await _zkDeviceService.TestDeviceConnectionAsync(deviceId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection to device {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Test connection by IP and port
        /// </summary>
        [HttpPost("test-connection")]
        public async Task<ActionResult<DeviceConnectionTestDto>> TestConnectionByIp([FromBody] TestConnectionRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _zkDeviceService.TestDeviceConnectionByIpAsync(request.IpAddress, request.Port);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing connection to {IpAddress}:{Port}", request.IpAddress, request.Port);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Test connection to all devices
        /// </summary>
        [HttpPost("devices/test-all-connections")]
        public async Task<ActionResult<List<DeviceConnectionTestDto>>> TestAllDevicesConnection()
        {
            try
            {
                var results = await _zkDeviceService.TestAllDevicesConnectionAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing all device connections");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Log Download

        /// <summary>
        /// Download logs from a specific device
        /// </summary>
        [HttpPost("devices/{deviceId}/download-logs")]
        public async Task<ActionResult<DownloadLogsResponseDto>> DownloadLogsFromDevice(int deviceId, [FromBody] DownloadLogsRequestDto? request = null)
        {
            try
            {
                var result = await _zkDeviceService.DownloadLogsFromDeviceAsync(deviceId, request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading logs from device {DeviceId}", deviceId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Download logs from all devices
        /// </summary>
        [HttpPost("devices/download-all-logs")]
        public async Task<ActionResult<DownloadLogsResponseDto>> DownloadLogsFromAllDevices([FromBody] DownloadLogsRequestDto? request = null)
        {
            try
            {
                var result = await _zkDeviceService.DownloadLogsFromAllDevicesAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading logs from all devices");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Log Management

        /// <summary>
        /// Get attendance logs with filtering and pagination
        /// </summary>
        [HttpGet("logs")]
        public async Task<ActionResult<AttendanceLogResponseDto>> GetAttendanceLogs([FromQuery] AttendanceLogQueryDto query)
        {
            try
            {
                var result = await _zkDeviceService.GetAttendanceLogsAsync(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance logs");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get attendance log by ID
        /// </summary>
        [HttpGet("logs/{logId}")]
        public async Task<ActionResult<AttendanceLogDto>> GetAttendanceLogById(int logId)
        {
            try
            {
                var log = await _zkDeviceService.GetAttendanceLogByIdAsync(logId);
                if (log == null)
                    return NotFound(new { message = "Log not found" });

                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting attendance log {LogId}", logId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Mark log as processed
        /// </summary>
        [HttpPut("logs/{logId}/mark-processed")]
        public async Task<ActionResult> MarkLogAsProcessed(int logId)
        {
            try
            {
                var result = await _zkDeviceService.MarkLogAsProcessedAsync(logId);
                if (!result)
                    return NotFound(new { message = "Log not found" });

                return Ok(new { message = "Log marked as processed" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking log as processed {LogId}", logId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Delete attendance log
        /// </summary>
        [HttpDelete("logs/{logId}")]
        public async Task<ActionResult> DeleteAttendanceLog(int logId)
        {
            try
            {
                var result = await _zkDeviceService.DeleteAttendanceLogAsync(logId);
                if (!result)
                    return NotFound(new { message = "Log not found" });

                return Ok(new { message = "Log deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting attendance log {LogId}", logId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Statistics

        /// <summary>
        /// Get device statistics
        /// </summary>
        [HttpGet("statistics/devices")]
        public async Task<ActionResult<object>> GetDeviceStatistics()
        {
            try
            {
                var stats = await _zkDeviceService.GetDeviceStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting device statistics");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Get log statistics
        /// </summary>
        [HttpGet("statistics/logs")]
        public async Task<ActionResult<object>> GetLogStatistics()
        {
            try
            {
                var stats = await _zkDeviceService.GetLogStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting log statistics");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion

        #region Anonymous Test Endpoints

        /// <summary>
        /// Test endpoint - Get all devices (Anonymous)
        /// </summary>
        [HttpGet("test/devices")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ZkDeviceDto>>> TestGetAllDevices()
        {
            try
            {
                var devices = await _zkDeviceService.GetAllDevicesAsync();
                return Ok(devices);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test getting all devices");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint - Download logs from all devices (Anonymous)
        /// </summary>
        [HttpPost("test/download-all-logs")]
        [AllowAnonymous]
        public async Task<ActionResult<DownloadLogsResponseDto>> TestDownloadAllLogs()
        {
            try
            {
                var result = await _zkDeviceService.DownloadLogsFromAllDevicesAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test downloading all logs");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// Test endpoint - Test all device connections (Anonymous)
        /// </summary>
        [HttpPost("test/test-all-connections")]
        [AllowAnonymous]
        public async Task<ActionResult<List<DeviceConnectionTestDto>>> TestAllConnections()
        {
            try
            {
                var results = await _zkDeviceService.TestAllDevicesConnectionAsync();
                return Ok(results);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in test testing all connections");
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        #endregion
    }

    public class TestConnectionRequestDto
    {
        [Required]
        public string IpAddress { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 65535)]
        public int Port { get; set; } = 4370;
    }
}
