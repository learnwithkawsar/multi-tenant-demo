# 🏢 Multi-Tenant ASP.NET Core API

A robust, production-ready multi-tenant API built with ASP.NET Core 8.0 and Finbuckle.MultiTenant, featuring database-per-tenant architecture with PostgreSQL.

## 🌟 Features

- **🔐 Multi-Tenant Architecture**: Isolated tenant data with database-per-tenant strategy
- **🚀 ASP.NET Core 8.0**: Latest framework with high performance
- **🗄️ PostgreSQL**: Robust relational database with tenant isolation
- **🔄 Entity Framework Core 7.0**: Modern ORM with multi-tenant support
- **📊 Finbuckle.MultiTenant**: Enterprise-grade multi-tenancy library
- **🔍 Swagger/OpenAPI**: Interactive API documentation
- **⚡ Auto-Migration**: Automatic database creation and migration
- **🎯 Route & Header Strategy**: Flexible tenant resolution

## 🏗️ Architecture

```
MultiTenantDemo/
├── Controllers/          # API Controllers
├── Models/              # Domain Models & DTOs
├── DbContexts/          # Entity Framework Contexts
├── Implementation/      # Service Implementations
├── Interfaces/          # Service Contracts
├── Constants/           # Application Constants
├── Migrations/          # EF Core Migrations
└── Extensions.cs        # Extension Methods
```

## 🚀 Quick Start

### Prerequisites

- **.NET 8.0 SDK**
- **PostgreSQL 12+**
- **Visual Studio 2022** or **VS Code**

### Installation

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd MultiTenantPOCMain
   ```

2. **Configure Database**

   ```json
   // appsettings.json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=multitenant_default;Username=postgres;Password=1234"
     }
   }
   ```

3. **Install Dependencies**

   ```bash
   dotnet restore
   ```

4. **Run Migrations**

   ```bash
   dotnet ef database update
   ```

5. **Start the Application**
   ```bash
   dotnet run
   ```

## 📡 API Endpoints

### Base URL

```
http://localhost:5020
```

### Tenant-Specific Routes

| Method   | Endpoint                      | Description                   |
| -------- | ----------------------------- | ----------------------------- |
| `GET`    | `/{tenant}/api/Products`      | Get all products for tenant   |
| `GET`    | `/{tenant}/api/Products/{id}` | Get product by ID for tenant  |
| `POST`   | `/{tenant}/api/Products`      | Create new product for tenant |
| `PUT`    | `/{tenant}/api/Products/{id}` | Update product for tenant     |
| `DELETE` | `/{tenant}/api/Products/{id}` | Delete product for tenant     |

### Header Strategy

| Method | Endpoint        | Header              | Description               |
| ------ | --------------- | ------------------- | ------------------------- |
| `GET`  | `/api/Products` | `X-Tenant: tenant1` | Get products using header |

## 🧪 Testing

### Using the HTTP File

The project includes `products.http` with comprehensive test cases:

```http
### Get all products for tenant1
GET http://localhost:5020/tenant1/api/Products
Accept: application/json

### Create a new product for tenant1
POST http://localhost:5020/tenant1/api/Products
Content-Type: application/json

{
  "name": "Product from Tenant 1",
  "price": 99.99
}
```

### Using Swagger UI

1. Navigate to `http://localhost:5020/swagger`
2. Select your tenant from the dropdown
3. Test all endpoints interactively

## 🏗️ Multi-Tenant Configuration

### Tenant Resolution Strategies

The application supports multiple tenant resolution strategies:

1. **Route Strategy**: `/tenant1/api/Products`
2. **Header Strategy**: `X-Tenant: tenant1`

### Tenant Configuration

```csharp
// Program.cs
builder.Services.AddMultitenancy()
    .WithRouteStrategy("tenant")
    .WithHeaderStrategy("X-Tenant");
```

### Database Per Tenant

Each tenant gets its own database:

- `multitenant_tenant1`
- `multitenant_tenant2`
- `multitenant_default` (fallback)

## 📦 Dependencies

| Package                                     | Version | Purpose                  |
| ------------------------------------------- | ------- | ------------------------ |
| `Finbuckle.MultiTenant`                     | 6.10.0  | Multi-tenancy core       |
| `Finbuckle.MultiTenant.AspNetCore`          | 6.10.0  | ASP.NET Core integration |
| `Finbuckle.MultiTenant.EntityFrameworkCore` | 6.10.0  | EF Core integration      |
| `Microsoft.EntityFrameworkCore`             | 7.0.0   | ORM framework            |
| `Npgsql.EntityFrameworkCore.PostgreSQL`     | 7.0.0   | PostgreSQL provider      |
| `Swashbuckle.AspNetCore`                    | 6.6.2   | API documentation        |
| `Mapster`                                   | 7.4.0   | Object mapping           |

## 🔧 Configuration

### Environment Variables

```bash
# Database
DATABASE_HOST=localhost
DATABASE_NAME=multitenant_default
DATABASE_USER=postgres
DATABASE_PASSWORD=1234

# Application
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=http://localhost:5020
```

### Tenant Store Configuration

```csharp
// In-memory tenant store (for demo)
options.Tenants.Add(new TenantInfo
{
    Id = "tenant1",
    Identifier = "tenant1",
    Name = "Tenant 1",
    ConnectionString = "Host=localhost;Database=multitenant_tenant1;Username=postgres;Password=1234"
});
```

## 🗄️ Database Schema

### Products Table

```sql
CREATE TABLE "Products" (
    "Id" SERIAL PRIMARY KEY,
    "Name" VARCHAR(1024) NOT NULL,
    "Price" DECIMAL(18,2) NOT NULL,
    "TenantId" VARCHAR(255)
);
```

### Multi-Tenant Filtering

Entity Framework automatically filters data by tenant:

```csharp
// Automatically filters by current tenant
var products = await _context.Products.ToListAsync();
```

## 🔒 Security Considerations

- **Tenant Isolation**: Complete data separation between tenants
- **Route Protection**: Tenant-specific route validation
- **Database Security**: Separate databases per tenant
- **Connection String Isolation**: Unique connection strings per tenant

## 🚀 Deployment

### Docker Support

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MultiTenantDemo.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MultiTenantDemo.dll"]
```

### Production Considerations

1. **Use external tenant store** (database, configuration service)
2. **Implement proper authentication/authorization**
3. **Add logging and monitoring**
4. **Configure connection pooling**
5. **Set up backup strategies per tenant**

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

- **Issues**: [GitHub Issues](https://github.com/your-repo/issues)
- **Documentation**: [Wiki](https://github.com/your-repo/wiki)
- **Email**: support@yourcompany.com

## 🔄 Version History

- **v1.0.0** - Initial release with basic multi-tenant functionality
- **v1.1.0** - Added header strategy support
- **v1.2.0** - Enhanced database initialization

---

**Built with ❤️ using ASP.NET Core and Finbuckle.MultiTenant**
