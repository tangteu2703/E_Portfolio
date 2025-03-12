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

#region add connection string
var cmsConnection = builder.Configuration.GetConnectionString("CMSConnection");
if (cmsConnection != null)
    Connection.AddConnectionString("CMSConnection", cmsConnection);
#endregion add connection string

// Add services to the container.
builder.Services.AddControllersWithViews();
#region Authentication setting

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});
builder.Services.AddSingleton<JwtHelper>();
builder.Services.AddScoped<LoggedAuthorrizeAttribute>();
builder.Services.AddScoped<RoleAuthorizeAttribute>();

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
builder.Services.AddScoped<MustLoggedFilter>();

#endregion Authentication setting
#region DI setting

builder.Services.AddTransient<IServiceWrapper, ServiceWrapper>();
builder.Services.AddTransient<IRepositoryWrapper, RepositoryWrapper>();

#endregion DI setting

#region App setting
var app = builder.Build();
app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
#endregion App setting
