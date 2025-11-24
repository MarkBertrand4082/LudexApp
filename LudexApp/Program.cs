using LudexApp.Repositories.Implementation;
using LudexApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using RestEase;

namespace LudexApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //t73n320sd26wp6i0ja3bxfn8fml83k DONT DELETE
            IRestApi api = RestClient.For<IRestApi>();

            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
