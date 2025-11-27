using LudexApp.Data;
using LudexApp.Models;
using LudexApp.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LudexApp.Controllers
{
    public class ForumController : Controller
    {
        private readonly LudexDbContext _context;
        public ForumController(LudexDbContext context) => _context = context;

        // -------------------------
        // View a specific forum
        // -------------------------
        public IActionResult ViewForum(int id)
        {
            var forum = _context.Forums
                .Include(f => f.Posts)
                    .ThenInclude(p => p.User)
                .Include(f => f.Game)
                .FirstOrDefault(f => f.Id == id);

            if (forum == null) return NotFound();

            var forumVM = new ForumViewModel
            {
                Id = forum.Id,
                Name = forum.Name,
                GameId = forum.GameId,
                GameTitle = forum.Game?.Name ?? "",
                Posts = forum.Posts.Select(p => new PostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    ForumId = p.ForumId,
                    UserId = p.UserId // just store the user ID now
                }).ToList()
            };

            ViewBag.IsLoggedIn = User.Identity?.IsAuthenticated ?? false;

            return View(forumVM);
        }

        // -------------------------
        // Create a post
        // -------------------------
        [HttpPost]
        public IActionResult CreatePost(int forumId, string title, string content)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var post = new Post
            {
                ForumId = forumId,
                Title = title,
                Content = content,
                UserId = userId
            };

            _context.Posts.Add(post);
            _context.SaveChanges();

            return RedirectToAction("ViewForum", new { id = forumId });
        }

        // -------------------------
        // Delete a post (only owner)
        // -------------------------
        [HttpPost]
        public IActionResult DeletePost(int postId)
        {
            if (!User.Identity.IsAuthenticated) return Unauthorized();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
            if (post == null) return NotFound();
            if (post.UserId != userId) return Forbid();

            int forumId = post.ForumId;
            _context.Posts.Remove(post);
            _context.SaveChanges();

            return RedirectToAction("ViewForum", new { id = forumId });
        }

        // -------------------------
        // Navigate to Game page
        // -------------------------
        public IActionResult ViewGame(int gameId)
        {
            return RedirectToAction("Details", "Game", new { id = gameId });
        }
    }
}