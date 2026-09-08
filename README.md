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

## Image bauen und ausführen

# Build mit Dockerfile
docker build -t securecatalog-api .

# Run with port
docker run --rm -p 8080:8080 securecatalog-api

# Testing in Ubuntu
curl -v http://localhost:8080/api/products

# Testing is possible with http file
Switch address to http://localhost:8080
and use the VS Code extension

### CI/CD

