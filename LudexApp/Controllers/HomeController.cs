using Microsoft.AspNetCore.Mvc;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using LudexApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;

namespace LudexApp.Controllers
{
    public class HomeController
    {
        private readonly IGameRepository m_gameRepository;
        private readonly IUserRepository m_userRepository;
        private readonly ILogger<HomeController> m_logger;

        public HomeController(IGameRepository gameRepository, IUserRepository userRepository, ILogger<HomeController> logger)
        {
            m_gameRepository = gameRepository;
            m_userRepository = userRepository;
            m_logger = logger;
        }

        // ------------------------------------------------------------------------------
        // View Home Page -> DisplayedFeaturedGames(), DisplayFriends(), DisplayList()
        // ------------------------------------------------------------------------------

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = new HomePageViewModel();

            // Determine whether current visitor is a logged-in user or a visitor
            model.IsLoggedIn = User.Identity?.IsAuthenticated ?? false;

            // HomeController.GetGames()
            model.FeaturedGames = await GetGames();

            if (model.IsLoggedIn)
            {
                int currentUserId = GetCurrentUserId(); // TODO: attach to Auth / UserModel id

                // Display List
                model.UserGameList = await m_gameRepository.GetUserGameListAsync(currentUserId);

                // Display Friends
                model.Friends = await m_userRepository.GetFriendsAsync(currentUserId);
            }

            return View("HomePage", model);
        }

        private Task<List<GameSummaryViewModel>> GetGames()
        {
            return m_gameRepository.GetFeaturedGamesAsync();
        }

        // ------------------------------------
        // Navigation methods from HomePage
        // ------------------------------------

        [HttpPost]
        public IActionResult GoToLogin()
        {
            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public IActionResult ContinueAsGuest()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult NavigateToGameLibrary()
        {
            return RedirectToAction("Index", "Library");
        }

        [HttpPost]
        public async Task<IActionResult> Logout(
            [FromServices] Microsoft.AspNetCore.Identity.SignInManager<IdentityUser> signInManager)
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper to map ASP.NET Identity → your User Id
        private int GetCurrentUserId()
        {
            var idClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            return idClaim != null ? int.Parse(idClaim.Value) : 0;
        }
    }
}
