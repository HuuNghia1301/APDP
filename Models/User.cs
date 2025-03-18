using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Demo.Models
{
    public class User
    {
     
        public int IdUser { get;  set; }
        
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Mời nhập họ và tên")]
        public string LastName { get;  set; }
        [Required(ErrorMessage = "Mời nhập họ và tên")]
        public string Email { get;  set; }

        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Mời nhập PhoneNumber ")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Mời nhập PassWord")]
        public DateOnly DoB { get; set; }
        [Required(ErrorMessage = "Nhập tuổi")]
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
        public class UserList
        {
            private static readonly string filePath = Path.Combine(Directory.GetCurrentDirectory(), "CsvService", "User.csv");

            public List<User> Users { get; set; }

            public UserList()
            {
                Users = new List<User>();
                LoadUsersFromCSV();
            }

            public void LoadUsersFromCSV()
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine("❌ File CSV không tồn tại!");
                    return;
                }

                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines.Skip(1))
                {
                    var values = line.Split(',');

                    if (values.Length < 9) continue;

                    try
                    {
                        var user = new User
                        {
                            IdUser = int.Parse(values[0]),
                            FirstName = values[1],
                            LastName = values[2],
                            Email = values[3],
                            PhoneNumber = values[4],
                            Password = "@@@@@@",
                            DoB = DateOnly.ParseExact(values[6], "MM/dd/yyyy", CultureInfo.InvariantCulture),
                            Role = values[7],
                            CreatedAt = DateTime.ParseExact(values[8], "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture)
                        };

                        Users.Add(user);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ Lỗi khi đọc dòng: {line} - {ex.Message}");
                    }
                }
            }


           

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
