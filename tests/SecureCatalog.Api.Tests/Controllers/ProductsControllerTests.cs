using Microsoft.AspNetCore.Mvc;
using Moq;
using SecureCatalog.Api.Controllers;
using SecureCatalog.Api.Contracts;
using SecureCatalog.Api.Models;
using SecureCatalog.Api.Repositories;

namespace SecureCatalog.Api.Tests.Controllers;

public class ProductsControllerTests
{
    [Fact]
    public async Task GetById_WhenProductDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var repository = new Mock<IProductRepository>();

        repository
            .Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var controller = new ProductsController(repository.Object);

        // Act
        var result = await controller.GetById(42, CancellationToken.None);

        // Assert
        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var product = new Product
        {
            Id = 42,
            Name = "Keyboard",
            Price = 99,
        };

        var repository = new Mock<IProductRepository>();

        repository
            .Setup(r => r.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var controller = new ProductsController(repository.Object);

        // Act
        var result = await controller.GetById(42, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProduct = Assert.IsType<Product>(okResult.Value);

        Assert.Equal(42, returnedProduct.Id);
        Assert.Equal("Keyboard", returnedProduct.Name);
        Assert.Equal(99, returnedProduct.Price);
    }

    [Fact]
    public async Task CreateProduct_ReturnsProductWithId()
    {
        // Arrange
        var repository = new Mock<IProductRepository>();

        repository
            .Setup(r => r.CreateAsync(
                It.IsAny<Product>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product product, CancellationToken _) =>
            {
                product.Id = 42; // Simulate database-generated ID
                return product;
            });

        var controller = new ProductsController(repository.Object);

        var request = new ProductRequestDTO(
            "Keyboard",
            "Mechanical keyboard",
            99
        );

        // Act
        var result = await controller.Create(request, CancellationToken.None);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var createdProduct = Assert.IsType<Product>(createdAtActionResult.Value);

        Assert.Equal(42, createdProduct.Id);
        Assert.Equal("Keyboard", createdProduct.Name);
        Assert.Equal("Mechanical keyboard", createdProduct.Description);
        Assert.Equal(99, createdProduct.Price);

        // Verify that the repository's CreateAsync method was called with the expected product
        repository.Verify(
            r => r.CreateAsync(
                It.Is<Product>(p =>
                p.Name == "Keyboard" &&
                p.Description == "Mechanical keyboard" &&
                p.Price == 99),
            It.IsAny<CancellationToken>()),
        Times.Once);
    }
}
