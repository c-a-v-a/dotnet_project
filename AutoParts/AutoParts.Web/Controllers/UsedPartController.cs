namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Data;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class UsedPartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UsedPartMapper _mapper;

    public UsedPartController(ApplicationDbContext context, UsedPartMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int serviceTaskId, int serviceOrderId)
    {
        var parts = await _context.Parts.ToListAsync();

        ViewBag.Parts = parts;
        ViewBag.Order = serviceOrderId;

        var usedPart = new UsedPartModel
        {
            ServiceTaskId = serviceTaskId
        };

        return View(usedPart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(UsedPartModel model, int serviceOrderId)
    {
        if (!ModelState.IsValid)
        {
            var parts = await _context.Parts.ToListAsync();

            ViewBag.Parts = parts;
            ViewBag.Order = serviceOrderId;

            return View(model);
        }

        _context.UsedParts.Add(_mapper.ToEntity(model));
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int usedPartId, int serviceOrderId)
    {
        var usedPart = await _context.UsedParts.FindAsync(usedPartId);

        if (usedPart == null)
        {
            return NotFound();
        }

        var parts = await _context.Parts.ToListAsync();

        ViewBag.Parts = parts;
        ViewBag.Order = serviceOrderId;

        var model = _mapper.ToViewModel(usedPart);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(UsedPartModel model, int serviceOrderId)
    {
        if (!ModelState.IsValid)
        {
            var parts = await _context.Parts.ToListAsync();

            ViewBag.Parts = parts;
            ViewBag.Order = serviceOrderId;

            return View(model);
        }

        var usedPart = await _context.UsedParts.FindAsync(model.Id);

        if (usedPart == null)
        {
            return NotFound();
        }

        _mapper.ToEntity(model, usedPart);
        _context.UsedParts.Update(usedPart);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int serviceOrderId)
    {
        Console.WriteLine(id);
        Console.WriteLine(serviceOrderId);
        var usedPart = _context.UsedParts.Find(id);

        if (usedPart != null)
        {
            _context.UsedParts.Remove(usedPart);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
    }
}
