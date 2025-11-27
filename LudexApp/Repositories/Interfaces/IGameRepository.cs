// Mark Bertrand
using IGDB.Models;
using LudexApp.Models.ViewModels;

namespace LudexApp.Repositories.Interfaces
{
    public interface IGameRepository
    {
        // For Home page / Game library
        Task<IEnumerable<Game>> GetFeaturedGamesAsync();
        Task<IEnumerable<Game>> GetAllGamesAsync();
        Task<Game?> GetGameByIdAsync(long id);

        // For search (used on Game Library / others)
        IEnumerable<GameSummaryViewModel> SearchGames(string name);
        GameSummaryViewModel SearchSpecificGame(string name, string dev);
    }
}
