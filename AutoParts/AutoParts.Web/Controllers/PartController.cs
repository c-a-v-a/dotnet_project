namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Policy = "RequiredAdminOrReceptionistRole")]
public class PartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PartMapper _mapper;

    public PartController(ApplicationDbContext context, PartMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // GET: /Part/Index
    [HttpGet]
    public IActionResult Index()
    {
        var parts = _context.Parts.ToList();
        var models = parts
            .Select(part => _mapper.ToViewModel(part))
            .OrderBy(part => part.Type)
            .ToList();
        return View(models);
    }

    // GET: /Part/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Part/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PartModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Part part = _mapper.ToEntity(model);

        _context.Parts.Add(part);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // POST: /Part/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(PartModel model)
    {
        if (!ModelState.IsValid)
        {
            var parts = await _context.Parts
                .Select(part => _mapper.ToViewModel(part))
                .ToListAsync();

            var index = parts.FindIndex(part => part.Id == model.Id);

            if (index >= 0)
            {
                parts[index] = model;
            }

            return View("Index", parts);
        }

        Part? part = await _context.Parts.FindAsync(model.Id);

        if (part == null)
        {
            return NotFound();
        }

        _mapper.ToEntity(model, part);
        _context.Parts.Update(part);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    // POST: /Part/Delete?id=id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var part = await _context.Parts.FindAsync(id);

        if (part != null)
        {
            _context.Parts.Remove(part);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}

