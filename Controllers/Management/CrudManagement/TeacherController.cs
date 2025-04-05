using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;

public class TeacherController : Controller
{
    private readonly CSVServices _csvService;

    public TeacherController(CSVServices _csvService)
    {
        this._csvService = _csvService;
    }

    public IActionResult Index()
    {
        var grades = _csvService.GetGrades();  
        ViewBag.Grade = grades; 
        return View(grades);
    }
    public IActionResult ViewCourse()
    {
        // Lấy họ tên từ session
        string? firstName = HttpContext.Session.GetString("FirstName")?.Trim();
        string? lastName = HttpContext.Session.GetString("LastName")?.Trim();

        // Ghép lại thành tên đầy đủ giáo viên
        string? teacherName = $"{firstName} {lastName}".Trim();

        // Kiểm tra nếu thiếu thông tin
        if (string.IsNullOrEmpty(teacherName) || string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            TempData["Error"] = "Không tìm thấy tên giáo viên trong phiên.";
            return RedirectToAction("Index", "Home");
        }

        // Gọi hàm lấy danh sách môn của giáo viên
        var courses = Course.GetCoursesByTeacher(_csvService, teacherName);

        // Truyền thông tin sang view
        ViewBag.TeacherName = teacherName;

        return View(courses); 
    }
    // Xử lý thêm khóa học (POST)
    public IActionResult Create()
    {
        // Lấy danh sách sinh viên và danh sách môn học
        var student = _csvService.GetStudents();
        var courseName = _csvService.GetCoursesName(); // Lấy danh sách tên môn học

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

        _csvService.writeGrade(grade);

        return RedirectToAction("Index");
    }
    // Hiển thị trang Edit (GET)
    [HttpGet]
    public IActionResult Edit(int gradeId)
    {
        var grade = _csvService.GetGrades().FirstOrDefault(g => g.GradeId == gradeId);
        if (grade == null)
        {
            TempData["Error"] = $"Không tìm thấy điểm với ID: {gradeId}";
            return RedirectToAction("Index");
        }

        return View(grade); // Trả về thông tin điểm cần sửa
    }
    // Xử lý cập nhật điểm (POST)
    [HttpPost]
    public IActionResult Edit(int GradeId, double Score, string CodeUserStudent, string CourseName)
    {
        if (GradeId == 0)
        {
            TempData["Error"] = "ID không hợp lệ.";
            return RedirectToAction("Index");
        }

        // Cập nhật điểm trong hệ thống
        _csvService.updateGrade(GradeId, Score, CodeUserStudent, CourseName);

        TempData["Success"] = "Điểm đã được cập nhật thành công!";
        return RedirectToAction("Index"); // Quay lại trang danh sách điểm
    }
    // Hiển thị trang xác nhận xóa điểm (GET)
    [HttpGet]
    public IActionResult Delete(int gradeId)
    {
        var grade = _csvService.GetGrades().FirstOrDefault(g => g.GradeId == gradeId);
        if (grade == null)
        {
            TempData["Error"] = $"Không tìm thấy điểm với ID: {gradeId}";
            return RedirectToAction("Index");
        }

        return View(grade); // Trả về thông tin điểm cần xóa
    }
    // Xử lý xóa điểm (POST)
    [HttpPost]
    public IActionResult DeleteConfirmed(int gradeId)
    {
        if (gradeId == 0)
        {
            TempData["Error"] = "ID không hợp lệ.";
            return RedirectToAction("Index");
        }

        // Xóa điểm khỏi hệ thống
        _csvService.DeleteGrade(gradeId);

        TempData["Success"] = "Điểm đã được xóa thành công!";
        return RedirectToAction("Index"); // Quay lại trang danh sách điểm
    }
    public IActionResult StudentsByCourse(string courseName)
    {
        if (string.IsNullOrEmpty(courseName))
        {
            TempData["Error"] = "Không xác định được môn học.";
            return RedirectToAction("ViewCourse");
        }

        // Lấy toàn bộ điểm
        var grades = _csvService.GetGrades();

        // Lọc danh sách điểm theo tên môn học
        var filteredGrades = grades.Where(g => g.CourseName == courseName).ToList();

        // Lấy thông tin sinh viên
        var students = _csvService.GetStudents();

        // Gộp thông tin sinh viên + điểm
        var studentWithGrades = from g in filteredGrades
                                join s in students on g.CodeUserStudent equals s.CodeUser
                                select new
                                {
                                    Student = s,
                                    g.Score,
                                    g.GradeId 
                                };


        ViewBag.CourseName = courseName;
        return View(studentWithGrades);
    }

    [HttpPost]
    public IActionResult EditGradeforTeacher(int GradeId, double Score, string CodeUserStudent, string CourseName)
    {
        Console.WriteLine($"📥 GradeId: {GradeId}, Score: {Score}, User: {CodeUserStudent}, Course: {CourseName}");

        if (string.IsNullOrEmpty(CodeUserStudent))
        {
            return BadRequest("Không tìm thấy người dùng");
        }

        _csvService.updateGrade(GradeId, Score, CodeUserStudent, CourseName);

        return RedirectToAction("StudentsByCourse", "Teacher"); 
    }
    [HttpPost]
    public IActionResult ChangePasswordForTeacher(string email, string newPassword)
    {
        var user = _csvService.GetUserByEmailOrPhoneNumber(email, null);
        if (user == null)
        {
            ModelState.AddModelError("", "Người dùng không tồn tại.");
            return View();
        }

        _csvService.UpdateUserPassword(email, newPassword);
        ViewBag.Message = "Đổi mật khẩu thành công.";
        return View();
    }
    [HttpGet]
    public IActionResult ChangePasswordForTeacher()
    {
        return View();
    }

}
