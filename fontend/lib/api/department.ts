import api from '../axios'

export interface Department {
  id: number
  name: string
  nameBangla: string
  companyId: number
  companyName: string
  isActive: boolean
  createdAt: string
  updatedAt: string
}

export interface DepartmentCreateRequest {
  name: string
  nameBangla: string
  companyId: number
}

export interface DepartmentResponse {
  success: boolean
  message: string
  data: Department
  errors: string[]
}

export interface DepartmentsResponse {
  success: boolean
  message: string
  data: Department[]
  errors: string[]
}

// Get all departments
export const getAllDepartments = async (): Promise<DepartmentsResponse> => {
  const response = await api.get<DepartmentsResponse>('/Department')
  return response.data
}

// Create a new department
export const createDepartment = async (department: DepartmentCreateRequest): Promise<DepartmentResponse> => {
  const response = await api.post<DepartmentResponse>('/Department', department)
  return response.data
}

// Get department by ID
export const getDepartmentById = async (id: string | number): Promise<DepartmentResponse> => {
  const response = await api.get<DepartmentResponse>(`/Department/${id}`)
  return response.data
}

// Update an existing department
export const updateDepartment = async (id: string | number, department: DepartmentCreateRequest): Promise<DepartmentResponse> => {
  const response = await api.put<DepartmentResponse>(`/Department/${id}`, department)
  return response.data
}

// Delete a department
export const deleteDepartment = async (id: string | number): Promise<{ success: boolean; message: string }> => {
  const response = await api.delete(`/Department/${id}`)
  return response.data
}

// Get departments by company ID
export const getDepartmentsByCompany = async (companyId: string | number): Promise<DepartmentsResponse> => {
  const response = await api.get<DepartmentsResponse>(`/Department/company/${companyId}`)
  return response.data
}
