using LudexApp.Data;
using LudexApp.Models;
using LudexApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            return View(new RegisterViewModel { ReturnUrl = returnUrl });
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
                Password = model.Password // TODO: Hash this!
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
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public IActionResult ValidateLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Login", model);

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || user.Password != model.Password)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View("Login", model);
            }

            HttpContext.Session.SetInt32("UserId", user.Id);

            if (!string.IsNullOrEmpty(model.ReturnUrl))
                return Redirect(model.ReturnUrl);

            return RedirectToAction("HomePage", "Home");
        }

        // -------------------------
        // View a user profile
        // -------------------------
        public IActionResult Profile(int id)
        {
            var user = _context.Users
                .Include(u => u.Posts)
                .Include(u => u.GameReviews)
                    .ThenInclude(r => r.Game)
                .Include(u => u.GameLibrary)
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            var vm = new UserViewModel
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Posts = user.Posts.Select(p => new PostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content
                }).ToList(),
                Reviews = user.GameReviews.Select(r => new ReviewViewModel
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Content = r.Content,
                    GameTitle = r.Game?.Name ?? ""
                }).ToList(),
                GameLibrary = user.GameLibrary.Select(g => new GameViewModel
                {
                    GameId = g.Id,
                    Name = g.Name
                }).ToList(),
                Friends = user.Friends.Select(f => new FriendViewModel
                {
                    Id = f.Id,
                    Username = f.Username
                }).ToList()
            };

            return View("User", vm);
        }

        // -------------------------
        // User's Game Library
        // -------------------------
        public IActionResult UserLibrary(int id)
        {
            var user = _context.Users
                .Include(u => u.GameLibrary)
                .FirstOrDefault(u => u.Id == id);

            if (user == null) return NotFound();

            var games = user.GameLibrary.Select(g => new GameViewModel
            {
                GameId = g.Id,
                Name = g.Name
            }).ToList();

            return View(games);
        }

        // -------------------------
        // View Friends
        // -------------------------
        public IActionResult Friends()
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");
            if (currentId == null) return RedirectToAction("Login");

            var user = _context.Users
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == currentId);

            var friends = user?.Friends.Select(f => new FriendViewModel
            {
                Id = f.Id,
                Username = f.Username
            }).ToList() ?? new List<FriendViewModel>();

            return View(friends);
        }

        // -------------------------
        // Add a friend
        // -------------------------
        public IActionResult AddFriend(int friendId)
        {
            int? currentId = HttpContext.Session.GetInt32("UserId");
            if (currentId == null) return RedirectToAction("Login");

            var user = _context.Users.Include(u => u.Friends).First(u => u.Id == currentId);
            var friend = _context.Users.FirstOrDefault(u => u.Id == friendId);
            if (friend != null)
            {
                user.Friends.Add(friend);
                _context.SaveChanges();
            }

            return RedirectToAction("Friends");
        }
    }
}