namespace AutoParts.Web.Controllers;

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
[Route("api/ServiceOrder")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ServiceOrderApiController : ControllerBase
{
    private readonly ServiceOrderService _service;

    public ServiceOrderApiController(ServiceOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        List<ServiceOrderModel> models = await _service.GetAllAsync();

        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        ServiceOrderModel? model = await _service.GetAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] ServiceOrderModel model)
    {
        model.StartDate = DateTime.Now;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ServiceOrderModel created = await _service.CreateAsync(model);

        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] ServiceOrderModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("Id missmatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        ServiceOrderModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, [FromQuery] int serviceOrderId)
    {
        int deletedId = await _service.DeleteAsync(id);

        return Ok(new { DeletedId = deletedId });
    }
}
