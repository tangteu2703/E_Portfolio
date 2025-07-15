# Use the official ASP.NET runtime image as a base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["E_Portfolio/E_Portfolio.csproj", "E_Portfolio/"]
COPY ["E_Common/E_Common.csproj", "E_Common/"]
COPY ["E_Contract/E_Contract.csproj", "E_Contract/"]
COPY ["E_Model/E_Model.csproj", "E_Model/"]
COPY ["E_Repository/E_Repository.csproj", "E_Repository/"]
COPY ["E_Service/E_Service.csproj", "E_Service/"]
RUN dotnet restore "E_Portfolio/E_Portfolio.csproj"
COPY . .
WORKDIR "/src/E_Portfolio"
RUN dotnet publish "E_Portfolio.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage: copy published output and run the app
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "E_Portfolio.dll"]
