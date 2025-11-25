//Mark Bertrand
namespace LudexApp.Data
{
    public class DBInitializer
    {
        public static void Initialize(LudexDbContext context)
        {
            context.Database.EnsureCreated();

        }
    }
}
