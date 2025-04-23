using Microsoft.AspNetCore.Mvc;
using SimpleCRM.Data;
namespace SimpleCRM.Controllers;
public class AuthController : Controller
{
    private readonly ApplicationDbContext _dbContext;

    public AuthController(ApplicationDbContext applicationContext)
    {
        _dbContext = applicationContext;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    public IActionResult Index()
    {
        return View();
    }
}
