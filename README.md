### Informations

Kleines Übungsprojekt für .NET Entwickler für einen ASP.Net Core WebService mit:

Small training project for .NET Developers for a ASP.Net Core WebService with:

* Authorisation
* Authentification
* EF Integration
* Dependency Injection (DI)
* API Controllers (REST)
* CRUD
* SOA – Service-Oriented Architecture
* Docker Container incl. Volumes
* Unit Testing  <== current milestone
* Continuous Delivery (CD)

### WIP

This project is in progress // Das Projekt ist in Entwicklung.
Ich bitte um Entschuldigung für das Denglisch, das wird nach und nach sauber aufgeräumt.

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

# Build with Dockerfile
docker build -t securecatalog-api .

# Run with port
docker run --rm -p 8080:8080 securecatalog-api

# Testing in Ubuntu
curl -v http://localhost:8080/api/products

# Avoid problems with proxies in WSL
hostname -I
=> 172.18.241.250
Set address to http://172.18.241.250:8080 in HTTP for testing

# Finally test API with Docker Container

### CI/CD

