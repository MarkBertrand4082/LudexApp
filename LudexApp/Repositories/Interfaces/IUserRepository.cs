//Mark Bertrand
using LudexApp.Models;
using LudexApp.Models.ViewModels;

namespace LudexApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<FriendSummaryViewModel>> GetFriendsAsync(int userId);

        // Used by Login.ValidateLogin()
        Task<User?> GetUserByCredentialsAsync(string email, string password);

        // NEW: used by Register to check duplicates
        Task<User?> GetUserByEmailAsync(string email);

        // NEW: used by Register to add user
        Task<IEnumerable<User?>> GetUsersAsync();

        Task<User?> GetUserByIdAsync(int id);
        List<UserViewModel?> GetUsersByUsername(string username);
    }
}
