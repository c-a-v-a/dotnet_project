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

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OpenOrdersPdf()
        {
            var openOrders = await _context.ServiceOrders
                .Include(o => o.Vehicle)
                .Include(o => o.Mechanic)
                .Where(o => o.Status == OrderStatus.New || o.Status == OrderStatus.InProgress)
                .Select(o => new OpenOrderItem
                {
                    Vehicle = o.Vehicle.Make + " " + o.Vehicle.ModelName,
                    Description = o.Description,
                    StartDate = (DateTime)o.StartDate,
                    Mechanic = o.Mechanic != null ? o.Mechanic.FirstName + " " + o.Mechanic.LastName : "Brak"
                })
                .ToListAsync();

            return new ViewAsPdf("OpenOrders", openOrders)
            {
                FileName = "raport-otwarte-naprawy.pdf"
            };
        }
    }
}
