using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Demo.Controllers.Management.CrudManagement
{
    public class GradeController : Controller
    {
        private readonly ILogger<GradeController> _logger;
        private readonly CSVServices _csvServices;

        // Constructor: inject logger and CSV service
        public GradeController(ILogger<GradeController> logger, CSVServices csvServices)
        {
            _logger = logger;
            _csvServices = csvServices;
        }

        // Display list of grades
        public IActionResult Index()
        {
            var grades = _csvServices.GetGrades();
            ViewBag.Grade = grades;
            return View(grades);
        }

        // Show create grade page (GET)
        [HttpGet]
        public IActionResult Create()
        {
            var students = _csvServices.GetStudents();
            var courseNames = _csvServices.GetCoursesName();

            if (courseNames == null || !courseNames.Any())
            {
                ModelState.AddModelError("", "No courses available.");
            }

            ViewData["Student"] = students;
            ViewData["CourseName"] = courseNames;

            return View();
        }

        // Handle create grade (POST)
        [HttpPost]
        public IActionResult Create(double Score, string CodeUserStudent, string CourseName)
        {
            if (double.IsNaN(Score) || string.IsNullOrEmpty(CodeUserStudent) || string.IsNullOrEmpty(CourseName))
            {
                return View();
            }

            var grade = new Grade
            {
                Score = Score,
                CodeUserStudent = CodeUserStudent,
                CourseName = CourseName
            };

            _csvServices.writeGrade(grade);

            return RedirectToAction("Index");
        }

        // Show delete confirmation page (GET)
        [HttpGet]
        public IActionResult Delete(int gradeId)
        {
            var grade = _csvServices.GetGrades().FirstOrDefault(g => g.GradeId == gradeId);

            if (grade == null)
            {
                return NotFound("Grade not found with ID: " + gradeId);
            }

            return View(grade);
        }

        // Handle delete confirmation (POST)
        [HttpPost]
        public IActionResult DeleteConfirmed(int gradeId)
        {
            if (gradeId == 0)
            {
                Console.WriteLine("Invalid ID!");
                return BadRequest("Invalid ID.");
            }

            Console.WriteLine($"Deleting grade with ID: {gradeId}");
            _csvServices.DeleteGrade(gradeId);

            return RedirectToAction("Index");
        }

        // Show edit grade page (GET)
        [HttpGet]
        public IActionResult Edit(int gradeId)
        {
            var grade = _csvServices.GetGrades().FirstOrDefault(g => g.GradeId == gradeId);

            if (grade == null)
            {
                return NotFound($"Grade not found with ID: {gradeId}");
            }

            return View(grade);
        }

        // Handle edit grade (POST)
        [HttpPost]
        public IActionResult Edit(int GradeId, double Score, string CodeUserStudent, string CourseName)
        {
            if (GradeId == 0)
            {
                return BadRequest("Invalid ID.");
            }

            _csvServices.updateGrade(GradeId, Score, CodeUserStudent, CourseName);
            return RedirectToAction("Index");
        }
    }
}
