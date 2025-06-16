using System.Text;
using AutoParts.Web.Authorization;
using AutoParts.Web.Configuration;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using Microsoft.OpenApi.Models;
using NLog;

// Inicjalizacja NLog
var logger = NLog.LogManager.Setup().LoadConfigurationFromFile("nlog.config").GetCurrentClassLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    //  Konfiguracja NLog jako provider logowania
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);

    builder.Host.UseNLog();

    builder.WebHost.UseUrls("http://localhost:7252");

    // Add services to the container.
    builder.Services.AddControllersWithViews();

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

    var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];
    var key = Encoding.UTF8.GetBytes(jwtSecretKey ?? "");

    builder.Services
      .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
      .AddEntityFrameworkStores<ApplicationDbContext>();

    builder.Services.AddAuthentication().AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = false
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequiredAdminRole", policy =>
            policy.Requirements.Add(new RoleRequirement(UserRole.Admin)));

        options.AddPolicy("RequiredAdminOrReceptionistRole", policy =>
            policy.Requirements.Add(new RoleRequirement(UserRole.Admin, UserRole.Receptionist)));
    });

    builder.Services.AddScoped<IAuthorizationHandler, RoleHandler>();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Mappers
    builder.Services.AddSingleton(new CommentMapper());
    builder.Services.AddSingleton(new CustomerMapper());
    builder.Services.AddSingleton(new PartMapper());
    builder.Services.AddSingleton(new ServiceOrderMapper());
    builder.Services.AddSingleton(new ServiceTaskMapper());
    builder.Services.AddSingleton(new UsedPartMapper());
    builder.Services.AddSingleton(new UserMapper());
    builder.Services.AddSingleton(new VehicleMapper());

    // Services
    builder.Services.AddScoped<CommentService>();
    builder.Services.AddScoped<CustomerService>();
    builder.Services.AddScoped<PartService>();
    builder.Services.AddScoped<ServiceTaskService>();
    builder.Services.AddScoped<ServiceOrderService>();
    builder.Services.AddScoped<UsedPartService>();
    builder.Services.AddScoped<VehicleService>();

    builder.Services.AddHostedService<OpenOrderReportBackgroundService>();
    builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

    builder.Services.Configure<ReportEmailSettings>(
        builder.Configuration.GetSection("ReportEmail"));

    var app = builder.Build();

    Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseAuthentication();
    app.UseRouting();
    app.UseAuthorization();

    app.MapRazorPages();

    app.UseSwagger();
    app.UseSwaggerUI();

    app.MapControllerRoute(
      name: "default",
      pattern: "{controller=Home}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    //  Jeśli aplikacja się nie uruchomi – zapisz do NLog
    logger.Error(ex, "❌ Aplikacja nie uruchomiła się.");
    throw;
}
finally
{
    // Zamykanie NLog poprawnie
    NLog.LogManager.Shutdown();
}
