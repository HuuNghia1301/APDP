using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Demo.Controllers.Management.Authentication;

namespace Demo.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthManagement _authManager;

        public AuthController()
        {
            _authManager = new AuthManagement();
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ Email và Mật khẩu.");
                return View();
            }

            bool isValid = _authManager.Login(model.Email, model.Password);

            if (isValid)
            {
                HttpContext.Session.SetString("email", model.Email);
             
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
            return View(model);
        }
    }
}
