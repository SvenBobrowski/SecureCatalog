using Microsoft.EntityFrameworkCore;
using SecureCatalog.Api.Models;

namespace SecureCatalog.Api.Data;

public sealed class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
}