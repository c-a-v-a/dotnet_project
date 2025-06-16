using AutoParts.Web.Data;
using AutoParts.Web.Enums;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace AutoParts.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(ApplicationDbContext context, ILogger<ReportsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> OpenOrdersPdf()
        {
            try
            {
                _logger.LogInformation("Generowanie PDF z otwartymi zleceniami rozpoczęte.");

                var openOrders = await _context.ServiceOrders
                    .Include(o => o.Vehicle)
                    .Include(o => o.Mechanic)
                    .Where(o => o.Status == OrderStatus.New || o.Status == OrderStatus.InProgress)
                    .Select(o => new OpenOrderItem
                    {
                        Vehicle = o.Vehicle.Make + " " + o.Vehicle.ModelName,
                        Description = o.Description,
                        StartDate = (DateTime)o.StartDate,
                        Mechanic = o.Mechanic != null
                            ? o.Mechanic.FirstName + " " + o.Mechanic.LastName
                            : "Brak"
                    })
                    .ToListAsync();

                _logger.LogInformation($"Znaleziono {openOrders.Count} otwartych zleceń.");

                return new ViewAsPdf("OpenOrders", openOrders)
                {
                    FileName = "raport-otwarte-naprawy.pdf"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas generowania raportu PDF.");
                return StatusCode(500, "Błąd podczas generowania raportu.");
            }
        }
    }
}
