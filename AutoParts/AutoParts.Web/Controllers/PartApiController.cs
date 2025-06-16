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
[Route("api/Part")]
[Authorize(Policy = "RequiredAdminOrReceptionistRole", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class PartApiController : ControllerBase
{
    private readonly PartService _service;

    public PartApiController(PartService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        List<PartModel> models = await _service.GetAllAsync();

        return Ok(models);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        PartModel? model = await _service.GetAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return Ok(model);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] PartModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        PartModel created = await _service.CreateAsync(model);

        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] PartModel model)
    {
        if (id != model.Id)
        {
            return BadRequest("Id missmatch");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        PartModel? updated = await _service.UpdateAsync(model);

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
