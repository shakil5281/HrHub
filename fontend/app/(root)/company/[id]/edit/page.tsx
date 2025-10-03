import { CompanyEditForm } from "@/components/company/company-edit-form"

interface EditCompanyPageProps {
  params: {
    id: string
  }
}

export default function EditCompanyPage({ params }: EditCompanyPageProps) {
  return <CompanyEditForm companyId={params.id} />
}
