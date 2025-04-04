namespace Demo.Models
{
    public class Grade : IDisplayinfor
    {
        public int? GradeId { get; set; }
        public double? Score { get; set; }
        public string? CodeUserStudent { get; set; }
        public string? CourseName { get; set; }
        public User? User { get; set; } 

        public string ShowInfor()
        {
            return $"Grade for {CodeUserStudent} in {CourseName}: {Score} - {User.FirstName} {User.LastName}";
        }
    }
}
