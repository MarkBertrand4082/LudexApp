using LudexApp.Models;
using LudexApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LudexApp.Controllers
{
    public class ForumController : Controller
    {
        private readonly LudexDbContext _context;

        public ForumController(LudexDbContext context)
        {
            _context = context;
        }

        // -------------------------
        // View a Specific Forum
        // -------------------------
        public IActionResult ViewForum(int id)
        {
            var forum = _context.Forums
                .Include(f => f.Posts)
                    .ThenInclude(p => p.User)
                .Include(f => f.Game)
                .FirstOrDefault(f => f.Id == id);

            if (forum == null)
                return NotFound();

            ViewBag.IsLoggedIn = User.Identity?.IsAuthenticated ?? false;

            return View(forum);
        }

        // -------------------------
        // View a Specific Post
        // -------------------------
        public IActionResult ViewPost(int forumId, int postId)
        {
            var post = _context.Posts
                .Include(p => p.Forum)
                .Include(p => p.User)
                .FirstOrDefault(p => p.Id == postId && p.ForumId == forumId);

            if (post == null)
                return NotFound();

            return View(post);
        }

        // -------------------------
        // Create a Post
        // -------------------------
        [HttpPost]
        public IActionResult CreatePost(int forumId, string title, string content)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

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
        // Delete a Post (User-Owned Only)
        // -------------------------
        [HttpPost]
        public IActionResult DeletePost(int postId)
        {
            if (!User.Identity.IsAuthenticated)
                return Unauthorized();

            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var post = _context.Posts.FirstOrDefault(p => p.Id == postId);

            if (post == null)
                return NotFound();

            if (post.UserId != userId)
                return Forbid(); // prevent deleting other people's posts

            int forumId = post.ForumId;

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return RedirectToAction("ViewForum", new { id = forumId });
        }

        // -------------------------
        // Navigate Back to Game Page
        // -------------------------
        public IActionResult ViewGame(int gameId)
        {
            return RedirectToAction("Details", "Game", new { id = gameId });
        }
    }
}