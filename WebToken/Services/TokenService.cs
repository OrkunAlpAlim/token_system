using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebToken.Services
{
  public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public TokenResponse GenerateToken(string username)
    {
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);
        var tokenExpirationInSeconds = 30;
        var tokenExpiration = TimeSpan.FromSeconds(tokenExpirationInSeconds);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.Add(tokenExpiration),
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResponse
        {
            Token = tokenString,
            TokenSuresi = tokenExpiration.TotalSeconds,
            TokenKalanSure = tokenExpiration.TotalSeconds,
            TokenGecerliMi = true,
            IslemYapabilirMi = true
        };
    }
}

    public class TokenResponse
    {
        public string Token { get; set; }
        public double TokenSuresi { get; set; }
        public double TokenKalanSure { get; set; }
        public bool TokenGecerliMi { get; set; }
        public bool IslemYapabilirMi { get; set; }
    }
}
