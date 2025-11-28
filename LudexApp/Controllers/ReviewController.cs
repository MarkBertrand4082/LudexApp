// Andrew Neto
using LudexApp.Data;
using LudexApp.Models;
using LudexApp.Models.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LudexApp.Controllers
{
    public class ReviewController : Controller
    {
        private readonly LudexDbContext _context;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(LudexDbContext context, ILogger<ReviewController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Review display for Game of gameId, passed from GameController
        [HttpGet]
        public async Task<IActionResult> Display(int gameId, string? gameTitle)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)                 
                .Where(r => r.GameId == gameId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var currentUserId = GetCurrentUserId();

            var model = new ReviewDisplayViewModel
            {
                GameId = gameId,
                GameTitle = gameTitle ?? "Game",     
                ReviewCount = reviews.Count,
                AverageRating = reviews.Any()
                    ? reviews.Average(r => r.Rating)
                    : 0,
                Reviews = reviews.Select(r => new ReviewItemViewModel
                {
                    ReviewId = r.ReviewId,
                    Username = r.User != null ? r.User.Username : "Unknown",
                    Rating = r.Rating,
                    Content = r.Content,
                    CreatedAt = r.CreatedAt,
                    CanDelete = currentUserId.HasValue && currentUserId.Value == r.UserId
                }).ToList()
            };

            return View("Reviews", model); // Views/Review/Reviews.cshtml
        }

        [HttpGet]
        [Authorize] // must be logged in
        public IActionResult Post(int gameId, string gameTitle)
        {
            var model = new ReviewCreateViewModel
            {
                GameId = gameId,
                GameTitle = gameTitle
            };

            return View("PostReview", model); // Views/Review/PostReview.cshtml
        }

        // Posting a Review
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Post(ReviewCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("PostReview", model);
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                // not logged? –-> send to login
                return RedirectToAction("Login", "User",
                    new { returnUrl = Url.Action("Post", "Review", new { gameId = model.GameId }) });
            }

            var review = new Review
            {
                GameId = model.GameId,
                UserId = currentUserId.Value,
                Rating = model.Rating,
                Content = model.Content,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} posted review {ReviewId} for game {GameId}",
                currentUserId.Value, review.ReviewId, model.GameId);

            return RedirectToAction("Display", new { gameId = model.GameId });
        }

        // Deleting a Review. Checks to make sure the user is the same user as the one who created the Review
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int gameId)
        {
            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return Forbid();
            }

            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            if (review.UserId != currentUserId.Value)
            {
                return Forbid();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} deleted review {ReviewId}", currentUserId.Value, id);

            return RedirectToAction("Display", new { gameId });
        }

        // Gets user ID of the user viewing the page
        private int? GetCurrentUserId()
        {
            var idString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(idString, out var id) ? id : null;
        }
    }
}
