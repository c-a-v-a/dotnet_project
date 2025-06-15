namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Policy = "RequiredAdminOrReceptionistRole")]
public class PartController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly PartMapper _mapper;
    private readonly PartService _service;

    public PartController(ApplicationDbContext context, PartMapper mapper, PartService service)
    {
        _context = context;
        _mapper = mapper;
        _service = service;
    }

    // GET: /Part/Index
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        List<PartModel> models = await _service.GetAllAsync();

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

        await _service.CreateAsync(model);

        return RedirectToAction("Index");
    }

    // POST: /Part/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(PartModel model)
    {
        if (!ModelState.IsValid)
        {
            List<PartModel> models = await _service.GetAllAsync();

            var index = models.FindIndex(part => part.Id == model.Id);

            if (index >= 0)
            {
                models[index] = model;
            }

            return View("Index", models);
        }

        PartModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return RedirectToAction("Index");
    }

    // POST: /Part/Delete?id=id
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);

        return RedirectToAction("Index");
    }
}

