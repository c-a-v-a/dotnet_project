namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Policy = "RequiredAdminOrReceptionistRole")]
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CustomerMapper _mapper;

    public CustomerController(ApplicationDbContext context, CustomerMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: /Customer/Index
    [HttpGet]
    public IActionResult Index()
    {
        var customers = _context.Customers.ToList();
        var models = customers.Select(customer => _mapper.ToViewModel(customer)).ToList();
        return View(models);
    }

    // GET: /Customer/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Customer/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CustomerModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Customer customer = _mapper.ToEntity(model);

        _context.Customers.Add(customer);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // POST: /Customer/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(CustomerModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Customer? customer = await _context.Customers.FindAsync(model.Id);

        if (customer == null)
        {
            return NotFound();
        }

        _mapper.ToEntity(model, customer);
        _context.Customers.Update(customer);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // GET: /Customer/Details?id=id
    public async Task<IActionResult> Details(int id)
    {
        var customer = await _context.Customers
            .Include(c => c.Vehicles)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (customer == null)
        {
            return NotFound();
        }

        var model = _mapper.ToViewModel(customer);

        return View(model);
    }

    // POST: /Customer/Delete?id=id
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var customer = _context.Customers.Find(id);

        if (customer != null)
        {
            _context.Customers.Remove(customer);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
