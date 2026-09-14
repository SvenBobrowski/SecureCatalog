using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SecureCatalog.Api.Data;
using SecureCatalog.Api.Repositories;
using SecureCatalog.Api.Security;
using SecureCatalog.Api.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// db connection
var connectionString =
    builder.Configuration.GetConnectionString("CatalogDatabase")
    ?? throw new InvalidOperationException("Connection string 'CatalogDatabase' not found.");

builder.Services.AddDbContext<CatalogDbContext>(options => options.UseSqlite(connectionString));

// Add repository
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Permissions and Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        Permissions.Products.Read,
        policy => policy.RequireClaim(Permissions.ClaimType, Permissions.Products.Read)
    );

    options.AddPolicy(
        Permissions.Products.Write,
        policy => policy.RequireClaim(Permissions.ClaimType, Permissions.Products.Write)
    );

    options.AddPolicy(
        Permissions.Products.Delete,
        policy => policy.RequireClaim(Permissions.ClaimType, Permissions.Products.Delete)
    );

    options.AddPolicy(
        Permissions.Products.Reset,
        policy => policy.RequireClaim(Permissions.ClaimType, Permissions.Products.Reset)
    );
});

// Authentification

// the jwt key should never come from an cleartype configuration file
var jwtKey =
    builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT key is missing");

// use JWT scheme fpr auth
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var issuer = builder.Configuration["Jwt:Issuer"];
        var audience = builder.Configuration["Jwt:Audience"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = issuer,
            ValidAudience = audience,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    // make sure db is exists and migrated on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await db.Database.MigrateAsync();

        var passwordHasher = scope.ServiceProvider
            .GetRequiredService<IPasswordHasher<User>>();

        if (!await db.Users.AnyAsync())
        {
            var editorUser = new User
            {
                Username = "editor",
                PasswordHash = string.Empty,
                Role = "Editor"
            };

            editorUser.PasswordHash = passwordHasher.HashPassword(
                editorUser,
                "editor123");

            db.Users.Add(editorUser);

            var readerUser = new User
            {
                Username = "reader",
                PasswordHash = string.Empty,
                Role = "Reader"
            };

            readerUser.PasswordHash = passwordHasher.HashPassword(
                readerUser,
                "reader123");

            db.Users.Add(readerUser);

            var ownerUser = new User
            {
                Username = "owner",
                PasswordHash = string.Empty,
                Role = "Owner"
            };

            ownerUser.PasswordHash = passwordHasher.HashPassword(
                ownerUser,
                "owner123");

            db.Users.Add(ownerUser);

            await db.SaveChangesAsync();
        }
    }
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
