# Default value if not provided during build
ARG APP_VERSION=1.0.0

# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers (Optimized for caching)
# We copy the Solution and Props first to ensure global config is respected
COPY DSA.sln ./
COPY Directory.Build.props ./
COPY src/DSA.Api/DSA.Api.csproj ./src/DSA.Api/

# Restore dependencies
RUN dotnet restore "src/DSA.Api/DSA.Api.csproj"

# 2. Copy Source Code (The Meat)
COPY src/ ./src/

WORKDIR "/src/src/DSA.Api"

RUN dotnet build "DSA.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
ARG APP_VERSION
RUN dotnet publish "DSA.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false /p:Version=$APP_VERSION

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Create a non-root user
USER app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DSA.Api.dll"]

# Expose port
EXPOSE 80

