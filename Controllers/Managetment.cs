using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using static Demo.Models.User;

namespace Demo.Controllers
{
    public class Managetment : Controller
    {

        private static UserList _userList = new UserList(); // Tạo danh sách User

        public IActionResult Index()
        {
            var users = _userList.Users; // Lấy danh sách user từ UserList
            return View(users); // Truyền danh sách user ra View
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(User user)
        {
            if (_userList.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "❌ Email đã tồn tại! Vui lòng nhập email khác.");
                return View(user); // Trả lại view kèm theo lỗi
            }

            _userList.Users.Add(user);
            return RedirectToAction("Index");
        }
        public IActionResult Delete()
        {
            return View();
        }

    }
}
