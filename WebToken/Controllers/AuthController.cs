using Microsoft.AspNetCore.Mvc;
using WebToken.Services;
using System.Collections.Generic;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using WebToken.Models;


namespace WebToken.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(TokenService tokenService, UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null)
                return BadRequest("Bu kullanıcı adı zaten kullanılıyor.");

            var passwordErrors = ValidatePassword(model.Password);
            if (passwordErrors.Any())
                return BadRequest(new { Errors = passwordErrors });

            var newUser = new ApplicationUser
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

            var jwtSettings = _configuration.GetSection("Jwt");
            var tokenExpirationInSeconds = int.Parse(jwtSettings["TokenExpirationInSeconds"]);

            var expiration = tokenResponse.TokenSuresi;
            var remainingTime = expiration - (DateTime.UtcNow - tokenResponse.TokenIssuedAt).TotalSeconds;
            if (remainingTime < 0)
                remainingTime = 0;

            return Ok(new
            {
                KullaniciAdi = user.UserName,
                Ad = user.UserName,
                Soyad = user.UserName,
                Token = tokenResponse.Token,
                TokenSuresi = tokenExpirationInSeconds.ToString("F2"),
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
            var jwtSettings = _configuration.GetSection("Jwt");
            var tokenExpirationInSeconds = int.Parse(jwtSettings["TokenExpirationInSeconds"]);
            var remainingTime = (expiration - currentTime).TotalSeconds;
            if (remainingTime < 0)
                remainingTime = 0;

            var isTokenValid = remainingTime > 0;
            var hasAccess = isTokenValid;

            return Ok(new
            {
                TokenSuresi = tokenExpirationInSeconds.ToString("F2"),
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
