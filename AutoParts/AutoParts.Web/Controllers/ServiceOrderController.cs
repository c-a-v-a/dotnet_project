using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ServiceOrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ServiceOrderController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new CreateServiceOrderViewModel
        {
            Customers = await _context.Customers
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.FirstName + " " + c.LastName
                }).ToListAsync(),

            Vehicles = await _context.Vehicles
                .Select(v => new SelectListItem
                {
                    Value = v.Id.ToString(),
                    Text = v.Make + " " + v.ModelName + " (" + v.LicensePlate + ")"
                }).ToListAsync(),

            Mechanics = await _userManager.Users
                .Where(u => u.Role == UserRole.Mechanic)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.FirstName + " " + u.LastName
                }).ToListAsync()
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateServiceOrderViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return await Create(); // Reload dropdowny, jeśli błąd
        }

        var order = new ServiceOrder
        {
            Description = model.Description,
            CustomerId = model.CustomerId,
            VehicleId = model.VehicleId,
            MechanicId = model.MechanicId,
            Status = ServiceOrder.OrderStatus.New
        };

        _context.ServiceOrders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Home"); // lub inny widok
    }
}
