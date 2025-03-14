using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class User
    {
     
        public int IdUser { get;  set; }

        public string FirstName { get; set; }

        public string LastName { get;  set; }

        public string Email { get;  set; }

        public string PhoneNumber { get; set; }

        public string Password { get; set; }

        public DateOnly DoB { get; set; }

        public string Role { get; set; }

        public DateTime CreatedAt { get; set; }

        public User(){}

        public User(int id, string firstName, string lastName, string email, string phoneNumber, string password, DateOnly dob, string role, DateTime createdAt)
        {
            IdUser = id;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            DoB = dob;
            Role = role; 
            CreatedAt = createdAt;
        }

        public int GetId()
        {
            return IdUser;
        }

        public string GetFirstName()
        {
            return FirstName;
        }

        public string GetLastName()
        {
            return LastName;
        }

        public string GetPassword()
        {
            return Password;
        }
        public string GetEmail()
        {
            return Email;
        }
        public string GetPhoneNumber()
        {
            return PhoneNumber;
        }
        public DateOnly GetDoB()
        {
            return DoB;
        }
        public string GetRole()
        {
            return Role;
        }
        public DateTime GetCreatedAt()
        {
            return CreatedAt;
        }
    }
}
