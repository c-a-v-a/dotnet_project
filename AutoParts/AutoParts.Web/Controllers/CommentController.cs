namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using AutoParts.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class CommentController : Controller
{
    private readonly CommentService _service;
    private readonly CommentMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CommentController(CommentService service, CommentMapper mapper, UserManager<User> userManager)
    {
        _service = service;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int serviceOrderId)
    {
        User? user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Forbid();
        }

        CommentModel model = new CommentModel
        {
            ServiceOrderId = serviceOrderId,
            AuthorId = user.Id,
            Author = _mapper.ToShortDto(user)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(CommentModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _service.CreateAsync(model);

        return RedirectToAction("Details", "ServiceOrder", new { id = model.ServiceOrderId });
    }
}
