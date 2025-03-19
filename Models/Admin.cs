namespace Demo.Models
{
    public class Admin : User,IDisplayinfor
    {
        public string display()
        {
            return "Admin";
        }

    }
}
