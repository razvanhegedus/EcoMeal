using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

public class UserController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}