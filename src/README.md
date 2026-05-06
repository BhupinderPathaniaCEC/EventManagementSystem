# Source Code Documentation

This folder contains the main application source code organized by Clean Architecture principles.

## Architecture Overview

We follow **Clean Architecture** (also known as Onion Architecture) with 4 layers:

```
┌─────────────────────────────────────┐
│  Web Layer (Presentation)           │  ← Controllers, Views, jQuery
│  EventManagement.Web                │
├─────────────────────────────────────┤
│  Application Layer                  │  ← Business logic, DTOs, Services
│  EventManagement.Application        │
├─────────────────────────────────────┤
│  Infrastructure Layer               │  ← Database, EF Core, external services
│  EventManagement.Infrastructure     │
├─────────────────────────────────────┤
│  Domain Layer (Core)                │  ← Entities, business rules
│  EventManagement.Domain             │
└─────────────────────────────────────┘
```

**Dependency Rule**: Inner layers (Domain) don't depend on outer layers. Dependencies point inward.

---

## Layer Details

### 1. Domain Layer (`EventManagement.Domain`)

**Purpose**: Contains core business entities. Has NO dependencies on other projects.

**Key Files**:
- `Entities/Event.cs` - Event entity with Name, Description, StartDate, EndDate, Organizer, and Ticket collection
- `Entities/Ticket.cs` - Ticket entity with TicketNo, Price, EventId foreign key

**Why this matters**: Domain is pure C# objects. Can be used anywhere without dragging in database or web dependencies.

---

### 2. Application Layer (`EventManagement.Application`)

**Purpose**: Orchestrates use cases. Defines interfaces that Infrastructure implements.

**Key Files**:
- `DTOs/EventDto.cs` - Data Transfer Object with validation attributes
- `DTOs/TicketDto.cs` - Ticket DTO for form binding
- `Interfaces/IEventService.cs` - Service contract (GetAll, GetById, Create, Update, Delete)
- `Interfaces/IApplicationDbContext.cs` - Abstraction over EF Core DbContext
- `Services/EventService.cs` - Business logic implementation

**Pattern**: 
- Controller calls `IEventService` (interface)
- `EventService` implements the actual logic
- Service maps between Entities and DTOs

**Why DTOs?**: Decouples database entities from what the UI sees. Prevents over-posting attacks.

---

### 3. Infrastructure Layer (`EventManagement.Infrastructure`)

**Purpose**: Implements data access and external concerns.

**Key Files**:
- `Data/AppDbContext.cs` - EF Core DbContext implementing `IApplicationDbContext`

**DbContext Configuration**:
- Uses Fluent API for table mapping
- Cascade delete: Deleting Event deletes its Tickets
- SQLite provider

**Why interface?**: Application layer defines `IApplicationDbContext`. Infrastructure provides the concrete implementation. This keeps Application layer testable without a real database.

---

### 4. Web Layer (`EventManagement.Web`)

**Purpose**: Handles HTTP requests, renders views, serves static files.

**Key Files**:
- `Controllers/EventsController.cs` - CRUD actions for Events
- `Controllers/HomeController.cs` - Routes to Events/Create as homepage
- `Views/Events/Create.cshtml` - Main form with jQuery ticket management
- `Views/Events/Edit.cshtml` - Edit form (reuses same jQuery logic)
- `Views/Events/Index.cshtml` - List all events
- `Views/Shared/_Layout.cshtml` - Master page template
- `Program.cs` - App startup, DI registration, database setup

**jQuery Dynamic Tickets**:
```javascript
// Add new row
$('#addTicketBtn').click(() => addRow())

// Save validates and locks fields
$('.save-btn').click(() => { ... })

// Edit unlocks fields for modification
$('.edit-btn').click(() => { ... })

// Delete removes row and reindexes
$('.delete-btn').click(() => { ... })
```

**Key feature**: After any add/delete, `reindex()` updates the `name` attributes so ASP.NET model binding works correctly (`Tickets[0].TicketNo`, `Tickets[1].TicketNo`, etc.)

---

## Dependency Injection Setup

In `Program.cs`:

```csharp
// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Interface → Implementation mapping
builder.Services.AddScoped<IApplicationDbContext, AppDbContext>();
builder.Services.AddScoped<IEventService, EventService>();
```

This means:
- When controller asks for `IEventService`, it gets `EventService`
- When service asks for `IApplicationDbContext`, it gets `AppDbContext`

---

## Data Flow Example (Create Event)

1. **User** fills form, adds tickets, clicks "Save Event"
2. **Browser** POSTs to `/Events/Create` with form data
3. **EventsController.Create** receives `EventDto` (model binding maps form to DTO)
4. **Controller** calls `_eventService.CreateEventAsync(dto)`
5. **EventService** maps DTO → Entity, adds to DbContext, saves
6. **Redirect** to Index page

---

## Adding New Features

### To add a new field to Event:

1. **Domain**: Add property to `Event.cs`
2. **Application**: Add to `EventDto.cs`
3. **Infrastructure**: Run `dotnet ef migrations add AddNewField`
4. **Web**: Update `Create.cshtml` and `Edit.cshtml` views

### To add a new entity:

1. Create entity in `Domain/Entities/`
2. Add `DbSet<>` to `IApplicationDbContext`
3. Configure in `AppDbContext.OnModelCreating()`
4. Create DTO in `Application/DTOs/`
5. Create service interface in `Application/Interfaces/`
6. Implement service in `Application/Services/`
7. Add controller in `Web/Controllers/`
8. Create views in `Web/Views/`

---

## Testing

### Manual Testing Checklist

- [ ] Create event with multiple tickets
- [ ] Add ticket row, fill data, click Save (locks fields)
- [ ] Click Edit on saved ticket (unlocks fields)
- [ ] Delete middle ticket (verify IDs renumber)
- [ ] Save event, verify tickets appear in Index
- [ ] Edit event, modify tickets, verify changes persist
- [ ] Delete event, verify cascade deletes tickets

---

## Common Patterns

### Model Binding Array Data

ASP.NET MVC binds indexed form fields to lists:
```html
<input name="Tickets[0].TicketNo" value="TKT001" />
<input name="Tickets[0].Price" value="25.00" />
<input name="Tickets[1].TicketNo" value="TKT002" />
<input name="Tickets[1].Price" value="50.00" />
```

Binds to:
```csharp
public List<TicketDto> Tickets { get; set; }
```

### Validation

DTOs use Data Annotations:
```csharp
[Required(ErrorMessage = "Event name is required")]
public string Name { get; set; }
```

Controller checks:
```csharp
if (!ModelState.IsValid)
    return View(dto);
```

---

## Questions?

- **Why Clean Architecture?** Testability, maintainability, clear boundaries
- **Why SQLite?** Zero setup, portable, perfect for demos/tests
- **Why jQuery not React/Angular?** Requirements specified Razor Views only, simpler for this use case
- **Why separate DTOs?** Security (don't expose entities), validation, flexibility
