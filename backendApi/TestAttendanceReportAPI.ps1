# Test script for Attendance Report APIs
# Make sure the API is running on https://localhost:7000 or http://localhost:5000

$baseUrl = "https://localhost:7000"  # Change this to your API URL
$testDate = Get-Date -Format "yyyy-MM-dd"
$startDate = (Get-Date).AddDays(-7).ToString("yyyy-MM-dd")
$endDate = Get-Date -Format "yyyy-MM-dd"

Write-Host "Testing Attendance Report APIs..." -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Yellow
Write-Host "Test Date: $testDate" -ForegroundColor Yellow
Write-Host "Date Range: $startDate to $endDate" -ForegroundColor Yellow
Write-Host ""

# Test 1: Daily Attendance Report (Anonymous)
Write-Host "Test 1: Daily Attendance Report (Anonymous)" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/attendancereport/test/daily?reportDate=$testDate" -Method GET -ContentType "application/json"
    Write-Host "✓ Success: Daily attendance report retrieved" -ForegroundColor Green
    Write-Host "  Total Reports: $($response.totalCount)" -ForegroundColor White
    Write-Host "  Page: $($response.page) of $($response.totalPages)" -ForegroundColor White
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Employee Attendance Report (Anonymous)
Write-Host "Test 2: Employee Attendance Report (Anonymous)" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/attendancereport/test/employee?startDate=$startDate&endDate=$endDate" -Method GET -ContentType "application/json"
    Write-Host "✓ Success: Employee attendance report retrieved" -ForegroundColor Green
    Write-Host "  Total Reports: $($response.totalCount)" -ForegroundColor White
    Write-Host "  Page: $($response.page) of $($response.totalPages)" -ForegroundColor White
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Attendance Summary (Anonymous)
Write-Host "Test 3: Attendance Summary (Anonymous)" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/attendancereport/test/summary?startDate=$startDate&endDate=$endDate" -Method GET -ContentType "application/json"
    Write-Host "✓ Success: Attendance summary retrieved" -ForegroundColor Green
    Write-Host "  Total Employees: $($response.totalEmployees)" -ForegroundColor White
    Write-Host "  Present Employees: $($response.presentEmployees)" -ForegroundColor White
    Write-Host "  Absent Employees: $($response.absentEmployees)" -ForegroundColor White
    Write-Host "  Overall Attendance: $([math]::Round($response.overallAttendancePercentage, 2))%" -ForegroundColor White
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 4: Daily Attendance Report for All Employees
Write-Host "Test 4: Daily Attendance Report for All Employees" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/attendancereport/daily/all-employees?reportDate=$testDate" -Method GET -ContentType "application/json"
    Write-Host "✓ Success: Daily attendance report for all employees retrieved" -ForegroundColor Green
    Write-Host "  Total Reports: $($response.Count)" -ForegroundColor White
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 5: Attendance Log Summary
Write-Host "Test 5: Attendance Log Summary" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/attendancereport/log-summary?startDate=$startDate&endDate=$endDate" -Method GET -ContentType "application/json"
    Write-Host "✓ Success: Attendance log summary retrieved" -ForegroundColor Green
    Write-Host "  Total Log Summaries: $($response.Count)" -ForegroundColor White
} catch {
    Write-Host "✗ Failed: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "API Testing Complete!" -ForegroundColor Green
Write-Host ""
Write-Host "Available Endpoints:" -ForegroundColor Yellow
Write-Host "  GET /api/attendancereport/daily - Daily attendance report with filters" -ForegroundColor White
Write-Host "  GET /api/attendancereport/daily/all-employees - Daily report for all employees" -ForegroundColor White
Write-Host "  GET /api/attendancereport/daily/export - Export daily report to CSV" -ForegroundColor White
Write-Host "  GET /api/attendancereport/employee - Employee attendance report with filters" -ForegroundColor White
Write-Host "  GET /api/attendancereport/employee/{id} - Specific employee report" -ForegroundColor White
Write-Host "  GET /api/attendancereport/employee/export - Export employee report to CSV" -ForegroundColor White
Write-Host "  GET /api/attendancereport/summary - Attendance summary" -ForegroundColor White
Write-Host "  GET /api/attendancereport/log-summary - Attendance log summary" -ForegroundColor White
Write-Host ""
Write-Host "Test Endpoints (Anonymous):" -ForegroundColor Yellow
Write-Host "  GET /api/attendancereport/test/daily - Test daily report" -ForegroundColor White
Write-Host "  GET /api/attendancereport/test/employee - Test employee report" -ForegroundColor White
Write-Host "  GET /api/attendancereport/test/summary - Test summary" -ForegroundColor White
