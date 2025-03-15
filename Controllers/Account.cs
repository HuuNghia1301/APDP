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



namespace Demo.Controllers
{
    public class Account : Controller 
    {
        private readonly AddNewUser _addUser;

        public Account()
        {
            // Đường dẫn file CSV
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "CsvService", "User.csv");
            _addUser = new AddNewUser(filePath);
        }

        public IActionResult HomeMain()
        {
            return View();
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string email, string phoneNumber, string password)
        {
            var user = _addUser.GetUserByEmailOrPhoneNumber(email, phoneNumber);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                ViewBag.Message = "Login successful!";
                return RedirectToAction("HomeMain", "Account"); // Điều hướng đến Account/HomeMain
            }
            ViewBag.Message = "Invalid email or password.";
            return View();
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public IActionResult Register(string FirstName, string LastName, string Email,string PhoneNumber, string Password, DateOnly Dob, string Role)
        {
            if (_addUser.GetUserByEmailOrPhoneNumber(Email, PhoneNumber) != null)
            {
                ViewBag.Message = "Email or PhoneNumber already exists.";
                return View();
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(Password);

            var users = new User
            {
                IdUser = _addUser.GetAllUsers().Count + 1,
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PhoneNumber = PhoneNumber,
                Password = hashedPassword,
                DoB = Dob,
                Role = Role,
                CreatedAt = DateTime.Now
            };
       
            _addUser.AddUser(users);
            ViewBag.Message = "Registration successful!";
            return RedirectToAction("Login", "Account"); // Điều hướng đến Account/HomeMain
            return View();
        }
    }
}
