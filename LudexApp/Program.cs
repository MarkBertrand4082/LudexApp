namespace LudexApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //t73n320sd26wp6i0ja3bxfn8fml83k DONT DELETE
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}
