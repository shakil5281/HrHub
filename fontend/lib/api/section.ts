import api from '../axios'

export interface Section {
  id: number
  departmentId: number
  departmentName: string
  name: string
  nameBangla: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface SectionCreateRequest {
  departmentId: number
  name: string
  nameBangla: string
}

export interface SectionResponse {
  success: boolean
  message: string
  data: Section
  errors: string[]
}

export interface SectionsResponse {
  success: boolean
  message: string
  data: Section[]
  errors: string[]
}

// Get all sections
export const getAllSections = async (): Promise<SectionsResponse> => {
  const response = await api.get<SectionsResponse>('/Section')
  return response.data
}

// Create a new section
export const createSection = async (section: SectionCreateRequest): Promise<SectionResponse> => {
  const response = await api.post<SectionResponse>('/Section', section)
  return response.data
}

// Get section by ID
export const getSectionById = async (id: string | number): Promise<SectionResponse> => {
  const response = await api.get<SectionResponse>(`/Section/${id}`)
  return response.data
}

// Update an existing section
export const updateSection = async (id: string | number, section: SectionCreateRequest): Promise<SectionResponse> => {
  const response = await api.put<SectionResponse>(`/Section/${id}`, section)
  return response.data
}

// Delete a section
export const deleteSection = async (id: string | number): Promise<{ success: boolean; message: string }> => {
  const response = await api.delete(`/Section/${id}`)
  return response.data
}

// Get sections by department ID
export const getSectionsByDepartment = async (departmentId: string | number): Promise<SectionsResponse> => {
  const response = await api.get<SectionsResponse>(`/Section/department/${departmentId}`)
  return response.data
}

// Get sections by company ID
export const getSectionsByCompany = async (companyId: string | number): Promise<SectionsResponse> => {
  const response = await api.get<SectionsResponse>(`/Section/company/${companyId}`)
  return response.data
}
