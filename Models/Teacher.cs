namespace Demo.Models
{
    public class Teacher : User, IDisplayinfor
    {
        public string display()
        {
            return "Teacher";
        }
    }

}

