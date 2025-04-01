using Demo.Controllers.utilities;
using Demo.Models;

namespace Demo.Controllers.Management.Authentication
{
    public class AuthManagement
    {
        private readonly CSVServices _csvService;

        public AuthManagement(CSVServices csvService)
        {
            _csvService = csvService;
        }

        private List<User> ReadUsers()
        {
            
            return _csvService.ReadUser();
        }

        public User? GetUserByEmailOrPhoneNumber(string email, string? phoneNumber)
        {
            var users = ReadUsers();
            return users.FirstOrDefault(u => u.Email == email || u.PhoneNumber == phoneNumber);
        }
        public User? GetUserById(int id)
        {
            var users = ReadUsers();
            return users.FirstOrDefault(u => u.IdUser == id);
        }
        public int GetUserCount()
        {
            var users = ReadUsers();
            return users.Count;
        }

        public void WriteUsers(User user)
        {
            _csvService.WriteUsers(user);
        }
        public void ChangeUserPassword(string email, string newPassword)
        {
            _csvService.UpdateUserPassword(email, newPassword);
        }
        

    }
}
