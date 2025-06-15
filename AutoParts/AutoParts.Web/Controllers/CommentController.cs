namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data;
using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize]
public class CommentController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly CommentMapper _mapper;
    private readonly UserManager<User> _userManager;

    public CommentController(ApplicationDbContext context, CommentMapper mapper, UserManager<User> userManager)
    {
        _context = context;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Create(int serviceOrderId)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null)
        {
            return Forbid();
        }

        var model = new CommentModel
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
        model.CreatedAt = DateTime.Now;

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        _context.Comments.Add(_mapper.ToEntity(model));
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "ServiceOrder", new { id = model.ServiceOrderId });
    }

    [HttpGet]
    public async Task<IActionResult> Update(int commentId)
    {
        var user = await _userManager.GetUserAsync(User);

        var comment = await _context.Comments.FindAsync(commentId);

        if (comment == null)
        {
            return NotFound();
        }

        if (user == null || comment.AuthorId != user.Id)
        {
            return Forbid();
        }

        var model = _mapper.ToViewModel(comment);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(CommentModel model)
    {
        var user = await _userManager.GetUserAsync(User);

        if (user == null || model.AuthorId != user.Id)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var comment = await _context.Comments.FindAsync(model.Id);

        if (comment == null)
        {
            return NotFound();
        }

        _mapper.ToEntity(model, comment);
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync();

        return RedirectToAction("Details", "ServiceOrder", new { id = model.ServiceOrderId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, int serviceOrderId)
    {
        var comment = await _context.Comments.FindAsync(id);

        if (comment != null)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null || comment.AuthorId != user.Id)
            {
                return Forbid();
            }

            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Details", "ServiceOrder", new { id = serviceOrderId });
    }
}
