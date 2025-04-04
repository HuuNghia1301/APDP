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

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewGrades()
        {
            
            // Lấy mã người dùng từ session
            string? codeUser = HttpContext.Session.GetString("CodeUser");
            string firstName = HttpContext.Session.GetString("FirstName");
            string lastName = HttpContext.Session.GetString("LastName");

            Console.WriteLine($"Id User : {codeUser}");

            // Kiểm tra nếu session không có giá trị
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                firstName = "Người dùng";
                lastName = "";

            }
            // Truyền thông tin vào View
            ViewBag.FirstName = firstName;
            ViewBag.LastName = lastName;
            ViewBag.CodeUser = codeUser;

            if (string.IsNullOrEmpty(codeUser))
            {
                TempData["Error"] = "Không tìm thấy mã sinh viên trong phiên.";
                return RedirectToAction("Index");
            }

            // Gọi hàm GetStudentGrades trong class Grade
            var grade = new Grade();
            string studentGrades = grade.GetStudentGrades(_csvService, codeUser);

            ViewBag.Grades = studentGrades;

            return View(); // ViewGrades.cshtml
        }


    }
}
