using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Demo.Models;
using CsvHelper;
using System.Globalization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace Demo.Controllers.utilities
{
    public class CSVServices : DatabaseService, IReadWriteUser
    {
        private readonly string _csvFilePath;
        private readonly string _courseCsvFilePath;
        private readonly string _gradeCsvFilePath;  
        public CSVServices(string csvFilePath, string courseCsvFilePath, string gradeCsvFilePath) : base(csvFilePath)
        {
            _csvFilePath = csvFilePath;
            _courseCsvFilePath = courseCsvFilePath;
            _gradeCsvFilePath = gradeCsvFilePath;


        }

        public List<User> GetAllUsers()
        {
            if (!File.Exists(_csvFilePath)) return new List<User>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<User>().ToList();
        }
           public List<Grade> GetAllGrade()
             {
        if (!File.Exists(_gradeCsvFilePath)) return new List<Grade>();  // Kiểm tra tệp tồn tại

        using var reader = new StreamReader(_gradeCsvFilePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.Configuration.HeaderValidated = null;  // Vô hiệu hóa kiểm tra header
        csv.Context.Configuration.MissingFieldFound = null;  // Vô hiệu hóa kiểm tra trường thiếu

        return csv.GetRecords<Grade>().ToList();  // Đọc và trả về danh sách Grade
          }





        public void WriteUsers(User user)
        {
            var users = GetAllUsers();
            user.IdUser = users.Count > 0 ? users.Max(u => u.IdUser) + 1 : 1;
            user.CodeUser = CodeUser();
            user.Created = DateTime.Now;
            users.Add(user);
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(users);
        }

        public string CodeUser()
        {
            var users = GetAllUsers();
            HashSet<string> existingCodes = new HashSet<string>(users.Select(u => u.CodeUser ?? string.Empty));

            Random random = new Random();
            string codename;
            do
            {
                int num = random.Next(1000, 9999);
                codename = "BH" + num;
            } while (existingCodes.Contains(codename)); // Lặp lại nếu mã đã tồn tại

            return codename;
        }

        public List<User> ReadUser()
        {
            if (!File.Exists(_csvFilePath)) return new List<User>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<User>().ToList();
        }

        public void WriteUser(List<User> users)
        {
            var existingUsers = GetAllUsers();
            foreach (var user in users)
            {
                user.IdUser = existingUsers.Count > 0 ? existingUsers.Max(u => u.IdUser) + 1 : 1;
                user.CodeUser = CodeUser();
                user.Created = DateTime.Now;
                existingUsers.Add(user);
            }
            using var writer = new StreamWriter(_csvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(existingUsers);
        }

        public User? GetUserByEmailOrPhoneNumber(string email, string phoneNumber)
        {
            var users = ReadUser();
            return users.FirstOrDefault(u => u.Email == email || u.PhoneNumber == phoneNumber);
        }

        List<string[]> IReadWriteUser.ReadUser()
        {
            if (!File.Exists(_csvFilePath)) return new List<string[]>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<dynamic>()
               .Select(record => ((IDictionary<string, object>)record)
               .Values
               .Select(v => v?.ToString() ?? string.Empty)
               .ToArray())
               .ToList();
        }
        public void UpdateUserPassword(string email, string newPassword)
        {
            if (!File.Exists(_csvFilePath)) return;

            var tempFile = Path.GetTempFileName();
            using (var reader = new StreamReader(_csvFilePath))
            using (var writer = new StreamWriter(tempFile))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var users = csvReader.GetRecords<User>().ToList();
                csvWriter.WriteHeader<User>();
                csvWriter.NextRecord();

                foreach (var user in users)
                {
                    if (user.Email == email)
                    {
                        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                    }
                    csvWriter.WriteRecord(user);
                    csvWriter.NextRecord();
                }
            }

            File.Delete(_csvFilePath);
            File.Move(tempFile, _csvFilePath);
        }
        public void UpdateCourse(int id, string courseName, string Description)
        {
            if (!File.Exists(_courseCsvFilePath))
            {           
                return;
            }
            var tempFile = Path.GetTempFileName();
            bool isUpdated = false;

            using (var reader = new StreamReader(_courseCsvFilePath))
            using (var writer = new StreamWriter(tempFile))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var courses = csvReader.GetRecords<Course>().ToList();
                csvWriter.WriteHeader<Course>();
                csvWriter.NextRecord();

                foreach (var course in courses)
                {
                    if (course.courseId == id)
                    {
                        course.courseName = courseName;
                        course.Description = Description;
                        isUpdated = true;
                    }
                    csvWriter.WriteRecord(course);
                    csvWriter.NextRecord();
                }
                writer.Flush();
            }
            File.Delete(_courseCsvFilePath); // Xóa file cũ
            File.Move(tempFile, _courseCsvFilePath); // Thay thế bằng file mới

        }

        public List<Course> GetCourses()
        {
            if (!File.Exists(_courseCsvFilePath)) return new List<Course>();

            using var reader = new StreamReader(_courseCsvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Course>().ToList();
        }
        public void writeCourse(Course course)
        {
            var existingCourses = GetCourses();

            // Tự động tăng courseId
            course.courseId = existingCourses.Count > 0
                ? existingCourses.Max(c => c.courseId) + 1
                : 1;

            existingCourses.Add(course);

            using var writer = new StreamWriter(_courseCsvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(existingCourses);
        }
        public void DeleteCourse(int courseId)
        {
            var existingCourses = GetCourses();
            var courseToDelete = existingCourses.FirstOrDefault(c => c.courseId == courseId);

            if (courseToDelete == null)
            {
                Console.WriteLine($"Không tìm thấy khóa học có ID: {courseId}");
                return;
            }

            existingCourses.RemoveAll(c => c.courseId == courseId);
            Console.WriteLine($"Danh sách sau khi xóa: {existingCourses.Count} khóa học");

            try
            {
                using var writer = new StreamWriter(_courseCsvFilePath, false);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(existingCourses);
                writer.Flush(); // Đảm bảo dữ liệu được ghi ngay
                Console.WriteLine($"Đã xóa khóa học có ID: {courseId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi ghi file: {ex.Message}");
            }
        }

        #region Các phương thức xử lý Grade
        // Đọc tất cả Grade từ file CSV
        public List<Grade> GetAllGrades()
        {
            if (!File.Exists(_gradeCsvFilePath)) return new List<Grade>();
            using var reader = new StreamReader(_gradeCsvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var grades = csv.GetRecords<Grade>().ToList();
            var courses = GetCourses();

            foreach (var grade in grades)
            {
                grade.CourseName = courses.FirstOrDefault(c => c.courseId == grade.CourseId)?.courseName ?? "Unknown";
            }
            return grades;
        }

        // Thêm mới Grade vào file CSV
        public void AddGrade(Grade grade)
        {
            //var grades = GetAllGrades();
            //grade.GradeId = grades.Count > 0 ? grades.Max(g => g.GradeId) + 1 : 1;
            //grades.Add(grade);

            //// Ghi dữ liệu vào file CSV
            //WriteGradesToFile(grades);
            var grades = GetAllGrades();
            grade.GradeId = grades.Count > 0 ? grades.Max(g => g.GradeId) + 1 : 1;
            grades.Add(grade);

            using var writer = new StreamWriter(_gradeCsvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(grades);
        }

        // Cập nhật Grade dựa trên GradeId
        public bool UpdateGrade(int gradeId, Grade updatedGrade)
        {
            var grades = GetAllGrades();
            var grade = grades.FirstOrDefault(g => g.GradeId == gradeId);

            if (grade == null)
                return false;

            // Cập nhật thông tin
            grade.Score = updatedGrade.Score;
            grade.UserId = updatedGrade.UserId;
            grade.CourseId = updatedGrade.CourseId;

            // Ghi lại file CSV
            WriteGradesToFile(grades);
            return true;
        }

        // Xóa Grade theo GradeId
        public bool DeleteGradeById(int gradeId)  // Đổi tham số sang int
        {
            var grades = GetAllGrades();
            var gradeToDelete = grades.FirstOrDefault(g => g.GradeId == gradeId);

            if (gradeToDelete == null)
                return false;

            grades.Remove(gradeToDelete);

            // Ghi lại file CSV sau khi xóa
            WriteGradesToFile(grades);
            return true;
        }


        // Hàm ghi lại danh sách Grade vào file CSV
        private void WriteGradesToFile(List<Grade> grades)
        {
            using var writer = new StreamWriter(_gradeCsvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(grades);
        }

        

        internal void UpdateGrade(int gradeId, int studentId, int courseId, double score)
        {
            throw new NotImplementedException();
        }
        public string GetCourseNameById(int courseId)
        {
            var courses = GetCourses();
            return courses.FirstOrDefault(c => c.courseId == courseId)?.courseName ?? "Unknown";
        }

        #endregion

    }
}