using Microsoft.AspNetCore.Mvc;

namespace AutoParts.Web.Controllers;
public class PdfController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
