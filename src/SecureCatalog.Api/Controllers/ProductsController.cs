using Microsoft.AspNetCore.Mvc;
using SecureCatalog.Api.Contracts;
using SecureCatalog.Api.Models;
using SecureCatalog.Api.Repositories;

namespace SecureCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(
    IProductRepository repository)
    : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products =
            await repository.GetAllAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var product =
            await repository.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(
        [FromBody] ProductRequestDTO request,
        CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price
        };
            
        await repository.CreateAsync(product, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Product>> Update(
        int id,
        [FromBody] ProductRequestDTO request,
        CancellationToken cancellationToken)
    {
        var existingProduct =
            await repository.GetByIdAsync(id, cancellationToken);

        if (existingProduct is null)
        {
            return NotFound();
        }

        existingProduct.Name = request.Name;
        existingProduct.Description = request.Description;
        existingProduct.Price = request.Price;

        await repository.UpdateAsync(existingProduct, cancellationToken);

        return Ok(existingProduct);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var existingProduct =
            await repository.GetByIdAsync(id, cancellationToken);

        if (existingProduct is null)
        {
            return NotFound();
        }

        await repository.DeleteAsync(existingProduct.Id, cancellationToken);

        return NoContent();
    }
}