using LudexApp.Data;
using LudexApp.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LudexApp.Controllers
{
    public class ForumController : Controller
    {
        private readonly LudexDbContext _context;
        private readonly IGameRepository _gameRepository;

        public ForumController(LudexDbContext context, IGameRepository gameRepository)
        {
            _context = context;
            _gameRepository = gameRepository;
        }

        // -------------------------
        // View a forum
        // -------------------------
        [HttpGet]
        public IActionResult Forum(int id)
        {
            var forum = _context.Forums
                .Include(f => f.Posts)
                .FirstOrDefault(f => f.Id == id);

            if (forum == null) return NotFound();

            var forumVM = new ForumViewModel
            {
                Id = forum.Id,
                Name = forum.Name,
                GameId = forum.GameId,
                GameTitle = "", // Will fetch from IGDB
                Posts = forum.Posts.Select(p => new PostViewModel
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    ForumId = p.ForumId,
                    UserId = p.UserId,
                    CanDelete = User.Identity.IsAuthenticated &&
                                User.FindFirstValue(ClaimTypes.NameIdentifier) == p.UserId.ToString()
                }).ToList()
            };

            // Fetch game title from IGDB
            var game = _gameRepository.GetGameByIdAsync(forum.GameId).Result;
            forumVM.GameTitle = game?.Name ?? "";

            ViewBag.IsLoggedIn = User.Identity?.IsAuthenticated ?? false;

            return View("Forum", forumVM);
        }

        // -------------------------
        // View a single post
        // -------------------------
        [HttpGet]
        public IActionResult Post(int forumId, int postId)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == postId && p.ForumId == forumId);
            if (post == null) return NotFound();

            var postVM = new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                ForumId = forumId,
                UserId = post.UserId,
                CanDelete = User.Identity.IsAuthenticated &&
                            User.FindFirstValue(ClaimTypes.NameIdentifier) == post.UserId.ToString()
            };

            return View("Post", postVM);
        }

        // -------------------------
        // Create a post
        // -------------------------
        [HttpPost]
        [Authorize]
        public IActionResult CreatePost(int forumId, string title, string content)
        {
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

            return RedirectToAction("Post", new { forumId = forumId, postId = post.Id });
        }

        // -------------------------
        // Delete a post (owner only)
        // -------------------------
        [HttpPost]
        [Authorize]
        public IActionResult DeletePost(int postId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var post = _context.Posts.FirstOrDefault(p => p.Id == postId);
            if (post == null) return NotFound();
            if (post.UserId != userId) return Forbid();

            int forumId = post.ForumId;

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return RedirectToAction("Forum", new { id = forumId });
        }

        // -------------------------
        // Create forum for a game
        // -------------------------
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateForumForGame(int gameId)
        {
            var existingForum = _context.Forums.FirstOrDefault(f => f.GameId == gameId);
            if (existingForum != null)
                return RedirectToAction("Forum", new { id = existingForum.Id });

            var igdbGame = await _gameRepository.GetGameByIdAsync(gameId);
            if (igdbGame == null) return NotFound();

            var forum = new Forum
            {
                GameId = gameId,
                Name = $"{igdbGame.Name} Forum"
            };

            _context.Forums.Add(forum);
            _context.SaveChanges();

            return RedirectToAction("Forum", new { id = forum.Id });
        }

        // -------------------------
        // Navigate to game page
        // -------------------------
        [HttpGet]
        public IActionResult ViewGame(int gameId)
        {
            return RedirectToAction("Details", "Game", new { id = gameId });
        }
    }
}