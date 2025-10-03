import { SectionEditForm } from "@/components/section/section-edit-form"

interface EditSectionPageProps {
  params: {
    id: string
  }
}

export default function EditSectionPage({ params }: EditSectionPageProps) {
  return <SectionEditForm sectionId={params.id} />
}
