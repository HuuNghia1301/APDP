using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Demo.Controllers.utilities;

namespace Demo.Controllers.Management.CrudManagement
{
    public class GradeController : Controller
    {
        private readonly ILogger<GradeController> _logger;
        private readonly CSVServices _csvServices;

        public GradeController(ILogger<GradeController> logger, CSVServices csvServices)
        {
            _logger = logger;
            _csvServices = csvServices;
        }

        public IActionResult Index()
        {
            var grades = _csvServices.GetAllGrades(); // Sửa GetAllGrade() thành GetAllGrades()
            ViewBag.Grades = grades;
            return View(grades);
            
        }

        // Hiển thị trang Create (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý thêm điểm (POST)
        [HttpPost]
        public IActionResult Create(int studentId, int courseId, double score)
        {
            //if (studentId == 0 || courseId == 0 || score < 0)
            //{
            //    ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin hợp lệ.");
            //    return View();
            //}

            //var grade = new Grade
            //{
            //    UserId = studentId,
            //    CourseId = courseId,
            //    Score = score,
            //    CourseName = "Course Name", 
            //    GradeId = _csvServices.GetAllGrades().Count + 1 

            //};

            //_csvServices.AddGrade(grade); // Ghi dữ liệu vào file CSV

            //return RedirectToAction("Index");
            Console.WriteLine($"StudentId: {studentId}, CourseId: {courseId}, Score: {score}");
            if (studentId == 0 || courseId == 0 || score < 0)
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin hợp lệ.");
                return View();
            }

            var grade = new Grade { Score = score, UserId = studentId, CourseId = courseId };
            _csvServices.AddGrade(grade);

            return RedirectToAction("Index");
        }

        // Hiển thị trang xác nhận xóa điểm
        [HttpGet]
        public IActionResult Delete(int gradeId)
        {
            var grade = _csvServices.GetAllGrades().FirstOrDefault(g => g.GradeId == gradeId);

            if (grade == null)
            {
                return NotFound($"Không tìm thấy điểm có ID: {gradeId}");
            }

            return View(grade);
        }

        // Xử lý xóa điểm (POST)
        [HttpPost]
        public IActionResult DeleteConfirmed(int gradeId)
        {
            if (gradeId == 0)
            {
                return BadRequest("ID không hợp lệ.");
            }

            bool deleted = _csvServices.DeleteGradeById(gradeId);

            if (!deleted)
            {
                return NotFound($"Không tìm thấy điểm có ID: {gradeId}");
            }

            return RedirectToAction("Index");
        }

        // Hiển thị trang chỉnh sửa điểm
        [HttpGet]
        public IActionResult Edit(int gradeId)
        {
            var grade = _csvServices.GetAllGrades().FirstOrDefault(g => g.GradeId == gradeId);
            if (grade == null)
            {
                return NotFound($"Không tìm thấy điểm có ID: {gradeId}");
            }
            return View(grade);
        }

        // Xử lý cập nhật điểm (POST)
        [HttpPost]
        public IActionResult Edit(int gradeId, int studentId, int courseId, double score)
        {
            if (gradeId == 0 || studentId == 0 || courseId == 0 || score < 0)
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin hợp lệ.");
                return View();
            }

            var updatedGrade = new Grade
            {
                GradeId = gradeId,
                UserId = studentId,
                CourseId = courseId,
                Score = score
            };

            bool updated = _csvServices.UpdateGrade(gradeId, updatedGrade);

            if (!updated)
            {
                return NotFound($"Không tìm thấy điểm có ID: {gradeId} để cập nhật.");
            }

            return RedirectToAction("Index");
        }
    }
}
