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
            var courses = _csvService.GetCourses();
            ViewBag.Courses = courses;  // Truyền dữ liệu cho View
            return View(courses);
        }
        public IActionResult GetUserInfo()
        {
            string codeUser = HttpContext.Session.GetString("CodeUser");

            if (string.IsNullOrEmpty(codeUser))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index");
            }

            User? user = _csvService.GetUserInfoByCode(codeUser);

            if (user != null)
            {
                var studentCourses = _csvService.GetCoursesNameForStudent(user.CodeUser);
                var studentGrades = _csvService.GetGradesByStudent(user.CodeUser);
                var teachers = _csvService.GetTeacherOfStudent(user.CodeUser);

                ViewBag.StudentCourses = studentCourses;
                ViewBag.StudentGrades = studentGrades;
                ViewBag.Teachers = teachers;

                return View("UserPage", user);
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng với mã này.";
                return RedirectToAction("Index");
            }
        }
        public IActionResult UserPage()
        {
            string codeUser = HttpContext.Session.GetString("CodeUser");

            if (string.IsNullOrEmpty(codeUser))
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin người dùng.";
                return RedirectToAction("Index");
            }

            User? user = _csvService.GetUserInfoByCode(codeUser);

            if (user != null)
            {
                var studentCourses = _csvService.GetCoursesNameForStudent(user.CodeUser);
                var studentGrades = _csvService.GetGradesByStudent(user.CodeUser);
                var teachers = _csvService.GetTeacherOfStudent(user.CodeUser) ?? new List<User>(); // Đảm bảo không null

                ViewBag.StudentCourses = studentCourses ?? new List<string>(); // Tránh null
                ViewBag.StudentGrades = studentGrades ?? new List<Grade>(); // Tránh null
                ViewBag.Teachers = teachers; // Đã được gán giá trị rỗng nếu null

                return View("UserPage", user);
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy người dùng với mã này.";
                return RedirectToAction("Index");
            }
        }

    }
}
