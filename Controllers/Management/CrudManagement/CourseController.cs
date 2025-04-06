using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Demo.Controllers.utilities;
using Demo.Controllers.Management.Authentication;

namespace Demo.Controllers.Management.CrudManagement
{
    public class CourseController : Controller
    {
        private readonly ILogger<CourseController> _logger;
        private readonly CSVServices _csvServices;

        // Constructor with dependency injection
        // Lớp CourseController nhận logger và CSVServices qua DI (Dependency Injection)
        private CourseController(ILogger<CourseController> logger, CSVServices csvServices)
        {
            _logger = logger;
            _csvServices = csvServices;
        }

        // Default constructor with CSV path initialization
        // Constructor mặc định thiết lập các đường dẫn CSV cho người dùng, khóa học và điểm số
        public CourseController()
        {
            string userCsvPath = "Data/users.csv";
            string courseCsvPath = "Data/courses.csv";
            string gradeCsvPath = "Data/grades.csv";
            // Khởi tạo dịch vụ CSV với các đường dẫn file
            _csvServices = CSVServices.GetInstance(userCsvPath, courseCsvPath, gradeCsvPath);
        }

        // Show list of courses
        // Hiển thị danh sách các khóa học từ file CSV
        public IActionResult Index()
        {
            var courses = _csvServices.GetCourses();
            ViewBag.Courses = courses;  // Truyền dữ liệu khóa học vào view
            return View(courses);
        }

        // Display the Create page (GET)
        // Hiển thị trang tạo khóa học (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Lấy danh sách giảng viên từ dịch vụ CSV
            var teachers = _csvServices.GetTeachers();
            // Truyền danh sách giảng viên vào View
            ViewData["Teachers"] = teachers;

            return View();
        }

        // Handle course creation (POST)
        // Xử lý việc tạo khóa học (POST)
        [HttpPost]
        public IActionResult Create(string courseName, string Description, string StringnameTeacher)
        {
            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (!IsValidCourseInput(courseName, Description, StringnameTeacher))
            {
                ModelState.AddModelError("", "Please fill in all fields.");
                ViewData["Teachers"] = _csvServices.GetTeachers();
                return View();
            }

            var course = new Course
            {
                courseName = courseName,
                Description = Description,
                StringnameTeacher = StringnameTeacher
            };

            // Ghi khóa học vào file CSV
            _csvServices.writeCourse(course);

            return RedirectToAction("Index");
        }

        // Helper method to validate course input
        // Phương thức phụ để kiểm tra tính hợp lệ của dữ liệu đầu vào
        private bool IsValidCourseInput(string courseName, string description, string teacherName)
        {
            return !string.IsNullOrWhiteSpace(courseName) && 
                   !string.IsNullOrWhiteSpace(description) && 
                   !string.IsNullOrEmpty(teacherName);
        }

        // Show confirmation page for deleting a course
        // Hiển thị trang xác nhận xóa khóa học
        [HttpGet]
        public IActionResult Delete(int courseId)
        {
            var course = GetCourseById(courseId);

            // Nếu không tìm thấy khóa học, trả về lỗi NotFound
            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found.");
            }

            return View(course); // Trả về view xác nhận xóa khóa học
        }

        // Handle course deletion (POST)
        // Xử lý việc xóa khóa học (POST)
        [HttpPost]
        public IActionResult DeleteConfirmed(int courseId)
        {
            // Kiểm tra tính hợp lệ của courseId trước khi xóa
            if (courseId == 0)
            {
                return BadRequest("Invalid ID.");
            }

            // Gọi phương thức xóa khóa học từ dịch vụ CSV
            _csvServices.DeleteCourse(courseId);

            return RedirectToAction("Index");
        }

        // Helper method to get a course by ID
        // Phương thức phụ để tìm khóa học theo ID
        private Course GetCourseById(int courseId)
        {
            return _csvServices.GetCourses().FirstOrDefault(c => c.courseId == courseId);
        }

        // Show edit page (GET)
        // Hiển thị trang chỉnh sửa khóa học (GET)
        [HttpGet]
        public IActionResult Edit(int courseId)
        {
            var course = GetCourseById(courseId);

            // Kiểm tra nếu không tìm thấy khóa học
            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found.");
            }

            // Lấy danh sách giảng viên để chỉnh sửa
            var teachers = _csvServices.GetTeachers();
            ViewData["Teachers"] = teachers;

            return View(course); // Trả về view chỉnh sửa khóa học
        }

        // Handle course editing (POST)
        // Xử lý việc chỉnh sửa khóa học (POST)
        [HttpPost]
        public IActionResult Edit(int courseId, string courseName, string Description, string StringnameTeacher)
        {
            // Kiểm tra tính hợp lệ của courseId
            if (courseId == 0)
            {
                return BadRequest("Invalid ID.");
            }

            // Kiểm tra tính hợp lệ của dữ liệu đầu vào
            if (!IsValidCourseInput(courseName, Description, StringnameTeacher))
            {
                ModelState.AddModelError("", "Please enter all required information.");
                return View();
            }

            // Gọi phương thức cập nhật khóa học
            _csvServices.UpdateCourse(courseId, courseName, Description, StringnameTeacher);
            return RedirectToAction("Index");
        }
    }
}
