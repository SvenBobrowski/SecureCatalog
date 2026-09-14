using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using SecureCatalog.Api.Tests.Factories;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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

    [Fact]
    public async Task GetAllProducts_WithAuthorization_ReturnsOk()
    {
        // Arrange: Login
        var loginResponse = await _client.PostAsJsonAsync(
            "/api/auth/token",
            new
            {
                username = "reader",
                password = "reader123"
            });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var tokenResponse =
            await loginResponse.Content.ReadFromJsonAsync<Api.Controllers.TokenResponse>();

        Assert.NotNull(tokenResponse);
        Assert.False(string.IsNullOrWhiteSpace(tokenResponse.AccessToken));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                tokenResponse.AccessToken);

        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}