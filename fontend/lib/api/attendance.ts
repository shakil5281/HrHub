import api from '../axios'

// ===== ATTENDANCE DEVICE INTERFACES =====
export interface AttendanceDevice {
  id: number
  deviceName: string
  ipAddress: string
  port: number
  serialNumber: string
  productName: string
  machineNumber: string
  userCount: number
  adminCount: number
  fpCount: number
  fcCount: number
  passwordCount: number
  logCount: number
  isConnected: boolean
  lastConnectionTime: string
  lastLogDownloadTime: string
  location: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface AttendanceDeviceCreateRequest {
  deviceName: string
  ipAddress: string
  port: number
  serialNumber: string
  productName: string
  machineNumber: string
  location: string
}

export interface AttendanceDeviceUpdateRequest {
  deviceName?: string
  ipAddress?: string
  port?: number
  serialNumber?: string
  productName?: string
  machineNumber?: string
  location?: string
  isActive?: boolean
}

export interface AttendanceDevicesResponse {
  success: boolean
  message: string
  data: AttendanceDevice[]
  errors: string[]
}

export interface AttendanceDeviceResponse {
  success: boolean
  message: string
  data: AttendanceDevice
  errors: string[]
}

// ===== ATTENDANCE LOG INTERFACES =====
export interface AttendanceLog {
  id: number
  employeeId: number
  employeeName: string
  deviceId: number
  deviceName: string
  logTime: string
  logType: 'IN' | 'OUT'
  isProcessed: boolean
  createdAt: string
}

export interface AttendanceLogsResponse {
  success: boolean
  message: string
  data: AttendanceLog[]
  totalCount: number
  page: number
  pageSize: number
  errors: string[]
}

export interface AttendanceLogResponse {
  success: boolean
  message: string
  data: AttendanceLog
  errors: string[]
}


// ===== ATTENDANCE STATISTICS INTERFACES =====
export interface AttendanceStatistics {
  TotalDevices: number
  TotalLogs: number
  TotalSyncLogs: number
  LastSyncTime: string
  DatabaseSize: number
}

export interface AttendanceStatisticsResponse {
  success: boolean
  message: string
  data: AttendanceStatistics
  errors: string[]
}


// ===== DEVICE STATUS INTERFACES =====
export interface DeviceStatus {
  id: number
  deviceName: string
  ipAddress: string
  port: number
  isConnected: boolean
  lastConnectionTime: string
  lastLogDownloadTime: string
  logCount: number
  userCount: number
  location: string
  isActive: boolean
  status: 'ONLINE' | 'OFFLINE' | 'ERROR'
  errorMessage?: string
}

export interface DeviceStatusResponse {
  success: boolean
  message: string
  data: DeviceStatus[]
  errors: string[]
}


// ===== API FUNCTIONS =====

// Device Management
export const getAllAttendanceDevices = async (): Promise<AttendanceDevicesResponse> => {
  const response = await api.get<AttendanceDevice[]>('/ZkDevice/devices')
  return {
    success: true,
    message: 'Devices retrieved successfully',
    data: response.data,
    errors: []
  }
}

export const createAttendanceDevice = async (device: AttendanceDeviceCreateRequest): Promise<AttendanceDeviceResponse> => {
  const response = await api.post<AttendanceDevice>('/ZkDevice/devices', device)
  return {
    success: true,
    message: 'Device created successfully',
    data: response.data,
    errors: []
  }
}

export const getAttendanceDeviceById = async (deviceId: string): Promise<AttendanceDeviceResponse> => {
  const response = await api.get<AttendanceDevice>(`/ZkDevice/devices/${deviceId}`)
  return {
    success: true,
    message: 'Device retrieved successfully',
    data: response.data,
    errors: []
  }
}

export const updateAttendanceDevice = async (deviceId: string, device: AttendanceDeviceUpdateRequest): Promise<AttendanceDeviceResponse> => {
  const response = await api.put<AttendanceDevice>(`/ZkDevice/devices/${deviceId}`, device)
  return {
    success: true,
    message: 'Device updated successfully',
    data: response.data,
    errors: []
  }
}

export const deleteAttendanceDevice = async (deviceId: string): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.delete(`/ZkDevice/devices/${deviceId}`)
  return {
    success: true,
    message: 'Device deleted successfully',
    errors: []
  }
}

export const testDeviceConnection = async (deviceId: string): Promise<{ success: boolean; message: string; isConnected?: boolean; errors: string[] }> => {
  const response = await api.post<{ isConnected: boolean }>(`/ZkDevice/devices/${deviceId}/test-connection`)
  return {
    success: true,
    message: response.data.isConnected ? 'Connection test successful' : 'Connection test failed',
    isConnected: response.data.isConnected,
    errors: []
  }
}

export const testConnectionByIpPort = async (ipAddress: string, port: number): Promise<{ success: boolean; message: string; isConnected?: boolean; errors: string[] }> => {
  const response = await api.post<{ isConnected: boolean }>('/ZkDevice/test-connection', { ipAddress, port })
  return {
    success: true,
    message: response.data.isConnected ? 'Connection test successful' : 'Connection test failed',
    isConnected: response.data.isConnected,
    errors: []
  }
}

export const testAllDeviceConnections = async (): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post('/ZkDevice/devices/test-all-connections')
  return response.data
}

export const downloadDeviceLogs = async (deviceId: string): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post(`/ZkDevice/devices/${deviceId}/download-logs`)
  return response.data
}

export const downloadAllDeviceLogs = async (): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.post('/ZkDevice/devices/download-all-logs')
  return response.data
}

export const getDeviceStatus = async (): Promise<DeviceStatusResponse> => {
  const response = await api.get<DeviceStatus[]>('/ZkDevice/statistics/devices')
  return {
    success: true,
    message: 'Device status retrieved successfully',
    data: response.data,
    errors: []
  }
}

// Attendance Logs
export const getAttendanceLogs = async (params?: {
  page?: number
  pageSize?: number
  employeeId?: number
  deviceId?: string
  startDate?: string
  endDate?: string
  logType?: 'IN' | 'OUT'
}): Promise<AttendanceLogsResponse> => {
  const response = await api.get<AttendanceLogsResponse>('/ZkDevice/logs', { params })
  return response.data
}

export const getAttendanceLogById = async (logId: number): Promise<AttendanceLogResponse> => {
  const response = await api.get<AttendanceLogResponse>(`/ZkDevice/logs/${logId}`)
  return response.data
}

export const deleteAttendanceLog = async (logId: number): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.delete(`/ZkDevice/logs/${logId}`)
  return response.data
}

export const markLogAsProcessed = async (logId: number): Promise<{ success: boolean; message: string; errors: string[] }> => {
  const response = await api.put(`/ZkDevice/logs/${logId}/mark-processed`)
  return response.data
}


// Statistics
export const getAttendanceStatistics = async (): Promise<AttendanceStatisticsResponse> => {
  const response = await api.get<AttendanceStatistics>('/ZkDevice/statistics/logs')
  return {
    success: true,
    message: 'Statistics retrieved successfully',
    data: response.data,
    errors: []
  }
}

export const getDeviceStatistics = async (): Promise<{ success: boolean; message: string; data: unknown; errors: string[] }> => {
  const response = await api.get('/ZkDevice/statistics/devices')
  return {
    success: true,
    message: 'Device statistics retrieved successfully',
    data: response.data,
    errors: []
  }
}


// ===== DEPARTMENT, SECTION, DESIGNATION INTERFACES =====
export interface Department {
  id: number
  name: string
  nameBangla?: string
  description?: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface Section {
  id: number
  name: string
  nameBangla?: string
  description?: string
  departmentId: number
  departmentName: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface Designation {
  id: number
  name: string
  nameBangla?: string
  description?: string
  grade: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

// ===== DEPARTMENT, SECTION, DESIGNATION API FUNCTIONS =====
export const getAllDepartments = async (): Promise<{ success: boolean; data: Department[] }> => {
  const response = await api.get('/Department')
  return response.data
}

export const getAllSections = async (): Promise<{ success: boolean; data: Section[] }> => {
  const response = await api.get('/Section')
  return response.data
}

export const getAllDesignations = async (): Promise<{ success: boolean; data: Designation[] }> => {
  const response = await api.get('/Designation')
  return response.data
}