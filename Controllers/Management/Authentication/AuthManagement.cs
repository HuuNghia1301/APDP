using Demo.Controllers.utilities;
using Demo.Models;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Controllers.Management.Authentication
{
    public class AuthManagement
    {
        private readonly CSVServices _csvService;
        private List<User> _cachedUsers; // Bộ nhớ đệm chứa dữ liệu người dùng

        // Constructor tiêm CSVServices vào
        public AuthManagement(CSVServices csvService)
        {
            _csvService = csvService;
            _cachedUsers = _csvService.ReadUser(); // Đọc người dùng ngay khi khởi tạo và lưu vào bộ nhớ đệm
        }

        // Phương thức để lấy danh sách người dùng từ bộ nhớ đệm
        private List<User> GetUsers()
        {
            return _cachedUsers;
        }

        // Tìm người dùng theo email hoặc số điện thoại
        public User? GetUserByEmailOrPhoneNumber(string email, string? phoneNumber)
        {
            var users = GetUsers(); // Lấy người dùng từ bộ nhớ đệm
            return users.FirstOrDefault(u => u.Email == email || u.PhoneNumber == phoneNumber);
        }

        // Tìm người dùng theo ID
        public User? GetUserById(int id)
        {
            var users = GetUsers(); // Lấy người dùng từ bộ nhớ đệm
            return users.FirstOrDefault(u => u.IdUser == id);
        }

        // Lấy số lượng người dùng
        public int GetUserCount()
        {
            var users = GetUsers(); // Lấy người dùng từ bộ nhớ đệm
            return users.Count;
        }

        // Ghi người dùng mới vào tệp
        public void WriteUsers(User user)
        {
            _csvService.WriteUsers(user); // Ghi người dùng vào tệp
            _cachedUsers.Add(user); // Cập nhật lại bộ nhớ đệm
        }

        // Cập nhật mật khẩu người dùng
        public void ChangeUserPassword(string email, string newPassword)
        {
            _csvService.UpdateUserPassword(email, newPassword); // Cập nhật mật khẩu trong tệp CSV
            var user = _cachedUsers.FirstOrDefault(u => u.Email == email); // Tìm người dùng trong bộ nhớ đệm
            if (user != null)
            {
                user.Password = newPassword; // Cập nhật mật khẩu trong bộ nhớ đệm
            }
        }
    }
}
