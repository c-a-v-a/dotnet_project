namespace AutoParts.Web.Controllers;

using Rotativa.AspNetCore.Options;
using Rotativa.AspNetCore;
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

    public async Task<IActionResult> ClientRepairReport(int customerId, DateTime? month, int? vehicleId)
    {
        var vehicles = await _context.Vehicles
       .Where(v => v.CustomerId == customerId)
       .Select(v => new ReportVehicleItem
       {
           Id = v.Id,
           Name = v.Make + " " + v.ModelName + " (" + v.LicensePlate + ")"
       }).ToListAsync();

        IQueryable<ServiceOrder> query = _context.ServiceOrders
    .Include(o => o.Vehicle)
    .Include(o => o.Tasks)
        .ThenInclude(t => t.UsedParts)
            .ThenInclude(p => p.Part);

        if (month.HasValue)
        {
            var start = new DateTime(month.Value.Year, month.Value.Month, 1);
            var end = start.AddMonths(1);
            query = query.Where(o => o.EndDate >= start && o.EndDate < end);
        }

        if (vehicleId.HasValue)
        {
            query = query.Where(o => o.VehicleId == vehicleId);
        }

        var orders = await query.ToListAsync();

        var model = new ClientRepairReportViewModel
        {
            CustomerId = customerId,
            SelectedMonth = month,
            SelectedVehicleId = vehicleId,
            Vehicles = await _context.Vehicles
                .Where(v => v.CustomerId == customerId)
                .Select(v => new ReportVehicleItem
                {
                    Id = v.Id,
                    Name = v.Make + " " + v.ModelName + " (" + v.LicensePlate + ")"
                }).ToListAsync(),
            Orders = orders.Select(o => new RepairReportItem
            {
                // Jeśli EndDate jest null, daj minimalną datę lub obsłuż to w widoku
                Date = o.EndDate ?? DateTime.MinValue,
                Vehicle = o.Vehicle.Make + " " + o.Vehicle.ModelName,
                Description = o.Description,
                LaborCost = o.Tasks.Sum(t => t.LaborCost),
                PartsCost = o.Tasks.SelectMany(t => t.UsedParts).Sum(p => p.TotalPrice)
            }).ToList()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GeneratePdfReport(int customerId, DateTime? month, int? vehicleId)
    {
        // Ładujesz dane identycznie jak w ClientRepairReport:
        var vehicles = await _context.Vehicles
            .Where(v => v.CustomerId == customerId)
            .Select(v => new ReportVehicleItem
            {
                Id = v.Id,
                Name = v.Make + " " + v.ModelName + " (" + v.LicensePlate + ")"
            }).ToListAsync();

        IQueryable<ServiceOrder> query = _context.ServiceOrders
            .Where(o => o.CustomerId == customerId)
            .Include(o => o.Vehicle)
            .Include(o => o.Tasks)
                .ThenInclude(t => t.UsedParts)
                    .ThenInclude(p => p.Part);

        if (month.HasValue)
        {
            var start = new DateTime(month.Value.Year, month.Value.Month, 1);
            var end = start.AddMonths(1);
            query = query.Where(o => o.EndDate >= start && o.EndDate < end);
        }

        if (vehicleId.HasValue)
        {
            query = query.Where(o => o.VehicleId == vehicleId);
        }

        var orders = await query.ToListAsync();

        var model = new ClientRepairReportViewModel
        {
            CustomerId = customerId,
            SelectedMonth = month,
            SelectedVehicleId = vehicleId,
            Vehicles = vehicles,
            Orders = orders.Select(o => new RepairReportItem
            {
                Date = o.EndDate ?? DateTime.MinValue,
                Vehicle = o.Vehicle.Make + " " + o.Vehicle.ModelName,
                Description = o.Description,
                LaborCost = o.Tasks.Sum(t => t.LaborCost),
                PartsCost = o.Tasks.SelectMany(t => t.UsedParts).Sum(p => p.TotalPrice)
            }).ToList()
        };

        return new ViewAsPdf("ClientRepairReportPdf", model)
        {
            FileName = "RaportKosztow.pdf",
            PageOrientation = Orientation.Portrait,
            PageSize = Size.A4
        };
    }


}
