using E_API;
using E_Common;
using E_Contract.Repository;
using E_Contract.Service;
using E_Contract.Service.AI;
using E_Repository;
using E_Service;
using E_Service.AI;
using E_Service.Hosted;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Connection Strings
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (defaultConnection != null)
    Connection.AddConnectionString("DefaultConnection", defaultConnection);

var hrmConnection = builder.Configuration.GetConnectionString("HRMConnection");
if (hrmConnection != null)
    Connection.AddConnectionString("HRMConnection", hrmConnection);

var bioConnection = builder.Configuration.GetConnectionString("BioStarConnection");
if (bioConnection != null)
    Connection.AddConnectionString("BioStarConnection", bioConnection);

var neoeConnection = builder.Configuration.GetConnectionString("NEOEConnection");
if (neoeConnection != null)
    Connection.AddConnectionString("NEOEConnection", neoeConnection);

var portfolioConnection = builder.Configuration.GetConnectionString("PortfolioConnection");
if (portfolioConnection != null)
    Connection.AddConnectionString("PortfolioConnection", portfolioConnection);

var E_PortalConnection = builder.Configuration.GetConnectionString("E_PortalConnection");
if (E_PortalConnection != null)
    Connection.AddConnectionString("E_PortalConnection", E_PortalConnection);
#endregion

#region Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IServiceWrapper, ServiceWrapper>();
builder.Services.AddTransient<IRepositoryWrapper, RepositoryWrapper>();

//Hosted Service
builder.Services.AddHostedService<JobHostedService>();

// Register AI Services riêng biệt
builder.Services.AddSingleton<IVisionAIService, VisionAIService>();

// Logging
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
#endregion

#region CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhostFrontend", policy =>
    {
        policy.WithOrigins(
            "https://localhost:44344",        // Cổng frontend đúng!
            "http://192.168.22.124"           // Nếu chạy IIS
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // Cho phép gửi cookie/token
    });
});

#endregion

#region Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? string.Empty);

builder.Services.AddSingleton<JwtHelper>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "E-Contract API", Version = "v1" });
    c.OperationFilter<FileUploadOperation>(); // 👈 custom hỗ trợ IFormFile
});

#endregion

var app = builder.Build();

#region Middleware
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // để truy cập Swagger UI khi push lên server
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();  mục đích là để sử dụng HTTPS, nhưng nếu bạn đang chạy trên localhost có thể bỏ qua
app.UseStaticFiles();

app.UseRouting();

// 💡 THÊM CORS Ở ĐÂY
app.UseCors("AllowLocalhostFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion
