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
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                HttpContext.Session.SetString("UserEmail", user.Email);
                return RedirectToAction("HomeMain", "Account");
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
                    IdUser = authManager.GetUserCount() + 1, // Changed line
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

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}