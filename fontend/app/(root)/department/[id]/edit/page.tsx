import { DepartmentEditForm } from "@/components/department/department-edit-form"

interface EditDepartmentPageProps {
  params: {
    id: string
  }
}

export default function EditDepartmentPage({ params }: EditDepartmentPageProps) {
  return <DepartmentEditForm departmentId={params.id} />
}
