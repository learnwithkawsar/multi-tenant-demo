using Microsoft.EntityFrameworkCore;
using MultiTenantDemo;
using MultiTenantDemo.DbContexts;
using MultiTenantDemo.Implementation;
using MultiTenantDemo.Interfaces;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMultitenancy();
builder.Services.AddDbContext<ApplicationDbContext>((p, m) =>
{

    m.UseNpgsql("Host=localhost;Database=multitenant_default;Username=postgres;Password=1234");
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
