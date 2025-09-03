#!/usr/bin/env bash
set -euo pipefail

echo "🚀 Bootstrapping Contoso Energy Backend API..."

BACKEND_DIR="$(cd "$(dirname "$0")/../src/backend" && pwd)"
SOLUTION_NAME="ContosoEnergy"
SOLUTION_FILE="$BACKEND_DIR/$SOLUTION_NAME.sln"

mkdir -p "$BACKEND_DIR"
cd "$BACKEND_DIR"

DOTNET="dotnet"

# Create solution
[ ! -f "$SOLUTION_FILE" ] && $DOTNET new sln -n "$SOLUTION_NAME"

# Projects
declare -A PROJECTS=( ["Domain"]="classlib" ["Application"]="classlib" ["Infrastructure"]="classlib" ["API"]="webapi" ["Tests"]="xunit" )

for PROJ in "${!PROJECTS[@]}"; do
    TYPE="${PROJECTS[$PROJ]}"
    [ ! -d "$PROJ" ] && $DOTNET new $TYPE -n "$PROJ"
done

# References
$DOTNET add Application reference Domain
$DOTNET add Infrastructure reference Application
$DOTNET add API reference Application
$DOTNET add API reference Infrastructure
$DOTNET add Tests reference Application
$DOTNET add Tests reference Infrastructure

# NuGet packages
$DOTNET add Application package Ardalis.Specification
$DOTNET add Application package Ardalis.Result
$DOTNET add Infrastructure package Microsoft.EntityFrameworkCore
$DOTNET add Infrastructure package Npgsql.EntityFrameworkCore.PostgreSQL
$DOTNET add API package Swashbuckle.AspNetCore
$DOTNET add API package Serilog.AspNetCore
$DOTNET add API package OpenTelemetry
$DOTNET add API package OpenTelemetry.Extensions.Hosting
$DOTNET add API package OpenTelemetry.Instrumentation.AspNetCore
$DOTNET add API package OpenTelemetry.Instrumentation.Http
$DOTNET add API package OpenTelemetry.Instrumentation.EntityFrameworkCore
$DOTNET add API package OpenTelemetry.Exporter.Console

# Add projects to solution
for PROJ in "${!PROJECTS[@]}"; do
    if ! $DOTNET sln list | grep -q "$PROJ"; then
        $DOTNET sln add "$PROJ"
    fi
done

# Create Dockerfile
cat > API/Dockerfile <<'EOF'
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "API/API.csproj"
COPY . .
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c Release -o /app/build
RUN dotnet publish "API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "API.dll"]
EOF

echo "✅ Backend bootstrap complete!"
