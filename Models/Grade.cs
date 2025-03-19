namespace Demo.Models
{
    public class Grade: IDisplayinfor
    {
        public string GradeId { get; set; }
        public string StudentID { get; set; }
        public string courseID { get; set; }
        public string ShowInfor()
        {
            return "Grade";
        }
    }
}
