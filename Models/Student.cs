namespace Demo.Models
{
    public class Student : User, IDisplayinfor
    {
        public string display()
        {
            return "Student";
        }
    }

}
