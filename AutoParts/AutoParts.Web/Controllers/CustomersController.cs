namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class CustomersController : Controller
{
    private readonly ApplicationDbContext _context;

    public CustomersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Customers/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Customers/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerModel model)
    {
        if (ModelState.IsValid)
        {
            var customer = new Customer
            {
                Name = model.Name,
                SecondName = model.SecondName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                Email = model.Email,
                Vehicles = new List<Vehicle>()
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Klient został dodany pomyślnie";
            return RedirectToAction(nameof(Index));
        }
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        return View(customer); // Przekazujemy bezpośrednio encję Customer
    }

    // GET: Customers
    public async Task<IActionResult> Index()
    {
        return View(await _context.Customers.ToListAsync());
    }
}
