
using Microsoft.AspNetCore.Mvc;

namespace EduGameMVC.Controllers
{
    public class GameController : Controller
    {
        // Displays the battle page
        public IActionResult Index()
        {
            return View();
        }
    }
}
