# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .

# all projects .csproj files
COPY RibbitReels.Api/*.csproj RibbitReels.Api/
COPY RibbitReels.Data/*.csproj RibbitReels.Data/
COPY RibbitReels.Services/*.csproj RibbitReels.Services/
COPY RibbitReels.UnitTests/*.csproj RibbitReels.UnitTests/
COPY RibbitReels.IntegrationTests/*.csproj RibbitReels.IntegrationTests/

# restore all projects
RUN dotnet restore

# copy
COPY . .
WORKDIR /src/RibbitReels.Api
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "RibbitReels.Api.dll"]
