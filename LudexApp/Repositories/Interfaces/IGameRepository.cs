// Andrew Neto
using LudexApp.Models.ViewModels;

namespace LudexApp.Repositories.Interfaces
{
    // Pull Featured Games and User's Game List Asynchronously
    public interface IGameRepository
    {
        Task<List<GameSummaryViewModel>> GetFeaturedGamesAsync();
        Task<List<GameSummaryViewModel>> GetUserGameListAsync(int userID);
    }
}
