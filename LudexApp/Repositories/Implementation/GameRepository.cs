//Mark Bertrand
using IGDB;
using IGDB.Models;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using System.Reflection.Metadata.Ecma335;
namespace LudexApp.Repositories.Implementation
{
    public class GameRepository : IGameRepository
    {
        public static IGDBClient Igdb { get {return IGDBClient.CreateWithDefaults("9cm2gxrs70uz3tsepmq63txsb9grz2", "t73n320sd26wp6i0ja3bxfn8fml83k"); } }
        public Task<IEnumerable<Game>> Games { get { return GetGamesAsync(); } }
        public GameRepository()
        {
        }

        public IGDBClient GetIgdb() { return Igdb; }
        public async Task<IEnumerable<Game>> GetFeaturedGamesAsync()
        {
            //Gotta change interace type to Game


            return await Igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields id, name, cover.url");
        }


        private static async Task<IEnumerable<Game>> GetGamesAsync()
        {

            return await Igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games, query: "fields id, name, cover.url, involved_companies;");
        }

        public IEnumerable<GameSummaryViewModel> SearchGames(string name)
        {
            IEnumerable<Game> searchResult =  from game in Games.Result 
                   where game.Name.Contains(name)
                   select game;

            List<GameSummaryViewModel> gameSummaries = new List<GameSummaryViewModel>();

            foreach (Game g in searchResult)
            {
                gameSummaries.Add(new GameSummaryViewModel { Title = g.Name, GameId = (int)g.Id, AverageRating = g.AggregatedRating });
            }

            return gameSummaries;
        }

        public GameSummaryViewModel SearchSpecificGame(string name, string dev)
        {
            var result = (from game in Games.Result
                          where game.Name.Contains(name)
                          where game.InvolvedCompanies.Values.SingleOrDefault(item => item.Developer == true && item.Company.Value.Name.Contains(dev)) != null
                          select game).ToList()[0];

            return new GameSummaryViewModel() { Title = result.Name, Platform = result.Platforms.Values[0].Name };
        }
    }
}
