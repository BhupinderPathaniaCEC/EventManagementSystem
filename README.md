# Event Management System

An ASP.NET Core MVC application built with Clean Architecture for managing events and their associated tickets.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (or .NET 8+)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# extension
- SQLite (included via NuGet, no separate install needed)

## Quick Start

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd EventManagementSystem
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run the application**
   ```bash
   dotnet run --project src/EventManagement.Web
   ```

4. **Open in browser**
   ```
   http://localhost:5000
   ```

The application will automatically create the SQLite database (`events.db`) on first run.

## Project Structure

```
EventManagementSystem/
├── src/
│   ├── EventManagement.Domain/          # Entities (Event, Ticket)
│   ├── EventManagement.Application/     # DTOs, Services, Interfaces
│   ├── EventManagement.Infrastructure/  # DbContext, EF Core
│   └── EventManagement.Web/             # Controllers, Views, wwwroot
├── README.md                            # This file
└── EventManagementSystem.slnx           # Solution file
```

See [src/README.md](src/README.md) for detailed architecture documentation.

## Configuration

### Database Connection

The app uses SQLite by default. Connection string is in `appsettings.json`:

```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=events.db"
}
```

To use a different database path, modify this connection string.

### Environment Variables

You can override settings using environment variables:

```bash
set ASPNETCORE_ENVIRONMENT=Production
dotnet run
```

## Features

- **Clean Architecture**: Separation of Domain, Application, Infrastructure, and Web layers
- **Dynamic Ticket Management**: Add/Edit/Delete tickets using jQuery without page reload
- **Entity Framework Core**: Code-first approach with SQLite
- **Model Validation**: Data annotations on DTOs
- **Dependency Injection**: Service layer pattern with interfaces

## Database Migrations (Optional)

If you need to create migrations manually:

```bash
cd src/EventManagement.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../EventManagement.Web
dotnet ef database update --startup-project ../EventManagement.Web
```

Note: The app uses `Database.EnsureCreated()` in development mode, so migrations are optional.

## Troubleshooting

### Build Errors

If you see `DbSet<> could not be found`:
```bash
dotnet restore
```

### Port Already in Use

Change the port in `launchSettings.json` or use:
```bash
dotnet run --urls "http://localhost:5001"
```

## License

MIT License
