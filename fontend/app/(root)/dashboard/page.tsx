export default function DashboardPage() {
  return (
    <div className="container mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Dashboard</h1>
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-semibold mb-4">Welcome to HrHub</h2>
          <p className="text-gray-600">
            You have successfully logged in to the HR management system.
          </p>
        </div>
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-semibold mb-4">Quick Actions</h2>
          <ul className="space-y-2">
            <li>
              <a href="/employees" className="text-blue-600 hover:underline">
                Manage Employees
              </a>
            </li>
            <li>
              <a href="/reports" className="text-blue-600 hover:underline">
                View Reports
              </a>
            </li>
            <li>
              <a href="/settings" className="text-blue-600 hover:underline">
                Settings
              </a>
            </li>
          </ul>
        </div>
        <div className="bg-white p-6 rounded-lg shadow-md">
          <h2 className="text-xl font-semibold mb-4">System Status</h2>
          <p className="text-green-600">All systems operational</p>
        </div>
      </div>
    </div>
  )
}
