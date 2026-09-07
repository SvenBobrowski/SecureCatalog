using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureCatalog.Api.Security;

namespace SecureCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    IConfiguration configuration)
    : ControllerBase
{
    [HttpPost("token")]
    public ActionResult<TokenResponse> CreateToken(LoginRequest login) 
    {
        // demo only, normally we use ASP.NET Identity
        if (login.Username == "reader" && login.Password == "reader123")
        {
            return Ok(CreateJwt(
                login.Username,
                new[]
                {
                    Permissions.Products.Read
                }));
        }
        if (login.Username == "writer" && login.Password == "writer123")
        {
            return Ok(CreateJwt(
                login.Username,
                new[]
                {
                    Permissions.Products.Read,
                    Permissions.Products.Write,
                    Permissions.Products.Delete
                }));
        }

        return Unauthorized();
    }

   private TokenResponse CreateJwt(
        string username,
        IEnumerable<string> permissions)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

        Console.WriteLine($"{issuer}:{audience}");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username)
        };

        claims.AddRange(
            permissions.Select(permission =>
                new Claim(Permissions.ClaimType, permission)));

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);

        var tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new TokenResponse(tokenValue, expires);
    }
}

public record LoginRequest(
    string Username,
    string Password
);

public record TokenResponse(
    string AccessToken,
    DateTime ExpiresAt
);
