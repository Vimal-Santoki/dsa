# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/DSA.Api/DSA.Api.csproj", "./"]

RUN dotnet restore "DSA.Api.csproj"

# Copy everything else and build
COPY . .

WORKDIR "/src/."

RUN dotnet build "DSA.Api.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "DSA.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DSA.Api.dll"]

# Expose port
EXPOSE 80

