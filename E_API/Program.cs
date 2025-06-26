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
//builder.Services.AddSingleton<YoloService>();
//builder.Services.AddSingleton<OcrService>();
//builder.Services.AddSingleton<FaceService>();
//builder.Services.AddTransient<IImageService, ImageService>();

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
    options.AddPolicy("AllowLocalhostFrontend",
        policy =>
        {
            policy.WithOrigins("https://localhost:32771") // Đổi theo cổng frontend bạn dùng
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Nếu frontend có gửi cookie/token
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
#endregion

var app = builder.Build();

#region Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 💡 THÊM CORS Ở ĐÂY
app.UseCors("AllowLocalhostFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
#endregion
