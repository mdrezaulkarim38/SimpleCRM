using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SimpleCRM.Data;
using SimpleCRM.Models;
using SimpleCRM.Services;
namespace SimpleCRM.Controllers
{
    public class AuthController : Controller

    {
        private readonly ApplicationDbContext _dbContext;
        private readonly TokenService _tokenService;

        public AuthController(ApplicationDbContext applicationContext, TokenService tokenService)
        {
            _dbContext = applicationContext;
            _tokenService = tokenService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(User model)
        {          
            if (!ModelState.IsValid)
            {             
                return View(model);
            }

            try
            {               
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower()
                                              || u.Email.ToLower() == model.Email.ToLower());
             
                if (existingUser != null)
                {                   
                    TempData["ErrorMessage"] = "Username or email is already taken. Please use a different one.!";
                    return View(model);
                }                
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);
                
                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Email = model.Email,
                    Password = hashedPassword 
                };
                
                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Registration successful! Please login to continue.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred during registration. Please try again.!";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] User model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Username or Password is missing.");
                return View(model);
            }
            try
            {
                var user = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == model.Username.ToLower());

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    return View(model);
                }

                // Generate JWT token
                var token = _tokenService.GenerateToken(user.Id.ToString(), user.Username);
                return Json(new { success = true, token }); // Return the token
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "An error occurred during login." });                
            }
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
