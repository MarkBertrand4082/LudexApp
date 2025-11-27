using IGDB;
using LudexApp.Data;
using LudexApp.Repositories.Implementation;
using LudexApp.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace LudexApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //t73n320sd26wp6i0ja3bxfn8fml83k DONT DELETE

            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<IGDBClient>(_ => IGDB.IGDBClient.CreateWithDefaults(
                "9cm2gxrs70uz3tsepmq63txsb9grz2", // client id
                "t73n320sd26wp6i0ja3bxfn8fml83k"  // client secret
                )
            );

            // builder.Services.AddHttpClient("game")
                // .ConfigureHttpClient(x => x.BaseAddress = new Uri("https://api.igdb.com/v4/games"));

            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var connstring = builder.Configuration.GetConnectionString("connString")
                ?? throw new InvalidOperationException("Connection String 'connString' not found.");

            builder.Services.AddDbContext<LudexDbContext>(
                opt => opt.UseSqlServer(connstring));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/User/Login";
                    options.LogoutPath = "/User/Logout";
                    options.AccessDeniedPath = "/User/Login";
                    options.Cookie.Name = "LudexAuth";
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}
