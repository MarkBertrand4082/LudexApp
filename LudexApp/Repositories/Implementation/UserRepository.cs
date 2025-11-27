// Mark          Bertrand
using LudexApp.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly Data.LudexDbContext m_gameContext;

        public UserRepository(Data.LudexDbContext gameContext)
        {
            m_gameContext = gameContext;
        }

        // ----------------------------------------------------
        // Get all users (if you ever need this in admin / debug)
        // ----------------------------------------------------
        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            return await m_gameContext.Users.ToListAsync();
        }

        // ----------------------------------------------------
        // Get List of Friends for specific (logged-in) user
        // Returns FriendSummaryViewModel for Home page / sidebar
        // ----------------------------------------------------
        public async Task<List<FriendSummaryViewModel>> GetFriendsAsync(int userId)
        {
            // Load user including Friends and their GameLibrary
            var user = await m_gameContext.Users
                .Include(u => u.Friends)
                .SingleOrDefaultAsync(x => x.Id == userId);

            if (user == null || user.Friends == null || user.Friends.Count == 0)
            {
                return new List<FriendSummaryViewModel>();
            }

            var friends = new List<FriendSummaryViewModel>();

            foreach (var f in user.Friends)
            {
                if (f.Friend != null) // make sure the navigation property is loaded
                {
                    friends.Add(new FriendSummaryViewModel
                    {
                        UserId = f.Friend.Id,
                        Username = f.Friend.Username,
                        SharedGamesCount = f.Friend.GameLibrary?.Count ?? 0
                    });
                }
            }

            return friends;
        }

        // ----------------------------------------------------
        // Look up user by email + password (for login)
        // Returns null if not found; DOES NOT throw
        // ----------------------------------------------------
        public async Task<User?> GetUserByCredentialsAsync(string email, string password)
        {
            return await m_gameContext.Users
                .SingleOrDefaultAsync(x => x.Email == email && x.Password == password);
        }

        // ----------------------------------------------------
        // Look up user by email (e.g., for registration checks)
        // Returns null if not found
        // ----------------------------------------------------
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await m_gameContext.Users
                .SingleOrDefaultAsync(x => x.Email == email);
        }

        // ----------------------------------------------------
        // Look up user by Id (for profile, etc.)
        // Returns null if not found
        // ----------------------------------------------------
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await m_gameContext.Users
                .SingleOrDefaultAsync(x => x.Id == id);
        }
    }
}
