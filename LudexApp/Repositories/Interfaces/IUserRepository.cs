using LudexApp.Models.ViewModels;

namespace LudexApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<FriendSummaryViewModel>> GetFriendsAsync(int userId);
    }
}
