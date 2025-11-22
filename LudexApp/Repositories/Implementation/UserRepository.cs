using LudexApp.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {

        private readonly GameContext m_gameContext;
        public UserRepository(GameContext gameContext)
        {
            m_gameContext = gameContext;
        }

        // Get List of Friends for specific (logged-in) user
        public async Task<List<FriendSummaryViewModel>> GetFriendsAsync(int userId)
        {
            // Assuming I can grab friends from User Database
            return await m_gameContext.Friends;
                // TODO: implement asynchrounous way to fill Friends
        }
    }
}
