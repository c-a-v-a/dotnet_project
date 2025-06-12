using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ServiceTaskController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServiceTaskController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> AddUsedPart(int taskId)
    {
        var task = await _context.ServiceTasks.FindAsync(taskId);
        if (task == null) return NotFound();

        var parts = await _context.Parts
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} - {p.UnitPrice} zł"
            }).ToListAsync();

        return View(new AddUsedPartViewModel
        {
            ServiceTaskId = taskId,
            AvailableParts = parts
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddUsedPart(AddUsedPartViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableParts = await _context.Parts
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.Name} - {p.UnitPrice} zł"
                }).ToListAsync();

            return View(model);
        }

        var part = await _context.Parts.FindAsync(model.PartId);
        if (part == null) return BadRequest();

        var usedPart = new UsedPart
        {
            PartId = part.Id,
            Quantity = model.Quantity,
            
            ServiceTaskId = model.ServiceTaskId
        };

        _context.UsedParts.Add(usedPart);
        await _context.SaveChangesAsync();

        var orderId = await _context.ServiceTasks
            .Where(t => t.Id == model.ServiceTaskId)
            .Select(t => t.ServiceOrderId)
            .FirstAsync();

        return RedirectToAction("Details", "ServiceOrder", new { id = orderId });
    }
}
