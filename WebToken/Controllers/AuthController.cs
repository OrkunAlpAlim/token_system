using Microsoft.AspNetCore.Mvc;
using WebToken.Services;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System;

namespace WebToken.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AuthController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        private static readonly List<UserModel> _users = new List<UserModel>
        {
            new UserModel { Name = "Test", Surname = "User", Username = "testuser", Password = "password123" },
            new UserModel { Name = "Admin", Surname = "User", Username = "admin", Password = "admin123" }
        };

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (_users.Any(u => u.Username == model.Username))
                return BadRequest("Bu kullanıcı adı zaten kullanılıyor.");

            var newUser = new UserModel
            {
                Name = model.Name,
                Surname = model.Surname,
                Username = model.Username,
                Password = model.Password
            };

            _users.Add(newUser);

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            var user = _users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            if (user == null)
                return Unauthorized("Geçersiz kullanıcı adı veya şifre.");

            var tokenResponse = _tokenService.GenerateToken(user.Username);
            var remainingTime = tokenResponse.TokenKalanSure;
            if (remainingTime < 0)
                remainingTime = 0;

            return Ok(new
            {
                Token = tokenResponse.Token,
                TokenSuresi = tokenResponse.TokenSuresi.ToString("F2"),
                TokenKalanSure = remainingTime.ToString("F2"),
                TokenGecerliligi = tokenResponse.TokenGecerliMi,
                IslemYapabilmeYetkisi = tokenResponse.IslemYapabilirMi
            });
        }

        [HttpGet("token-info")]
        public IActionResult GetTokenInfo([FromHeader] string Authorization)
        {
            if (string.IsNullOrEmpty(Authorization) || !Authorization.StartsWith("Bearer "))
                return BadRequest("Geçersiz token formatı.");

            var tokenString = Authorization.Substring(7);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(tokenString);

            var expiration = token.ValidTo;
            var issuedAt = token.ValidFrom;
            var currentTime = DateTime.UtcNow;
            var tokenSuresi = (expiration - issuedAt).TotalSeconds;
            var remainingTime = (expiration - currentTime).TotalSeconds;
            if (remainingTime < 0)
                remainingTime = 0;

            var isTokenValid = remainingTime > 0;
            var hasAccess = isTokenValid;

            return Ok(new
            {
                TokenSuresi = tokenSuresi.ToString("F2"),
                TokenKalanSure = remainingTime.ToString("F2"),
                TokenGecerliligi = isTokenValid,
                IslemYapabilmeYetkisi = hasAccess
            });
        }
    }

    public class UserModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
