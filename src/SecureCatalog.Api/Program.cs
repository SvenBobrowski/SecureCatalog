using SecureCatalog.Api.Data;
using SecureCatalog.Api.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// db connection
var connectionString = builder.Configuration.GetConnectionString("CatalogDatabase") ?? throw new InvalidOperationException("Connection string 'CatalogDatabase' not found.");

builder.Services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlite(connectionString));

// Add repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();