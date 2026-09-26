# 📦 UserManagementAPI

A fully-featured **RESTful Web API** built with **ASP.NET Core (.NET 10)** that performs complete CRUD operations on users.  
This project is written with learning in mind — every concept is explained below.

---

## 📚 Table of Contents

1. [Project Overview](#1-project-overview)
2. [Tech Stack](#2-tech-stack)
3. [Project Structure](#3-project-structure)
4. [Core Concepts Explained](#4-core-concepts-explained)
   - 4.1 [What is a REST API?](#41-what-is-a-rest-api)
   - 4.2 [ASP.NET Core & Program.cs](#42-aspnet-core--programcs)
   - 4.3 [Model — The Database Shape](#43-model--the-database-shape)
   - 4.4 [DTO — Data Transfer Object](#44-dto--data-transfer-object)
   - 4.5 [DbContext — The Database Bridge](#45-dbcontext--the-database-bridge)
   - 4.6 [Entity Framework Core & Migrations](#46-entity-framework-core--migrations)
   - 4.7 [Repository / Service Pattern](#47-repository--service-pattern)
   - 4.8 [Interface — IUserService](#48-interface--iuserservice)
   - 4.9 [Dependency Injection](#49-dependency-injection)
   - 4.10 [AutoMapper](#410-automapper)
   - 4.11 [Controller](#411-controller)
   - 4.12 [Data Annotations & Validation](#412-data-annotations--validation)
   - 4.13 [Async / Await](#413-async--await)
   - 4.14 [Swagger / OpenAPI](#414-swagger--openapi)
5. [API Endpoints](#5-api-endpoints)
6. [Request & Response Examples](#6-request--response-examples)
7. [Getting Started](#7-getting-started)
8. [Configuration](#8-configuration)
9. [How Data Flows Through the App](#9-how-data-flows-through-the-app)
10. [What to Learn Next](#10-what-to-learn-next)

---

## 1. Project Overview

**UserManagementAPI** is a backend service that lets you:

| Action | What it does |
|--------|-------------|
| `GET /api/user` | Fetch all users from the database |
| `GET /api/user/{id}` | Fetch a single user by ID |
| `POST /api/user` | Create a new user |
| `PUT /api/user/{id}` | Update an existing user |
| `DELETE /api/user/{id}` | Delete a user |

There is **no frontend** — this is a pure backend API consumed by tools like **Swagger UI**, **Postman**, or any frontend app.

---

## 2. Tech Stack

| Technology | Version | Purpose |
|---|---|---|
| ASP.NET Core | .NET 10 | Web framework |
| Entity Framework Core | 10.0.12 | ORM — talk to the database |
| SQL Server (SQLEXPRESS) | — | Relational database |
| AutoMapper | 16.2.0 | Object-to-object mapping |
| Swashbuckle (Swagger) | 10.2.3 | API documentation & testing UI |

---

## 3. Project Structure

```
UserManagementAPI/
│
├── Controllers/
│   └── UserController.cs       ← Handles HTTP requests/responses
│
├── DTOs/
│   ├── UserDto.cs              ← What the API sends back (response)
│   ├── CreateUserDto.cs        ← What the API accepts for creation
│   └── UpdateUserDto.cs        ← What the API accepts for updates
│
├── Models/
│   └── User.cs                 ← Database entity (the table shape)
│
├── Data/
│   └── ApplicationDbContext.cs ← EF Core database bridge
│
├── Services/
│   ├── IUserService.cs         ← Contract/interface
│   └── UserService.cs          ← Business logic implementation
│
├── Mappings/
│   └── MappingProfile.cs       ← AutoMapper configuration
│
├── Migrations/
│   └── ...                     ← EF Core database migration history
│
├── appsettings.json            ← App configuration (connection string etc.)
└── Program.cs                  ← App entry point & service registration
```

---

## 4. Core Concepts Explained

### 4.1 What is a REST API?

**REST** (Representational State Transfer) is a standard way for systems to talk to each other over HTTP.

Think of it like a **waiter** in a restaurant:
- You (the client) give an order (HTTP request)
- The waiter (API) takes it to the kitchen (database)
- The waiter brings you the food (HTTP response)

**HTTP Verbs used in this project:**

| Verb | Meaning | Example |
|------|---------|---------|
| `GET` | Read data | Get all users |
| `POST` | Create new data | Add a new user |
| `PUT` | Update existing data | Edit a user |
| `DELETE` | Remove data | Delete a user |

**HTTP Status Codes used:**

| Code | Meaning |
|------|---------|
| `200 OK` | Success, returns data |
| `201 Created` | New resource was created |
| `204 No Content` | Success, nothing to return (delete) |
| `404 Not Found` | Resource doesn't exist |

---

### 4.2 ASP.NET Core & Program.cs

`Program.cs` is the **entry point** of the application. It does two things:

**1. Register services** (tell the app what tools are available):
```csharp
builder.Services.AddControllers();          // Enable API controllers
builder.Services.AddDbContext<...>();       // Register EF Core
builder.Services.AddSwaggerGen();           // Register Swagger
builder.Services.AddAutoMapper(...);        // Register AutoMapper
```

**2. Configure the HTTP pipeline** (what happens to every request):
```csharp
app.UseSwagger();           // Serve the swagger.json
app.UseSwaggerUI();         // Serve the visual Swagger page
app.UseHttpsRedirection();  // Force HTTPS
app.UseAuthorization();     // Check permissions
app.MapControllers();       // Route requests to controllers
```

> 💡 **Learn**: The pipeline order matters. Each `Use...()` is middleware that wraps around the request like layers of an onion.

---

### 4.3 Model — The Database Shape

**File:** [`Models/User.cs`](Models/User.cs)

A **Model** is a C# class that represents a **database table**. Each property = one column.

```csharp
public class User
{
    public int Id { get; set; }              // Primary Key (auto-generated)
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; } // Nullable — optional field
    public int Age { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }
    public bool IsActive { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; }  // Set automatically in code
    public required string Role { get; set; }
}
```

| C# Type | SQL Type |
|---------|----------|
| `int` | `INT` |
| `string` | `NVARCHAR` |
| `bool` | `BIT` |
| `DateTime` | `DATETIME2` |
| `string?` | `NVARCHAR NULL` |

> 💡 **Learn**: The `?` after a type (e.g., `string?`) makes it **nullable** — the value is allowed to be missing. `required` means it must be set.

---

### 4.4 DTO — Data Transfer Object

**Folder:** [`DTOs/`](DTOs/)

A **DTO** is a class used to **transfer data** between the API and the outside world. It is separate from the Model on purpose.

#### Why not just use the `User` model directly?

| Problem with using Model directly | Solution with DTOs |
|---|---|
| You expose internal fields (e.g., `CreatedAt`) that clients shouldn't set | DTOs only expose what's needed |
| No validation rules on the model | DTOs have `[Required]`, `[EmailAddress]` etc. |
| If the DB schema changes, the API contract breaks | DTOs insulate the API from DB changes |

**This project has 3 DTOs:**

| DTO | Used for | Has `Id`? | Has `CreatedAt`? |
|-----|----------|-----------|-----------------|
| `CreateUserDto` | POST requests | ❌ (DB auto-assigns) | ❌ (set in code) |
| `UpdateUserDto` | PUT requests | ❌ (comes from URL) | ❌ |
| `UserDto` | All responses | ✅ | ✅ |

---

### 4.5 DbContext — The Database Bridge

**File:** [`Data/ApplicationDbContext.cs`](Data/ApplicationDbContext.cs)

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<User> Users { get; set; }
}
```

`DbContext` is your **gateway to the database**. Think of it as a smart connector that knows how to translate C# code into SQL queries.

- `DbSet<User> Users` → represents the `Users` table
- You query it like: `_context.Users.ToListAsync()` → becomes `SELECT * FROM Users`

> 💡 **Learn**: You never write raw SQL in this project. EF Core translates LINQ (C# query syntax) into SQL automatically. This is called an **ORM** (Object-Relational Mapper).

---

### 4.6 Entity Framework Core & Migrations

**EF Core** is the ORM used to interact with SQL Server.

#### What are Migrations?

Migrations are like a **version history for your database schema**. Every time you change the `User` model, you create a migration to update the database.

```
dotnet ef migrations add InitialCreate   ← Creates the migration file
dotnet ef database update                ← Applies it to the database
```

The migration file describes:
- What tables to create (`UP` method)
- How to undo it (`DOWN` method)

**Files in `Migrations/`:**

| File | Purpose |
|------|---------|
| `20260926094824_InitialCreate.cs` | Creates the `Users` table |
| `20260926094824_InitialCreate.Designer.cs` | EF Core metadata (auto-generated) |
| `ApplicationDbContextModelSnapshot.cs` | Current state snapshot (auto-generated) |

> ⚠️ **Never delete migration files** — they are the history of your DB schema and needed by EF Core to track state.

#### `AsNoTracking()` — Performance Tip

In read-only queries, `AsNoTracking()` is used:
```csharp
_context.Users.AsNoTracking().ToListAsync()
```
This tells EF Core **not to track changes** to these objects, which is faster when you only need to read data.

---

### 4.7 Repository / Service Pattern

**Folder:** [`Services/`](Services/)

Instead of putting database logic directly inside controllers, we use a **Service** (also called Repository pattern).

```
Controller  →  calls  →  Service  →  calls  →  DbContext  →  SQL Server
```

**Why this pattern?**

| Without Service Pattern | With Service Pattern |
|---|---|
| Controller has DB code + HTTP code = messy | Each class has one job (Single Responsibility) |
| Hard to test | Easy to test — swap real DB for a fake |
| Logic duplicated if used in multiple controllers | Reusable service |

---

### 4.8 Interface — IUserService

**File:** [`Services/IUserService.cs`](Services/IUserService.cs)

```csharp
public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(int id);
    Task<UserDto> CreateUserAsync(CreateUserDto dto);
    Task<UserDto?> UpdateUserAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(int id);
}
```

An **interface** is a **contract**. It says: *"any class that claims to be a UserService MUST have these methods."*

> 💡 **Learn**: Interfaces are the backbone of **Dependency Injection**. The controller depends on `IUserService` (the contract), not `UserService` (the concrete class). This allows you to swap implementations without touching the controller.

---

### 4.9 Dependency Injection

**Dependency Injection (DI)** is how ASP.NET Core provides services to your classes automatically — you don't create them with `new`.

**Without DI (bad):**
```csharp
// You manually create everything — tightly coupled
public UserController()
{
    _userService = new UserService(new ApplicationDbContext(...), new Mapper(...));
}
```

**With DI (good):**
```csharp
// ASP.NET Core injects what you need automatically
public UserController(IUserService userService)
{
    _userService = userService;
}
```

**Registration in `Program.cs`:**
```csharp
// You would add this line to register UserService:
builder.Services.AddScoped<IUserService, UserService>();
```

| Lifetime | Means |
|----------|-------|
| `AddTransient` | New instance every time it's requested |
| `AddScoped` | One instance per HTTP request |
| `AddSingleton` | One instance for the entire app lifetime |

> 💡 **Learn**: `AddScoped` is the most common for services that use `DbContext` — because DbContext itself is scoped per request.

---

### 4.10 AutoMapper

**File:** [`Mappings/MappingProfile.cs`](Mappings/MappingProfile.cs)

**AutoMapper** automatically copies matching properties from one object to another — no manual assignment needed.

**Without AutoMapper (tedious):**
```csharp
var dto = new UserDto
{
    Id = user.Id,
    FirstName = user.FirstName,
    LastName = user.LastName,
    Email = user.Email,
    // ... 8 more lines
};
```

**With AutoMapper (clean):**
```csharp
var dto = _mapper.Map<UserDto>(user); // Done in one line!
```

**The MappingProfile defines the rules:**
```csharp
CreateMap<User, UserDto>();           // Entity → Response
CreateMap<CreateUserDto, User>();     // Request → Entity (create)
CreateMap<UpdateUserDto, User>();     // Request → Entity (update)
```

**Update mapping is special:**
```csharp
// Maps DTO properties onto an EXISTING tracked entity
_mapper.Map(dto, user);
```
This updates the existing `user` object in-place, so EF Core can detect the changes and save them.

---

### 4.11 Controller

**File:** [`Controllers/UserController.cs`](Controllers/UserController.cs)

The controller is the **entry point for HTTP requests**. Each method = one API endpoint.

```csharp
[Route("api/[controller]")]   // → api/user
[ApiController]               // Enables automatic model validation
public class UserController : ControllerBase
```

| Attribute | What it does |
|-----------|-------------|
| `[Route("api/[controller]")]` | Sets the base URL. `[controller]` is replaced by `User` |
| `[ApiController]` | Auto-validates request body, returns 400 if invalid |
| `[HttpGet]` | Maps to `GET /api/user` |
| `[HttpGet("{id}")]` | Maps to `GET /api/user/5` |
| `[HttpPost]` | Maps to `POST /api/user` |
| `[HttpPut("{id}")]` | Maps to `PUT /api/user/5` |
| `[HttpDelete("{id}")]` | Maps to `DELETE /api/user/5` |

**`CreatedAtAction` — the right way to return 201:**
```csharp
return CreatedAtAction(
    nameof(GetUserById),   // Name of the GET endpoint
    new { id = user.Id },  // Route values for the Location header
    user                   // The response body
);
```
This returns `201 Created` with a `Location: /api/user/42` header — proper REST behaviour.

---

### 4.12 Data Annotations & Validation

**Used in:** [`DTOs/CreateUserDto.cs`](DTOs/CreateUserDto.cs) and [`DTOs/UpdateUserDto.cs`](DTOs/UpdateUserDto.cs)

Data Annotations are **attributes** that define validation rules on properties.

```csharp
[Required]                          // Field cannot be empty
[StringLength(50, MinimumLength=2)] // Between 2–50 characters
[EmailAddress]                      // Must be a valid email format
[Phone]                             // Must be a valid phone format
[Range(18, 100)]                    // Number must be between 18 and 100
```

Because `[ApiController]` is on the controller, ASP.NET Core **automatically checks these rules** before your method runs. If validation fails, it returns:
```json
HTTP 400 Bad Request
{
  "errors": {
    "Email": ["The Email field is not a valid e-mail address."]
  }
}
```
No manual validation code needed.

---

### 4.13 Async / Await

Every service method and controller action is **asynchronous**:

```csharp
public async Task<List<UserDto>> GetAllUsersAsync()
{
    var users = await _context.Users.ToListAsync();
    return _mapper.Map<List<UserDto>>(users);
}
```

| Keyword | Meaning |
|---------|---------|
| `async` | This method runs asynchronously |
| `await` | Wait here (without blocking the thread) until the operation finishes |
| `Task<T>` | The async version of returning `T` |

**Why async?**

When querying the database, the CPU just waits for the disk/network. With `async/await`, the thread is **freed** during that wait to handle other requests. This makes the API handle more users simultaneously.

> 💡 **Learn**: `async` methods return `Task` or `Task<T>`. Always use `await` when calling them. Never use `.Result` or `.Wait()` — it causes deadlocks.

---

### 4.14 Swagger / OpenAPI

**Swagger** auto-generates an **interactive API documentation page** from your code.

Visit: `https://localhost:{port}/swagger`

You can:
- See all endpoints
- Send test requests directly from the browser
- View request/response schemas

Configured in `Program.cs`:
```csharp
builder.Services.AddSwaggerGen();   // Generate the OpenAPI spec
// ...
app.UseSwagger();                   // Serve swagger.json
app.UseSwaggerUI();                 // Serve the visual UI
```

Only enabled in Development mode to avoid exposing internals in production.

---

## 5. API Endpoints

| Method | URL | Description | Body | Returns |
|--------|-----|-------------|------|---------|
| `GET` | `/api/user` | Get all users | None | `200` + `List<UserDto>` |
| `GET` | `/api/user/{id}` | Get user by ID | None | `200` + `UserDto` or `404` |
| `POST` | `/api/user` | Create new user | `CreateUserDto` | `201` + `UserDto` |
| `PUT` | `/api/user/{id}` | Update user | `UpdateUserDto` | `200` + `UserDto` or `404` |
| `DELETE` | `/api/user/{id}` | Delete user | None | `204` or `404` |

---

## 6. Request & Response Examples

### Create a User — `POST /api/user`

**Request Body:**
```json
{
  "firstName": "Ahmed",
  "lastName": "Khan",
  "email": "ahmed@example.com",
  "phoneNumber": "+923001234567",
  "age": 25,
  "city": "Lahore",
  "country": "Pakistan",
  "isActive": true,
  "dateOfBirth": "2001-01-15",
  "role": "Admin"
}
```

**Response — `201 Created`:**
```json
{
  "id": 1,
  "firstName": "Ahmed",
  "lastName": "Khan",
  "email": "ahmed@example.com",
  "phoneNumber": "+923001234567",
  "age": 25,
  "city": "Lahore",
  "country": "Pakistan",
  "isActive": true,
  "dateOfBirth": "2001-01-15T00:00:00",
  "createdAt": "2026-09-26T16:14:53Z",
  "role": "Admin"
}
```

### Validation Error Example — `POST /api/user` with bad email

**Request Body:**
```json
{ "email": "not-an-email", ... }
```

**Response — `400 Bad Request`:**
```json
{
  "errors": {
    "Email": ["The Email field is not a valid e-mail address."]
  }
}
```

---

## 7. Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server / SQL Server Express
- Any IDE: Visual Studio, VS Code, or Rider

### Steps

```bash
# 1. Clone the repository
git clone <your-repo-url>
cd UserManagementAPI

# 2. Update the connection string in appsettings.json
#    (change Server name to your SQL Server instance)

# 3. Apply database migrations
dotnet ef database update

# 4. Run the API
dotnet run

# 5. Open Swagger UI
# Navigate to: https://localhost:{port}/swagger
```

---

## 8. Configuration

**File:** `appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=UserManagementDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Change `YOUR_SERVER` to your SQL Server instance name (e.g., `localhost\SQLEXPRESS`).

> ⚠️ **Never commit real credentials** to Git. Use `appsettings.Development.json` (gitignored) for local secrets.

---

## 9. How Data Flows Through the App

```
HTTP Request
     │
     ▼
┌────────────────────┐
│   UserController   │  ← Receives request, calls service
│  (Controllers/)    │
└────────┬───────────┘
         │  calls IUserService
         ▼
┌────────────────────┐
│    UserService     │  ← Business logic, uses AutoMapper
│   (Services/)      │
└────────┬───────────┘
         │  queries via DbContext
         ▼
┌────────────────────┐
│ ApplicationDbContext│  ← Translates C# → SQL
│     (Data/)        │
└────────┬───────────┘
         │
         ▼
┌────────────────────┐
│    SQL Server      │  ← Stores the data
│  (UserManagementDB)│
└────────────────────┘
         │  returns User entity
         ▼
    AutoMapper maps User → UserDto
         │
         ▼
HTTP Response (JSON)
```

---

## 10. What to Learn Next

Now that you understand this project, here's what to add/learn next:

| Topic | Why |
|-------|-----|
| **Authentication & JWT** | Protect endpoints — only logged-in users can call the API |
| **Pagination** | Return 10 users at a time instead of all at once |
| **Global Error Handling** | Return consistent error responses instead of crashes |
| **FluentValidation** | More powerful alternative to Data Annotations |
| **Repository Pattern** | Another layer between Service and DbContext for testability |
| **Unit Testing (xUnit)** | Write tests to verify your service logic automatically |
| **HTTPS & CORS** | Security — control which frontends can call your API |
| **Logging (Serilog)** | Record what happens in your app to a file or cloud |
| **Docker** | Package and run your API in any environment consistently |

---

> Built with ❤️ using ASP.NET Core .NET 10

