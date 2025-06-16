namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
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
    private readonly ServiceOrderService _service;

    public ServiceOrderController(ApplicationDbContext context, UserManager<User> userManager, ServiceOrderMapper mapper, ServiceOrderService service)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<ServiceOrderModel> models = await _service.GetAllAsync();

        return View(models);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        ServiceOrderModel? model = await _service.GetAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        var vehicles = await _context.Vehicles.Include(vehicle => vehicle.Customer).ToListAsync();
        var mechanics = await _userManager.Users.Where(user => user.Role == UserRole.Mechanic).ToListAsync();

        ViewBag.Vehicles = vehicles;
        ViewBag.Mechanics = mechanics;

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

        ServiceOrderModel? created = await _service.CreateAsync(model);

        if (created == null)
        {
            var vehicles = await _context.Vehicles.Include(vehicle => vehicle.Customer).ToListAsync();
            var mechanics = await _userManager.Users.Where(user => user.Role == UserRole.Mechanic).ToListAsync();

            ViewBag.Vehicles = vehicles;
            ViewBag.Mechanics = mechanics;

            return View(model);
        }

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

        model.Vehicle = null;
        model.Mechanic = null;
        model.Customer = null;

        await _service.UpdateAsync(model);

        return RedirectToAction("Details", new { id = model.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Policy = "RequiredAdminRole")]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return RedirectToAction("Index");
    }
}
