import api from '../axios'

export interface Employee {
  id: number
  name: string
  nameBangla: string
  nidNo: string
  fatherName?: string
  motherName?: string
  fatherNameBangla?: string
  motherNameBangla?: string
  dateOfBirth?: string
  address?: string
  joiningDate: string
  departmentName: string
  sectionName: string
  designationName: string
  designationGrade: string
  grossSalary: number
  basicSalary: number
  bankAccountNo?: string
  isActive: boolean
  createdAt: string
}

export interface CreateEmployeePayload {
  name: string
  nameBangla: string
  nidNo: string
  fatherName?: string
  motherName?: string
  fatherNameBangla?: string
  motherNameBangla?: string
  dateOfBirth?: string
  address?: string
  joiningDate: string
  departmentId: number
  sectionId: number
  designationId: number
  grossSalary: number
  basicSalary: number
  bankAccountNo?: string
}

export interface UpdateEmployeePayload {
  name?: string
  nameBangla?: string
  nidNo?: string
  fatherName?: string
  motherName?: string
  fatherNameBangla?: string
  motherNameBangla?: string
  dateOfBirth?: string
  address?: string
  joiningDate?: string
  departmentId?: number
  sectionId?: number
  designationId?: number
  grossSalary?: number
  basicSalary?: number
  bankAccountNo?: string
}

export interface EmployeeResponse {
  success: boolean
  message: string
  data: Employee
  errors: string[]
}

export interface EmployeesResponse {
  success: boolean
  message: string
  data: Employee[]
  errors: string[]
}

export interface EmployeeSummaryResponse {
  success: boolean
  message: string
  data: {
    id: number
    name: string
    nameBangla: string
    departmentName: string
    designationName: string
  }[]
  errors: string[]
}

export interface DeleteEmployeeResponse {
  success: boolean
  message: string
  errors: string[]
}

// Get all employees with optional filtering
export const getAllEmployees = async (): Promise<EmployeesResponse> => {
  const response = await api.get<EmployeesResponse>('/Employee')
  return response.data
}

// Create a new employee
export const createEmployee = async (payload: CreateEmployeePayload): Promise<EmployeeResponse> => {
  const response = await api.post<EmployeeResponse>('/Employee', payload)
  return response.data
}

// Get employee by ID
export const getEmployeeById = async (id: number): Promise<EmployeeResponse> => {
  const response = await api.get<EmployeeResponse>(`/Employee/${id}`)
  return response.data
}

// Update an existing employee
export const updateEmployee = async (id: number, payload: UpdateEmployeePayload): Promise<EmployeeResponse> => {
  const response = await api.put<EmployeeResponse>(`/Employee/${id}`, payload)
  return response.data
}

// Soft delete an employee (Admin and IT only)
export const deleteEmployee = async (id: number): Promise<DeleteEmployeeResponse> => {
  const response = await api.delete<DeleteEmployeeResponse>(`/Employee/${id}`)
  return response.data
}

// Get employee summary (minimal info for dropdowns, etc.)
export const getEmployeeSummary = async (): Promise<EmployeeSummaryResponse> => {
  const response = await api.get<EmployeeSummaryResponse>('/Employee/summary')
  return response.data
}
