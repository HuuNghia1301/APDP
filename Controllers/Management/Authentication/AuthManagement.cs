using Demo.Controllers.utilities;

namespace Demo.Controllers.Management.Authentication
{
    public class AuthManagement
    {
        private readonly CSVServices _csvService;

        public AuthManagement()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "User.csv");
            Console.WriteLine($"[DEBUG] Đường dẫn file CSV: {filePath}");
            _csvService = new CSVServices(filePath);
        }
        public bool Login(string email, string password)
        {
            // Đọc danh sách người dùng từ CSV
            var users = _csvService.ReadUsers();

            // Kiểm tra xem có user nào trùng email và password không
            return users.Any(u => u.Email.Trim() == email.Trim() && u.Password.Trim() == password.Trim());

        }
    }
}
