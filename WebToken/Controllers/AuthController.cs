using Microsoft.AspNetCore.Mvc;
using WebToken.Services;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace WebToken.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserManager<IdentityUser> _userManager;

        public AuthController(TokenService tokenService, UserManager<IdentityUser> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }

        private static readonly List<UserModel> users = new List<UserModel>
        {
            new UserModel { Name = "Test", Surname = "User", Username = "testuser", Password = "password123" },
            new UserModel { Name = "Admin", Surname = "User", Username = "admin", Password = "admin123" }
        };

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
                return BadRequest("Bu kullanıcı adı zaten kullanılıyor.");

            var passwordErrors = ValidatePassword(model.Password);
            if (passwordErrors.Any())
                return BadRequest(new { Errors = passwordErrors });

            var newUser = new IdentityUser
            {
                UserName = model.Username,
                Email = $"{model.Username}@example.com" 
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        private List<string> ValidatePassword(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrEmpty(password))
                errors.Add("Şifre boş olamaz.");

            if (!password.Any(char.IsUpper))
                errors.Add("Şifre en az bir büyük harf içermelidir.");

            if (!password.Any(c => !char.IsLetterOrDigit(c)))
                errors.Add("Şifre en az bir özel karakter içermelidir.");

            if (!password.Any(char.IsDigit))
                errors.Add("Şifre en az bir rakam ('0'-'9') içermelidir.");

            return errors;
        }


        [HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginModel model)
{

    var user = await _userManager.FindByNameAsync(model.Username);
    
    if (user == null)
        return Unauthorized("Geçersiz kullanıcı adı veya şifre.");

    var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.Password);
    if (!isPasswordValid)
        return Unauthorized("Geçersiz kullanıcı adı veya şifre.");

    var tokenResponse = _tokenService.GenerateToken(user.UserName);
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
