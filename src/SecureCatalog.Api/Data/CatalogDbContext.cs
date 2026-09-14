using Microsoft.EntityFrameworkCore;
using SecureCatalog.Api.Models;
using SecureCatalog.Api.Data.Entities;

namespace SecureCatalog.Api.Data;

public sealed class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Username)
            .IsUnique();
    }
}