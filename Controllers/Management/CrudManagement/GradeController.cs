using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers.Management.CrudManagement
{
    public class GradeController : Controller
    {
        private readonly ILogger<GradeController> _logger;
        private readonly CSVServices _csvServices;

        public GradeController(ILogger<GradeController> logger, CSVServices csvServices)
        {
            _logger = logger;
            _csvServices = csvServices; // Inject CSVServices
        }
        public IActionResult Index()
        {
            var grades = _csvServices.GetGrades();  // Lấy các khóa học từ courses.csv
            ViewBag.Grade = grades; // Truyền dữ liệu cho View
            return View(grades);
        }
        [HttpGet]
        // Hiển thị trang Create (GET)
        public IActionResult Create()
        {
            // Lấy danh sách sinh viên và danh sách môn học
            var student = _csvServices.GetStudents();
            var courseName = _csvServices.GetCoursesName(); // Lấy danh sách tên môn học

            // Đảm bảo rằng courseName không phải null
            if (courseName == null || !courseName.Any())
            {
                ModelState.AddModelError("", "No courses available.");
            }

            // Truyền danh sách sinh viên và môn học vào ViewData
            ViewData["Student"] = student;
            ViewData["CourseName"] = courseName;

            return View();
        }

        // Xử lý thêm khóa học (POST)
        [HttpPost]
        public IActionResult Create(double Score, string CodeUserStudent, string CourseName)
        {
            if (double.IsNaN(Score) || string.IsNullOrEmpty(CodeUserStudent) || string.IsNullOrEmpty(CourseName))
            {
                return View();
            }

            // Tạo đối tượng Grade và lưu vào
            var grade = new Grade
            {
                //GradeId = GradeId,
                Score = Score,
                CodeUserStudent = CodeUserStudent,
                CourseName = CourseName
            };

            _csvServices.writeGrade(grade);

            return RedirectToAction("Index");
        }
        // Hiển thị trang xác nhận xóa khóa học
        [HttpGet]
        public IActionResult Delete(int gradeId)
        {
            var course = _csvServices.GetGrades().FirstOrDefault(c => c.GradeId == gradeId);

            if (course == null)
            {
                return NotFound("Không tìm thấy khóa học có ID: " + gradeId);
            }

            return View(course); // Truyền thông tin khóa học sang View
        }
        [HttpPost]
        public IActionResult DeleteConfirmed(int gradeId)
        {
            if (gradeId == 0)
            {
                Console.WriteLine("⚠ ID không hợp lệ!");
                return BadRequest("ID không hợp lệ.");
            }

            Console.WriteLine($"🗑 Đang xóa khóa học có ID: {gradeId}");
            _csvServices.DeleteGrade(gradeId); // Gọi hàm xóa trong CSVServices

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int gradeId)
        {
            var course = _csvServices.GetGrades().FirstOrDefault(c => c.GradeId == gradeId);
            if (course == null)
            {
                return NotFound($"Không tìm thấy khóa học có ID: {gradeId}");
            }
            return View(course); // Trả về View để chỉnh sửa khóa học
        }
        [HttpPost]
        public IActionResult Edit(int GradeId, double Score, string CodeUserStudent, string CourseName)
        {
            if (GradeId == 0)
            {
                return BadRequest("ID không hợp lệ.");
            }
            _csvServices.updateGrade(GradeId, Score, CodeUserStudent, CourseName);
            return RedirectToAction("Index");
        }



    }
}
