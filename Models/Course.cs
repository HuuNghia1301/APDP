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
            return "Course";
        }
    }
}
