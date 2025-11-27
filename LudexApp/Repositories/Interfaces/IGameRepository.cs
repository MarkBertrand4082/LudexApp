// Andrew Neto
using IGDB;
using IGDB.Models;
using LudexApp.Models.ViewModels;
using RestEase;

namespace LudexApp.Repositories.Interfaces
{
    // Pull Featured Games and User's Game List Asynchronously
    public interface IGameRepository
    {
        [Get]
        Task<IEnumerable<Game>> GetFeaturedGamesAsync();
    }
}
