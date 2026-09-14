using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SecureCatalog.Api.Security;
using SecureCatalog.Api.Data.Entities;
using Microsoft.AspNetCore.Identity;
using SecureCatalog.Api.Data;

namespace SecureCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    IConfiguration configuration,
    IPasswordHasher<User> passwordHasher,
    CatalogDbContext dbContext)
    : ControllerBase
{
    [HttpPost("token")]
    public async Task<ActionResult<TokenResponse>> CreateToken(
        LoginRequest login,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Username == login.Username, cancellationToken);

        if (user is null || !user.IsActive)
        {
            return Unauthorized();
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            login.Password);

        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return Unauthorized();
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = passwordHasher.HashPassword(
                user,
                login.Password);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        IEnumerable<string>? permissions = user.Role switch
        {
            "Reader" =>
            [
                Permissions.Products.Read
            ],

            "Editor" =>
            [
                Permissions.Products.Read,
                Permissions.Products.Write,
                Permissions.Products.Delete
            ],

            "Owner" =>
            [
                Permissions.Products.Read,
                Permissions.Products.Write,
                Permissions.Products.Delete,
                Permissions.Products.Reset
            ],

            _ => null
        };

        if (permissions is null)
        {
            return Unauthorized();
        }

        return Ok(CreateJwt(
            user.Username,
            permissions));
    }

    private TokenResponse CreateJwt(
         string username,
         IEnumerable<string> permissions)
    {
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key is missing.");

        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];

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
