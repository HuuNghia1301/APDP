using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class Course : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
