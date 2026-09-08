using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SecureCatalog.Api.Data;
using SecureCatalog.Api.Models;

namespace SecureCatalog.Api.Repositories;

public sealed class ProductRepository(
    CatalogDbContext dbContext)
    : IProductRepository
{
    public async Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .OrderBy(product => product.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Products
            .AsNoTracking()
            .SingleOrDefaultAsync(
                product => product.Id == id,
                cancellationToken);
    }

    public async Task<Product> CreateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        dbContext.Products.Add(product);

        await dbContext.SaveChangesAsync(cancellationToken);

        return product;
    }

    public async Task<bool> UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default)
    {
        var exists = await dbContext.Products
            .AnyAsync(
                existingProduct => existingProduct.Id == product.Id,
                cancellationToken);

        if (!exists)
        {
            return false;
        }

        dbContext.Products.Update(product);

        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var product = await dbContext.Products.FindAsync(
            [id],
            cancellationToken);

        if (product is null)
        {
            return false;
        }

        dbContext.Products.Remove(product);

        return await dbContext.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task ResetAsync(
        CancellationToken cancellationToken = default)
    {
        await dbContext.Products.ExecuteDeleteAsync();
    }
}