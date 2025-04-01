using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers.Management.CrudManagement
{
    public class StudentController : Controller
    {
        private readonly CSVServices _csvService;

        public StudentController(CSVServices csvService)
        {
            _csvService = csvService;
        }

        // Hàm tìm thông tin user qua CodeUser
        public IActionResult GetUserInfo()
        {
            // Lấy CodeUser từ session
            string codeUser = HttpContext.Session.GetString("CodeUser");

            // Nếu không có CodeUser trong session, trả về thông báo lỗi
            if (string.IsNullOrEmpty(codeUser))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index");
            }

            // Lấy thông tin người dùng từ service
            User? user = _csvService.GetUserInfoByCode(codeUser);

            if (user != null)
            {
                // Nếu tìm thấy người dùng, trả về thông tin người dùng qua view
                return View(user);
            }
            else
            {
                // Nếu không tìm thấy, thông báo lỗi
                TempData["ErrorMessage"] = "Không tìm thấy người dùng với mã này.";
                return RedirectToAction("Index");
            }
        }

        // Index có thể để trống hoặc chỉ trả về một view chung
        public IActionResult Index()
        {
            return View();
        }
    }
}
