import api from '../axios'

export interface Backup {
  id: string
  name: string
  description?: string
  fileName: string
  fileSize: number
  filePath: string
  backupType: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  status: 'PENDING' | 'IN_PROGRESS' | 'COMPLETED' | 'FAILED' | 'CANCELLED'
  createdAt: string
  completedAt?: string
  createdBy: string
  isValidated: boolean
  validationDate?: string
  checksum?: string
  compressionType?: 'NONE' | 'GZIP' | 'ZIP'
  encryptionEnabled: boolean
  retentionDays?: number
  expiresAt?: string
}

export interface CreateBackupPayload {
  name: string
  description?: string
  backupType?: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  compressionType?: 'NONE' | 'GZIP' | 'ZIP'
  encryptionEnabled?: boolean
  retentionDays?: number
}

export interface BackupListResponse {
  success: boolean
  message: string
  data: {
    backups: Backup[]
    totalCount: number
    pageNumber: number
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

export interface BackupStatusResponse {
  success: boolean
  message: string
  data: {
    systemStatus: 'HEALTHY' | 'WARNING' | 'ERROR'
    lastBackupDate?: string
    nextScheduledBackup?: string
    totalBackups: number
    totalSize: number
    availableSpace: number
    backupLocation: string
    isBackupInProgress: boolean
    healthChecks: {
      diskSpace: 'OK' | 'WARNING' | 'ERROR'
      backupLocation: 'OK' | 'WARNING' | 'ERROR'
      systemResources: 'OK' | 'WARNING' | 'ERROR'
    }
  }
  errors: string[]
}

export interface BackupHealthResponse {
  success: boolean
  message: string
  data: {
    totalBackups: number
    successfulBackups: number
    failedBackups: number
    totalSize: number
    averageBackupTime: number
    lastBackupDate?: string
    oldestBackupDate?: string
    backupSuccessRate: number
    storageUtilization: number
    healthScore: number
  }
  errors: string[]
}

export interface BackupValidationResponse {
  success: boolean
  message: string
  data: {
    isValid: boolean
    checksumMatch: boolean
    fileIntegrity: boolean
    validationDate: string
    issues?: string[]
  }
  errors: string[]
}

export interface RestoreBackupPayload {
  backupId: string
  restoreToDatabase?: string
  confirmRestore: boolean
  backupBeforeRestore?: boolean
}

export interface RestoreBackupResponse {
  success: boolean
  message: string
  data: {
    restoreId: string
    status: 'PENDING' | 'IN_PROGRESS' | 'COMPLETED' | 'FAILED'
    estimatedTime?: number
    progress?: number
  }
  errors: string[]
}

export interface BackupSchedule {
  id: string
  name: string
  description?: string
  scheduleType: 'DAILY' | 'WEEKLY' | 'MONTHLY' | 'CUSTOM'
  scheduleExpression: string
  backupType: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  isActive: boolean
  nextRun?: string
  lastRun?: string
  retentionDays: number
  compressionType: 'NONE' | 'GZIP' | 'ZIP'
  encryptionEnabled: boolean
  createdAt: string
  createdBy: string
}

export interface CreateSchedulePayload {
  name: string
  description?: string
  scheduleType: 'DAILY' | 'WEEKLY' | 'MONTHLY' | 'CUSTOM'
  scheduleExpression: string
  backupType: 'FULL' | 'INCREMENTAL' | 'DIFFERENTIAL'
  retentionDays: number
  compressionType?: 'NONE' | 'GZIP' | 'ZIP'
  encryptionEnabled?: boolean
}

export interface ScheduleResponse {
  success: boolean
  message: string
  data: BackupSchedule
  errors: string[]
}

export interface SchedulesResponse {
  success: boolean
  message: string
  data: BackupSchedule[]
  errors: string[]
}

export interface CleanupResponse {
  success: boolean
  message: string
  data: {
    deletedBackups: number
    freedSpace: number
    cleanupDate: string
  }
  errors: string[]
}

// Create a new database backup
export const createBackup = async (payload: CreateBackupPayload): Promise<BackupResponse> => {
  const response = await api.post<BackupResponse>('/Backup/create', payload)
  return response.data
}

// Get list of backups with filtering and pagination
export const getBackups = async (params?: {
  page?: number
  pageSize?: number
  status?: string
  backupType?: string
  dateFrom?: string
  dateTo?: string
}): Promise<BackupListResponse> => {
  const response = await api.get<BackupListResponse>('/Backup/list', { params })
  return response.data
}

// Get backup details by ID
export const getBackupById = async (id: string): Promise<BackupResponse> => {
  const response = await api.get<BackupResponse>(`/Backup/${id}`)
  return response.data
}

// Delete a backup
export const deleteBackup = async (id: string): Promise<{ success: boolean; message: string }> => {
  const response = await api.delete(`/Backup/${id}`)
  return response.data
}

// Validate backup integrity
export const validateBackup = async (id: string): Promise<BackupValidationResponse> => {
  const response = await api.get<BackupValidationResponse>(`/Backup/${id}/validate`)
  return response.data
}

// Restore database from backup
export const restoreBackup = async (payload: RestoreBackupPayload): Promise<RestoreBackupResponse> => {
  const response = await api.post<RestoreBackupResponse>('/Backup/restore', payload)
  return response.data
}

// Get backup system status
export const getBackupStatus = async (): Promise<BackupStatusResponse> => {
  const response = await api.get<BackupStatusResponse>('/Backup/status')
  return response.data
}

// Download backup file
export const downloadBackup = async (id: string): Promise<Blob> => {
  const response = await api.get(`/Backup/${id}/download`, {
    responseType: 'blob'
  })
  return response.data
}

// Schedule automatic backup
export const scheduleBackup = async (payload: CreateSchedulePayload): Promise<ScheduleResponse> => {
  const response = await api.post<ScheduleResponse>('/Backup/schedule', payload)
  return response.data
}

// Get list of scheduled backups
export const getScheduledBackups = async (): Promise<SchedulesResponse> => {
  const response = await api.get<SchedulesResponse>('/Backup/scheduled')
  return response.data
}

// Cancel scheduled backup
export const cancelScheduledBackup = async (scheduleId: string): Promise<{ success: boolean; message: string }> => {
  const response = await api.delete(`/Backup/schedule/${scheduleId}`)
  return response.data
}

// Clean up old backups
export const cleanupBackups = async (params?: {
  olderThanDays?: number
  keepCount?: number
  dryRun?: boolean
}): Promise<CleanupResponse> => {
  const response = await api.post<CleanupResponse>('/Backup/cleanup', params)
  return response.data
}

// Get backup statistics and health check
export const getBackupHealth = async (): Promise<BackupHealthResponse> => {
  const response = await api.get<BackupHealthResponse>('/Backup/health')
  return response.data
}
