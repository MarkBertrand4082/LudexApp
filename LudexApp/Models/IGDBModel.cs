//Mark Bertrand
using IGDB;
using IGDB.Models;


var igdb = IGDBClient.CreateWithDefaults(
            Environment.GetEnvironmentVariable("9cm2gxrs70uz3tsepmq63txsb9grz2"),
            Environment.GetEnvironmentVariable("t73n320sd26wp6i0ja3bxfn8fml83k")
);

var games = await igdb.QueryAsync<Game>(IGDBClient.Endpoints.Games,query:"fields id, name, cover.url; where id = 1942;");

games.ToString();
