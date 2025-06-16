namespace AutoParts.Web.Controllers;

using System.Threading.Tasks;
using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class ServiceTaskController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly ServiceTaskService _service;
    private readonly UserManager<User> _userManager;

    public ServiceTaskController(ApplicationDbContext context, ServiceTaskService service, UserManager<User> userManager)
    {
        _context = context;
        _service = service;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int serviceOrderId)
    {
        if (await _VerifyOrder(serviceOrderId) == false)
        {
            return Forbid();
        }

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
        if (await _VerifyOrder(model.ServiceOrderId) == false)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _service.CreateAsync(model);

        return RedirectToAction("Details", "ServiceOrder", new { id = model.ServiceOrderId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int serviceTaskId)
    {
        if (await _VerifyTask(serviceTaskId) == false)
        {
            return Forbid();
        }

        ServiceTaskModel? model = await _service.GetAsync(serviceTaskId);

        if (model == null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(ServiceTaskModel model)
    {
        if (await _VerifyOrder(model.ServiceOrderId) == false)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        ServiceTaskModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return RedirectToAction("Details", "ServiceOrder", new { id = updated.ServiceOrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int serviceOrderId)
    {
        if (await _VerifyOrder(serviceOrderId) == false)
        {
            return Forbid();
        }

        await _service.DeleteAsync(id);

        return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
    }

    private async Task<bool> _VerifyOrder(int serviceOrderId)
    {
        var serviceOrder = await _context.ServiceOrders.FindAsync(serviceOrderId);
        var user = await _userManager.GetUserAsync(User);

        if (serviceOrder == null || user == null || user.Role != UserRole.Admin && user.Id != serviceOrder.MechanicId)
        {
            return false;
        }

        return true;
    }

    private async Task<bool> _VerifyTask(int serviceTaskId)
    {
        var serviceTask = await _context.ServiceTasks.FindAsync(serviceTaskId);

        if (serviceTask == null)
        {
            return false;
        }

        var serviceOrder = await _context.ServiceOrders.FindAsync(serviceTask.ServiceOrderId);
        var user = await _userManager.GetUserAsync(User);

        if (serviceOrder == null || user == null || user.Role != UserRole.Admin && user.Id != serviceOrder.MechanicId)
        {
            return false;
        }

        return true;
    }
}
