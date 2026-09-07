using SecureCatalog.Api.Models;

namespace SecureCatalog.Api.Repositories;

public interface IProductRepository
{
    Task<IReadOnlyList<Product>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<Product?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<Product> CreateAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(
        Product product,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}