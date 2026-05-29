folder structure
```
DriveSmart
│
├── assets
├── data
├── helpers
├── models
├── reports
├── services
├── viewModels
├── views
│
├── App.xaml
└── MainWindow.xaml
#2
DriveSmart
│
├── Models           (Shared)
├── Data             (Shared)
├── Helpers          (Shared)
│
├── Views
│   ├── LoginView.xaml          ← Member 1
│   ├── VehicleView.xaml        ← Member 2
│   ├── RentalView.xaml         ← Member 3
│   ├── ReportsView.xaml        ← Member 4
│   └── DashboardView.xaml      ← Member 5
│
├── ViewModels
│   ├── LoginViewModel.cs
│   ├── VehicleViewModel.cs
│   ├── RentalViewModel.cs
│   ├── ReportsViewModel.cs
│   └── DashboardViewModel.cs
│
└── Services
    ├── AuthService.cs
    ├── VehicleService.cs
    ├── RentalService.cs
    ├── ReportService.cs
    └── DashboardService.cs
```
