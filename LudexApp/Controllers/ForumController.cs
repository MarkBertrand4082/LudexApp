using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LudexApp.Models;
using LudexApp.Data;

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
        // View All Forums
        // -------------------------
        public IActionResult Index()
        {
            var forums = _context.Forums
                .Include(f => f.GetPosts())
                .ToList();

            return View(forums);
        }

        // -------------------------
        // View a Single Forum
        // -------------------------
        public IActionResult ViewForum(int id)
        {
            Forum? forum = _context.Forums
                .Include(f => f.posts)
                .FirstOrDefault(f => f.id == id);

            if (forum == null)
                return NotFound();

            return View(forum);
        }

        // -------------------------
        // View a Specific Post inside a Forum
        // -------------------------
        public IActionResult ViewPost(int forumId, int postId)
        {
            Post? post = _context.Posts
                .Include(p => p.forum)
                .FirstOrDefault(p => p.id == postId && p.forum.id == forumId);

            if (post == null)
                return NotFound();

            return View(post);
        }

        // -------------------------
        // Navigate to User Page (from post)
        // -------------------------
        public IActionResult ViewUser(int userId)
        {
            return RedirectToAction("Profile", "User", new { id = userId });
        }

        // -------------------------
        // Navigate to Game Page (from forum)
        // -------------------------
        public IActionResult ViewGame(int gameId)
        {
            return RedirectToAction("Details", "Game", new { id = gameId });
        }
    }
}