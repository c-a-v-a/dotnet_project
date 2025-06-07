using AutoParts.Web.Data.Entities;
using AutoParts.Web.Data;
using Microsoft.AspNetCore.Mvc;
using AutoParts.Web.Models;

public class VehiclesController : Controller
{
    private readonly ApplicationDbContext _context;

    public VehiclesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Vehicles/Create?customerId=5
    [HttpGet]
    public IActionResult Create(int customerId)
    {
        var model = new VehicleModel
        {
            CustomerId = customerId
        };
        return View(model);
    }

    // POST: Vehicles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VehicleModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var vehicle = new Vehicle
                {
                    Make = model.Make,
                    Model = model.Model,
                    Year = model.Year,
                    Color = model.Color,
                    VIN = model.VIN,
                    LicensePlate = model.LicensePlate,
                    Fuel = model.Fuel,
                    CustomerId = model.CustomerId
                };

                _context.Vehicles.Add(vehicle);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pojazd został dodany pomyślnie";
                return RedirectToAction("Details", "Customers", new { id = model.CustomerId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania pojazdu");
                // Logowanie błędu
                Console.WriteLine($"Błąd: {ex.Message}");
            }
        }

        // Jeśli doszliśmy tutaj, coś poszło nie tak - pokaż formularz ponownie
        return View(model);
    }
}
