using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using MultiTenantDemo;
using MultiTenantDemo.DbContexts;
using MultiTenantDemo.Implementation;
using MultiTenantDemo.Interfaces;
using MultiTenantDemo.Models;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMultitenancy();


builder.Services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
{

    // Get the tenant context
    var accessor = serviceProvider.GetRequiredService<IMultiTenantContextAccessor>();
    var tenant = accessor.MultiTenantContext?.TenantInfo as AppTenantInfo;

    // Use tenant's connection string, fallback to default
    var connectionString = tenant?.ConnectionString ?? "Host=localhost;Database=multitenant_default;Username=postgres;Password=1234";

    options.UseNpgsql(connectionString); // or UseSqlServer if SQL Server

    // Log what connection string is being used
    var logger = serviceProvider.GetService<ILogger<ApplicationDbContext>>();
    logger?.LogInformation($"[DbContext Registration] Using connection for tenant: {tenant?.Name ?? "DEFAULT"}");
    //logger?.LogInformation($"[DbContext Registration] Connection string: {connectionString?.Substring(0, 50)}...");

    //  m.UseNpgsql("Host=localhost;Database=multitenant_default;Username=postgres;Password=1234");
})
    .AddTransient<IDatabaseInitializer, DatabaseInitializer>()
    .AddTransient<ApplicationDbInitializer>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

using var scope = app.Services.CreateScope();

await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
    .InitializeDatabasesAsync();

// Configure the HTTP request pipeline.                 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMultiTenant();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
