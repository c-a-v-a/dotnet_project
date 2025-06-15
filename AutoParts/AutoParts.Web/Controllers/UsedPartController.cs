namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class UsedPartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UsedPartMapper _mapper;
    private readonly UserManager<User> _userManager;

    public UsedPartController(ApplicationDbContext context, UsedPartMapper mapper, UserManager<User> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int serviceTaskId, int serviceOrderId)
    {
        if (await _Verify(serviceOrderId) == false)
        {
            return Forbid();
        }

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
        if (await _Verify(serviceOrderId) == false)
        {
            return Forbid();
        }

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
        if (await _Verify(serviceOrderId) == false)
        {
            return Forbid();
        }

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
        if (await _Verify(serviceOrderId) == false)
        {
            return Forbid();
        }

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
        if (await _Verify(serviceOrderId) == false)
        {
            return Forbid();
        }

        var usedPart = _context.UsedParts.Find(id);

        if (usedPart != null)
        {
            _context.UsedParts.Remove(usedPart);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
    }

    private async Task<bool> _Verify(int serviceOrderId)
    {
        var serviceOrder = await _context.ServiceOrders.FindAsync(serviceOrderId);
        var user = await _userManager.GetUserAsync(User);

        if (serviceOrder == null || user == null || user.Role != UserRole.Admin && user.Id != serviceOrder.MechanicId)
        {
            return false;
        }

        return true;
    }
}
