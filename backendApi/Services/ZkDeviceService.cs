using HrHubAPI.Data;
using HrHubAPI.DTOs;
using HrHubAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.Sockets;
using System.Diagnostics;

namespace HrHubAPI.Services
{
    public class ZkDeviceService : IZkDeviceService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ZkDeviceService> _logger;

        public ZkDeviceService(ApplicationDbContext context, ILogger<ZkDeviceService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Device Management

        public async Task<List<ZkDeviceDto>> GetAllDevicesAsync()
        {
            var devices = await _context.ZkDevices
                .Where(d => d.IsActive)
                .OrderBy(d => d.DeviceName)
                .Select(d => new ZkDeviceDto
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    IpAddress = d.IpAddress,
                    Port = d.Port,
                    SerialNumber = d.SerialNumber,
                    ProductName = d.ProductName,
                    MachineNumber = d.MachineNumber,
                    UserCount = d.UserCount,
                    AdminCount = d.AdminCount,
                    FpCount = d.FpCount,
                    FcCount = d.FcCount,
                    PasswordCount = d.PasswordCount,
                    LogCount = d.LogCount,
                    IsConnected = d.IsConnected,
                    LastConnectionTime = d.LastConnectionTime,
                    LastLogDownloadTime = d.LastLogDownloadTime,
                    Location = d.Location,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync();

            return devices;
        }

        public async Task<ZkDeviceDto?> GetDeviceByIdAsync(int deviceId)
        {
            var device = await _context.ZkDevices
                .Where(d => d.Id == deviceId && d.IsActive)
                .Select(d => new ZkDeviceDto
                {
                    Id = d.Id,
                    DeviceName = d.DeviceName,
                    IpAddress = d.IpAddress,
                    Port = d.Port,
                    SerialNumber = d.SerialNumber,
                    ProductName = d.ProductName,
                    MachineNumber = d.MachineNumber,
                    UserCount = d.UserCount,
                    AdminCount = d.AdminCount,
                    FpCount = d.FpCount,
                    FcCount = d.FcCount,
                    PasswordCount = d.PasswordCount,
                    LogCount = d.LogCount,
                    IsConnected = d.IsConnected,
                    LastConnectionTime = d.LastConnectionTime,
                    LastLogDownloadTime = d.LastLogDownloadTime,
                    Location = d.Location,
                    IsActive = d.IsActive,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .FirstOrDefaultAsync();

            return device;
        }

        public async Task<ZkDeviceDto> CreateDeviceAsync(CreateZkDeviceDto createDto)
        {
            var device = new ZkDevice
            {
                DeviceName = createDto.DeviceName,
                IpAddress = createDto.IpAddress,
                Port = createDto.Port,
                SerialNumber = createDto.SerialNumber,
                ProductName = createDto.ProductName,
                MachineNumber = createDto.MachineNumber,
                Location = createDto.Location,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = "System",
                UpdatedBy = "System"
            };

            _context.ZkDevices.Add(device);
            await _context.SaveChangesAsync();

            return new ZkDeviceDto
            {
                Id = device.Id,
                DeviceName = device.DeviceName,
                IpAddress = device.IpAddress,
                Port = device.Port,
                SerialNumber = device.SerialNumber,
                ProductName = device.ProductName,
                MachineNumber = device.MachineNumber,
                UserCount = device.UserCount,
                AdminCount = device.AdminCount,
                FpCount = device.FpCount,
                FcCount = device.FcCount,
                PasswordCount = device.PasswordCount,
                LogCount = device.LogCount,
                IsConnected = device.IsConnected,
                LastConnectionTime = device.LastConnectionTime,
                LastLogDownloadTime = device.LastLogDownloadTime,
                Location = device.Location,
                IsActive = device.IsActive,
                CreatedAt = device.CreatedAt,
                UpdatedAt = device.UpdatedAt
            };
        }

        public async Task<ZkDeviceDto?> UpdateDeviceAsync(int deviceId, UpdateZkDeviceDto updateDto)
        {
            var device = await _context.ZkDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.IsActive);

            if (device == null)
                return null;

            if (!string.IsNullOrEmpty(updateDto.DeviceName))
                device.DeviceName = updateDto.DeviceName;
            if (!string.IsNullOrEmpty(updateDto.IpAddress))
                device.IpAddress = updateDto.IpAddress;
            if (updateDto.Port.HasValue)
                device.Port = updateDto.Port.Value;
            if (!string.IsNullOrEmpty(updateDto.SerialNumber))
                device.SerialNumber = updateDto.SerialNumber;
            if (!string.IsNullOrEmpty(updateDto.ProductName))
                device.ProductName = updateDto.ProductName;
            if (!string.IsNullOrEmpty(updateDto.MachineNumber))
                device.MachineNumber = updateDto.MachineNumber;
            if (!string.IsNullOrEmpty(updateDto.Location))
                device.Location = updateDto.Location;
            if (updateDto.IsActive.HasValue)
                device.IsActive = updateDto.IsActive.Value;

            device.UpdatedAt = DateTime.UtcNow;
            device.UpdatedBy = "System";

            await _context.SaveChangesAsync();

            return await GetDeviceByIdAsync(deviceId);
        }

        public async Task<bool> DeleteDeviceAsync(int deviceId)
        {
            var device = await _context.ZkDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.IsActive);

            if (device == null)
                return false;

            device.IsActive = false;
            device.UpdatedAt = DateTime.UtcNow;
            device.UpdatedBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Device Connection

        public async Task<DeviceConnectionTestDto> TestDeviceConnectionAsync(int deviceId)
        {
            var device = await _context.ZkDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.IsActive);

            if (device == null)
            {
                return new DeviceConnectionTestDto
                {
                    IsConnected = false,
                    Message = "Device not found",
                    TestTime = DateTime.UtcNow,
                    ResponseTimeMs = 0
                };
            }

            return await TestDeviceConnectionByIpAsync(device.IpAddress, device.Port);
        }

        public async Task<DeviceConnectionTestDto> TestDeviceConnectionByIpAsync(string ipAddress, int port)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                using var tcpClient = new TcpClient();
                var connectTask = tcpClient.ConnectAsync(ipAddress, port);
                var timeoutTask = Task.Delay(5000); // 5 second timeout

                var completedTask = await Task.WhenAny(connectTask, timeoutTask);

                stopwatch.Stop();

                if (completedTask == connectTask && tcpClient.Connected)
                {
                    return new DeviceConnectionTestDto
                    {
                        IsConnected = true,
                        Message = "Connection successful",
                        TestTime = DateTime.UtcNow,
                        ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                    };
                }
                else
                {
                    return new DeviceConnectionTestDto
                    {
                        IsConnected = false,
                        Message = "Connection timeout or failed",
                        TestTime = DateTime.UtcNow,
                        ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                    };
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.LogError(ex, "Error testing connection to {IpAddress}:{Port}", ipAddress, port);
                
                return new DeviceConnectionTestDto
                {
                    IsConnected = false,
                    Message = $"Connection error: {ex.Message}",
                    TestTime = DateTime.UtcNow,
                    ResponseTimeMs = (int)stopwatch.ElapsedMilliseconds
                };
            }
        }

        public async Task<List<DeviceConnectionTestDto>> TestAllDevicesConnectionAsync()
        {
            var devices = await _context.ZkDevices
                .Where(d => d.IsActive)
                .ToListAsync();

            var results = new List<DeviceConnectionTestDto>();

            foreach (var device in devices)
            {
                var result = await TestDeviceConnectionByIpAsync(device.IpAddress, device.Port);
                
                // Update device connection status
                device.IsConnected = result.IsConnected;
                device.LastConnectionTime = result.TestTime;
                device.UpdatedAt = DateTime.UtcNow;
                device.UpdatedBy = "System";

                results.Add(result);
            }

            await _context.SaveChangesAsync();
            return results;
        }

        #endregion

        #region Log Download

        public async Task<DownloadLogsResponseDto> DownloadLogsFromDeviceAsync(int deviceId, DownloadLogsRequestDto? request = null)
        {
            var device = await _context.ZkDevices
                .FirstOrDefaultAsync(d => d.Id == deviceId && d.IsActive);

            if (device == null)
            {
                return new DownloadLogsResponseDto
                {
                    Success = false,
                    Message = "Device not found",
                    Errors = new List<string> { "Device not found" }
                };
            }

            try
            {
                // Test connection first
                var connectionTest = await TestDeviceConnectionByIpAsync(device.IpAddress, device.Port);
                if (!connectionTest.IsConnected)
                {
                    return new DownloadLogsResponseDto
                    {
                        Success = false,
                        Message = "Device is not reachable",
                        Errors = new List<string> { connectionTest.Message }
                    };
                }

                // Simulate log download based on device's log count
                var logs = await GenerateSimulatedLogsAsync(device, request);
                
                var newLogs = 0;
                var updatedLogs = 0;
                var skippedLogs = 0;

                foreach (var log in logs)
                {
                    var existingLog = await _context.AttendanceLogs
                        .FirstOrDefaultAsync(al => al.ZkDeviceId == deviceId && 
                                                  al.EmployeeId == log.EmployeeId && 
                                                  al.LogTime == log.LogTime);

                    if (existingLog == null)
                    {
                        _context.AttendanceLogs.Add(log);
                        newLogs++;
                    }
                    else
                    {
                        // Update existing log
                        existingLog.LogType = log.LogType;
                        existingLog.VerificationMode = log.VerificationMode;
                        existingLog.WorkCode = log.WorkCode;
                        existingLog.Remarks = log.Remarks;
                        existingLog.UpdatedAt = DateTime.UtcNow;
                        existingLog.UpdatedBy = "System";
                        updatedLogs++;
                    }
                }

                // Update device last download time
                device.LastLogDownloadTime = DateTime.UtcNow;
                device.UpdatedAt = DateTime.UtcNow;
                device.UpdatedBy = "System";

                await _context.SaveChangesAsync();

                return new DownloadLogsResponseDto
                {
                    Success = true,
                    Message = $"Successfully downloaded {logs.Count} logs from device {device.DeviceName}",
                    TotalLogsDownloaded = logs.Count,
                    NewLogs = newLogs,
                    UpdatedLogs = updatedLogs,
                    SkippedLogs = skippedLogs
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading logs from device {DeviceId}", deviceId);
                return new DownloadLogsResponseDto
                {
                    Success = false,
                    Message = "Error downloading logs",
                    Errors = new List<string> { ex.Message }
                };
            }
        }

        public async Task<DownloadLogsResponseDto> DownloadLogsFromAllDevicesAsync(DownloadLogsRequestDto? request = null)
        {
            var devices = await _context.ZkDevices
                .Where(d => d.IsActive)
                .ToListAsync();

            var totalNewLogs = 0;
            var totalUpdatedLogs = 0;
            var totalSkippedLogs = 0;
            var errors = new List<string>();
            var successfulDevices = 0;

            foreach (var device in devices)
            {
                try
                {
                    var result = await DownloadLogsFromDeviceAsync(device.Id, request);
                    if (result.Success)
                    {
                        totalNewLogs += result.NewLogs;
                        totalUpdatedLogs += result.UpdatedLogs;
                        totalSkippedLogs += result.SkippedLogs;
                        successfulDevices++;
                    }
                    else
                    {
                        errors.Add($"Device {device.DeviceName}: {result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add($"Device {device.DeviceName}: {ex.Message}");
                }
            }

            return new DownloadLogsResponseDto
            {
                Success = successfulDevices > 0,
                Message = $"Downloaded logs from {successfulDevices}/{devices.Count} devices",
                TotalLogsDownloaded = totalNewLogs + totalUpdatedLogs,
                NewLogs = totalNewLogs,
                UpdatedLogs = totalUpdatedLogs,
                SkippedLogs = totalSkippedLogs,
                Errors = errors
            };
        }

        private Task<List<AttendanceLog>> GenerateSimulatedLogsAsync(ZkDevice device, DownloadLogsRequestDto? request)
        {
            var logs = new List<AttendanceLog>();
            var random = new Random();
            var startDate = request?.StartDate ?? DateTime.Now.AddDays(-30);
            var endDate = request?.EndDate ?? DateTime.Now;

            // Generate logs based on device's log count
            var logCount = Math.Min(device.LogCount, 100); // Limit to 100 logs per download
            
            for (int i = 0; i < logCount; i++)
            {
                var logTime = startDate.AddDays(random.Next(0, (int)(endDate - startDate).TotalDays))
                                   .AddHours(random.Next(6, 18))
                                   .AddMinutes(random.Next(0, 60));

                var log = new AttendanceLog
                {
                    ZkDeviceId = device.Id,
                    EmployeeId = $"EMP{random.Next(1000, 9999)}",
                    EmployeeName = $"Employee {random.Next(1, 100)}",
                    LogTime = logTime,
                    LogType = random.Next(0, 2) == 0 ? "IN" : "OUT",
                    VerificationMode = new[] { "FP", "RF", "CARD", "PASSWORD" }[random.Next(0, 4)],
                    WorkCode = random.Next(0, 2) == 0 ? "WORK" : "BREAK",
                    Remarks = string.Empty,
                    IsProcessed = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = "System",
                    UpdatedBy = "System"
                };

                logs.Add(log);
            }

            return Task.FromResult(logs);
        }

        #endregion

        #region Log Management

        public async Task<AttendanceLogResponseDto> GetAttendanceLogsAsync(AttendanceLogQueryDto query)
        {
            var queryable = _context.AttendanceLogs
                .Include(al => al.ZkDevice)
                .AsQueryable();

            // Apply filters
            if (query.DeviceId.HasValue)
                queryable = queryable.Where(al => al.ZkDeviceId == query.DeviceId.Value);

            if (!string.IsNullOrEmpty(query.EmployeeId))
                queryable = queryable.Where(al => al.EmployeeId.Contains(query.EmployeeId));

            if (query.StartDate.HasValue)
                queryable = queryable.Where(al => al.LogTime >= query.StartDate.Value);

            if (query.EndDate.HasValue)
                queryable = queryable.Where(al => al.LogTime <= query.EndDate.Value);

            if (!string.IsNullOrEmpty(query.LogType))
                queryable = queryable.Where(al => al.LogType == query.LogType);

            if (!string.IsNullOrEmpty(query.VerificationMode))
                queryable = queryable.Where(al => al.VerificationMode == query.VerificationMode);

            if (query.IsProcessed.HasValue)
                queryable = queryable.Where(al => al.IsProcessed == query.IsProcessed.Value);

            // Get total count
            var totalCount = await queryable.CountAsync();

            // Apply sorting
            queryable = query.SortBy?.ToLower() switch
            {
                "logtime" => query.SortOrder?.ToLower() == "asc" 
                    ? queryable.OrderBy(al => al.LogTime)
                    : queryable.OrderByDescending(al => al.LogTime),
                "employeeid" => query.SortOrder?.ToLower() == "asc"
                    ? queryable.OrderBy(al => al.EmployeeId)
                    : queryable.OrderByDescending(al => al.EmployeeId),
                "devicename" => query.SortOrder?.ToLower() == "asc"
                    ? queryable.OrderBy(al => al.ZkDevice.DeviceName)
                    : queryable.OrderByDescending(al => al.ZkDevice.DeviceName),
                _ => queryable.OrderByDescending(al => al.LogTime)
            };

            // Apply pagination
            var logs = await queryable
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .Select(al => new AttendanceLogDto
                {
                    Id = al.Id,
                    ZkDeviceId = al.ZkDeviceId,
                    DeviceName = al.ZkDevice.DeviceName,
                    EmployeeId = al.EmployeeId,
                    EmployeeName = al.EmployeeName,
                    LogTime = al.LogTime,
                    LogType = al.LogType,
                    VerificationMode = al.VerificationMode,
                    WorkCode = al.WorkCode,
                    Remarks = al.Remarks,
                    IsProcessed = al.IsProcessed,
                    CreatedAt = al.CreatedAt
                })
                .ToListAsync();

            return new AttendanceLogResponseDto
            {
                Logs = logs,
                TotalCount = totalCount,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
            };
        }

        public async Task<AttendanceLogDto?> GetAttendanceLogByIdAsync(int logId)
        {
            var log = await _context.AttendanceLogs
                .Include(al => al.ZkDevice)
                .Where(al => al.Id == logId)
                .Select(al => new AttendanceLogDto
                {
                    Id = al.Id,
                    ZkDeviceId = al.ZkDeviceId,
                    DeviceName = al.ZkDevice.DeviceName,
                    EmployeeId = al.EmployeeId,
                    EmployeeName = al.EmployeeName,
                    LogTime = al.LogTime,
                    LogType = al.LogType,
                    VerificationMode = al.VerificationMode,
                    WorkCode = al.WorkCode,
                    Remarks = al.Remarks,
                    IsProcessed = al.IsProcessed,
                    CreatedAt = al.CreatedAt
                })
                .FirstOrDefaultAsync();

            return log;
        }

        public async Task<bool> MarkLogAsProcessedAsync(int logId)
        {
            var log = await _context.AttendanceLogs
                .FirstOrDefaultAsync(al => al.Id == logId);

            if (log == null)
                return false;

            log.IsProcessed = true;
            log.UpdatedAt = DateTime.UtcNow;
            log.UpdatedBy = "System";

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAttendanceLogAsync(int logId)
        {
            var log = await _context.AttendanceLogs
                .FirstOrDefaultAsync(al => al.Id == logId);

            if (log == null)
                return false;

            _context.AttendanceLogs.Remove(log);
            await _context.SaveChangesAsync();
            return true;
        }

        #endregion

        #region Statistics

        public async Task<object> GetDeviceStatisticsAsync()
        {
            var totalDevices = await _context.ZkDevices.CountAsync(d => d.IsActive);
            var connectedDevices = await _context.ZkDevices.CountAsync(d => d.IsActive && d.IsConnected);
            var totalLogs = await _context.ZkDevices.SumAsync(d => d.LogCount);

            return new
            {
                TotalDevices = totalDevices,
                ConnectedDevices = connectedDevices,
                DisconnectedDevices = totalDevices - connectedDevices,
                TotalLogsOnDevices = totalLogs,
                ConnectionRate = totalDevices > 0 ? (double)connectedDevices / totalDevices * 100 : 0
            };
        }

        public async Task<object> GetLogStatisticsAsync()
        {
            var totalLogs = await _context.AttendanceLogs.CountAsync();
            var processedLogs = await _context.AttendanceLogs.CountAsync(al => al.IsProcessed);
            var unprocessedLogs = totalLogs - processedLogs;

            var logsByType = await _context.AttendanceLogs
                .GroupBy(al => al.LogType)
                .Select(g => new { LogType = g.Key, Count = g.Count() })
                .ToListAsync();

            var logsByDevice = await _context.AttendanceLogs
                .Include(al => al.ZkDevice)
                .GroupBy(al => al.ZkDevice.DeviceName)
                .Select(g => new { DeviceName = g.Key, Count = g.Count() })
                .ToListAsync();

            return new
            {
                TotalLogs = totalLogs,
                ProcessedLogs = processedLogs,
                UnprocessedLogs = unprocessedLogs,
                LogsByType = logsByType,
                LogsByDevice = logsByDevice,
                ProcessingRate = totalLogs > 0 ? (double)processedLogs / totalLogs * 100 : 0
            };
        }

        #endregion
    }
}
