namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ServiceOrderController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ServiceOrderMapper _mapper;
    private readonly UserManager<User> _userManager;

    public ServiceOrderController(ApplicationDbContext context, UserManager<User> userManager, ServiceOrderMapper mapper)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var orders = await _context.ServiceOrders
            .Include(o => o.Vehicle)
            .Include(o => o.Mechanic)
            .ToListAsync();
        var models = orders
            .Select(order =>
            {
                var customer = _context.Customers.Find(order.Vehicle.CustomerId);

                if (customer != null)
                {
                    var model = _mapper.ToViewModel(order);
                    model.Customer = _mapper.ToShortDto(customer);
                    model.CustomerId = customer.Id;

                    return model;
                }

                return _mapper.ToViewModel(order);
            })
            .ToList();


        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var order = await _context.ServiceOrders
            .Include(o => o.Vehicle)
            .ThenInclude(v => v.Customer)
            .Include(o => o.Mechanic)
            .Include(o => o.Tasks)
            .ThenInclude(t => t.UsedParts)
            .ThenInclude(p => p.Part)
            .Include(o => o.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order == null)
        {
            return NotFound();
        }

        var vehicles = await _context.Vehicles.Include(vehicle => vehicle.Customer).ToListAsync();
        var mechanics = await _userManager.Users.Where(user => user.Role == UserRole.Mechanic).ToListAsync();

        ViewBag.Vehicles = vehicles;
        ViewBag.Mechanics = mechanics;

        var model = _mapper.ToViewModel(order);
        model.CustomerId = order.Vehicle.CustomerId;
        model.Customer = _mapper.ToShortDto(order.Vehicle.Customer!);
        model.Comments = model.Comments.OrderByDescending(comment => comment.CreatedAt).ToList();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vehicles = await _context.Vehicles.Include(vehicle => vehicle.Customer).ToListAsync();
        var mechanics = await _userManager.Users.Where(user => user.Role == UserRole.Mechanic).ToListAsync();

        ViewBag.Vehicles = vehicles;
        ViewBag.Mechanics = mechanics;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ServiceOrderModel model)
    {
        var vehicle = await _context.Vehicles.FindAsync(model.VehicleId);

        if (vehicle == null)
        {
            return NotFound();
        }

        model.CustomerId = vehicle.CustomerId;
        model.StartDate = DateTime.Now;

        if (!ModelState.IsValid || vehicle == null)
        {
            var vehicles = await _context.Vehicles.Include(vehicle => vehicle.Customer).ToListAsync();
            var mechanics = await _userManager.Users.Where(user => user.Role == UserRole.Mechanic).ToListAsync();

            ViewBag.Vehicles = vehicles;
            ViewBag.Mechanics = mechanics;

            return View(model);
        }

        var order = _mapper.ToEntity(model);

        _context.ServiceOrders.Add(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "ServiceOrder"); // lub inny widok
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ServiceOrderModel model)
    {
        var order = await _context.ServiceOrders.FindAsync(model.Id);

        if (order == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);

        if (user == null || user.Role != UserRole.Admin && user.Id != model.MechanicId)
        {
            return Forbid();
        }

        order.Status = model.Status;
        order.MechanicId = model.MechanicId;

        if (model.Status == OrderStatus.Finished)
        {
            order.EndDate = DateTime.Now;
        }

        _context.ServiceOrders.Update(order);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "RequiredAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        ServiceOrder? serviceOrder = await _context.ServiceOrders.FindAsync(id);

        if (serviceOrder != null)
        {
            _context.ServiceOrders.Remove(serviceOrder);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
