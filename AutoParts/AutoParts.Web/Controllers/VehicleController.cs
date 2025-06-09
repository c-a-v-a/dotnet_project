namespace AutoParts.Web.Controllers;

using System.IO;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Policy = "RequiredAdminOrReceptionistRole")]
public class VehicleController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IWebHostEnvironment _environment;
    private readonly VehicleMapper _mapper;

    public VehicleController(ApplicationDbContext context, VehicleMapper mapper, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
        _mapper = mapper;
    }

    // GET: /Vehicle/Details?id=id
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var vehicle = await _context.Vehicles
            .Include(v => v.Customer)
            .FirstOrDefaultAsync(v => v.Id == id);

        if (vehicle == null)
        {
            return NotFound();
        }

        var model = _mapper.ToViewModel(vehicle);

        return View(model);
    }

    // GET: /Vehicle/Create?id=id
    [HttpGet]
    public IActionResult Create(int id)
    {
        var model = new VehicleModel
        {
            CustomerId = id
        };
        return View(model);
    }

    // POST: /Vehicle/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleModel vehicleModel, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
        {
            return View(vehicleModel);
        }

        Vehicle vehicle = _mapper.ToEntity(vehicleModel);
        vehicle.Id = 0;

        if (ImageFile != null)
        {
            vehicle.ImageUrl = await SaveImage(ImageFile);
        }

        _context.Vehicles.Add(vehicle);

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Customer", new { id = vehicleModel.CustomerId });
    }

    // POST: /Vehicle/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(VehicleModel model, IFormFile? ImageFile)
    {
        if (!ModelState.IsValid)
        {
            return View("Details", model);
        }

        Vehicle? vehicle = await _context.Vehicles.FindAsync(model.Id);

        if (vehicle == null)
        {
            return NotFound();
        }

        _mapper.ToEntity(model, vehicle);

        if (ImageFile != null)
        {
            vehicle.ImageUrl = await SaveImage(ImageFile);
        }

        _context.Vehicles.Update(vehicle);

        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "Vehicle", new { id = model.Id });
    }

    private async Task<string> SaveImage(IFormFile ImageFile)
    {
        string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
        string imageUrl = Path.Combine(uploadsFolder, uniqueName);

        using (var stream = new FileStream(imageUrl, FileMode.Create))
        {
            await ImageFile.CopyToAsync(stream);
        }

        return ("/uploads/" + uniqueName);
    }
}
