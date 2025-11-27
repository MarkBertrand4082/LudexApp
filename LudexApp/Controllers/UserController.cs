using LudexApp.Data;
using LudexApp.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LudexApp.Controllers
{
    public class UserController : Controller
    {
        private readonly LudexDbContext _context;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            LudexDbContext context,
            IUserRepository userRepository,
            ILogger<UserController> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _logger = logger;
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
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // NOTE: In a real app you'd hash the password!
            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                Password = model.Password
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            // After register, you can either auto-login or send them to Login.
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidateLogin(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            // Use repository to find user by email + password
            // (For production, you'd hash & verify instead of plain compare)
            var user = await _userRepository.GetUserByCredentialsAsync(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password.");
                return View("Login", model);
            }

            // Build claims for cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            // Create authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {
                    IsPersistent = model.RememberMe
                });

            _logger.LogInformation("User {UserId} logged in.", user.Id);

            // If ReturnUrl is set and is local, go back there
            if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            // Otherwise go to Home/Index (your HomePage)
            return RedirectToAction("Index", "Home");
        }

        // -------------------------
        // Logout (belongs here logically)
        // -------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // -------------------------
        // View a user profile
        // -------------------------
        [HttpGet]
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
                Reviews = user.GameReviews.Select(r => new ReviewItemViewModel
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Content = r.Content,
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
        [HttpGet]
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
        // View Friends (current logged-in user)
        // -------------------------
        [HttpGet]
        [Authorize]
        public IActionResult Friends()
        {
            var currentId = GetCurrentUserId();
            if (!currentId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == currentId.Value);

            var friends = user?.Friends.Select(f => new FriendViewModel
            {
                FriendId = f.FriendId
            }).ToList() ?? new List<FriendViewModel>();

            return View(friends);
        }

        // -------------------------
        // Add a friend (for current logged-in user)
        // -------------------------
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult AddFriend(int friendId)
        {
            var currentId = GetCurrentUserId();
            if (!currentId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = _context.Users
                .Include(u => u.Friends)
                .FirstOrDefault(u => u.Id == currentId.Value);

            if (user == null)
            {
                return NotFound();
            }

            var friend = _context.Users.FirstOrDefault(u => u.Id == friendId);
            if (friend != null && !user.Friends.Any(f => f.FriendId == friend.Id))
            {
                user.Friends.Add(new UserFriend
                {
                    UserId = user.Id,
                    FriendId = friend.Id
                });
                _context.SaveChanges();
            }

            return RedirectToAction("Friends");
        }

        // -------------------------
        // Helper: current user id from cookie auth
        // -------------------------
        private int? GetCurrentUserId()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idString, out var id) ? id : null;
        }
    }
}
