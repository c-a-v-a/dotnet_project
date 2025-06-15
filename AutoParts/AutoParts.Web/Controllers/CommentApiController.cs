namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/Comment")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class CommentApiController : ControllerBase
{
    private readonly CommentService _service;
    private readonly CommentMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CommentApiController(CommentService service, CommentMapper mapper, UserManager<User> userManager)
    {
        _service = service;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CommentModel model)
    {
        User? user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Unauthorized();
        }

        model.AuthorId = user.Id;

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        CommentModel? created = await _service.CreateAsync(model);

        if (created == null)
        {
            return BadRequest(new { Message = "Invalid service order id" });
        }

        return Ok(created);
    }
}
