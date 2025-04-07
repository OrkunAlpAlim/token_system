using Microsoft.AspNetCore.Mvc;
using WebToken.Models;
using System.Linq;

namespace WebToken.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("/login")]
        public IActionResult Login([FromForm] string username, [FromForm] string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
                return Ok("Giriş başarılı.");
            else
                return Unauthorized("Kullanıcı adı veya şifre yanlış.");
        }

        [HttpPost("/register")]
        public IActionResult Register([FromForm] User newUser)
        {
            var exists = _context.Users.Any(u => u.Username == newUser.Username);
            if (exists)
                return Conflict("Bu kullanıcı adı zaten mevcut.");

            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Ok("Kayıt başarılı.");
        }
    }
}