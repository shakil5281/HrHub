import api from '../axios'

export interface Company {
  companyId?: string
  name: string
  companyNameBangla: string
  description: string
  phone: string
  email: string
  address: string
  addressBangla: string
  city: string
  state: string
  postalCode: string
  country: string
  logoUrl: string
  authorizedSignature: string
  // Legacy fields for backward compatibility
  id?: string
  createdAt?: string
  updatedAt?: string
}

export interface CompanyResponse {
  success: boolean
  message: string
  data: Company
  errors: string[]
}

export interface CompaniesResponse {
  success: boolean
  message: string
  data: Company[]
  errors: string[]
}

export interface CompanyStatistics {
  totalCompanies: number
  totalEmployees: number
  averageEmployeeCount: number
  companiesByIndustry: Record<string, number>
}

export interface CompanyStatisticsResponse {
  success: boolean
  message: string
  data: CompanyStatistics
  errors: string[]
}

// Get all companies (Admin only)
export const getAllCompanies = async (): Promise<CompaniesResponse> => {
  const response = await api.get<CompaniesResponse>('/Company')
  return response.data
}

// Create a new company (Admin only)
export const createCompany = async (company: Omit<Company, 'id' | 'createdAt' | 'updatedAt'>): Promise<CompanyResponse> => {
  const response = await api.post<CompanyResponse>('/Company', company)
  return response.data
}

// Get company by ID (Admin only)
export const getCompanyById = async (id: string): Promise<CompanyResponse> => {
  const response = await api.get<CompanyResponse>(`/Company/${id}`)
  return response.data
}

// Update an existing company (Admin only)
export const updateCompany = async (id: string, company: Partial<Company>): Promise<CompanyResponse> => {
  const response = await api.put<CompanyResponse>(`/Company/${id}`, company)
  return response.data
}

// Delete a company (Admin only)
export const deleteCompany = async (id: string): Promise<{ success: boolean; message: string }> => {
  const response = await api.delete(`/Company/${id}`)
  return response.data
}

// Get company statistics (Admin only)
export const getCompanyStatistics = async (): Promise<CompanyStatisticsResponse> => {
  const response = await api.get<CompanyStatisticsResponse>('/Company/statistics')
  return response.data
}
