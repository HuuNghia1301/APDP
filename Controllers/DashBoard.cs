using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    public class DashBoard : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
