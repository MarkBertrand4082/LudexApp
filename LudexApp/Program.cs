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


            builder.Services.AddScoped<IGameRepository, GameRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddDbContext<LudexDbContext>(
                opt => opt.UseSqlite("Data Source=ludex.db"));

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

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<LudexDbContext>();
                db.Database.EnsureCreated();
            }

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
