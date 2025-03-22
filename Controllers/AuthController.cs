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


namespace Demo.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthManagement authManager;


        public AuthController(AuthManagement _authManager)
        {
            this.authManager = _authManager;
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
            if ((user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) && user.Role == "Admin")
            {
                HttpContext.Session.SetString(key: "FirstName ", user.FirstName ?? string.Empty);
                HttpContext.Session.SetString(key: "LastName", user.LastName ?? string.Empty);
                HttpContext.Session.SetString(key: "CodeUser", user.CodeUser ?? string.Empty);

                return RedirectToAction("Admin_Page");
            }
            if ((user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) && user.Role == "Student")
            {
                HttpContext.Session.SetString(key: "FirstName ", user.FirstName ?? string.Empty);
                HttpContext.Session.SetString(key: "LastName", user.LastName ?? string.Empty);
                HttpContext.Session.SetString(key: "CodeUser", user.CodeUser ?? string.Empty);
                return RedirectToAction("UserPage");
            }

            if ((user != null && BCrypt.Net.BCrypt.Verify(password, user.Password)) && user.Role == "Teacher")
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

        [HttpPost]
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
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {

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
    }
}