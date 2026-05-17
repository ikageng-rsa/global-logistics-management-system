# GLMS - Global Logistics Management System
### TechMove Logistics

---

## Overview

The Global Logistics Management System (GLMS) is an enterprise-grade web application built with **ASP.NET Core MVC (.NET 8)** for TechMove Logistics. It centralises contract management, automates service request validation, handles multi-currency financial reporting, and implements enterprise design patterns.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | ASP.NET Core MVC + Tabler UI |
| Backend | ASP.NET Core 8 |
| Database | SQL Server (LocalDB for development) |
| ORM | Entity Framework Core 8 |
| Auth | ASP.NET Core Identity |
| Testing | xUnit + Moq |
| External API | open.er-api.com (Exchange Rate) |

---

## Design Patterns Implemented

| Pattern | Purpose |
|---|---|
| **Factory Method** | Creates contracts based on service level (Standard, Premium, Express) |
| **Observer** | Automatically rejects pending service requests when a contract expires or goes on hold |
| **Repository** | Abstracts all data access, controllers never touch DbContext directly |

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb) (included with Visual Studio)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or later

---

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/ikageng-rsa/global-logistics-management-system.git
cd global-logistics-management-system
```

### 2. Configure the Connection String

Open `GLMS.Web/appsettings.json` and verify the connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GLMSDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

> If using a full SQL Server instance, update the server name accordingly.

### 3. Apply Migrations

In **Visual Studio Package Manager Console** (with `GLMS.Web` as the default project):

```bash
Update-Database
```

Or via CLI:

```bash
cd GLMS.Web
dotnet ef database update
```

### 4. Run the Application

```bash
cd GLMS.Web
dotnet run
```

Or press **F5** in Visual Studio.

> On first run, the application automatically seeds the database with roles and default users.

---

## Default Login Credentials

| Role | Email | Password |
|---|---|---|
| **Admin** | admin@techmore.co.za | Admin@123 |
| **User** | user@techmore.co.za | User@123 |

### Role Permissions

| Feature | Admin | User |
|---|---|---|
| View Clients, Contracts, Service Requests | x | x |
| Create & Edit | x | x |
| Delete | x |  |
| Change Contract Status | x | x |
| User Management | x |  |

---

## Running the Tests

In **Visual Studio**:
```
Test -> Test Explorer -> Run All
```

Via CLI:
```bash
cd GLMS.Tests
dotnet test
```

Expected result:
```
Passed! — Failed: 0, Passed: 17, Skipped: 0, Total: 17
```

### Test Coverage

| Test Class | Tests | What's Covered |
|---|---|---|
| `CurrencyServiceTests` | 5 | USD -> ZAR conversion, zero/negative amounts, API fallback |
| `FileServiceTests` | 8 | PDF validation, file size, null handling, UUID naming |
| `ContractFactoryTests` | 4 | SLA rules per service level |

---

## Key Features

### Contract Lifecycle
- Contracts progress through: **Draft -> Active -> Expired / On Hold**
- Status changes trigger the Observer pattern automatically

### Service Request Validation
- Service requests can only be created on **Active** contracts
- When a contract expires or goes on hold, all **Pending** requests are automatically **Rejected**

### Currency Conversion
- Service request costs are entered in **USD**
- ZAR equivalent is automatically calculated via the [open.er-api.com](https://open.er-api.com) API
- Falls back to R18.50 if the API is unavailable

### PDF Agreement Upload
- Contracts support PDF agreement uploads
- Files are UUID-prefixed to prevent collisions
- Stored under `wwwroot/uploads/agreements/`

---

### Architectural Framework
TOGAF ADM was used for planning and mapping business processes, information systems, and technology architecture before development began.

