using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using SecureCatalog.Api.Tests.Factories;

namespace SecureCatalog.Api.Tests.Integration;

public class AuthorizationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthorizationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllProducts_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}