using Demo.Controllers.utilities;
using System.Text;

namespace Demo.Models
{
    public class Grade : IDisplayinfor
    {
        public int? GradeId { get; set; }
        public double? Score { get; set; }
        public string? CodeUserStudent { get; set; }
        public string? CourseName { get; set; }

        public string ShowInfor()
        {
            return "Grade";
        }
        public string GetStudentGrades(CSVServices CsvServices, string codeUserStudent)
        {
            var grades = CsvServices.GetGrades();
            var studentGrades = grades.Where(g => g.CodeUserStudent == codeUserStudent).ToList();

            if (studentGrades.Any())
            {
                var result = new StringBuilder();
                foreach (var grade in studentGrades)
                {
                    var course = CsvServices.GetCourses().FirstOrDefault(c => c.courseName == grade.CourseName)?.courseName ?? "không có tên";
                    result.AppendLine($"Môn học: {course} - Điểm: {grade.Score}");
                }
                return result.ToString();
            }
            else
            {
                return "Không có điểm nào cho sinh viên này.";
            }
        }
       
    }
}

