using Microsoft.AspNetCore.Mvc;
using SecureCatalog.Api.Contracts;
using SecureCatalog.Api.Models;
using SecureCatalog.Api.Repositories;
using Microsoft.AspNetCore.Authorization;
using SecureCatalog.Api.Security;

namespace SecureCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(
    IProductRepository repository)
    : ControllerBase
{
    [Authorize(Policy = Permissions.Products.Read)]
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAll(
        CancellationToken cancellationToken)
    {
        var products =
            await repository.GetAllAsync(cancellationToken);

        return Ok(products);
    }

    [Authorize(Policy = Permissions.Products.Read)]
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

    [Authorize(Policy = Permissions.Products.Write)]
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

    [Authorize(Policy = Permissions.Products.Write)]
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

    [Authorize(Policy = Permissions.Products.Delete)]
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

    [Authorize(Policy = Permissions.Products.Reset)]
    [HttpPost("reset")]
    public async Task<ActionResult> Reset(CancellationToken cancellationToken)
    {
        await repository.ResetAsync(cancellationToken);

        return NoContent();
    }
}