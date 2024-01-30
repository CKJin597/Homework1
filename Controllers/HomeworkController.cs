using Microsoft.AspNetCore.Mvc;

namespace MSIT155.Controllers
{
    public class HomeworkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
