using IGDB.Models;
using LudexApp.Models.ViewModels;

namespace LudexApp.Repositories.Interfaces
{
    public interface IGameRepository
    {
        // Used by HomeController
        Task<IEnumerable<Game>> GetFeaturedGamesAsync();

        // Used for the Game Library (all/search)
        Task<IEnumerable<Game>> GetAllGamesAsync();

        // Simple search that returns summary VMs (you already had this)
        IEnumerable<GameSummaryViewModel> SearchGames(string name);

        GameSummaryViewModel SearchSpecificGame(string name, string dev);

        // New: used by GameController.Details
        Task<Game?> GetGameByIdAsync(long id);
    }
}
