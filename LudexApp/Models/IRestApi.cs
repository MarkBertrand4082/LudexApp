//Mark Bertrand
using IGDB.Models;
using RestEase;

namespace LudexApp.Models
{
    public interface IRestApi
    {
        [Get("whatever/{Id}")]
        Task<Game> GetGamesAsync([Path] int Id);
    }
}
