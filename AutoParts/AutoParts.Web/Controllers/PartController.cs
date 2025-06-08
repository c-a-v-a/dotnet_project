namespace AutoParts.Web.Controllers;

using System;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
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

    [HttpPost]
    public async Task<IActionResult> Update(PartModel model)
    {
        if (!ModelState.IsValid)
        {
            var parts = _context.Parts
                .Select(part => _mapper.ToViewModel(part))
                .ToList();

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

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var part = _context.Parts.Find(id);

        if (part != null)
        {
            _context.Parts.Remove(part);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}

