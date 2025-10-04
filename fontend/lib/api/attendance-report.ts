import api from '../axios'

// Types for attendance reports
export interface DailyAttendanceReport {
  date: string
  totalEmployees: number
  presentEmployees: number
  absentEmployees: number
  lateEmployees: number
  earlyDepartures: number
  attendanceRate: number
  reports: EmployeeDailyReport[]
}

export interface EmployeeDailyReport {
  employeeId: number
  employeeName: string
  employeeCode: string
  department: string
  designation: string
  checkInTime?: string
  checkOutTime?: string
  totalHours?: number
  status: 'PRESENT' | 'ABSENT' | 'LATE' | 'EARLY_DEPARTURE' | 'PARTIAL'
  remarks?: string
}

export interface EmployeeAttendanceReport {
  employeeId: number
  employeeName: string
  employeeCode: string
  department: string
  designation: string
  startDate: string
  endDate: string
  totalDays: number
  presentDays: number
  absentDays: number
  lateDays: number
  earlyDepartureDays: number
  attendanceRate: number
  totalHours: number
  averageHoursPerDay: number
  reports: DailyAttendanceRecord[]
}

export interface DailyAttendanceRecord {
  date: string
  checkInTime?: string
  checkOutTime?: string
  totalHours?: number
  status: 'PRESENT' | 'ABSENT' | 'LATE' | 'EARLY_DEPARTURE' | 'PARTIAL'
  remarks?: string
}

export interface AttendanceSummary {
  startDate: string
  endDate: string
  totalEmployees: number
  totalWorkingDays: number
  totalPresentDays: number
  totalAbsentDays: number
  totalLateDays: number
  totalEarlyDepartureDays: number
  overallAttendanceRate: number
  averageHoursPerDay: number
  departmentSummary: DepartmentSummary[]
}

export interface DepartmentSummary {
  departmentId: number
  departmentName: string
  totalEmployees: number
  presentDays: number
  absentDays: number
  lateDays: number
  earlyDepartureDays: number
  attendanceRate: number
  averageHoursPerDay: number
}

export interface AttendanceLogSummary {
  startDate: string
  endDate: string
  totalLogs: number
  totalCheckIns: number
  totalCheckOuts: number
  totalHours: number
  averageHoursPerDay: number
  deviceSummary: DeviceLogSummary[]
}

export interface DeviceLogSummary {
  deviceId: number
  deviceName: string
  totalLogs: number
  checkIns: number
  checkOuts: number
  totalHours: number
}

export interface AttendanceReportParams {
  date?: string
  startDate?: string
  endDate?: string
  employeeId?: number
  departmentId?: number
  format?: 'json' | 'csv'
}

// API Functions
export const getDailyAttendanceReport = async (date: string): Promise<DailyAttendanceReport> => {
  const response = await api.get(`/AttendanceReport/daily?date=${date}`)
  return response.data
}

export const getAllEmployeesDailyReport = async (date: string): Promise<DailyAttendanceReport> => {
  const response = await api.get(`/AttendanceReport/daily/all-employees?date=${date}`)
  return response.data
}

export const exportDailyAttendanceReport = async (date: string): Promise<Blob> => {
  const response = await api.get(`/AttendanceReport/daily/export?date=${date}`, {
    responseType: 'blob'
  })
  return response.data
}

export const getEmployeeAttendanceReport = async (params: {
  startDate: string
  endDate: string
  employeeId?: number
}): Promise<EmployeeAttendanceReport[]> => {
  const queryParams = new URLSearchParams({
    startDate: params.startDate,
    endDate: params.endDate
  })
  
  if (params.employeeId) {
    queryParams.append('employeeId', params.employeeId.toString())
  }
  
  const response = await api.get(`/AttendanceReport/employee?${queryParams}`)
  return response.data
}

export const getEmployeeAttendanceReportById = async (employeeId: number, params: {
  startDate: string
  endDate: string
}): Promise<EmployeeAttendanceReport> => {
  const queryParams = new URLSearchParams({
    startDate: params.startDate,
    endDate: params.endDate
  })
  
  const response = await api.get(`/AttendanceReport/employee/${employeeId}?${queryParams}`)
  return response.data
}

export const exportEmployeeAttendanceReport = async (params: {
  startDate: string
  endDate: string
  employeeId?: number
}): Promise<Blob> => {
  const queryParams = new URLSearchParams({
    startDate: params.startDate,
    endDate: params.endDate
  })
  
  if (params.employeeId) {
    queryParams.append('employeeId', params.employeeId.toString())
  }
  
  const response = await api.get(`/AttendanceReport/employee/export?${queryParams}`, {
    responseType: 'blob'
  })
  return response.data
}

export const getAttendanceSummary = async (params: {
  startDate: string
  endDate: string
  departmentId?: number
}): Promise<AttendanceSummary> => {
  const queryParams = new URLSearchParams({
    startDate: params.startDate,
    endDate: params.endDate
  })
  
  if (params.departmentId) {
    queryParams.append('departmentId', params.departmentId.toString())
  }
  
  const response = await api.get(`/AttendanceReport/summary?${queryParams}`)
  return response.data
}

export const getAttendanceLogSummary = async (params: {
  startDate: string
  endDate: string
  deviceId?: number
}): Promise<AttendanceLogSummary> => {
  const queryParams = new URLSearchParams({
    startDate: params.startDate,
    endDate: params.endDate
  })
  
  if (params.deviceId) {
    queryParams.append('deviceId', params.deviceId.toString())
  }
  
  const response = await api.get(`/AttendanceReport/log-summary?${queryParams}`)
  return response.data
}

// Test endpoints (Anonymous)
export const testDailyAttendanceReport = async (date: string): Promise<DailyAttendanceReport> => {
  const response = await api.get(`/AttendanceReport/test/daily?date=${date}`)
  return response.data
}

export const testEmployeeAttendanceReport = async (params: {
  startDate: string
  endDate: string
  employeeId?: number
}): Promise<EmployeeAttendanceReport[]> => {
  const queryParams = new URLSearchParams({
    startDate: params.startDate,
    endDate: params.endDate
  })
  
  if (params.employeeId) {
    queryParams.append('employeeId', params.employeeId.toString())
  }
  
  const response = await api.get(`/AttendanceReport/test/employee?${queryParams}`)
  return response.data
}

export const testAttendanceSummary = async (params: {
  startDate: string
  endDate: string
  departmentId?: number
}): Promise<AttendanceSummary> => {
  const queryParams = new URLSearchParams({
    startDate: params.startDate,
    endDate: params.endDate
  })
  
  if (params.departmentId) {
    queryParams.append('departmentId', params.departmentId.toString())
  }
  
  const response = await api.get(`/AttendanceReport/test/summary?${queryParams}`)
  return response.data
}
