# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["src/DSA.API/DSA.API.csproj", "./"]

RUN dotnet restore "DSA.API.csproj"

# Copy everything else and build
COPY . .

WORKDIR "/src/."

RUN dotnet build "DSA.API.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "DSA.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "DSA.API.dll"]

# Expose port
EXPOSE 80

