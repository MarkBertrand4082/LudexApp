using LudexApp.Data;
using LudexApp.Models;
using LudexApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Controllers
{
    public class UserController : Controller
    {
        private readonly LudexDbContext _context;

        public UserController(LudexDbContext context)
        {
            _context = context;
        }

        // -------------------------
        // Registration
        // -------------------------

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            return View(new RegisterViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("Login");
        }

        // -------------------------
        // Login
        // -------------------------

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public IActionResult ValidateLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Login", model);

            // Try to find user by email
            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View("Login", model);
            }

            // TODO: Replace this with password hash verification.
            if (user.Password != model.Password)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View("Login", model);
            }

            // Store logged-in user ID in session
            HttpContext.Session.SetInt32("UserId", user.Id);

            // If a return URL was supplied, go there
            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            // Default: Go to HomeController → HomePage view
            return RedirectToAction("HomePage", "Home");
        }

        // -------------------------
        // View a User Profile
        // -------------------------
        public IActionResult Profile(int id)
        {
            var user = _context.Users
                .Include(u => u.Posts)
                .Include(u => u.GameReviews)
                .Include(u => u.GameLibrary)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            return View("User", user); // loads User.cshtml
        }

        public IActionResult UserLibrary(int id)
        {
            var user = _context.Users
                .Include(u => u.GameLibrary)
                .FirstOrDefault(u => u.Id == id);

            if (user == null)
                return NotFound();

            return View(user); // loads UserLibrary.cshtml
        }

        // -------------------------
        // View your Friends List
        // -------------------------
        public IActionResult Friends()
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");

            if (currentId == null)
                return RedirectToAction("Login");

            User user = _context.Users
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == currentId);

            return View(user.Friends);
        }

        // -------------------------
        // Add a Friend
        // -------------------------
        public IActionResult AddFriend(int friendId)
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");
            if (currentId == null) return RedirectToAction("Login");

            User user = _context.Users.Include(u => u.Friends).First(u => u.Id == currentId);
            User friend = _context.Users.FirstOrDefault(u => u.Id == friendId);

            if (friend != null)
            {
                user.Friends.Add(friend);
                _context.SaveChanges();
            }

            return RedirectToAction("Friends");
        }

        // -------------------------
        // Logout
        // -------------------------
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}