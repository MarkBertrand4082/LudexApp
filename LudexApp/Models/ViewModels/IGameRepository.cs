using IGDB;
using IGDB.Models;
namespace LudexApp.Models.ViewModels
{
    public interface IGameRepository
    {
        public IGDBClient Igdb { get;}
        public Task<IEnumerable<Game>> Games { get; }
    }
}
