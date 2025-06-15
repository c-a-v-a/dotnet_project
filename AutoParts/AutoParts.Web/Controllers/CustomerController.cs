namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;

[Authorize(Policy = "RequiredAdminOrReceptionistRole")]
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CustomerMapper _mapper;
    private readonly CustomerService _service;

    public CustomerController(ApplicationDbContext context, CustomerMapper mapper, CustomerService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    // GET: /Customer/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<CustomerModel> models = await _service.GetAllAsync();

        return View(models);
    }

    // GET: /Customer/Create
    [HttpGet]
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

        await _service.CreateAsync(model);

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

        CustomerModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }

    // GET: /Customer/Details?id=id
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        CustomerModel? model = await _service.GetAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    // POST: /Customer/Delete?id=id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

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
            .Where(o => o.Vehicle.CustomerId == customerId)
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
