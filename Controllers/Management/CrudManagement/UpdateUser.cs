using Demo.Controllers.utilities;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace Demo.Controllers.Management.CrudManagement
{
    public class UpdateUser
    {
        private readonly CSVServices _csvService;

        public UpdateUser(CSVServices csvService)
        {
            _csvService = csvService;
        }

        private List<User> ReadUsers()
        {
            return _csvService.ReadUser();
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

        public void ChangeUserInfor(string Email, string PassWord, string newPassword = null, string newFirstName = null,
                            string newLastName = null, string newEmail = null, string newAddress = null, string newPhoneNumber = null)
        {
            var users = ReadUsers();
            var user = users.FirstOrDefault(u => u.Email == Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(PassWord, user.Password))
            {
                user.FirstName = string.IsNullOrEmpty(newFirstName) ? user.FirstName : newFirstName;
                user.LastName = string.IsNullOrEmpty(newLastName) ? user.LastName : newLastName;
                user.Email = string.IsNullOrEmpty(newEmail) ? user.Email : newEmail;
                user.Address = string.IsNullOrEmpty(newAddress) ? user.Address : newAddress;
                user.PhoneNumber = string.IsNullOrEmpty(newPhoneNumber) ? user.PhoneNumber : newPhoneNumber;

                if (!string.IsNullOrEmpty(newPassword))
                {
                    user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
                }

                WriteUsers(users);
            }
        }

        private void WriteUsers(List<User> users)
        {
            _csvService.WriteUsers(users);
        }
    }
}
