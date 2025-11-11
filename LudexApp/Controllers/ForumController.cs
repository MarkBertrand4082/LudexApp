using Microsoft.AspNetCore.Mvc;

namespace LudexApp.Controllers
{
    public class ForumController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
