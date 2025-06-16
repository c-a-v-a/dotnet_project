namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/ServiceTask")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ServiceTaskApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ServiceTaskService _service;
    private readonly UserManager<User> _userManager;

    public ServiceTaskApiController(ApplicationDbContext context, ServiceTaskService service, UserManager<User> userManager)
    {
        _context = context;
        _service = service;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        User? user = await _userManager.GetUserAsync(User);

        if (user == null || user.Role != UserRole.Admin)
        {
            return Forbid();
        }

        List<ServiceTaskModel> models = await _service.GetAllAsync();

        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        ServiceTaskModel? model = await _service.GetAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        if (!await VerifyTask(id))
        {
            return Forbid();
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ServiceTaskModel model)
    {
        if (!await VerifyOrder(model.ServiceOrderId))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ServiceTaskModel created = await _service.CreateAsync(model);

        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] ServiceTaskModel model)
    {
        if (!await VerifyOrder(model.ServiceOrderId))
        {
            return Forbid();
        }

        if (id != model.Id)
        {
            return BadRequest("Id missmatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ServiceTaskModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, [FromQuery] int serviceOrderId)
    {
        if (!await VerifyOrder(serviceOrderId))
        {
            return Forbid();
        }

        int deletedId = await _service.DeleteAsync(id);

        return Ok(new { DeletedId = deletedId });
    }

    private async Task<bool> VerifyOrder(int serviceOrderId)
    {
        var serviceOrder = await _context.ServiceOrders.FindAsync(serviceOrderId);
        var user = await _userManager.GetUserAsync(User);

        if (serviceOrder == null || user == null || (user.Role != UserRole.Admin && user.Id != serviceOrder.MechanicId))
        {
            return false;
        }

        return true;
    }

    private async Task<bool> VerifyTask(int serviceTaskId)
    {
        var serviceTask = await _context.ServiceTasks.FindAsync(serviceTaskId);
        if (serviceTask == null)
        {
            return false;
        }

        var serviceOrder = await _context.ServiceOrders.FindAsync(serviceTask.ServiceOrderId);
        var user = await _userManager.GetUserAsync(User);

        if (serviceOrder == null || user == null || (user.Role != UserRole.Admin && user.Id != serviceOrder.MechanicId))
        {
            return false;
        }

        return true;
    }
}
