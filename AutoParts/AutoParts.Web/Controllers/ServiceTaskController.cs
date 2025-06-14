namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ServiceTaskController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ServiceTaskMapper _mapper;

    public ServiceTaskController(ApplicationDbContext context, ServiceTaskMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult Create(int serviceOrderId)
    {
        var serviceTask = new ServiceTaskModel
        {
            ServiceOrderId = serviceOrderId
        };

        return View(serviceTask);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(ServiceTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        Console.WriteLine(model.ServiceOrderId);

        _context.ServiceTasks.Add(_mapper.ToEntity(model));
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "ServiceOrder", new { id = model.ServiceOrderId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int serviceTaskId)
    {
        var serviceTask = await _context.ServiceTasks.FindAsync(serviceTaskId);

        if (serviceTask == null)
        {
            return NotFound();
        }

        var model = _mapper.ToViewModel(serviceTask);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ServiceTaskModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var serviceTask = _context.ServiceTasks.Find(model.Id);

        if (serviceTask == null)
        {
            return NotFound();
        }

        _mapper.ToEntity(model, serviceTask);
        _context.ServiceTasks.Update(serviceTask);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "ServiceOrder", new { id = model.ServiceOrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int serviceOrderId)
    {
        var serviceTask = _context.ServiceTasks.Find(id);

        if (serviceTask != null)
        {
            _context.ServiceTasks.Remove(serviceTask);

            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
    }
}
