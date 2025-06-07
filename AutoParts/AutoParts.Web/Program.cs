using AutoParts.Web.Authorization;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services
  .AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = false)
  .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("RequiredAdminRole", policy =>
            policy.Requirements.Add(new RoleRequirement(UserRole.Admin)));
    });

builder.Services.AddScoped<IAuthorizationHandler, RoleHandler>();

builder.Services.AddSingleton(new UserMapper());

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
  name: "default",
  pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
