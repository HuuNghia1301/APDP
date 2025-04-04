using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Collections.Generic;

public class TeacherController : Controller
{
    private readonly CSVServices csvService;

    public TeacherController(CSVServices _csvService)
    {
        this.csvService = _csvService;
    }

    public IActionResult Index()
    {
        var grades = csvService.GetGrades();
        var courses = csvService.GetCourses();
        var students = csvService.GetStudents();
        ViewBag.Courses = courses;  // Truyền dữ liệu cho View
        return View(courses);
    }

    // Giảng viên xem danh sách sinh viên và điểm của họ trong môn học
    [HttpGet]
    public IActionResult ViewStudentsInCourse(string teacherCode)
    {
        // Lấy thông tin giảng viên từ mã giảng viên
        var teacher = csvService.GetUserInfoByCode(teacherCode);
        if (teacher == null || teacher.Role != "Teacher")
        {
            return Unauthorized("Bạn không có quyền truy cập.");
        }

        // Lấy danh sách môn học mà giảng viên này giảng dạy
        var courses = csvService.GetCourses().Where(c => c.StringnameTeacher == teacherCode).ToList();

        var studentGrades = new List<Grade>(); // Đúng loại dữ liệu ở đây là Grade

        foreach (var course in courses)
        {
            // Lấy danh sách điểm của sinh viên trong môn học
            var grades = csvService.GetGrades().Where(g => g.CourseName == course.courseName).ToList();
            foreach (var grade in grades)
            {
                // Lấy thông tin sinh viên từ mã sinh viên
                var student = csvService.GetUserInfoByCode(grade.CodeUserStudent);
                if (student != null)
                {
                    var gradeItem = new Grade
                    {
                      
                        Score = grade.Score,
                        CourseName = grade.CourseName,
                        CodeUserStudent = grade.CodeUserStudent
                    };
                    studentGrades.Add(gradeItem);
                }
            }
        }
        return View(studentGrades);
    }
}
