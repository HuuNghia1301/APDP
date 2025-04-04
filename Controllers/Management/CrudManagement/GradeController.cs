using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public class GradeController : Controller
{
    private readonly ILogger<GradeController> _logger;
    private readonly CSVServices _csvServices;

    public GradeController(ILogger<GradeController> logger, CSVServices csvServices)
    {
        _logger = logger;
        _csvServices = csvServices; // Inject CSVServices
    }

    // Phương thức tạo điểm (GET)
    [HttpGet]
    public IActionResult Create()
    {
        var students = _csvServices.GetStudents();
        var courseNames = _csvServices.GetCoursesName();
        // Đảm bảo rằng courseNames không phải null
        if (courseNames == null || !courseNames.Any())
        {
            ModelState.AddModelError("", "No courses available.");
        }

        ViewData["Students"] = students;
        ViewData["CourseNames"] = courseNames;

        return View();
    }

    // Phương thức tạo điểm (POST)
    [HttpPost]
    public IActionResult Create(double Score, string CodeUserStudent, string CourseName)
    {
        if (Score <= 0 || string.IsNullOrEmpty(CodeUserStudent) || string.IsNullOrEmpty(CourseName))
        {
            ModelState.AddModelError("", "Invalid data provided.");
            return View();
        }

        var student = _csvServices.GetStudents().FirstOrDefault(s => s.CodeUser == CodeUserStudent);
        if (student == null)
        {
            ModelState.AddModelError("", "Student not found.");
            return View();
        }

        // Kiểm tra quyền của người dùng (chỉ admin hoặc teacher có thể tạo điểm)
        if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
        {
            var grade = new Grade
            {
                Score = Score,
                CodeUserStudent = CodeUserStudent,
                CourseName = CourseName,
                FirstName = User.FindFirst(ClaimTypes.GivenName)?.Value,
                LastName = User.FindFirst(ClaimTypes.Surname)?.Value
            };
            _csvServices.writeGrade(grade);
            return RedirectToAction("Index");
        }
        else
        {
            return Unauthorized("Bạn không có quyền tạo điểm.");
        }
    }

    // Phương thức sửa điểm (GET)
    [HttpGet]
    public IActionResult Edit(int gradeId)
    {
        var grade = _csvServices.GetGrades().FirstOrDefault(g => g.GradeId == gradeId);
        if (grade == null)
        {
            return NotFound("Không tìm thấy điểm.");
        }

        // Kiểm tra quyền (chỉ admin hoặc teacher có thể sửa điểm)
        if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
        {
            return View(grade);
        }
        else
        {
            return Unauthorized("Bạn không có quyền sửa điểm.");
        }
    }

    // Phương thức sửa điểm (POST)
    [HttpPost]
    public IActionResult Edit(int GradeId, double Score, string CodeUserStudent, string CourseName)
    {
        if (GradeId == 0)
        {
            return BadRequest("ID không hợp lệ.");
        }

        // Kiểm tra quyền (chỉ admin hoặc teacher có thể sửa điểm)
        if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
        {
            _csvServices.updateGrade(GradeId, Score, CodeUserStudent, CourseName);
            return RedirectToAction("Index");
        }
        else
        {
            return Unauthorized("Bạn không có quyền sửa điểm.");
        }
    }

    // Phương thức xóa điểm (GET)
    [HttpGet]
    public IActionResult Delete(int gradeId)
    {
        var grade = _csvServices.GetGrades().FirstOrDefault(g => g.GradeId == gradeId);
        if (grade == null)
        {
            return NotFound("Không tìm thấy điểm.");
        }

        // Kiểm tra quyền (chỉ admin hoặc teacher có thể xóa điểm)
        if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
        {
            return View(grade);
        }
        else
        {
            return Unauthorized("Bạn không có quyền xóa điểm.");
        }
    }

    // Phương thức xóa điểm (POST)
    [HttpPost]
    public IActionResult DeleteConfirmed(int gradeId)
    {
        if (gradeId == 0)
        {
            return BadRequest("ID không hợp lệ.");
        }

        // Kiểm tra quyền (chỉ admin hoặc teacher có thể xóa điểm)
        if (User.IsInRole("Admin") || User.IsInRole("Teacher"))
        {
            _csvServices.DeleteGrade(gradeId); // Gọi hàm xóa trong CSVServices
            return RedirectToAction("Index");
        }
        else
        {
            return Unauthorized("Bạn không có quyền xóa điểm.");
        }
    }
}
