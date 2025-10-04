# Test ZK Device API Script
# This script tests the ZK device connection and log download functionality

$baseUrl = "http://localhost:5100/api/ZkDevice"
$headers = @{
    "Content-Type" = "application/json"
}

Write-Host "=== ZK Device API Test Script ===" -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Yellow
Write-Host ""

# Function to make HTTP requests
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Uri,
        [object]$Body = $null,
        [hashtable]$Headers = @{}
    )
    
    try {
        $params = @{
            Method = $Method
            Uri = $Uri
            Headers = $Headers
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-RestMethod @params
        return $response
    }
    catch {
        Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
            $responseBody = $reader.ReadToEnd()
            Write-Host "Response: $responseBody" -ForegroundColor Red
        }
        return $null
    }
}

# Test 1: Get all devices (Anonymous test endpoint)
Write-Host "1. Testing Get All Devices (Anonymous)..." -ForegroundColor Cyan
$devices = Invoke-ApiRequest -Method "GET" -Uri "$baseUrl/test/devices" -Headers $headers
if ($devices) {
    Write-Host "Found $($devices.Count) devices" -ForegroundColor Green
    foreach ($device in $devices) {
        Write-Host "  - Device $($device.deviceName): $($device.ipAddress):$($device.port) (Logs: $($device.logCount))" -ForegroundColor White
    }
} else {
    Write-Host "Failed to get devices" -ForegroundColor Red
}
Write-Host ""

# Test 2: Test all device connections
Write-Host "2. Testing All Device Connections..." -ForegroundColor Cyan
$connections = Invoke-ApiRequest -Method "POST" -Uri "$baseUrl/test/test-all-connections" -Headers $headers
if ($connections) {
    Write-Host "Connection test completed" -ForegroundColor Green
    foreach ($connection in $connections) {
        $status = if ($connection.isConnected) { "Connected" } else { "Failed" }
        $color = if ($connection.isConnected) { "Green" } else { "Red" }
        Write-Host "  - $status - Response: $($connection.responseTimeMs)ms - $($connection.message)" -ForegroundColor $color
    }
} else {
    Write-Host "Failed to test connections" -ForegroundColor Red
}
Write-Host ""

# Test 3: Download logs from all devices
Write-Host "3. Testing Download Logs from All Devices..." -ForegroundColor Cyan
$downloadResult = Invoke-ApiRequest -Method "POST" -Uri "$baseUrl/test/download-all-logs" -Headers $headers
if ($downloadResult) {
    Write-Host "Log download completed" -ForegroundColor Green
    Write-Host "  - Success: $($downloadResult.success)" -ForegroundColor White
    Write-Host "  - Message: $($downloadResult.message)" -ForegroundColor White
    Write-Host "  - Total Logs Downloaded: $($downloadResult.totalLogsDownloaded)" -ForegroundColor White
    Write-Host "  - New Logs: $($downloadResult.newLogs)" -ForegroundColor White
    Write-Host "  - Updated Logs: $($downloadResult.updatedLogs)" -ForegroundColor White
    Write-Host "  - Skipped Logs: $($downloadResult.skippedLogs)" -ForegroundColor White
    
    if ($downloadResult.errors -and $downloadResult.errors.Count -gt 0) {
        Write-Host "  - Errors:" -ForegroundColor Yellow
        foreach ($error in $downloadResult.errors) {
            Write-Host "    * $error" -ForegroundColor Red
        }
    }
} else {
    Write-Host "Failed to download logs" -ForegroundColor Red
}
Write-Host ""

# Test 4: Get attendance logs
Write-Host "4. Testing Get Attendance Logs..." -ForegroundColor Cyan
$logs = Invoke-ApiRequest -Method "GET" -Uri "$baseUrl/test/logs?page=1&pageSize=10" -Headers $headers
if ($logs) {
    Write-Host "Retrieved attendance logs" -ForegroundColor Green
    Write-Host "  - Total Count: $($logs.totalCount)" -ForegroundColor White
    Write-Host "  - Page: $($logs.page) of $($logs.totalPages)" -ForegroundColor White
    Write-Host "  - Logs in this page: $($logs.logs.Count)" -ForegroundColor White
    
    if ($logs.logs.Count -gt 0) {
        Write-Host "  - Sample logs:" -ForegroundColor Yellow
        foreach ($log in $logs.logs | Select-Object -First 3) {
            Write-Host "    * $($log.employeeId) - $($log.logType) at $($log.logTime) from Device $($log.deviceName)" -ForegroundColor White
        }
    }
} else {
    Write-Host "Failed to get attendance logs" -ForegroundColor Red
}
Write-Host ""

# Test 5: Test individual device connection
if ($devices -and $devices.Count -gt 0) {
    $firstDevice = $devices[0]
    Write-Host "5. Testing Individual Device Connection..." -ForegroundColor Cyan
    Write-Host "  Testing Device: $($firstDevice.deviceName) ($($firstDevice.ipAddress):$($firstDevice.port))" -ForegroundColor White
    
    $connectionTest = Invoke-ApiRequest -Method "POST" -Uri "$baseUrl/test-connection" -Body @{
        ipAddress = $firstDevice.ipAddress
        port = $firstDevice.port
    } -Headers $headers
    
    if ($connectionTest) {
        $status = if ($connectionTest.isConnected) { "Connected" } else { "Failed" }
        $color = if ($connectionTest.isConnected) { "Green" } else { "Red" }
        Write-Host "  - $status - Response: $($connectionTest.responseTimeMs)ms - $($connectionTest.message)" -ForegroundColor $color
    } else {
        Write-Host "  - Failed to test connection" -ForegroundColor Red
    }
    Write-Host ""
}

Write-Host "=== Test Summary ===" -ForegroundColor Green
Write-Host "All tests completed. Check the results above." -ForegroundColor Yellow
Write-Host ""
Write-Host "Available API Endpoints:" -ForegroundColor Cyan
Write-Host "  GET  /api/ZkDevice/test/devices - Get all devices (Anonymous)" -ForegroundColor White
Write-Host "  POST /api/ZkDevice/test/test-all-connections - Test all connections (Anonymous)" -ForegroundColor White
Write-Host "  POST /api/ZkDevice/test/download-all-logs - Download all logs (Anonymous)" -ForegroundColor White
Write-Host "  GET  /api/ZkDevice/test/logs - Get attendance logs (Anonymous)" -ForegroundColor White
Write-Host "  POST /api/ZkDevice/test-connection - Test connection by IP (Anonymous)" -ForegroundColor White
Write-Host ""
Write-Host "For authenticated endpoints, use JWT token in Authorization header:" -ForegroundColor Yellow
Write-Host "  Authorization: Bearer <your-jwt-token>" -ForegroundColor White
