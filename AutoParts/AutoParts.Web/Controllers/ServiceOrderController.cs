using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
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

    public async Task<IActionResult> Index()
    {
        var orders = await _context.ServiceOrders
            .Include(o => o.Mechanic)
            .ToListAsync();

        return View(orders);
    }

    
    public async Task<IActionResult> EditStatus(int id)
    {
        var order = await _context.ServiceOrders.FindAsync(id);
        if (order == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        var isAssignedMechanic = order.MechanicId == userId;
        var isAdmin = User.IsInRole("Admin");

        if (!isAssignedMechanic && !isAdmin)
            return Forbid();

        var viewModel = new EditOrderStatusViewModel
        {
            Id = order.Id,
            Status = order.Status,
            MechanicId = order.MechanicId,
            IsAssignedMechanic = isAssignedMechanic,
            IsAdmin = isAdmin
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditStatus(EditOrderStatusViewModel model)
    {
        var order = await _context.ServiceOrders.FindAsync(model.Id);
        if (order == null) return NotFound();

        var userId = _userManager.GetUserId(User);
        var isAssignedMechanic = order.MechanicId == userId;
        var isAdmin = User.IsInRole("Admin");

        if (!isAssignedMechanic && !isAdmin)
            return Forbid();

        order.Status = model.Status;

        if (model.Status == ServiceOrder.OrderStatus.Finished)
        {
            order.EndDate = DateTime.Now;
        }

        _context.ServiceOrders.Update(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = model.Id });
    }

   

public async Task<IActionResult> Details(int? id)
{
    if (id == null)
    {
        return NotFound();
    }

    var order = await _context.ServiceOrders
        .Include(o => o.Customer)
        .Include(o => o.Vehicle)
        .Include(o => o.Mechanic)
        .Include(o => o.Tasks)
        .Include(o => o.Comments)
        .ThenInclude(c => c.Author)
        .FirstOrDefaultAsync(o => o.Id == id);

    if (order == null)
    {
        return NotFound();
    }

    return View(order);
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> AddComment(AddCommentViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Details", new { id = model.ServiceOrderId });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
            return Forbid();

        var comment = new Comment
        {
            Text = model.Text,
            CreatedAt = DateTime.Now,
            AuthorId = user.Id,
            ServiceOrderId = model.ServiceOrderId
        };

        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = model.ServiceOrderId });
    }


}
