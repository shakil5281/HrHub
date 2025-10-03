import { UserEditForm } from "@/components/user/user-edit-form"

interface EditUserPageProps {
  params: {
    id: string
  }
}

export default function EditUserPage({ params }: EditUserPageProps) {
  return <UserEditForm userId={params.id} />
}
