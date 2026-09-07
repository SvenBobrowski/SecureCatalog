using SecureCatalog.Api.Data;
using SecureCatalog.Api.Repositories;
using Microsoft.EntityFrameworkCore;
using SecureCatalog.Api.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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

// Permissions and Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        Permissions.Products.Read,
        policy => policy.RequireClaim(
            Permissions.ClaimType,
            Permissions.Products.Read));
    
    options.AddPolicy(
        Permissions.Products.Write,
        policy => policy.RequireClaim(
            Permissions.ClaimType, 
            Permissions.Products.Write));

    options.AddPolicy(
        Permissions.Products.Delete,
        policy => policy.RequireClaim(
            Permissions.ClaimType, 
            Permissions.Products.Delete));
});

// Authentification

// the jwt key should never come from an cleartype configuration file
var jwtKey = builder.Configuration["Jwt:Key"]
  ?? throw new InvalidOperationException("JWT key is missing");

// use JWT scheme fpr auth
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt.Issuer"],
            ValidAudience = builder.Configuration["Jwt.Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

// make sure db is exists and migrated on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    await db.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();