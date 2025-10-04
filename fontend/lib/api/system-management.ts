import api from '../axios'

// Types for system management
export interface SystemInfo {
  systemName: string
  version: string
  buildDate: string
  environment: string
  uptime: string
  serverTime: string
  timezone: string
  os: string
  architecture: string
  cpuCores: number
  totalMemory: string
  availableMemory: string
  diskSpace: string
  networkInterfaces: NetworkInterface[]
}

export interface NetworkInterface {
  name: string
  ipAddress: string
  macAddress: string
  status: 'up' | 'down'
}

export interface DatabaseInfo {
  name: string
  version: string
  host: string
  port: number
  status: 'online' | 'offline'
  uptime: string
  connections: number
  maxConnections: number
  databaseSize: string
  lastBackup: string
  characterSet: string
  collation: string
}

export interface SystemHealth {
  status: 'healthy' | 'warning' | 'critical'
  overallScore: number
  checks: HealthCheck[]
  lastChecked: string
}

export interface HealthCheck {
  name: string
  status: 'pass' | 'fail' | 'warning'
  message: string
  responseTime?: number
  lastChecked: string
}

export interface SystemStatistics {
  totalUsers: number
  activeUsers: number
  totalEmployees: number
  activeEmployees: number
  totalCompanies: number
  totalDepartments: number
  totalSections: number
  totalDesignations: number
  attendanceRecords: number
  backupFiles: number
  systemLoad: SystemLoad
  memoryUsage: MemoryUsage
  diskUsage: DiskUsage
}

export interface SystemLoad {
  cpuUsage: number
  loadAverage1m: number
  loadAverage5m: number
  loadAverage15m: number
}

export interface MemoryUsage {
  total: string
  used: string
  free: string
  percentage: number
}

export interface DiskUsage {
  total: string
  used: string
  free: string
  percentage: number
}

export interface TableInfo {
  tableName: string
  rowCount: number
  dataSize: string
  indexSize: string
  totalSize: string
  engine: string
  collation: string
  createdAt: string
  updatedAt: string
}

export interface DatabasePerformance {
  queriesPerSecond: number
  averageQueryTime: number
  slowQueries: number
  connectionsActive: number
  connectionsIdle: number
  bufferPoolHitRate: number
  cacheHitRate: number
  lockWaits: number
  deadlocks: number
}

export interface DatabaseConnection {
  id: number
  user: string
  host: string
  database: string
  command: string
  time: number
  state: string
  info?: string
}

export interface SlowQuery {
  query: string
  executionTime: number
  rowsExamined: number
  rowsSent: number
  timestamp: string
  user: string
  host: string
}

export interface IndexUsage {
  tableName: string
  indexName: string
  cardinality: number
  usageCount: number
  lastUsed: string
  size: string
}

export interface DatabaseConfiguration {
  key: string
  value: string
  description: string
  category: string
}

export interface DatabaseAlert {
  id: number
  type: 'warning' | 'error' | 'info'
  message: string
  timestamp: string
  resolved: boolean
  severity: 'low' | 'medium' | 'high' | 'critical'
}

export interface SystemConfiguration {
  key: string
  value: string
  description: string
  category: string
  editable: boolean
}

export interface VersionInfo {
  applicationVersion: string
  buildNumber: string
  buildDate: string
  gitCommit: string
  gitBranch: string
  nodeVersion: string
  npmVersion: string
  dependencies: DependencyInfo[]
}

export interface DependencyInfo {
  name: string
  version: string
  latestVersion: string
  outdated: boolean
}

export interface EnvironmentInfo {
  nodeEnv: string
  port: number
  databaseUrl: string
  redisUrl?: string
  logLevel: string
  features: string[]
  configurations: Record<string, string | number | boolean>
}

// API Functions
export const getSystemInfo = async (): Promise<SystemInfo> => {
  const response = await api.get('/SystemManagement/info')
  return response.data
}

export const getDatabaseInfo = async (): Promise<DatabaseInfo> => {
  const response = await api.get('/SystemManagement/database/info')
  return response.data
}

export const getSystemHealth = async (): Promise<SystemHealth> => {
  const response = await api.get('/SystemManagement/health')
  return response.data
}

export const getSystemStatistics = async (): Promise<SystemStatistics> => {
  const response = await api.get('/SystemManagement/statistics')
  return response.data
}

export const getDatabaseTables = async (): Promise<TableInfo[]> => {
  const response = await api.get('/SystemManagement/database/tables')
  return response.data
}

export const getTableInfo = async (tableName: string): Promise<TableInfo> => {
  const response = await api.get(`/SystemManagement/database/tables/${tableName}`)
  return response.data
}

export const getDatabasePerformance = async (): Promise<DatabasePerformance> => {
  const response = await api.get('/SystemManagement/database/performance')
  return response.data
}

export const getDatabaseConnections = async (): Promise<DatabaseConnection[]> => {
  const response = await api.get('/SystemManagement/database/connections')
  return response.data
}

export const getSlowQueries = async (): Promise<SlowQuery[]> => {
  const response = await api.get('/SystemManagement/database/slow-queries')
  return response.data
}

export const getIndexUsage = async (): Promise<IndexUsage[]> => {
  const response = await api.get('/SystemManagement/database/index-usage')
  return response.data
}

export const getDatabaseStatus = async (): Promise<{ status: 'online' | 'offline'; responseTime: number }> => {
  const response = await api.get('/SystemManagement/database/status')
  return response.data
}

export const getDatabaseConfiguration = async (): Promise<DatabaseConfiguration[]> => {
  const response = await api.get('/SystemManagement/database/configuration')
  return response.data
}

export const getDatabaseAlerts = async (): Promise<DatabaseAlert[]> => {
  const response = await api.get('/SystemManagement/database/alerts')
  return response.data
}

export const getSystemConfiguration = async (): Promise<SystemConfiguration[]> => {
  const response = await api.get('/SystemManagement/configuration')
  return response.data
}

export const getVersionInfo = async (): Promise<VersionInfo> => {
  const response = await api.get('/SystemManagement/version')
  return response.data
}

export const getEnvironmentInfo = async (): Promise<EnvironmentInfo> => {
  const response = await api.get('/SystemManagement/environment')
  return response.data
}
