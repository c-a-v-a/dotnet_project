using System.Text;
using AutoParts.Web.Authorization;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

Rotativa.AspNetCore.RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
