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
            if (ModelState.IsValid)
            {
                // Check if the username or email already exists
                var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username || u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Username or email is already taken");
                    return View(model);
                }

                // Hash the password before storing it
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var newUser = new User
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Username = model.Username,
                    Email = model.Email,
                    Password = hashedPassword, // Save hashed password                   
                };

                _dbContext.Users.Add(newUser);
                await _dbContext.SaveChangesAsync();

                // Redirect to the Login page upon successful registration
                return RedirectToAction("Login");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        // Handle Login Form Submission
        [HttpPost]
        public async Task<IActionResult> Login(User model)
        {
            if (ModelState.IsValid)
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

                // Check if user exists and validate the password
                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
                {
                    ModelState.AddModelError(string.Empty, "Invalid credentials");
                    return View(model);
                }

                // Generate a token (optional) or create an authenticated session
                var token = _tokenService.GenerateToken(user.Id.ToString(), user.Username);

                // Redirect to a dashboard or home page upon successful login
                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
