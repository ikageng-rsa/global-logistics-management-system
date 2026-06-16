# GLMS - Global Logistics Management System
### TechMove Logistics

---

## Overview

The Global Logistics Management System (GLMS) is an enterprise-grade application built for TechMove Logistics. It has evolved from a monolithic MVC prototype into a fully containerised, service-oriented architecture consisting of a **RESTful Web API** and a **separate MVC frontend**, orchestrated via Docker Compose.

---

## Architecture

```
GLMS.sln
├── GLMS.Api/ — ASP.NET Core Web API (data, business logic, JWT auth)
├── GLMS.Web/ — ASP.NET Core MVC (presentation, consumes API via HttpClient)
├── GLMS.Tests/ — xUnit unit tests (17 tests)
└── GLMS.Api.Tests/ — xUnit integration tests (12 tests)
```

### System Flow
```
Browser → GLMS.Web (port 8081)
              ↓ HttpClient + JWT Bearer
         GLMS.Api (port 8080)
              ↓ Entity Framework Core
         SQL Server (port 1433)
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | ASP.NET Core MVC + Tabler UI |
| Backend | ASP.NET Core 8 Web API |
| Database | SQL Server 2022 |
| ORM | Entity Framework Core 8 |
| Auth | ASP.NET Core Identity + JWT Bearer |
| Testing | xUnit + Moq + WebApplicationFactory |
| Containerisation | Docker + Docker Compose |
| External API | open.er-api.com (Exchange Rate) |

---

## Design Patterns Implemented

| Pattern | Location | Purpose |
|---|---|---|
| **Factory Method** | `GLMS.Api/Factories/` | Creates contracts based on service level rules |
| **Observer** | `GLMS.Api/Observers/` | Auto-rejects pending requests when contract expires |
| **Repository** | `GLMS.Api/Repositories/` | Abstracts all data access from controllers |

---

## Prerequisites

### Running with Docker (Recommended)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### Running Locally
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb)
- [Visual Studio 2022](https://visualstudio.microsoft.com/)

---

## Running with Docker

**Step 1** - Clone the repository:
```bash
git clone https://github.com/<your-username>/GLMS.git
cd GLMS
```

**Step 2** — Start all containers:
```bash
docker-compose up --build
```

**Step 3** - Access the applications:

| Service | URL |
|---|---|
| MVC Frontend | http://localhost:8081 |
| API Swagger UI | http://localhost:8080/swagger |
| SQL Server | localhost:1433 |

> The database is created and seeded automatically on first run. No manual migration steps required.

**Step 4** - Stop containers:
```bash
docker-compose down
```

> To remove all data including the database volume:
> ```bash
> docker-compose down -v
> ```

---

## Running Locally (Without Docker)

**Step 1** - Configure connection string in `GLMS.Api/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=GLMSDB;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**Step 2** - Set `GLMS.Api` base URL in `GLMS.Web/appsettings.json`:
```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7100"
  }
}
```
> Replace `7100` with the actual port from `GLMS.Api/Properties/launchSettings.json`.

**Step 3** - Apply migrations (Package Manager Console, Default Project: `GLMS.Api`):
```bash
Update-Database
```

**Step 4** - Set multiple startup projects in Visual Studio:
```
Right-click Solution → Set Startup Projects → Multiple startup projects
Set GLMS.Api → Start
Set GLMS.Web → Start
```

**Step 5** - Press **F5**.

---

## Default Login Credentials

| Role | Email | Password |
|---|---|---|
| **Admin** | admin@techmore.co.za | Admin@123 |
| **User** | user@techmore.co.za | User@123 |

### Role Permissions

| Feature | Admin | User |
|---|---|---|
| View all data | x | x |
| Create & Edit | x | x |
| Delete | x |  |
| Change Contract Status | x |  |
| User Management | x |  |

---

## API Endpoints

### Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Returns JWT token |

### Clients
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/clients` | Any | Get all clients |
| GET | `/api/clients/{id}` | Any | Get client by ID |
| POST | `/api/clients` | Any | Create client |
| PUT | `/api/clients/{id}` | Any | Update client |
| DELETE | `/api/clients/{id}` | Admin | Delete client |

### Contracts
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/contracts` | Any | Get all (filterable by status/date) |
| GET | `/api/contracts/{id}` | Any | Get contract by ID |
| POST | `/api/contracts` | Any | Create via Factory pattern |
| PATCH | `/api/contracts/{id}/status` | Admin | Update status via Observer pattern |
| DELETE | `/api/contracts/{id}` | Admin | Delete contract |
| POST | `/api/contracts/{id}/agreement` | Any | Upload PDF agreement |

### Service Requests
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/servicerequests` | Any | Get all service requests |
| GET | `/api/servicerequests/{id}` | Any | Get by ID |
| POST | `/api/servicerequests` | Any | Create with USD to ZAR conversion |
| PUT | `/api/servicerequests/{id}` | Any | Update |
| DELETE | `/api/servicerequests/{id}` | Admin | Delete |

### Users (Admin only)
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/users` | Admin | List all users |
| GET | `/api/users/roles` | Admin | List available roles |
| POST | `/api/users` | Admin | Create user with role |
| PUT | `/api/users/{id}` | Admin | Update user/role/password |
| DELETE | `/api/users/{id}` | Admin | Delete user |

---

## Running Tests

### Unit Tests (GLMS.Tests)
```bash
cd GLMS.Tests
dotnet test
```

### Integration Tests (GLMS.Api.Tests)
```bash
cd GLMS.Api.Tests
dotnet test
```

### All Tests in Visual Studio
```
Test -> Test Explorer -> Run All
```

Expected result:
```
Passed! — Failed: 0, Passed: 29, Skipped: 0, Total: 29
```

### Test Coverage

| Project | Tests | Coverage |
|---|---|---|
| `GLMS.Tests` | 17 | Currency conversion, PDF validation, Factory rules |
| `GLMS.Api.Tests` | 12 | Auth, Clients, Contracts - JWT, roles, CRUD |

---

## Project Structure

```
GLMS/
├── docker-compose.yml
├── .dockerignore
├── GLMS.sln
├── GLMS.Api/
│   ├── Dockerfile
│   ├── Controllers/
│   ├── Data/ — DbContext, Migrations, SeedData
│   ├── DTOs/ — Request/Response shapes
│   ├── Factories/ — Factory Method pattern
│   │   └── Contracts/ — IContractFactory
│   ├── Models/ — Entity models
│   ├── Observers/ — Observer pattern
│   │   └── Contracts/ — IContractObserver
│   ├── Repositories/ — Repository pattern
│   │   └── Contracts/ — Repository interfaces
│   └── Services/ — Currency, File, Token services
│       └── Contracts/ — Service interfaces
├── GLMS.Web/
│   ├── Dockerfile
│   ├── Controllers/
│   ├── Handlers/ — JwtAuthHandler for HttpClient
│   ├── Models/ — Client-side DTOs
│   ├── Services/ — API client services
│   │   └── Contracts/ — Service interfaces
│   ├── ViewModels/ — Form models
│   └── Views/ — Razor views (Tabler UI)
├── GLMS.Tests/
│   └── Helpers/ — Mock HTTP handlers
└── GLMS.Api.Tests/
    └── Helpers/ — WebApplicationFactory, TestAuthHelper
```

---

## Key Features

### Contract Lifecycle
- Contracts progress through: `Draft -> Active -> Expired / On Hold`
- Status changes trigger the Observer pattern - pending service requests auto-rejected

### Service Request Validation
- Only creatable on **Active** contracts
- Cost entered in **USD**, auto-converted to **ZAR** via Exchange Rate API
- Fallback rate of R18.50 if API is unavailable

### PDF Agreement Upload
- UUID-prefixed filenames prevent collisions
- Stored under `wwwroot/uploads/agreements/`
- Download link shown on contract details

### Role-Based Access
- **Admin** - full access including user management and status changes
- **User** - read and create access, no delete or status change

### Dark / Light Theme
- Toggle in top-right header
- Preference saved in localStorage

---

### Architectural Framework
TOGAF ADM guided the architecture from planning through to containerisation:
- **Phase A** - Architecture Vision (business drivers, stakeholders)
- **Phase B** - Business Architecture (contract lifecycle, SLA processes)
- **Phase C** - Information Systems (entities, API surface)
- **Phase D** - Technology Architecture (Docker containers, networking)
