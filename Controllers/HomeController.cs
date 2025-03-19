using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Demo.Models;
using Demo.Controllers.utilities;
using System.Collections.Generic;
using System.IO;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CSVServices _csvService;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;

            // ✅ Đảm bảo file CSV có đường dẫn chính xác
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "User.csv");
            _csvService = new CSVServices(filePath);

            // Debug: Kiểm tra file tồn tại
            if (!System.IO.File.Exists(filePath))
            {
                _logger.LogError($"[ERROR] Không tìm thấy file CSV tại: {filePath}");
            }
        }

        public IActionResult Index()
        {
            // Đọc danh sách user từ file CSV
            List<User> users = _csvService.ReadUsers();

            // ✅ Debug: In danh sách user ra log
            _logger.LogInformation($"[DEBUG] Tổng số user đọc được: {users.Count}");
            foreach (var user in users)
            {
                _logger.LogInformation($"User: {user.Email} - {user.Role}");
            }

            return View(users); // Truyền dữ liệu sang View
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
