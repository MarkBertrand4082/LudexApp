//Mark Bertrand
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

        // Get List of Friends for specific (logged-in) user
        
        public async Task<List<FriendSummaryViewModel>> GetFriendsAsync(int userId)
        {
            // Assuming I can grab friends f    om User Database
            List<FriendSummaryViewModel> friends = [];
            var user = await m_gameContext.Users.SingleAsync(x => x.Id == userId);
            foreach(User f in user.Friends)
            {
                friends.Add(new FriendSummaryViewModel()
                {
                    UserId = f.Id,
                    Username = f.Username,
                    SharedGamesCount = f.GameLibrary.Count
                });

            
            }
            return friends;
                // TODO: implement asynchrounous way to fill Friends
        }

        public async Task<User>? GetUserByCredentialsAsync(string email, string password)
        {
            return await m_gameContext.Users.SingleAsync(x => x.Email == email && x.Password == password);
        }

        public async Task<User>? GetUserByEmailAsync(string email)
        {
            return await m_gameContext.Users.SingleAsync(x => x.Email == email);
        }
        
    }
}
