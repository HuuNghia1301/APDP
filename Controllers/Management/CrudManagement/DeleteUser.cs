using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers.Management.CrudManagement
{
    public class DeleteUser
    {
        private readonly CSVServices _csvService;
        public DeleteUser(CSVServices csvService)
        {
            _csvService = csvService;
        }

        public List<User> ReadUsers()
        {
            return _csvService.ReadUser();
        }

        public bool DeleteUserById(int userId)
        {
            var users = ReadUsers();

            Console.WriteLine("Danh sách user trước khi xóa:");
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.IdUser}, Email: {user.Email}");
            }

            var userToDelete = users.FirstOrDefault(u => u.IdUser == userId);

            if (userToDelete == null)
                return false;

            users.Remove(userToDelete);

            // Cập nhật lại ID
            for (int i = 0; i < users.Count; i++)
            {
                users[i].IdUser = i + 1;
            }

            _csvService.WriteUsers(users);

            Console.WriteLine("Danh sách user sau khi xóa:");
            foreach (var user in users)
            {
                Console.WriteLine($"ID: {user.IdUser}, Email: {user.Email}");
            }

            return true;
        }

        //public void WriteUsers(User user)
        //{
        //    _csvService.WriteUsers(user);
        //}

        public void ReadUsers(User user)
        {
            return;
        }

        //private void WriteUsers(List<User> users)
        //{
        //    _csvService.WriteUsers(users);
        //}
    }
}
