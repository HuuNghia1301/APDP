using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Demo.Controllers.utilities;

namespace Demo.Controllers.Management.CrudManagement
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly CSVServices _csvServices;

        public CourseController(ILogger<CourseController> logger, CSVServices csvServices)
        {
            _logger = logger;
            _csvServices = csvServices; // Khởi tạo đối tượng CSVServices
        }

        public IActionResult Index()
        {
            var courses = _csvServices.GetCourses();  // Lấy các khóa học từ courses.csv
            ViewBag.Courses = courses;  // Truyền dữ liệu cho View
            return View(courses);
        }

        // Hiển thị trang Create (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách giáo viên từ dịch vụ
            var teachers = _csvServices.GetTeachers(); //  GetTeachers() là phương thức lấy danh sách giảng viên
            // Truyền danh sách giáo viên vào ViewData
            ViewData["Teachers"] = teachers;

            return View();
        }
        // Xử lý thêm khóa học (POST)
        [HttpPost]
        public IActionResult Create(string courseName, string Description, string StringnameTeacher) // Đổi tên biến
        {
            if (string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(Description) || string.IsNullOrEmpty(StringnameTeacher))
            {
                ModelState.AddModelError("", "Please fill in all fields.");

                ViewData["Teachers"] = _csvServices.GetTeachers();
                return View();
            }
           
            var course = new Course
            {
                courseName = courseName,
                Description = Description,
                StringnameTeacher = StringnameTeacher  // Dữ liệu sẽ đúng
            };

            _csvServices.writeCourse(course);

            return RedirectToAction("Index");
        }




        // Hiển thị trang xác nhận xóa khóa học
        [HttpGet]
        public IActionResult Delete(int courseId)
        {
            var course = _csvServices.GetCourses().FirstOrDefault(c => c.courseId == courseId);

            if (course == null)
            {
                return NotFound("Không tìm thấy khóa học có ID: " + courseId);
            }

            return View(course); // Truyền thông tin khóa học sang View
        }

        // Xử lý xóa khóa học (POST)

        [HttpPost]
        public IActionResult DeleteConfirmed(int courseId)
        {
            if (courseId == 0)
            {
                Console.WriteLine("⚠ ID không hợp lệ!");
                return BadRequest("ID không hợp lệ.");
            }

            Console.WriteLine($"🗑 Đang xóa khóa học có ID: {courseId}");
            _csvServices.DeleteCourse(courseId); // Gọi hàm xóa trong CSVServices

            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Edit(int courseId)
        {
            var course = _csvServices.GetCourses().FirstOrDefault(c => c.courseId == courseId);
            if (course == null)
            {
                return NotFound($"Không tìm thấy khóa học có ID: {courseId}");
            }
            return View(course); // Trả về View để chỉnh sửa khóa học
        }
        [HttpPost]
        public IActionResult Edit(int courseId, string courseName, string Description, string StringnameTeacher)
        {
            if (courseId == 0)
            {
                return BadRequest("ID không hợp lệ.");
            }
            if (string.IsNullOrWhiteSpace(courseName) || string.IsNullOrWhiteSpace(Description))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ thông tin.");
                return View();
            }
            _csvServices.UpdateCourse(courseId, courseName, Description, StringnameTeacher); // Gọi hàm cập nhật trong CSVServices
            return RedirectToAction("Index");

        }

    }
}
