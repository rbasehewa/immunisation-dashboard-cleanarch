# Immunisation Dashboard - Clean Architecture

A production-grade immunisation tracking system demonstrating Clean Architecture principles, built with ASP.NET Core 9, Entity Framework Core, and SQL Server.

## 🎯 Overview

Healthcare dashboard for tracking vaccination status across user populations. Features real-time statistics, user management, and compliance tracking with performance-optimized database operations.

## ✨ Key Features

- **Clean/Onion Architecture** - Domain-driven design with proper layer separation
- **RESTful API** - Three endpoints for statistics, user summaries, and filtered queries
- **Performance Optimized** - Stored procedures and covering indexes for scalability
- **Domain Logic** - Business rules (IsOverdue, IsFullyCompliant) encapsulated in entities
- **Comprehensive Testing** - 17 unit tests with Moq for repository mocking
- **API Documentation** - Interactive Swagger UI for testing and exploration

## 🏗️ Architecture Layers
```
WebApi (Presentation)
    ↓
Application (Use Cases)
    ↓
Infrastructure (Data Access)
    ↓
Domain (Business Rules)
```

## 🛠️ Tech Stack

**Backend:**
- .NET 9 / C# 13
- ASP.NET Core Web API
- Entity Framework Core 9
- SQL Server 2022
- xUnit + Moq (Testing)
- Swagger/OpenAPI

**Database:**
- Code-First Migrations
- Stored Procedures
- Performance Indexes (B-Tree)
- Seed Data

**Frontend:** *(Planned)*
- React / TypeScript
- Tailwind CSS

## 📊 Database Schema

**Users Table:**
- Id, FirstName, LastName, Email (unique)
- Status (enum: NonImmunised, PartiallyImmunised, FullyImmunised, Overdue)
- LastImmunisationDate, CreatedAt, UpdatedAt

## 🚀 Getting Started

**Prerequisites:**
- .NET 9 SDK
- SQL Server 2022 (or LocalDB/Express)
- VS Code or Visual Studio 2022

**Setup:**
```bash
# Clone repository
git clone https://github.com/yourusername/immunisation-dashboard-cleanarch.git
cd immunisation-dashboard-cleanarch/backend

# Restore packages
dotnet restore

# Update connection string in appsettings.json
# Run migrations
cd src/Infrastructure
dotnet ef database update --startup-project ../WebApi

# Run API
cd ../WebApi
dotnet run

# Open Swagger
# Navigate to https://localhost:5065
```

**Run Tests:**
```bash
cd backend
dotnet test
```

## 📚 Project Structure
```
backend/
├── src/
│   ├── Domain/              # Entities, Enums, Business Rules
│   ├── Application/         # DTOs, Services, Interfaces
│   ├── Infrastructure/      # DbContext, Repositories, Migrations
│   └── WebApi/              # Controllers, Program.cs, Swagger
├── tests/
│   └── Tests/               # Unit Tests (xUnit + Moq)
└── Database/
    ├── StoredProcedures/    # SQL stored procedures
    └── Indexes/             # Performance indexes
```

## 🎓 Concepts Demonstrated

- **SOLID Principles** - Single Responsibility, Dependency Inversion, etc.
- **Repository Pattern** - Abstract data access behind interfaces
- **Dependency Injection** - Constructor injection throughout
- **DTO Pattern** - Separate API contracts from entities
- **Async/Await** - Non-blocking I/O operations
- **Unit Testing** - Isolated tests with mocking
- **Database Optimization** - Stored procedures, indexes, query tuning

## 📈 API Endpoints
```
GET /api/dashboard/statistics
    → Dashboard stats with completion rate

GET /api/dashboard/users
    → All users with calculated compliance fields

GET /api/dashboard/users/status/{status}
    → Filtered users by immunisation status
```

## 🧪 Testing

17 unit tests covering:
- Domain business rules (IsOverdue, IsFullyCompliant)
- Service layer calculations (completion rate)
- DTO mapping and validation
- Edge cases (zero users, boundary conditions)

## 🔜 Roadmap

- [ ] JWT Authentication
- [ ] React Frontend
- [ ] User CRUD operations
- [ ] Email notifications for overdue users
- [ ] Export to CSV/PDF
- [ ] Integration tests
- [ ] Docker containerization
- [ ] CI/CD pipeline

## 👨‍💻 Author

**Ryan Maddumahewa**
- Portfolio: [ryanmaddumahewa.dev](https://ryanmaddumahewa.dev)
- LinkedIn: [LinkedIn](https://www.linkedin.com/in/ryanmaddumahewa/)
- Location: Perth, Western Australia

## 📝 License

MIT License - feel free to use this project for learning or portfolio purposes.

---

Built with Clean Architecture principles for maintainability, testability, and scalability.
