# POS System - Deployment Guide

## 🚀 Quick Deployment

### Development Environment

1. **Prerequisites**
   ```bash
   - .NET 6.0+ SDK
   - Visual Studio 2022 / VS Code / Rider
   - SQL Server (optional, for database)
   ```

2. **Build & Run**
   ```bash
   # Navigate to project
   cd E_Portfolio
   
   # Restore packages
   dotnet restore
   
   # Build
   dotnet build
   
   # Run
   dotnet run
   ```

3. **Access Application**
   ```
   https://localhost:5001/Sale
   https://localhost:5001/Sale/Orders
   ```

### Production Deployment

#### Option 1: IIS Deployment

1. **Publish Profile**
   ```bash
   dotnet publish -c Release -o ./publish
   ```

2. **IIS Configuration**
   - Install .NET Hosting Bundle
   - Create new site in IIS
   - Point to publish folder
   - Configure app pool (No Managed Code)
   - Set permissions for IIS_IUSRS

3. **Web.config**
   ```xml
   <configuration>
     <system.webServer>
       <handlers>
         <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" />
       </handlers>
       <aspNetCore processPath="dotnet" 
                   arguments=".\E_Portfolio.dll" 
                   stdoutLogEnabled="false" />
     </system.webServer>
   </configuration>
   ```

#### Option 2: Docker Deployment

1. **Dockerfile** (already exists)
   ```dockerfile
   FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
   WORKDIR /app
   EXPOSE 80
   
   FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
   WORKDIR /src
   COPY ["E_Portfolio/E_Portfolio.csproj", "E_Portfolio/"]
   RUN dotnet restore "E_Portfolio/E_Portfolio.csproj"
   COPY . .
   WORKDIR "/src/E_Portfolio"
   RUN dotnet build "E_Portfolio.csproj" -c Release -o /app/build
   
   FROM build AS publish
   RUN dotnet publish "E_Portfolio.csproj" -c Release -o /app/publish
   
   FROM base AS final
   WORKDIR /app
   COPY --from=publish /app/publish .
   ENTRYPOINT ["dotnet", "E_Portfolio.dll"]
   ```

2. **Build & Run Docker**
   ```bash
   # Build image
   docker build -t pos-system:latest .
   
   # Run container
   docker run -d -p 8080:80 --name pos-app pos-system:latest
   
   # Access
   http://localhost:8080/Sale
   ```

3. **Docker Compose**
   ```yaml
   version: '3.8'
   services:
     pos-app:
       build: .
       ports:
         - "8080:80"
       environment:
         - ASPNETCORE_ENVIRONMENT=Production
       depends_on:
         - db
     
     db:
       image: mcr.microsoft.com/mssql/server:2019-latest
       environment:
         - ACCEPT_EULA=Y
         - SA_PASSWORD=YourStrong@Password
       ports:
         - "1433:1433"
   ```

#### Option 3: Azure App Service

1. **Azure CLI**
   ```bash
   # Login
   az login
   
   # Create resource group
   az group create --name POS-RG --location eastus
   
   # Create app service plan
   az appservice plan create --name POS-Plan --resource-group POS-RG --sku B1
   
   # Create web app
   az webapp create --name pos-system-app --resource-group POS-RG --plan POS-Plan
   
   # Deploy
   az webapp deployment source config-zip --resource-group POS-RG --name pos-system-app --src ./publish.zip
   ```

2. **Via Visual Studio**
   - Right-click project → Publish
   - Select Azure → Azure App Service
   - Create new or select existing
   - Publish

## 🗄️ Database Setup

### SQL Server Schema

1. **Products Table**
   ```sql
   CREATE TABLE Products (
       ProductId INT PRIMARY KEY IDENTITY(1,1),
       ProductCode NVARCHAR(50) NOT NULL UNIQUE,
       ProductName NVARCHAR(200) NOT NULL,
       Description NVARCHAR(MAX),
       CategoryId INT,
       Price DECIMAL(18,2) NOT NULL,
       SalePrice DECIMAL(18,2),
       ImageUrl NVARCHAR(500),
       IsActive BIT DEFAULT 1,
       CreatedAt DATETIME DEFAULT GETDATE(),
       ModifiedAt DATETIME
   );
   ```

2. **Orders Table**
   ```sql
   CREATE TABLE Orders (
       OrderId INT PRIMARY KEY IDENTITY(1,1),
       OrderNumber NVARCHAR(50) NOT NULL UNIQUE,
       CustomerName NVARCHAR(200),
       CustomerPhone NVARCHAR(20),
       Subtotal DECIMAL(18,2) NOT NULL,
       Discount DECIMAL(18,2) DEFAULT 0,
       Total DECIMAL(18,2) NOT NULL,
       PaymentMethod NVARCHAR(50),
       Status NVARCHAR(50) DEFAULT 'Completed',
       CreatedBy NVARCHAR(100),
       CreatedAt DATETIME DEFAULT GETDATE(),
       ModifiedAt DATETIME
   );
   ```

3. **OrderDetails Table**
   ```sql
   CREATE TABLE OrderDetails (
       OrderDetailId INT PRIMARY KEY IDENTITY(1,1),
       OrderId INT NOT NULL,
       ProductId INT NOT NULL,
       ProductCode NVARCHAR(50),
       ProductName NVARCHAR(200),
       Quantity INT NOT NULL,
       UnitPrice DECIMAL(18,2) NOT NULL,
       Subtotal DECIMAL(18,2) NOT NULL,
       FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
       FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
   );
   ```

4. **Categories Table**
   ```sql
   CREATE TABLE Categories (
       CategoryId INT PRIMARY KEY IDENTITY(1,1),
       CategoryName NVARCHAR(100) NOT NULL,
       Description NVARCHAR(500),
       Icon NVARCHAR(50),
       IsActive BIT DEFAULT 1,
       DisplayOrder INT DEFAULT 0
   );
   ```

### Connection String

**appsettings.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=POS_DB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

**appsettings.Production.json**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your-Production-Connection-String"
  }
}
```

## 🔒 Security Checklist

### Pre-Deployment

- [ ] Remove sensitive data from appsettings.json
- [ ] Use environment variables for secrets
- [ ] Enable HTTPS redirect
- [ ] Configure CORS properly
- [ ] Implement authentication/authorization
- [ ] Add input validation
- [ ] Sanitize user inputs
- [ ] Enable request rate limiting
- [ ] Configure logging (no sensitive data)
- [ ] Set secure cookie policies

### Code Security
```csharp
// Program.cs
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});

app.UseHttpsRedirection();
app.UseHsts();

// CORS
app.UseCors(policy => 
    policy.WithOrigins("https://yourdomain.com")
          .AllowAnyMethod()
          .AllowAnyHeader());

// Rate limiting (optional)
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
        context => RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
});
```

## ⚙️ Configuration

### Environment Variables

```bash
# Development
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://localhost:5001;http://localhost:5000

# Production
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=https://+:443;http://+:80
ConnectionStrings__DefaultConnection="Server=...;Database=...;"
```

### Logging Configuration

**appsettings.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "Console": {
      "IncludeScopes": true
    },
    "File": {
      "Path": "Logs/pos-{Date}.txt",
      "RollingInterval": "Day"
    }
  }
}
```

## 📊 Monitoring & Performance

### Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddSqlServer(connectionString)
    .AddUrlGroup(new Uri("https://api.example.com/health"), name: "api");

app.MapHealthChecks("/health");
```

### Application Insights (Azure)

```csharp
builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration["ApplicationInsights:ConnectionString"]);
```

### Response Compression

```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});
```

## 🔄 CI/CD Pipeline

### GitHub Actions

**.github/workflows/deploy.yml**
```yaml
name: Deploy POS System

on:
  push:
    branches: [ main ]

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v2
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v1
      with:
        dotnet-version: 6.0.x
    
    - name: Restore dependencies
      run: dotnet restore
    
    - name: Build
      run: dotnet build --configuration Release --no-restore
    
    - name: Test
      run: dotnet test --no-restore --verbosity normal
    
    - name: Publish
      run: dotnet publish -c Release -o ./publish
    
    - name: Deploy to Azure
      uses: azure/webapps-deploy@v2
      with:
        app-name: 'pos-system-app'
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

### Azure DevOps Pipeline

**azure-pipelines.yml**
```yaml
trigger:
  branches:
    include:
      - main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'

steps:
- task: UseDotNet@2
  inputs:
    version: '6.x'

- script: dotnet restore
  displayName: 'Restore packages'

- script: dotnet build --configuration $(buildConfiguration)
  displayName: 'Build'

- script: dotnet publish --configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)
  displayName: 'Publish'

- task: PublishBuildArtifacts@1
  inputs:
    PathtoPublish: '$(Build.ArtifactStagingDirectory)'
    ArtifactName: 'drop'
```

## 🧪 Testing

### Unit Tests
```bash
dotnet test --filter Category=Unit
```

### Integration Tests
```bash
dotnet test --filter Category=Integration
```

### Load Testing (JMeter/k6)
```javascript
// k6 script
import http from 'k6/http';
import { check } from 'k6';

export let options = {
  vus: 100,
  duration: '30s',
};

export default function() {
  let res = http.get('https://yourapp.com/Sale');
  check(res, { 'status is 200': (r) => r.status === 200 });
}
```

## 📦 Backup & Recovery

### Database Backup
```sql
-- Full backup
BACKUP DATABASE POS_DB 
TO DISK = 'C:\Backup\POS_DB_Full.bak'
WITH FORMAT, MEDIANAME = 'POS_Backup', NAME = 'Full Backup';

-- Differential backup
BACKUP DATABASE POS_DB 
TO DISK = 'C:\Backup\POS_DB_Diff.bak'
WITH DIFFERENTIAL;
```

### Automated Backup Script
```powershell
# backup.ps1
$date = Get-Date -Format "yyyyMMdd_HHmmss"
$backupPath = "C:\Backup\POS_DB_$date.bak"

sqlcmd -S localhost -Q "BACKUP DATABASE POS_DB TO DISK='$backupPath'"
```

## 🚨 Troubleshooting

### Common Issues

1. **Port already in use**
   ```bash
   # Find process
   netstat -ano | findstr :5001
   
   # Kill process
   taskkill /PID <PID> /F
   ```

2. **Database connection failed**
   - Check SQL Server is running
   - Verify connection string
   - Check firewall rules
   - Test connection with SSMS

3. **Static files not loading**
   ```csharp
   // Program.cs
   app.UseStaticFiles();
   ```

4. **CORS errors**
   ```csharp
   builder.Services.AddCors(options =>
   {
       options.AddDefaultPolicy(builder =>
       {
           builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
       });
   });
   ```

## 📝 Post-Deployment Checklist

- [ ] Test all features (Sale, Orders, etc.)
- [ ] Verify database connections
- [ ] Check static files loading
- [ ] Test payment flow
- [ ] Verify print functionality
- [ ] Check mobile responsiveness
- [ ] Test different browsers
- [ ] Monitor error logs
- [ ] Set up alerts
- [ ] Create backup schedule
- [ ] Document deployment process
- [ ] Train users

## 📞 Support

### Logs Location
- **Development**: Console output
- **Production**: `/Logs/pos-{Date}.txt`
- **IIS**: `C:\inetpub\logs\`
- **Docker**: `docker logs <container-id>`

### Useful Commands
```bash
# Check application status
systemctl status kestrel-posapp.service

# View logs
tail -f /var/log/posapp.log

# Restart application
systemctl restart kestrel-posapp.service

# Docker logs
docker logs -f pos-app
```

---

**Deployment Version**: 1.0.0  
**Last Updated**: October 2025  
**Status**: Production Ready ✅

