namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/Vehicle")]
[Authorize(Policy = "RequiredAdminOrReceptionistRole", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class VehicleApiController : ControllerBase
{
    private readonly VehicleService _service;

    public VehicleApiController(VehicleService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        List<VehicleModel> models = await _service.GetAllAsync();

        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        VehicleModel? model = await _service.GetAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] VehicleModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        VehicleModel created = await _service.CreateAsync(model);

        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] VehicleModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("Id missmatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        VehicleModel? updated = await _service.UpdateAsync(model);

        if (updated == null)
        {
            return NotFound();
        }

        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        int deletedId = await _service.DeleteAsync(id);

        return Ok(new { DeletedId = deletedId });
    }
}
