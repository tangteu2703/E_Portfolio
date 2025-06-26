# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["E_API/E_API.csproj", "E_API/"]
COPY ["E_Common/E_Common.csproj", "E_Common/"]
COPY ["E_Contract/E_Contract.csproj", "E_Contract/"]
COPY ["E_Model/E_Model.csproj", "E_Model/"]
COPY ["E_Repository/E_Repository.csproj", "E_Repository/"]
COPY ["E_Service/E_Service.csproj", "E_Service/"]
RUN dotnet restore "./E_API/E_API.csproj"
COPY . .
WORKDIR "/src/E_API"
RUN dotnet build "./E_API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./E_API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# FIX: Disable IPv6 in Linux .NET container (pre-login handshake TLS fix)
ENV DOTNET_SYSTEM_NET_DISABLEIPV6 true

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "E_API.dll"]