using HrHubAPI.DTOs;

namespace HrHubAPI.Services
{
    public interface IZkDeviceService
    {
        // Device Management
        Task<List<ZkDeviceDto>> GetAllDevicesAsync();
        Task<ZkDeviceDto?> GetDeviceByIdAsync(int deviceId);
        Task<ZkDeviceDto> CreateDeviceAsync(CreateZkDeviceDto createDto);
        Task<ZkDeviceDto?> UpdateDeviceAsync(int deviceId, UpdateZkDeviceDto updateDto);
        Task<bool> DeleteDeviceAsync(int deviceId);
        
        // Device Connection
        Task<DeviceConnectionTestDto> TestDeviceConnectionAsync(int deviceId);
        Task<DeviceConnectionTestDto> TestDeviceConnectionByIpAsync(string ipAddress, int port);
        Task<List<DeviceConnectionTestDto>> TestAllDevicesConnectionAsync();
        
        // Log Download
        Task<DownloadLogsResponseDto> DownloadLogsFromDeviceAsync(int deviceId, DownloadLogsRequestDto? request = null);
        Task<DownloadLogsResponseDto> DownloadLogsFromAllDevicesAsync(DownloadLogsRequestDto? request = null);
        
        // Log Management
        Task<AttendanceLogResponseDto> GetAttendanceLogsAsync(AttendanceLogQueryDto query);
        Task<AttendanceLogDto?> GetAttendanceLogByIdAsync(int logId);
        Task<bool> MarkLogAsProcessedAsync(int logId);
        Task<bool> DeleteAttendanceLogAsync(int logId);
        
        // Statistics
        Task<object> GetDeviceStatisticsAsync();
        Task<object> GetLogStatisticsAsync();
    }
}
