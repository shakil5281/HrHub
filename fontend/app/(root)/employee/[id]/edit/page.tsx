"use client"

import { EmployeeEditForm } from "@/components/employee/employee-edit-form"

interface EmployeeEditPageProps {
  params: {
    id: string
  }
}

export default function EmployeeEditPage({ params }: EmployeeEditPageProps) {
  const employeeId = parseInt(params.id)
  
  return <EmployeeEditForm employeeId={employeeId} />
}
