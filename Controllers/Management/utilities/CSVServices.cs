using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Demo.Models;
using CsvHelper;
using System.Globalization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Demo.Controllers.Management.CrudManagement;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Diagnostics;

namespace Demo.Controllers.utilities
{
    public class CSVServices : DatabaseService, IReadWriteUser
    {
        private static CSVServices _instance;
        private readonly string _csvFilePath;
        private readonly string _courseCsvFilePath;
        private readonly string _gradeCsvFilePath;
        private CSVServices(string csvFilePath, string courseCsvFilePath, string gradeCsvFilePath) : base(csvFilePath)
        {
            _csvFilePath = csvFilePath;
            _courseCsvFilePath = courseCsvFilePath;
            _gradeCsvFilePath = gradeCsvFilePath;
        }

        public static CSVServices GetInstance(string csvFilePath, string courseCsvFilePath, string gradeCsvFilePath)
        {
            if (_instance == null)
            {
                _instance = new CSVServices(csvFilePath, courseCsvFilePath, gradeCsvFilePath);
            }
            return _instance;
        }
        public string GetCSVFilePath()
        {
            return _csvFilePath;
        }
        public string GetCourseCSVFilePath()
        {
            return _courseCsvFilePath;
        }
        public string GetGradeCSVFilePath()
        {
            return _gradeCsvFilePath;
        }


        public List<User> GetAllUsers()
        {
            if (!File.Exists(_csvFilePath)) return new List<User>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<User>().ToList();
        }
        public List<User> ReadUser()
        {
            if (!File.Exists(_csvFilePath)) return new List<User>();

            using var reader = new StreamReader(_csvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<User>().ToList();
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
        public void UpdateCourse(int id, string courseName, string Description, string selectedTeacherId)
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
                        course.StringnameTeacher = selectedTeacherId;
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

        public void updateGrade(int GradeId , double Score, string CodeUserStudent , string CourseName)
        {
            if (!File.Exists(_gradeCsvFilePath))
            {
                return;
            }
            var tempFile = Path.GetTempFileName();
            bool isUpdated = false;
            using (var reader = new StreamReader(_gradeCsvFilePath))
            using (var writer = new StreamWriter(tempFile))
            using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
            using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                var grades = csvReader.GetRecords<Grade>().ToList();
                csvWriter.WriteHeader<Grade>();
                csvWriter.NextRecord();

                foreach (var grade in grades)
                {
                    if ( grade.GradeId == GradeId)
                    {
                        grade.CodeUserStudent = CodeUserStudent;
                        grade.Score = Score;
                        grade.CourseName = CourseName;
                        isUpdated = true;
                    }
                    csvWriter.WriteRecord(grade);
                    csvWriter.NextRecord();
                }
                writer.Flush();
            }
            File.Delete(_gradeCsvFilePath); //
            File.Move(tempFile, _gradeCsvFilePath); 
        }


        // Usage in GetCourses method
        public List<Course> GetCourses()
        {
            if (!File.Exists(_courseCsvFilePath)) return new List<Course>();
            using var reader = new StreamReader(_courseCsvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Course>().ToList();
        }

        
        public  List<Grade> GetGrades()
        {
            if (!File.Exists(_gradeCsvFilePath)) return new List<Grade>();
            using var reader = new StreamReader(_gradeCsvFilePath);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            return csv.GetRecords<Grade>().ToList();
        }
        public List<Grade> GetGradesByGradeName(string gradeName)
        {
            var allGrades = GetGrades(); // Lấy tất cả các grade
            var filteredGrades = allGrades.Where(g => g.CourseName.Equals(gradeName, StringComparison.OrdinalIgnoreCase)).ToList();
            return filteredGrades;
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
        public void WriteGrade(Grade grade)
        {
            var existingGrades = GetGrades();
            // Tự động tăng gradeId
            grade.GradeId = existingGrades.Count > 0
                ? existingGrades.Max(c => c.GradeId) + 1
                : 1;
            existingGrades.Add(grade);
            using var writer = new StreamWriter(_gradeCsvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(existingGrades);
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

        public void UpdateUser(string Email , string PassWord , string newPassword = null, string newFirstName = null,
                       string newLastName = null, string newEmail = null, string newAddress = null, string newPhoneNumber = null)
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
                    if (user.Email == Email && BCrypt.Net.BCrypt.Verify(PassWord, user.Password)) 
                    {
                        user.FirstName = newFirstName;
                        user.LastName = newLastName;
                        user.Email = newEmail;
                        user.Address = newAddress;
                        user.PhoneNumber = newPhoneNumber;
                        user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                       
                    }

                    csvWriter.WriteRecord(user);
                    csvWriter.NextRecord();
                }
            }
            File.Delete(_csvFilePath);
            File.Move(tempFile, _csvFilePath);
        }
        public void DeleteUser(int IdUser)
        {
            if (IdUser <= 0)
            {
                Console.WriteLine(" ID không hợp lệ!");
                return;
            }

            var existingUsers = GetAllUsers();
            var userToDelete = existingUsers.FirstOrDefault(c => c.IdUser == IdUser);

            if (userToDelete == null)
            {
                Console.WriteLine($"Không tìm thấy người dùng có ID: {IdUser}");
                return;
            }

            existingUsers.RemoveAll(c => c.IdUser == IdUser);
            Console.WriteLine($"Danh sách sau khi xóa: {existingUsers.Count} người dùng");

            try
            {
                using var writer = new StreamWriter(_csvFilePath, false);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(existingUsers);
                writer.Flush(); // Đảm bảo dữ liệu được ghi ngay
                Console.WriteLine($" Đã xóa người dùng có ID: {IdUser}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Lỗi khi ghi file: {ex.Message}");
            }
        }

        public void WriteUsers(List<User> users)
        {
            using (var writer = new StreamWriter(_csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(users);
            }
        }
        public User? GetUserInfoByCode(string codeUser)
        {
            // Lấy tất cả người dùng từ CSV
            var users = GetAllUsers();

            // Tìm kiếm người dùng theo CodeUser
            return users.FirstOrDefault(u => u.CodeUser == codeUser);
        }

        // Phương thức lọc người dùng có vai trò "Teacher"
        public List<User> GetTeachers()
        {
            var users = GetAllUsers(); // Lấy tất cả người dùng từ file CSV
            return FilterTeachers(users); // Lọc người dùng có Role là "Teacher"
        }

        // Phương thức lọc người dùng có Role là "Teacher"
        private List<User> FilterTeachers(List<User> users)
        {
            return users.Where(u => u.Role == "Teacher").ToList();
        }

        public List<User> GetTeacherOfStudent(string codeUser)
        {
            var users = GetAllUsers() ?? new List<User>();
            var courses = GetCourses() ?? new List<Course>();
            var studentCourses = GetCoursesNameForStudent(codeUser) ?? new List<string>(); 

            if (!studentCourses.Any())
            {
                return new List<User>(); // Trả về danh sách rỗng nếu không có khóa học
            }

            // Lấy danh sách tên giáo viên từ khóa học
            var teacherNames = studentCourses.Select(c => c).Distinct().ToList();

            // Lọc danh sách giáo viên theo tên
            return users.Where(u => u.Role == "Teacher" && teacherNames.Contains(u.FirstName + " " + u.LastName)).ToList();
        }





        public List<User> GetStudents()
        {
            try
            {
                var users = GetAllUsers(); // Lấy danh sách người dùng từ file CSV
                return users.Where(u => u.Role == "Student").ToList(); // Lọc người dùng có Role là "Student"
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc thông báo lỗi nếu có sự cố trong việc lấy danh sách người dùng
                Console.WriteLine($"Error getting students: {ex.Message}");
                return new List<User>(); // Trả về danh sách trống nếu có lỗi
            }
        }

        public List<Grade> GetGradesByStudent(string codeUser)
        {
            var grades = GetGrades(); // Lấy danh sách điểm từ file CSV
            return grades.Where(g => g.CodeUserStudent == codeUser).ToList(); // Lọc điểm theo mã sinh viên
        }

        public List<string> GetCoursesName()
        {
            var courses = GetCourses();
            if (courses == null || !courses.Any())
            {
                return new List<string>(); // Nếu không có môn học, trả về danh sách rỗng
            }
            return courses.Select(u => u.courseName).ToList(); // Trả về danh sách tên môn học
        }

        public List<string> GetCoursesNameForStudent(string codeUser)
        {
            var courses = GetCourses();
            if (courses == null || !courses.Any())
            {
                return new List<string>(); 
            }
            return courses.Where(c => c.StringnameTeacher == codeUser).Select(u => u.courseName).ToList(); 
        }

        public void writeGrade(Grade grade)
        {
            var existingGrades = GetGrades();
            var student = GetAllUsers().FirstOrDefault(u => u.CodeUser == grade.CodeUserStudent);

           

            grade.GradeId = existingGrades.Count > 0 ? existingGrades.Max(c => c.GradeId) + 1 : 1;
            existingGrades.Add(grade);

            using var writer = new StreamWriter(_gradeCsvFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteRecords(existingGrades);
        }


        public void DeleteGrade(int gradeId)
        {
            var existingCourses = GetGrades();
            var courseToDelete = existingCourses.FirstOrDefault(c => c.GradeId == gradeId);

            if (courseToDelete == null)
            {
                return;
            }
            existingCourses.RemoveAll(c => c.GradeId == gradeId);        
            try
            {
                using var writer = new StreamWriter(_gradeCsvFilePath, false);
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
                csv.WriteRecords(existingCourses);
                writer.Flush(); // Đảm bảo dữ liệu được ghi ngay
            
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi ghi file: {ex.Message}");
            }
        }
        public void updateGrade(double score, string codeUserStudent, string courseName)
        {
            var grades = GetGrades();
            var grade = grades.FirstOrDefault(g => g.CodeUserStudent == codeUserStudent && g.CourseName == courseName);
            if (grade != null)
            {
                grade.Score = score;
                // Sau đó ghi lại file CSV nếu cần
                WriteGrade(grade);
            }
        }


    }
}