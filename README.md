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

### Docker

* Docker Umgebung erstellen. Zum Testen reicht z.B. WSL2 mit einer Distri wie Ubuntu. Dazu noch Docker Desktop für Windows und dort WSL2 Integration aktivieren.
* Testen in BASH dann mit docker --version
* Sourcen z.B. im Home Verzeichnis, entweder auschecken oder via ZIP von GITHUB
* 

### CI/CD

