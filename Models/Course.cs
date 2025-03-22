namespace Demo.Models
{
    public class Course : IDisplayinfor
    {
        string? courseId { get; set; }
        string? courseName { get; set; }
        string? Description { get; set; }
    public string ShowInfor()
        {
            return "Course";
        }
    }
}