FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["SecureCatalog.Api.csproj", "./"]
RUN dotnet restore "SecureCatalog.Api.csproj"

COPY . .
RUN dotnet publish "SecureCatalog.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "SecureCatalog.Api.dll"]