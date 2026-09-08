using System.Data;

namespace SecureCatalog.Api.Security;

public static class Permissions
{
    public const string ClaimType = "permission";

    public static class Products
    {
        public const string Read = "products.read";
        public const string Write = "products.write";
        public const string Delete = "products.delete";
        public const string Reset = "products.reset";
    }
}