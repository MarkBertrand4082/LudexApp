//Mark Bertrand
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
            return await m_gameContext.Friends;
            m_gameContext.Users.
                // TODO: implement asynchrounous way to fill Friends
        }
        
    }
}
