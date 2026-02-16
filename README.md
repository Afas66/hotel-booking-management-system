# 🏨 Hotel Booking Management System

[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![MySQL](https://img.shields.io/badge/MySQL-4479A1?style=for-the-badge&logo=mysql&logoColor=white)](https://www.mysql.com/)
[![Windows Forms](https://img.shields.io/badge/Windows%20Forms-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

A comprehensive desktop application for managing hotel and hostel room bookings. Built with modern C# .NET 8.0 and Windows Forms, featuring complete CRUD operations, role-based authentication, real-time analytics, and professional reporting capabilities.
RoomBookingPro/docs/Screenshot 2026-02-16 151346.png
*Real-time dashboard with statistics and analytics*

---

## 🎯 Problem Statement

Hotels and hostels require efficient systems to manage room bookings, customer information, payments, and operational reports. This application solves that challenge by providing a centralized, user-friendly desktop solution with:

- Real-time room availability tracking
- Automated payment processing and invoicing
- Comprehensive customer relationship management
- Role-based access control for staff hierarchy
- Detailed reporting and analytics

---

## ✨ Key Features

### 🔐 Authentication & Authorization
- Secure login system with password hashing
- Role-based access control (Admin, Manager, Staff)
- Session management and user activity tracking
- Multi-user support with different permission levels

### 👥 Customer Management
RoomBookingPro/docs/Screenshot 2026-02-16 151455.png

- Complete CRUD operations on customer records
- Advanced search and filtering capabilities
- Customer profile management (personal info, contact details, ID documents)
- Emergency contact information storage
- Active/Inactive customer status tracking

### 🛏️ Room/Bed Management
RoomBookingPro/docs/Screenshot 2026-02-16 151521.png

- Real-time room availability dashboard
- Multiple room types (Single, Double, Deluxe, Suite, Studio)
- Area-wise room organization (North Wing, South Wing, etc.)
- Dynamic pricing management
- Room status tracking (Available, Occupied, Maintenance, Reserved)
- Amenities and features listing

### 📅 Booking Management
RoomBookingPro/docs/Screenshot 2026-02-16 151539.png

- Intuitive booking creation workflow
- Automated rent and deposit calculation
- Check-in/Check-out processing
- Booking status management (Pending, Active, Completed, Cancelled)
- Conflict detection for double-bookings
- Booking history and modification tracking

### 💳 Payment Processing


- Multiple payment types (Monthly Rent, Deposits, Utilities, Maintenance)
- Various payment methods (Cash, Bank Transfer, Card, Mobile Payment)
- Automated invoice generation
- Payment history and receipt management
- Outstanding balance tracking
- Date range filtering and search

### 📊 Reports & Analytics
RoomBookingPro/docs/Screenshot 2026-02-16 151604.png

- Occupancy reports with date ranges
- Revenue analytics and summaries
- Customer reports and demographics
- Payment history reports
- Maintenance tracking reports
- CSV export functionality for all reports
- Print-ready report formatting

### 🎨 User Experience
- Modern, professional UI design with consistent color scheme
- Intuitive navigation with menu system
- Real-time data validation and user feedback
- Responsive layout optimized for desktop screens
- Keyboard shortcuts for common actions
- Loading indicators for async operations

---

## 🛠️ Technology Stack

### Core Technologies
- **Language:** C# 11
- **Framework:** .NET 8.0
- **UI Framework:** Windows Forms
- **Database:** MySQL 8.0
- **IDE:** Visual Studio 2022

### Libraries & Packages
- **MySql.Data** (v8.0.38) - Database connectivity
- **System.Data** - Data operations and DataTable management
- **Windows Forms Controls** - UI components

### Architecture & Patterns
- **Layered Architecture** - Separation of concerns (UI, Business Logic, Data Access)
- **OOP Principles** - Encapsulation, inheritance, and polymorphism
- **Repository Pattern** - Data access abstraction
- **MVC Pattern** - Model-View-Controller for form management

---

## 📊 System Architecture

```
┌─────────────────────────────────────────────────┐
│          Presentation Layer (Windows Forms)      │
│  ┌─────────────┬─────────────┬──────────────┐  │
│  │   Login     │  Dashboard  │  Management  │  │
│  │   Forms     │   Forms     │    Forms     │  │
│  └─────────────┴─────────────┴──────────────┘  │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│            Business Logic Layer                  │
│  ┌─────────────┬─────────────┬──────────────┐  │
│  │   Models    │  Validators │   Services   │  │
│  └─────────────┴─────────────┴──────────────┘  │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│           Data Access Layer                      │
│  ┌─────────────────────────────────────────┐   │
│  │     DatabaseHelper (Repository)         │   │
│  └─────────────────────────────────────────┘   │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│              MySQL Database                      │
│  ┌─────────┬─────────┬─────────┬──────────┐   │
│  │  Users  │Customers│  Rooms  │ Bookings │   │
│  ├─────────┼─────────┼─────────┼──────────┤   │
│  │Payments │  Areas  │  Types  │Maintenance│   │
│  └─────────┴─────────┴─────────┴──────────┘   │
└─────────────────────────────────────────────────┘
```

---

## 🗄️ Database Schema

### Entity Relationship Overview

```sql
Users (4 records)
  ├─ UserID (PK)
  ├─ Username
  ├─ Password (Hashed)
  └─ Role (Admin/Manager/Staff)

Customers (30 records)
  ├─ CustomerID (PK)
  ├─ Name, DOB, Gender
  ├─ Phone, Email, Address
  └─ IDNumber, Occupation

Areas (5 records)
  ├─ AreaID (PK)
  └─ AreaName, Location

RoomTypes (5 records)
  ├─ TypeID (PK)
  ├─ TypeName
  └─ BasePrice

Beds/Rooms (50 records)
  ├─ BedID (PK)
  ├─ AreaID (FK)
  ├─ TypeID (FK)
  ├─ BedNumber
  ├─ MonthlyRent
  └─ Status

Bookings (20 records)
  ├─ BookingID (PK)
  ├─ CustomerID (FK)
  ├─ BedID (FK)
  ├─ CheckInDate
  ├─ CheckOutDate
  ├─ MonthlyRent
  ├─ Deposit
  └─ Status

Payments (40 records)
  ├─ PaymentID (PK)
  ├─ BookingID (FK)
  ├─ Amount
  ├─ PaymentDate
  ├─ PaymentType
  └─ PaymentMethod

Maintenance (10 records)
  ├─ MaintenanceID (PK)
  ├─ BedID (FK)
  ├─ Description
  ├─ Status
  └─ Cost
```

---

## 🚀 Getting Started

### Prerequisites

Ensure you have the following installed:

- **Visual Studio 2022** (Community Edition or higher)
- **.NET 8.0 SDK** (comes with Visual Studio)
- **MySQL Server 8.0+**
- **MySQL Workbench** (optional, for database management)

### Installation Steps

#### 1️⃣ Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/hotel-booking-management-system.git
cd hotel-booking-management-system
```

#### 2️⃣ Database Setup

Open MySQL Workbench or MySQL Command Line and execute:

```bash
# Navigate to database folder
cd database

# Execute the schema file
mysql -u root -p < schema.sql
```

This will:
- Create the `RoomBookingDB` database
- Create all 8 tables with proper relationships
- Insert 150+ sample records for testing

#### 3️⃣ Configure Database Connection

Open `src/RoomBookingSystem/Data/DatabaseHelper.cs` and update line 9:

```csharp
private readonly string connectionString = 
    "Server=localhost;Database=RoomBookingDB;Uid=root;Pwd=YOUR_PASSWORD;";
```

Replace `YOUR_PASSWORD` with your MySQL root password.

#### 4️⃣ Build and Run

1. Open `RoomBookingSystem.sln` in Visual Studio 2022
2. Wait for NuGet packages to restore automatically
3. Build the solution: `Ctrl+Shift+B` or **Build → Build Solution**
4. Run the application: `F5` or click **Start** button
5. Wait for the splash screen to connect to database
6. Login with demo credentials (see below)

---

## 🔑 Demo Credentials

The system comes with pre-configured user accounts:

| Username | Password   | Role    | Permissions                          |
|----------|------------|---------|--------------------------------------|
| admin    | admin123   | Admin   | Full access to all features          |
| manager1 | admin123   | Manager | Customer, Room, Booking, Payment     |
| staff1   | admin123   | Staff   | Customer view, Booking view only     |

**Note:** For production use, change all passwords and implement proper security measures.

---

## 📸 Screenshots

### Splash Screen & Login
<img src="RoomBookingPro/docs/Screenshot 2026-02-16 151346.png" width="400"/> <img src="RoomBookingPro/docs/Screenshot 2026-02-16 151346.png"400"/>

### Dashboard
<img src="docs/screenshots/3-dashboard.png" width="800"/>

### Management Modules
<img src="docs/screenshots/4-customer-management.png" width="400"/> <img src="docs/screenshots/5-room-management.png" width="400"/>

### Booking & Payment
<img src="docs/screenshots/6-booking-management.png" width="400"/> <img src="docs/screenshots/7-payment-management.png" width="400"/>

### Reports
<img src="docs/screenshots/8-reports.png" width="800"/>

---

## 📁 Project Structure

```
hotel-booking-management-system/
│
├── src/
│   └── RoomBookingSystem/
│       ├── Forms/                      # All UI forms
│       │   ├── SplashScreenForm.cs
│       │   ├── LoginForm.cs
│       │   ├── DashboardForm.cs
│       │   ├── CustomerManagementForm.cs
│       │   ├── RoomManagementForm.cs
│       │   ├── BookingManagementForm.cs
│       │   ├── PaymentManagementForm.cs
│       │   └── ReportsForm.cs
│       │
│       ├── Models/                     # Data models
│       │   ├── User.cs
│       │   ├── Customer.cs
│       │   ├── Bed.cs
│       │   ├── Booking.cs
│       │   ├── Payment.cs
│       │   └── Models.cs
│       │
│       ├── Data/                       # Data access layer
│       │   └── DatabaseHelper.cs
│       │
│       ├── Utils/                      # Utility classes
│       │
│       ├── Program.cs                  # Application entry point
│       └── RoomBookingSystem.csproj    # Project file
│
├── database/
│   └── schema.sql                      # Database creation script
│
├── docs/
│   ├── screenshots/                    # Application screenshots
│   ├── INSTALLATION.md                 # Detailed installation guide
│   └── USER_GUIDE.md                   # User manual
│
├── .gitignore                          # Git ignore rules
├── LICENSE                             # MIT License
├── README.md                           # This file
└── RoomBookingSystem.sln               # Visual Studio solution
```

---

## 🔧 Configuration

### Application Settings

Key configurations can be modified in the following files:

**Database Connection** (`DatabaseHelper.cs`):
```csharp
private readonly string connectionString = "Server=localhost;Database=RoomBookingDB;Uid=root;Pwd=yourpassword;";
```

**Form Sizes** (Individual form files):
```csharp
this.Size = new Size(1200, 700);  // Default form size
this.StartPosition = FormStartPosition.CenterScreen;
```

**Color Scheme** (Global settings can be added to a constants file):
```csharp
Color PrimaryColor = Color.FromArgb(41, 128, 185);      // Blue
Color SuccessColor = Color.FromArgb(39, 174, 96);       // Green
Color DangerColor = Color.FromArgb(192, 57, 43);        // Red
Color WarningColor = Color.FromArgb(230, 126, 34);      // Orange
```

---

## 🧪 Testing

### Sample Data
The database includes comprehensive test data:
- 4 system users with different roles
- 30 customer profiles with complete information
- 50 rooms across 5 areas
- 20 booking records in various states
- 40 payment transactions
- 10 maintenance requests

### Manual Testing Checklist
- [ ] User authentication with different roles
- [ ] CRUD operations on all entities
- [ ] Search and filter functionality
- [ ] Report generation and export
- [ ] Date validation and calculations
- [ ] Error handling and user feedback
- [ ] Navigation and menu functionality

---

## 🚧 Roadmap

### Planned Features
- [ ] **Email Notifications** - Automated booking confirmations and payment reminders
- [ ] **SMS Integration** - Customer communication via SMS
- [ ] **Advanced Analytics** - Revenue forecasting and trend analysis
- [ ] **Mobile App** - Companion mobile application for staff
- [ ] **Online Booking Portal** - Customer-facing web interface
- [ ] **Document Management** - Upload and store customer documents
- [ ] **Inventory Management** - Track room amenities and supplies
- [ ] **Staff Scheduling** - Manage employee shifts and assignments

### Known Issues
- No issues currently reported. Please open an issue if you find any bugs.

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Contribution Guidelines
- Follow C# coding conventions and best practices
- Add XML documentation comments to public methods
- Test your changes thoroughly
- Update documentation as needed
- Keep commits atomic and meaningful

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

```
MIT License

Copyright (c) 2026 [Your Name]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
```

---

## 👨‍💻 Author

Afas Ahamad

---

## 🙏 Acknowledgments

- Visual Studio for the excellent development environment
- MySQL for reliable database management
- Windows Forms for the UI framework
- The C# community for extensive documentation and support

---

## 📊 Project Statistics

![GitHub repo size](https://img.shields.io/github/repo-size/YOUR_USERNAME/hotel-booking-management-system)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/YOUR_USERNAME/hotel-booking-management-system)
![Lines of code](https://img.shields.io/tokei/lines/github/YOUR_USERNAME/hotel-booking-management-system)
![GitHub last commit](https://img.shields.io/github/last-commit/YOUR_USERNAME/hotel-booking-management-system)
![GitHub stars](https://img.shields.io/github/stars/YOUR_USERNAME/hotel-booking-management-system?style=social)

---

## 💡 Why This Project?

This project demonstrates:
- **Full-Stack Development** - Complete application from database to UI
- **Software Architecture** - Proper layering and separation of concerns
- **Database Design** - Normalized schema with relationships
- **UI/UX Design** - Professional, user-friendly interface
- **Code Quality** - Clean, maintainable, documented code
- **Real-World Application** - Solves actual business problem
- **Best Practices** - Error handling, validation, security

Perfect for portfolio demonstration and learning enterprise application development.

---

## 📞 Support

If you have questions or need help:

- 📖 Check the [documentation](docs/)
- 🐛 Open an [issue](https://github.com/YOUR_USERNAME/hotel-booking-management-system/issues)
- 💬 Start a [discussion](https://github.com/YOUR_USERNAME/hotel-booking-management-system/discussions)

---

<div align="center">

### ⭐ Star this repository if you find it helpful!

**Built with ❤️ using C# and .NET**

[Report Bug](https://github.com/YOUR_USERNAME/hotel-booking-management-system/issues) · 
[Request Feature](https://github.com/YOUR_USERNAME/hotel-booking-management-system/issues) · 
[Documentation](docs/)

</div>

---

**Last Updated:** February 2026  
**Version:** 1.0.0  
**Status:** ✅ Production Ready
