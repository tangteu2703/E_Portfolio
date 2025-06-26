using E_Common;
using E_Contract.Repository;
using E_Contract.Service;
using E_Portfolio.Filter;
using E_Repository;
using E_Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------- CẤU HÌNH CHUỖI KẾT NỐI ---------------------
var defaultConnection = builder.Configuration.GetConnectionString("DefaultConnection");
if (defaultConnection != null)
    Connection.AddConnectionString("DefaultConnection", defaultConnection);

var hrmConnection = builder.Configuration.GetConnectionString("HRMConnection");
if (hrmConnection != null)
    Connection.AddConnectionString("HRMConnection", hrmConnection);

var bioConnection = builder.Configuration.GetConnectionString("BioStarConnection");
if (bioConnection != null)
    Connection.AddConnectionString("BioStarConnection", bioConnection);

// --------------------- CẤU HÌNH CORS ---------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --------------------- CẤU HÌNH JWT AUTHENTICATION ---------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
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

// --------------------- CẤU HÌNH LOGGING ---------------------
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// --------------------- CẤU HÌNH DEPENDENCY INJECTION ---------------------
builder.Services.AddScoped<LoggedAuthorrizeAttribute>();
builder.Services.AddScoped<RoleAuthorizeAttribute>();
builder.Services.AddScoped<MustLoggedFilter>();

builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddTransient<IServiceWrapper, ServiceWrapper>();
builder.Services.AddTransient<IRepositoryWrapper, RepositoryWrapper>();

// --------------------- CẤU HÌNH MVC ---------------------
builder.Services.AddControllersWithViews();

// --------------------- BUILD APP ---------------------
var app = builder.Build();

// --------------------- MIDDLEWARE PIPELINE ---------------------

// CORS phải đặt đầu tiên sau app.Build()
app.UseCors("AllowAll");

// File tĩnh và default page
app.UseDefaultFiles();
app.UseStaticFiles();

// Xử lý lỗi môi trường Production
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Routing
app.UseRouting();
app.UseHttpsRedirection();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Định tuyến MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run ứng dụng
app.Run();
