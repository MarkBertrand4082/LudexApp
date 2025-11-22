//Mark Bertrand
using LudexApp.Models;

namespace LudexApp.Data
{
    public class DBInitializer
    {
        public static void Initialize(GameContext context)
        {
            context.Database.EnsureCreated();

        }
    }
}
