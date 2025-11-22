using LudexApp.Repositories.Implementation;
using LudexApp.Repositories.Interfaces;
using RestEase;

namespace LudexApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //t73n320sd26wp6i0ja3bxfn8fml83k DONT DELETE
            IRestApi api = RestClient.For<IRestApi>();
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var app = builder.Build();

            app.MapDefaultControllerRoute();

            app.Run();
        }
    }
}
