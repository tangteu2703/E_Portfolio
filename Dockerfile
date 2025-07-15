# -------------------------
# Stage 1: Build and Publish
# -------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and csproj
COPY E_Portfolio.sln ./
COPY E_Portfolio/E_Portfolio.csproj ./E_Portfolio/

# Restore packages
RUN dotnet restore "E_Portfolio/E_Portfolio.csproj"

# Copy all source
COPY . .

# Publish the app
WORKDIR /src/E_Portfolio
RUN dotnet publish "E_Portfolio.csproj" -c Release -o /app/publish /p:UseAppHost=false

# -------------------------
# Stage 2: Runtime
# -------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copy published output
COPY --from=build /app/publish .

# Expose port
EXPOSE 80

# Set entrypoint
ENTRYPOINT ["dotnet", "E_Portfolio.dll"]
