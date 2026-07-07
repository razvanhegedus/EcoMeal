using Microsoft.AspNetCore.Mvc;

namespace EcoMealApp.Controllers;

public class BusinessTypeController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}