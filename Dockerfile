# ------------------------------
# Base image for runtime
# ------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ------------------------------
# Build stage
# ------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files
COPY ["E_Portfolio.sln", "./"]
COPY ["E_Portfolio/E_Portfolio.csproj", "E_Portfolio/"]
COPY ["E_Common/E_Common.csproj", "E_Common/"]
COPY ["E_Contract/E_Contract.csproj", "E_Contract/"]
COPY ["E_Model/E_Model.csproj", "E_Model/"]
COPY ["E_Repository/E_Repository.csproj", "E_Repository/"]
COPY ["E_Service/E_Service.csproj", "E_Service/"]

# Restore dependencies
RUN dotnet restore "E_Portfolio.sln"

# Copy everything else
COPY . .

# Build app
WORKDIR "/src/E_Portfolio"
RUN dotnet build "E_Portfolio.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ------------------------------
# Publish stage
# ------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "E_Portfolio.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ------------------------------
# Final image
# ------------------------------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E_Portfolio.dll"]
