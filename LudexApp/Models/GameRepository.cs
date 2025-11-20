using IGDB;
using IGDB.Models;
using LudexApp.Models.ViewModels;
namespace LudexApp.Models
{
    public class GameRepository : IGameRepository
    {
        public IGDBClient Igdb { get; }
        public Task<IEnumerable<Game>> Games { get { return GetGamesAsync(Igdb); } }
        public GameRepository()
        {
            Igdb = IGDBClient.CreateWithDefaults(
            Environment.GetEnvironmentVariable("9cm2gxrs70uz3tsepmq63txsb9grz2"),
            Environment.GetEnvironmentVariable("t73n320sd26wp6i0ja3bxfn8fml83k")
            );
        }

        private static async Task<IEnumerable<Game>> GetGamesAsync(IGDBClient igdb)
        {
            

            return await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields id, name, cover.url;");
        }

        public IEnumerable<Game> SearchGames(string name)
        {
            return from game in Games.Result 
                   where game.Name.Contains(name)
                   select game;
        }
    }
}
