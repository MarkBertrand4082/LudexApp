using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LudexApp.Models;
using LudexApp.Data;

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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(User user, string password)
        {
            if (!ModelState.IsValid)
                return View(user);

            // TODO: Hash password before saving
            // user.PasswordHash = Hash(password);

            _context.Users.Add(user);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

        // -------------------------
        // Login
        // -------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(int id /* or username */, string password)
        {
            User user = _context.Users
                .Include(u => u.GetFriends())
                .FirstOrDefault(u => u.id == id);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View();
            }

            // TODO: Verify password hash
            // if (!Verify(password, user.PasswordHash))...

            // Store login session
            HttpContext.Session.SetInt32("UserId", user.id);

            return RedirectToAction("Profile", new { id = user.id });
        }

        // -------------------------
        // View a User Profile
        // -------------------------
        public IActionResult Profile(int id)
        {
            User? user = _context.Users
                .Include(u => u.GetPosts())
                .Include(u => u.GetFriends())
                .FirstOrDefault(u => u.id == id);

            if (user == null)
                return NotFound();

            return View(user);
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
                .Include(u => u.GetFriends())
                .FirstOrDefault(u => u.id == currentId);

            return View(user.GetFriends());
        }

        // -------------------------
        // Add a Friend
        // -------------------------
        public IActionResult AddFriend(int friendId)
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");
            if (currentId == null) return RedirectToAction("Login");

            User user = _context.Users.Include(u => u.GetFriends()).First(u => u.id == currentId);
            User friend = _context.Users.FirstOrDefault(u => u.id == friendId);

            if (friend != null)
            {
                user.GetFriends().Add(friend);
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