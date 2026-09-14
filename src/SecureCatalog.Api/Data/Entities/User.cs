namespace SecureCatalog.Api.Data.Entities;

public class User
{
    public int Id { get; set; }

    public required string Username { get; set; }

    public required string PasswordHash { get; set; }

    public bool IsActive { get; set; } = true;

    public required string Role { get; set; }
}