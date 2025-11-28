//Lew Lin
using IGDB.Models;
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
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            LudexDbContext context,
            IUserRepository userRepository,
            IGameRepository gameRepository,
            ILogger<UserController> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _gameRepository = gameRepository;   // ⬅️ ADDED
            _logger = logger;
        }

        // -------------------------
        // Registration
        // -------------------------
        [HttpGet] 

        public async Task<IActionResult> UserSearch(string? searchTerm = null)
        {
            var model = new UserSearchViewModel
            {
                SearchTerm = searchTerm
            };

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchResults = _userRepository.GetUsersByUsername(searchTerm);
                model.Users = searchResults;
            }
            else
            {
                var all = await _userRepository.GetUsersAsync();

                foreach(User i in all)
                {
                    var vm = new UserViewModel
                    {
                        Id = i.Id,
                        Username = i.Username,
                        Email = i.Email
                    };
                    model.Users.Add(vm);
                }
            }
            return View("UserSearch", model);
        }

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
                .Include(u => u.Reviews)
                .Include(u => u.GameLibrary)  // only need the junction table now
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
                Reviews = user.Reviews.Select(r => new ReviewItemViewModel
                {
                    ReviewId = r.ReviewId,
                    Rating = r.Rating,
                    Content = r.Content,
                    GameId = r.GameId   // use GameId only
                }).ToList(),
                Games = user.GameLibrary.Select(g => new GameSummaryViewModel
                {
                    GameId = g.GameId  // use GameId only
                }).ToList(),
                Friends = user.Friends.Select(f => new FriendViewModel
                {
                    FriendId = f.FriendId
                }).ToList()
            };

            return View("User", vm);
        }

        // -------------------------
        // User's Game Library
        // -------------------------
        [HttpGet]
        public async Task<IActionResult> UserLibrary(int id)
        {
            var user = await _context.Users
                .Include(u => u.GameLibrary)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null) return NotFound();

            var games = new List<GameSummaryViewModel>();

            foreach (var ug in user.GameLibrary)
            {
                var game = await _gameRepository.GetGameByIdAsync(ug.GameId); // fetch IGDB game
                if (game != null)
                {
                    games.Add(new GameSummaryViewModel
                    {
                        GameId = ug.GameId,
                        Title = game.Name,
                        Platform = game.Platforms?.Values.Any() == true
                            ? string.Join(", ", game.Platforms.Values.Select(p => p.Name))
                            : "",
                        AverageRating = game.Rating,
                        CoverUrl = game.Cover.Value.Url
                    });
                }
            }

            var vm = new GameLibraryViewModel
            {
                Games = games
            };

            return View(vm);
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
