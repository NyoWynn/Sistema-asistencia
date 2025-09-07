using Microsoft.AspNetCore.Mvc;
using SistemaAsistencia.Models;
using System.Linq;

namespace SistemaAsistencia.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                
                HttpContext.Session.SetInt32("UserID", user.Id);
                HttpContext.Session.SetString("UserName", user.Name);
                HttpContext.Session.SetString("IsAdmin", user.IsAdmin.ToString());

                return user.IsAdmin ? RedirectToAction("Index", "Admin") : RedirectToAction("Index", "Attendance");
            }

            ViewBag.Error = "Correo o contraseña inválidos.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}