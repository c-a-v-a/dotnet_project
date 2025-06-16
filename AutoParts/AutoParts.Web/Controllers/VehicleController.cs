namespace AutoParts.Web.Controllers;

using System.IO;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Policy = "RequiredAdminOrReceptionistRole")]
public class VehicleController : Controller
{
    private readonly VehicleService _service;
    private readonly IWebHostEnvironment _environment;

    public VehicleController(VehicleService service, IWebHostEnvironment environment)
    {
        _service = service;
        _environment = environment;
    }

    // GET: /Vehicle/Details?id=id
    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        VehicleModel model = await _service.GetAsync(id);

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

        if (ImageFile != null)
        {
            vehicleModel.ImageUrl = await SaveImage(ImageFile);
        }

        await _service.CreateAsync(vehicleModel);

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

        if (ImageFile != null)
        {
            model.ImageUrl = await SaveImage(ImageFile);
        }

        VehicleModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return RedirectToAction("Details", "Vehicle", new { id = model.Id });
    }

    // POST: /Vehicle/Delete/id=id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int customerId)
    {
        await _service.DeleteAsync(id);

        return RedirectToAction("Details", "Customer", new { id = customerId });
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
