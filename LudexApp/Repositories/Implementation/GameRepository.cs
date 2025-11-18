using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LudexApp.Data;
using LudexApp.Models.ViewModels;
using LudexApp.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LudexApp.Repositories.Implementation
{
    public class GameRepository : IGameRepository
    {
        private readonly GameContext m_gameContext;

        public GameRepository(GameContext _context)
        {
            _context = m_gameContext;
        }

        public async Task<List<GameSummaryViewModel>> GetFeaturedGamesAsync()
        {
            // Assuming database of Games, where each game has Id, Title, Platform, and Review Navigation
            return await m_gameContext.Games;
                // TODO: actual implementation of Game Database
        }

        public async Task<List<GameSummaryViewModel>> GetUserGameListAsync(int userId)
        {
            // Assuming I have a way to link User's Games with Database
            return await m_gameContext.UserGames;
                // TODO: asynchronously add User's Games to list
        }
    }
}
