using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SecureCatalog.Api.Data;

namespace SecureCatalog.Api.Tests.Factories;

public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    private SqliteConnection? _connection;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                     typeof(DbContextOptions<CatalogDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<CatalogDbContext>(options =>
            {
                options.UseSqlite(_connection);
            });

            var provider = services.BuildServiceProvider();

            using var scope = provider.CreateScope();

            var db = scope.ServiceProvider
                .GetRequiredService<CatalogDbContext>();

            db.Database.EnsureCreated();
        });
    }

    protected override void Dispose(bool disposing)
    {
        _connection?.Dispose();
        base.Dispose(disposing);
    }
}