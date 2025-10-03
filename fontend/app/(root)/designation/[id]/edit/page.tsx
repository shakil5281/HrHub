import { DesignationEditForm } from "@/components/designation/designation-edit-form"

interface EditDesignationPageProps {
  params: {
    id: string
  }
}

export default function EditDesignationPage({ params }: EditDesignationPageProps) {
  return <DesignationEditForm designationId={params.id} />
}
