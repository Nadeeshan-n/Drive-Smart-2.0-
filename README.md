# Drive Smart 2.0

Drive Smart 2.0 is a Windows desktop vehicle rental and fleet management application built with WPF and .NET 8. It provides role-based access for employees and includes modules for vehicles, customers, payments, maintenance, dashboards, and PDF reporting.

## Features

- Employee login, registration, session handling, and role-based permissions
- Admin, manager, and staff access controls
- Vehicle registration, public vehicle browsing, and maintenance management
- Customer management
- Payment entry and bill printing
- Dashboard and report views
- PDF report generation for vehicles, maintenance, and payments
- SQLite-backed local databases for each major module

## Tech Stack

- C# / .NET 8
- WPF
- SQLite
- Entity Framework Core
- BCrypt.Net for password hashing
- QuestPDF and ClosedXML for reports and exports
- LiveChartsCore for dashboard charts

## Project Structure

```text
Drive Smart 2.0/
├── Drive Smart 2.0.slnx
├── README.md
└── Drive Smart 2.0/
    ├── App.xaml
    ├── MainWindow.xaml
    ├── models/
    ├── Migrations/
    ├── UserControls/
    └── Views/
        ├── Auth/
        ├── Customer/
        ├── Dashboard/
        ├── Payment/
        ├── Reports/
        └── VehicleView/
```

## Prerequisites

- Windows 10 or later
- Visual Studio 2022 with the .NET desktop development workload
- .NET 8 SDK

## Getting Started

1. Clone the repository.

   ```powershell
   git clone <repository-url>
   cd "Drive Smart 2.0"
   ```

2. Restore dependencies.

   ```powershell
   dotnet restore
   ```

3. Build the application.

   ```powershell
   dotnet build
   ```

4. Run the application.

   ```powershell
   dotnet run --project "Drive Smart 2.0\Drive Smart 2.0.csproj"
   ```

You can also open `Drive Smart 2.0.slnx` in Visual Studio and run the project from there.

## Database

The application uses local SQLite database files stored inside module-specific folders:

- `Views\Auth\Database\EmployeeDB.db`
- `Views\Customer\Database\DriveSmart.db`
- `Views\Payment\Database\Payment.db`
- `Views\VehicleView\Database\vehicles.db`
- `Views\VehicleView\Database\maintenance.db`

Entity Framework Core migrations are included for the employee authentication database under `Migrations/`.

## Main Modules

- **Authentication:** Employee login, registration, password changes, and session management.
- **Dashboard:** Main navigation and overview screen.
- **Vehicles:** Vehicle listing, registration, and maintenance workflows.
- **Customers:** Customer records and administration.
- **Payments:** Payment details, billing, and print flow.
- **Reports:** PDF report output for operational data.

## Roles and Permissions

Drive Smart 2.0 supports three employee roles:

- **Admin:** Full access to all modules.
- **Manager:** Limited access; employee management is hidden.
- **Staff:** Restricted access to customer and public vehicle workflows.

## Notes for Developers

- The main WPF shell is `MainWindow.xaml`.
- Authentication state is managed through `SessionManager`.
- Report output files are generated under `Views\Reports\Output`.
- Keep database files backed up before making schema or migration changes.

