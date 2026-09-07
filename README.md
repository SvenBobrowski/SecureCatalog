### Database

## Creation

dotnet tool run dotnet-ef database update `
  --project src/SecureCatalog.Api/SecureCatalog.Api.csproj `
  --startup-project src/SecureCatalog.Api/SecureCatalog.Api.csproj

## Maintenance

* Necessary steps.

# Create new migration*
dotnet ef migrations add <MigrationName>

# Apply migration*
dotnet ef database update

# Remove last none applied migration
dotnet ef migrations remove

# List all migrations
dotnet ef migrations list