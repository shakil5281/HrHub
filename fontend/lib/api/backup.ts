import api from '../axios'

// ===== BACKUP INTERFACES =====
export interface Backup {
  id: number
  name: string
  description?: string
  fileName: string
  fileSize: number
  filePath: string
  backupType: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  status: 'CREATING' | 'COMPLETED' | 'FAILED' | 'VALIDATING' | 'VALIDATED' | 'INVALID'
  createdAt: string
  completedAt?: string
  validatedAt?: string
  isValid: boolean
  checksum: string
  compressionRatio?: number
  databaseVersion: string
  tablesIncluded: string[]
  recordsCount: number
}

export interface BackupCreateRequest {
  name: string
  description?: string
  backupType: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  includeTables?: string[]
  excludeTables?: string[]
  compress: boolean
  encrypt: boolean
}

export interface BackupListResponse {
  success: boolean
  message: string
  data: {
    backups: Backup[]
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
  }
  errors: string[]
}

export interface BackupResponse {
  success: boolean
  message: string
  data: Backup
  errors: string[]
}

export interface BackupValidationResponse {
  success: boolean
  message: string
  data: {
    isValid: boolean
    checksumMatch: boolean
    fileIntegrity: boolean
    databaseIntegrity: boolean
    errors: string[]
  }
  errors: string[]
}

export interface BackupRestoreRequest {
  backupId: number
  restoreToDatabase?: string
  includeTables?: string[]
  excludeTables?: string[]
  dropExistingTables: boolean
  createBackupBeforeRestore: boolean
}

export interface BackupRestoreResponse {
  success: boolean
  message: string
  data: {
    restoreId: string
    status: 'STARTING' | 'IN_PROGRESS' | 'COMPLETED' | 'FAILED'
    progress: number
    estimatedTimeRemaining?: string
  }
  errors: string[]
}

export interface BackupStatusResponse {
  success: boolean
  message: string
  data: {
    systemStatus: 'HEALTHY' | 'WARNING' | 'CRITICAL'
    lastBackup?: string
    nextScheduledBackup?: string
    totalBackups: number
    totalSize: number
    availableSpace: number
    backupLocation: string
    autoBackupEnabled: boolean
    compressionEnabled: boolean
    encryptionEnabled: boolean
  }
  errors: string[]
}

export interface BackupSchedule {
  id: number
  name: string
  description?: string
  scheduleType: 'DAILY' | 'WEEKLY' | 'MONTHLY'
  scheduleTime: string
  backupType: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  isActive: boolean
  retentionDays: number
  maxBackups: number
  createdAt: string
  lastRun?: string
  nextRun?: string
}

export interface BackupScheduleRequest {
  name: string
  description?: string
  scheduleType: 'DAILY' | 'WEEKLY' | 'MONTHLY'
  scheduleTime: string
  backupType: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  retentionDays: number
  maxBackups: number
  isActive: boolean
}

export interface BackupScheduleResponse {
  success: boolean
  message: string
  data: BackupSchedule
  errors: string[]
}

export interface BackupScheduleListResponse {
  success: boolean
  message: string
  data: BackupSchedule[]
  errors: string[]
}

export interface BackupCleanupRequest {
  olderThanDays?: number
  keepLatest?: number
  dryRun: boolean
}

export interface BackupCleanupResponse {
  success: boolean
  message: string
  data: {
    deletedBackups: number
    freedSpace: number
    deletedFiles: string[]
  }
  errors: string[]
}

export interface BackupHealthResponse {
  success: boolean
  message: string
  data: {
    systemHealth: 'EXCELLENT' | 'GOOD' | 'FAIR' | 'POOR'
    backupSuccessRate: number
    averageBackupTime: number
    averageRestoreTime: number
    compressionEfficiency: number
    storageUtilization: number
    lastHealthCheck: string
    recommendations: string[]
  }
  errors: string[]
}

// ===== BACKUP API FUNCTIONS =====

// Create a new database backup
export const createBackup = async (backupData: BackupCreateRequest): Promise<BackupResponse> => {
  const response = await api.post<Backup>('/Backup/create', backupData)
  return {
    success: true,
    message: 'Backup creation started successfully',
    data: response.data,
    errors: []
  }
}

// Get list of backups with filtering and pagination
export const getBackupList = async (params?: {
  page?: number
  pageSize?: number
  status?: string
  backupType?: string
  startDate?: string
  endDate?: string
}): Promise<BackupListResponse> => {
  const response = await api.get<{
    backups: Backup[]
    totalCount: number
    page: number
    pageSize: number
    totalPages: number
  }>('/Backup/list', { params })
  return {
    success: true,
    message: 'Backups retrieved successfully',
    data: response.data,
    errors: []
  }
}

// Get backup details by ID
export const getBackupById = async (backupId: string): Promise<BackupResponse> => {
  const response = await api.get<Backup>(`/Backup/${backupId}`)
  return {
    success: true,
    message: 'Backup details retrieved successfully',
    data: response.data,
    errors: []
  }
}

// Delete a backup
export const deleteBackup = async (backupId: string): Promise<{ success: boolean; message: string; errors: string[] }> => {
  await api.delete(`/Backup/${backupId}`)
  return {
    success: true,
    message: 'Backup deleted successfully',
    errors: []
  }
}

// Validate backup integrity
export const validateBackup = async (backupId: string): Promise<BackupValidationResponse> => {
  const response = await api.get<{
    isValid: boolean
    checksumMatch: boolean
    fileIntegrity: boolean
    databaseIntegrity: boolean
    errors: string[]
  }>(`/Backup/${backupId}/validate`)
  return {
    success: true,
    message: 'Backup validation completed',
    data: response.data,
    errors: []
  }
}

// Restore database from backup
export const restoreBackup = async (restoreData: BackupRestoreRequest): Promise<BackupRestoreResponse> => {
  const response = await api.post<{
    restoreId: string
    status: 'STARTING' | 'IN_PROGRESS' | 'COMPLETED' | 'FAILED'
    progress: number
    estimatedTimeRemaining?: string
  }>('/Backup/restore', restoreData)
  return {
    success: true,
    message: 'Backup restore started successfully',
    data: response.data,
    errors: []
  }
}

// Get backup system status
export const getBackupStatus = async (): Promise<BackupStatusResponse> => {
  const response = await api.get<{
    systemStatus: 'HEALTHY' | 'WARNING' | 'CRITICAL'
    lastBackup?: string
    nextScheduledBackup?: string
    totalBackups: number
    totalSize: number
    availableSpace: number
    backupLocation: string
    autoBackupEnabled: boolean
    compressionEnabled: boolean
    encryptionEnabled: boolean
  }>('/Backup/status')
  return {
    success: true,
    message: 'Backup status retrieved successfully',
    data: response.data,
    errors: []
  }
}

// Download backup file
export const downloadBackup = async (backupId: string): Promise<Blob> => {
  const response = await api.get(`/Backup/${backupId}/download`, {
    responseType: 'blob'
  })
  return response.data
}

// Schedule automatic backup
export const scheduleBackup = async (scheduleData: BackupScheduleRequest): Promise<BackupScheduleResponse> => {
  const response = await api.post<BackupSchedule>('/Backup/schedule', scheduleData)
  return {
    success: true,
    message: 'Backup schedule created successfully',
    data: response.data,
    errors: []
  }
}

// Get list of scheduled backups
export const getScheduledBackups = async (): Promise<BackupScheduleListResponse> => {
  const response = await api.get<BackupSchedule[]>('/Backup/scheduled')
  return {
    success: true,
    message: 'Scheduled backups retrieved successfully',
    data: response.data,
    errors: []
  }
}

// Cancel scheduled backup
export const cancelScheduledBackup = async (scheduleId: string): Promise<{ success: boolean; message: string; errors: string[] }> => {
  await api.delete(`/Backup/schedule/${scheduleId}`)
  return {
    success: true,
    message: 'Scheduled backup cancelled successfully',
    errors: []
  }
}

// Clean up old backups
export const cleanupBackups = async (cleanupData: BackupCleanupRequest): Promise<BackupCleanupResponse> => {
  const response = await api.post<{
    deletedBackups: number
    freedSpace: number
    deletedFiles: string[]
  }>('/Backup/cleanup', cleanupData)
  return {
    success: true,
    message: 'Backup cleanup completed successfully',
    data: response.data,
    errors: []
  }
}

// Get backup statistics and health check
export const getBackupHealth = async (): Promise<BackupHealthResponse> => {
  const response = await api.get<{
    systemHealth: 'EXCELLENT' | 'GOOD' | 'FAIR' | 'POOR'
    backupSuccessRate: number
    averageBackupTime: number
    averageRestoreTime: number
    compressionEfficiency: number
    storageUtilization: number
    lastHealthCheck: string
    recommendations: string[]
  }>('/Backup/health')
  return {
    success: true,
    message: 'Backup health data retrieved successfully',
    data: response.data,
    errors: []
  }
}