using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebToken.Models;

namespace WebToken.Controllers;

public class registerController : Controller
{
    private readonly ILogger<registerController> _logger;

    public registerController(ILogger<registerController> logger)
    {
        _logger = logger;
    }


    public IActionResult Index()
    {
        return View();
    }

  

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
