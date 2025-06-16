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
[Route("api/UsedPart")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class UsedPartApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly UsedPartService _service;
    private readonly UserManager<User> _userManager;

    public UsedPartApiController(ApplicationDbContext context, UsedPartService service, UserManager<User> userManager)
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

        List<UsedPartModel> models = await _service.GetAllAsync();

        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        User? user = await _userManager.GetUserAsync(User);

        if (user == null || user.Role != UserRole.Admin)
        {
            return Forbid();
        }

        UsedPartModel? model = await _service.GetAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] UsedPartModel model, [FromQuery] int serviceOrderId)
    {
        if (!await Verify(serviceOrderId))
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        UsedPartModel created = await _service.CreateAsync(model);

        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] UsedPartModel model, [FromQuery] int serviceOrderId)
    {
        if (!await Verify(serviceOrderId))
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

        UsedPartModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, [FromQuery] int serviceOrderId)
    {
        if (!await Verify(serviceOrderId))
        {
            return Forbid();
        }

        int deletedId = await _service.DeleteAsync(id);

        return Ok(new { DeletedId = deletedId });
    }

    private async Task<bool> Verify(int serviceOrderId)
    {
        var serviceOrder = await _context.ServiceOrders.FindAsync(serviceOrderId);
        var user = await _userManager.GetUserAsync(User);

        return serviceOrder != null && user != null &&
               (user.Role == UserRole.Admin || user.Id == serviceOrder.MechanicId);
    }
}
