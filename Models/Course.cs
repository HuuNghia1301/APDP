using Demo.Controllers.utilities;

namespace Demo.Models
{
    public class Course : IDisplayinfor
    {
        public int courseId { get; set; }
        public string courseName { get; set; }
        public string Description { get; set; }
        public string StringnameTeacher { get; set; }  
       

        public string ShowInfor()
        {
            return $"Course: {courseName}, Teacher: {StringnameTeacher}";
        }
        public static List<Course> GetCoursesByTeacher(CSVServices csvServices, string teacherName)
        {
            var allCourses = csvServices.GetCourses();

            var coursesOfTeacher = allCourses
                .Where(c =>
                    !string.IsNullOrEmpty(c.StringnameTeacher) &&
                    c.StringnameTeacher.Trim().ToLower() == teacherName.Trim().ToLower())
                .ToList();

            return coursesOfTeacher;
        }


    }
}
