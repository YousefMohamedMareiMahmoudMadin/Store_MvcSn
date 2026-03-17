Store MVC - Integrated E-Commerce System

Overview
An integrated e-commerce project developed using ASP.NET Core MVC to provide a complete shopping experience with an advanced permissions system and user management.

Key Features

Authentication & Registration System
- New user registration with email confirmation
- Secure login with "Remember Me" feature
- Password recovery via email
- Role-based system (Admin - Client) for permission control

E-Store
- Product display with search and filtering capabilities
- Integrated shopping cart
- Order management and order status tracking
- Purchase history for users

Admin Dashboard
- Product management (Add - Edit - Delete)
- User and permission management
- Order tracking and status updates
- Sales statistics

 Technologies Used

 Backend
- **ASP.NET Core MVC** - Web application development
- **Entity Framework Core** - Database interaction
- **SQL Server** - Data storage
- **ASP.NET Core Identity** - Authentication and authorization system
- **LINQ** - Data querying

### Frontend
- **Bootstrap 5** - Responsive interface design
- **HTML5 & CSS3** - Page structure and styling
- **JavaScript & jQuery** - User interaction
- **Razor Pages** - Code integration with interfaces

### Integrated Services
- **SMTP** - Automated email sending
- **Dependency Injection** - Dependency management
- **Repository Pattern** - Data access organization
- **DTOs & AutoMapper** - Efficient data transfer

Project Structure
Store_Mvc/
├── Controllers/ # Handling user requests
├── Views/ # User interfaces
├── Models/ # Data models
├── Services/ # Email services & Database and context
├── Migrations/ # Database updates
├── wwwroot/ # Static files
└── appsettings.json # Project configuration


