using Demo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using BCrypt.Net;
using System.Collections.Generic;
using Microsoft.AspNetCore.SignalR;
using Demo.Controllers.Management.Authentication;
using Demo.Controllers.utilities;
using Demo.Controllers.Management;
using Demo.Controllers.Management.CrudManagement;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;


namespace Demo.Controllers
{
    public class AuthController : Controller
    {
        
        private readonly CSVServices csvService;
        private readonly AuthManagement authManager;
        private readonly Management.CrudManagement.UpdateUser updateUser;

        public AuthController(AuthManagement _authManager, UpdateUser _updateUser , CSVServices _csvService)
        {
            this.authManager = _authManager;
            this.updateUser = _updateUser;
            this.csvService = _csvService;
            
        }

        [HttpGet]
        public IActionResult UserPage()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        public User? GetUserByEmail(string email)
        {
            return authManager.GetUserByEmailOrPhoneNumber(email, null);
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string phoneNumber)
        {

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                ModelState.AddModelError("", "Vui lòng nhập đầy đủ Email và Mật khẩu.");
                return View();
            }

            var user = authManager.GetUserByEmailOrPhoneNumber(email, phoneNumber);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password) && user.Role == "Admin")
            {
                HttpContext.Session.SetString(key: "FirstName ", user.FirstName ?? string.Empty);
                HttpContext.Session.SetString(key: "LastName", user.LastName ?? string.Empty);
                HttpContext.Session.SetString(key: "CodeUser", user.CodeUser ?? string.Empty);

                return RedirectToAction("ListUser");
            }
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password) && user.Role == "Student")
            {
                HttpContext.Session.SetString(key: "FirstName ", user.FirstName ?? string.Empty);
                HttpContext.Session.SetString(key: "LastName", user.LastName ?? string.Empty);
                HttpContext.Session.SetString(key: "CodeUser", user.CodeUser ?? string.Empty);
                return RedirectToAction("UserPage");
            }

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password) && user.Role == "Teacher")
            {
                HttpContext.Session.SetString(key: "FirstName ", user.FirstName ?? string.Empty);
                HttpContext.Session.SetString(key: "LastName", user.LastName ?? string.Empty);
                HttpContext.Session.SetString(key: "CodeUser", user.CodeUser ?? string.Empty);
                return RedirectToAction("TeacherPage");
            }

            ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
            return View();
        }
        [HttpPost]
        public IActionResult Register(string FirstName, string LastName, string Address, string Email, string PhoneNumber, string Password, string Role)
        {
            var existingUser = authManager.GetUserByEmailOrPhoneNumber(Email, PhoneNumber);
            if (existingUser == null)
            {
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);
                var users = new User
                {
                    IdUser = authManager.GetUserCount() + 1,
                    FirstName = FirstName,
                    LastName = LastName,
                    Address = Address,
                    Email = Email,
                    PhoneNumber = PhoneNumber,
                    Password = hashedPassword,
                    Role = Role,
                    Created = DateTime.Now
                };
                ViewBag.Message = "Đăng ký thành công";
                authManager.WriteUsers(users);
                return RedirectToAction("Login");
            }
            ModelState.AddModelError("", "Email hoặc số điện thoại đã tồn tại.");
            return View();
        }

       public IActionResult UpdateUser(string Email , string PassWord , string newPassword = null, 
                                string newFirstName = null, string newLastName = null, 
                                string newEmail = null, string newAddress = null, string newPhoneNumber = null)
{
    var user = authManager.GetUserByEmailOrPhoneNumber(Email, null);
    if (user == null)
    {
        ModelState.AddModelError("", "Người dùng không tồn tại.");
        return View();
    }

    if (!BCrypt.Net.BCrypt.Verify(PassWord, user.Password))
    {
        ModelState.AddModelError("", "Mật khẩu cũ không đúng.");
        return View();
    }

       updateUser.ChangeUserInfor(Email, PassWord, newPassword, newFirstName, newLastName, newEmail, newAddress, newPhoneNumber);

       ViewBag.Message = "Cập nhật thông tin thành công.";
      return View();
}


        [HttpGet]
        public IActionResult UpdateUser()
        {
            return View();
        }   

        [HttpPost]
        public IActionResult ChangePassword(string email, string newPassword)
        {
            var user = authManager.GetUserByEmailOrPhoneNumber(email, null);
            if (user == null)
            {
                ModelState.AddModelError("", "Người dùng không tồn tại.");
                return View();
            }

            authManager.ChangeUserPassword(email, newPassword);
            ViewBag.Message = "Đổi mật khẩu thành công.";
            return View();
        }

        [HttpGet]
        public IActionResult ConfirmDelete(int userId)
        {
            var user = csvService.GetAllUsers().FirstOrDefault(u => u.IdUser == userId);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }
            
            return View("DeleteUser", user);
        }

        // Action thực hiện xóa user (POST)
        [HttpPost]
        public IActionResult DeleteUser(int userId)
        {
            Console.WriteLine($"🔍 Đang xóa user có ID: {userId}");
            if (!csvService.DeleteUser(userId))
            {
                return NotFound("Người dùng không tồn tại.");
            }
            // Sau khi xóa thành công, chuyển về trang danh sách người dùng
            return RedirectToAction("ListUser", "Auth");
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Admin_Page()
        {
            return View();
        }

        [HttpGet]
        public IActionResult TeacherPage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ShowUser()
        {
            return View();
        }
        [HttpGet]
        public IActionResult DeleteUser()
        {
            return View();
        }
        public IActionResult ListUser()
        {
            var users = csvService.GetAllUsers();
            return View(users);
        }
    }
}