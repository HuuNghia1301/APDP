namespace Demo.Models
{
    public class Teacher : User, IDisplayinfor
    {
        public TeacherController()
        {
        }

        public string display()
        {
            return "Teacher";
        }

        public string ShowInfor()
        {
            return $"{FirstName} {LastName}, Email: {Email}";
        }
    }

}

