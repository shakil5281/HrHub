import api from '../axios'

export interface Employee {
  id: number
  empId: string
  name: string
  nameBangla: string
  nidNo: string
  fatherName?: string
  motherName?: string
  fatherNameBangla?: string
  motherNameBangla?: string
  dateOfBirth?: string
  permanentAddress?: string
  permanentDivision?: string
  permanentDistrict?: string
  permanentUpazila?: string
  permanentPostalCode?: string
  presentAddress?: string
  presentDivision?: string
  presentDistrict?: string
  presentUpazila?: string
  presentPostalCode?: string
  joiningDate: string
  bloodGroup?: string
  gender?: string
  religion?: string
  maritalStatus?: string
  education?: string
  companyId: number
  departmentId: number
  sectionId: number
  designationId: number
  lineId?: number
  shiftId?: number
  degreeId?: number
  floor?: string
  empType?: string
  group?: string
  house?: number
  rentMedical?: number
  food?: number
  conveyance?: number
  transport?: number
  nightBill?: number
  mobileBill?: number
  otherAllowance?: number
  grossSalary: number
  basicSalary: number
  salaryType?: string
  bankAccountNo?: string
  bank?: string
  departmentName: string
  sectionName: string
  designationName: string
  designationGrade: string
  isActive: boolean
  createdAt: string
}

export interface CreateEmployeePayload {
  empId: string
  name: string
  nameBangla: string
  nidNo: string
  fatherName?: string
  motherName?: string
  fatherNameBangla?: string
  motherNameBangla?: string
  dateOfBirth?: string
  permanentAddress?: string
  permanentDivision?: string
  permanentDistrict?: string
  permanentUpazila?: string
  permanentPostalCode?: string
  presentAddress?: string
  presentDivision?: string
  presentDistrict?: string
  presentUpazila?: string
  presentPostalCode?: string
  joiningDate: string
  bloodGroup?: string
  gender?: string
  religion?: string
  maritalStatus?: string
  education?: string
  companyId: number
  departmentId: number
  sectionId: number
  designationId: number
  lineId?: number
  shiftId?: number
  degreeId?: number
  floor?: string
  empType?: string
  group?: string
  house?: number
  rentMedical?: number
  food?: number
  conveyance?: number
  transport?: number
  nightBill?: number
  mobileBill?: number
  otherAllowance?: number
  grossSalary: number
  basicSalary: number
  salaryType?: string
  bankAccountNo?: string
  bank?: string
}

export interface UpdateEmployeePayload {
  empId?: string
  name?: string
  nameBangla?: string
  nidNo?: string
  fatherName?: string
  motherName?: string
  fatherNameBangla?: string
  motherNameBangla?: string
  dateOfBirth?: string
  permanentAddress?: string
  permanentDivision?: string
  permanentDistrict?: string
  permanentUpazila?: string
  permanentPostalCode?: string
  presentAddress?: string
  presentDivision?: string
  presentDistrict?: string
  presentUpazila?: string
  presentPostalCode?: string
  joiningDate?: string
  bloodGroup?: string
  gender?: string
  religion?: string
  maritalStatus?: string
  education?: string
  companyId?: number
  departmentId?: number
  sectionId?: number
  designationId?: number
  lineId?: number
  shiftId?: number
  degreeId?: number
  floor?: string
  empType?: string
  group?: string
  house?: number
  rentMedical?: number
  food?: number
  conveyance?: number
  transport?: number
  nightBill?: number
  mobileBill?: number
  otherAllowance?: number
  grossSalary?: number
  basicSalary?: number
  salaryType?: string
  bankAccountNo?: string
  bank?: string
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
