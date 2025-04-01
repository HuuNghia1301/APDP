namespace Demo.Models
{
    public class Grade : IDisplayinfor
    {
        public int? GradeId { get; set; }
        public double? Score { get; set; }
        public int? UserId { get;  set; }  // Thêm UserId
        public int? CourseId { get; set; }  // Thêm CourseId
        public string CourseName { get; set; } // Mapping course name
        public string ShowInfor()
        {
            return "Grade";
        }
    }
}
