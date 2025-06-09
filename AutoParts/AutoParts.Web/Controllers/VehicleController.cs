namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = "RequiredAdminOrReceptionistRole")]
public class VehicleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly VehicleMapper _mapper;

    public VehicleController(ApplicationDbContext context, VehicleMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: Vehicle/Create?id=id
    [HttpGet]
    public IActionResult Create(int id)
    {
        var model = new VehicleModel
        {
            CustomerId = id
        };
        return View(model);
    }

    // POST: Vehicle/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleModel vehicleModel)
    {
        if (!ModelState.IsValid)
        {
            return View(vehicleModel);
        }

        Vehicle vehicle = _mapper.ToEntity(vehicleModel);

        _context.Vehicles.Add(vehicle);

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Customer", new { id = vehicleModel.CustomerId });
    }
}
