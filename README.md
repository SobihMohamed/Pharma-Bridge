# PharmaBridge 

**PharmaBridge** is a digital marketplace connecting patients with nearby pharmacies in real-time. It allows patients to upload prescriptions and receive competitive delivery bids from local pharmacies instantly.

## Architecture

The backend is built following **Clean Architecture (Onion Architecture)** and **SOLID principles**, ensuring a highly scalable, decoupled, and maintainable codebase.

### Layers Breakdown:
* **Domain:** Contains enterprise logic, core entities (`Patient`, `Pharmacy`, `Bid`), and enums. (Zero dependencies).
* **Abstraction:** Contains interfaces/contracts for Repositories, Services, and Unit of Work.
* **Service:** Contains the core application business logic, validation, and use-case coordination.
* **Infrastructure:** Handles external integrations (e.g., Image processing, File storage, Notifications).
* **Persistence:** Entity Framework Core implementation, `DbContext`, Fluent API configurations, and Migrations.
* **Presentation:** RESTful API Controllers decoupled from the main hosting environment.
* **Web:** The application entry point. Handles Dependency Injection (DI), Middlewares, JWT Authentication, and **SignalR Hubs**.
* **Shared:** Contains Data Transfer Objects (DTOs), Query Params, and common response wrappers.

## Design Patterns

The project utilizes enterprise-level design patterns to ensure clean and reusable code:

* **Generic Repository Pattern:** Abstracts data access logic and reduces CRUD boilerplate.
* **Unit of Work (UOW):** Ensures atomic database transactions and data integrity.
* **Specification Pattern:** Encapsulates complex query logic, filtering, and pagination to keep repositories clean.
* **Strategy Pattern:** Allows dynamic switching of algorithms at runtime (e.g., distance calculations or payment methods).
* **CQRS (Command Query Responsibility Segregation):** Separates read operations from write operations to optimize performance and scalability.
* **DTO Pattern:** Protects domain entities and optimizes network payloads.

## Tech Stack
* **Framework:** .NET 8 / ASP.NET Core Web API
* **Database & ORM:** SQL Server & Entity Framework Core (Code-First)
* **Real-Time Communication:** SignalR (for instant pharmacy bids and patient notifications)
* **Security:** JWT Authentication & Role-Based Authorization
* **Documentation:** Swagger UI

