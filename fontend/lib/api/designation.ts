import api from '../axios'

export interface Designation {
  id?: number
  sectionId: number
  sectionName?: string
  departmentName?: string
  name: string
  nameBangla: string
  grade: string
  attendanceBonus: number
  isActive?: boolean
  createdAt?: string
}

export interface DesignationCreateRequest {
  sectionId: number
  name: string
  nameBangla: string
  grade: string
  attendanceBonus: number
}

export interface DesignationResponse {
  success: boolean
  message: string
  data: Designation
  errors: string[]
}

export interface DesignationsResponse {
  success: boolean
  message: string
  data: Designation[]
  errors: string[]
}

// Get all designations
export const getAllDesignations = async (): Promise<DesignationsResponse> => {
  const response = await api.get<DesignationsResponse>('/Designation')
  return response.data
}

// Create a new designation
export const createDesignation = async (designation: DesignationCreateRequest): Promise<DesignationResponse> => {
  const response = await api.post<DesignationResponse>('/Designation', designation)
  return response.data
}

// Get designation by ID
export const getDesignationById = async (id: string | number): Promise<DesignationResponse> => {
  const response = await api.get<DesignationResponse>(`/Designation/${id}`)
  return response.data
}

// Update an existing designation
export const updateDesignation = async (id: string | number, designation: DesignationCreateRequest): Promise<DesignationResponse> => {
  const response = await api.put<DesignationResponse>(`/Designation/${id}`, designation)
  return response.data
}

// Delete a designation
export const deleteDesignation = async (id: string | number): Promise<{ success: boolean; message: string }> => {
  const response = await api.delete(`/Designation/${id}`)
  return response.data
}

// Get designations by department ID
export const getDesignationsByDepartment = async (departmentId: string | number): Promise<DesignationsResponse> => {
  const response = await api.get<DesignationsResponse>(`/Designation/department/${departmentId}`)
  return response.data
}

// Get designations by section ID
export const getDesignationsBySection = async (sectionId: string | number): Promise<DesignationsResponse> => {
  const response = await api.get<DesignationsResponse>(`/Designation/section/${sectionId}`)
  return response.data
}
