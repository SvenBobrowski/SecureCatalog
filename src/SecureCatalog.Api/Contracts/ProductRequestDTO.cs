using System.ComponentModel.DataAnnotations;

namespace SecureCatalog.Api.Contracts;

public sealed record ProductRequestDTO(
    [Required]
    [StringLength(120, MinimumLength = 2)]
    string Name,

    [StringLength(1024)]
    string? Description,

    [Range(0, 1_000_000)]
    decimal Price);