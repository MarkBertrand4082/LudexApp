// Andrew Neto
using IGDB;
using IGDB.Models;
using LudexApp.Models.ViewModels;

namespace LudexApp.Repositories.Interfaces
{
    // Pull Featured Games and User's Game List Asynchronously
    public interface IGameRepository
    {
        Task<IEnumerable<Game>> GetFeaturedGamesAsync();
    }
}
