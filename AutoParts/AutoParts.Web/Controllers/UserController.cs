namespace AutoParts.Web.Controllers;

using AutoParts.Web.Data.Entities;
using AutoParts.Web.Enums;
using AutoParts.Web.Mappers;
using AutoParts.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[Authorize(Policy = "RequiredAdminRole")]
public class UserController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly UserMapper _mapper;

    public UserController(UserManager<User> userManager, UserMapper mapper)
    {
        _userManager = userManager;
        _mapper = mapper;
    }

    // GET: /User/Index
    [HttpGet]
    public IActionResult Index()
    {
        var users = _userManager.Users.ToList();
        var models = users.Select(user => _mapper.ToViewModel(user)).ToList();
        return View(models);
    }

    // POST: /User/UpdateRole
    [HttpPost]
    public async Task<IActionResult> UpdateRole(string id, UserRole role)
    {
        if (string.IsNullOrEmpty(id))
        {
            return BadRequest();
        }

        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return NotFound();
        }

        user.Role = role;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Failed to update user role.");
            var users = _userManager.Users.ToList();
            var models = users.Select(user => _mapper.ToViewModel(user)).ToList();
            return View("Index", models);
        }

        return RedirectToAction(nameof(Index));
    }
}
